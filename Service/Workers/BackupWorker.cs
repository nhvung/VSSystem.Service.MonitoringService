using VSSystem.ServiceProcess.Workers;
using VSSystem.ServiceProcess.Extensions;
using System.Threading.Tasks;
using System.Threading;
using VSSystem.Logger;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using VSSystem.Service.MonitoringService.Models;
using VSSystem.Service.MonitoringService.Processes;

namespace VSSystem.Service.MonitoringService.Workers
{
    class BackupWorker : PoolWorker
    {
        EProcessType _processType;
        EComponentType _componentType;
        IProcess _process;
        public BackupWorker(EComponentType componentType, EProcessType processType, bool enabled, string serviceName, int interval, int numberOfThreads, ALogger logger)
        : base(new IntervalWorkerStartInfo("BackupWorker_" + componentType + "_" + processType, enabled, serviceName, interval, numberOfThreads, EWorkerIntervalUnit.Second), logger)
        {
            _processType = processType;
            _componentType = componentType;
            _initPoolFolderAction = delegate
            {
                _poolFolder = new System.IO.DirectoryInfo(ServiceConfig.pools_backup_folder + $"/Execute/{_componentType}/{_processType}");
            };
            _signFileExtension = ".json";
            if (_processType == EProcessType.Schedule)
            {
                _deleteSignFileWhenFinish = false;
            }
            _process = default;
        }
        async protected override void ProcessSignFile(FileInfo signFile, CancellationToken cancellationToken)
        {
            try
            {
                _process = null;
                string json = File.ReadAllText(signFile.FullName);
                var bkRequestObj = JsonConvert.DeserializeObject<AddBackupRequest>(json);
                if (bkRequestObj != null)
                {

                    if (_processType == EProcessType.Schedule)
                    {
                        int hour, minute;
                        int.TryParse(bkRequestObj.Schedule?.Hour, out hour);
                        int.TryParse(bkRequestObj.Schedule?.Minute, out minute);
                        EPeriod period = EPeriod.Undefine;
                        Enum.TryParse(bkRequestObj.Schedule?.Period, true, out period);
                        if (period != EPeriod.Undefine)
                        {
                            List<DayOfWeek> dayOfWeekObjs = new List<DayOfWeek>();
                            if (period == EPeriod.Daily)
                            {
                                dayOfWeekObjs = new List<DayOfWeek>(){
                                    DayOfWeek.Sunday,
                                    DayOfWeek.Monday,
                                    DayOfWeek.Tuesday,
                                    DayOfWeek.Wednesday,
                                    DayOfWeek.Thursday,
                                    DayOfWeek.Friday,
                                    DayOfWeek.Saturday,
                                };
                            }
                            else
                            {
                                if (bkRequestObj.Schedule.DayOfWeeks?.Count > 0)
                                {
                                    foreach (var sDayOfWeek in bkRequestObj.Schedule.DayOfWeeks)
                                    {
                                        DayOfWeek dowObj;
                                        if (Enum.TryParse(sDayOfWeek, true, out dowObj))
                                        {
                                            dayOfWeekObjs.Add(dowObj);
                                        }
                                    }
                                }
                            }

                            if (DateTime.UtcNow.OnTime(dayOfWeekObjs, hour, minute))
                            {
                                if (_componentType == EComponentType.Database)
                                {
                                    var backupInfo = JsonConvert.DeserializeObject<BackupDatabaseInfo>(JsonConvert.SerializeObject(bkRequestObj.Specs));
                                    backupInfo.EncryptedPassword = VSSystem.Security.Cryptography.DecryptFromHexString<string>(backupInfo.EncryptedPassword, VSHost.PRIVATE_KEY);
                                    var dbBin = new DirectoryInfo(WorkingFolder.FullName + "/DBBin");
                                    _process = new BackupDatabaseProcess(dbBin, backupInfo);
                                }
                                else if (_componentType == EComponentType.Files)
                                {
                                    var backupInfo = JsonConvert.DeserializeObject<BackupFilesInfo>(JsonConvert.SerializeObject(bkRequestObj.Specs));
                                    _process = new BackupFilesProcess(backupInfo);
                                }
                            }
                        }
                    }
                    else if (_processType == EProcessType.RealTime)
                    {
                        if (_componentType == EComponentType.Database)
                        {
                            var backupInfo = JsonConvert.DeserializeObject<BackupDatabaseInfo>(JsonConvert.SerializeObject(bkRequestObj.Specs));

                            backupInfo.EncryptedPassword = VSSystem.Security.Cryptography.DecryptFromHexString<string>(backupInfo.EncryptedPassword, VSHost.PRIVATE_KEY);

                            var dbBin = new DirectoryInfo(WorkingFolder.FullName + "/DBBin");
                            _process = new BackupDatabaseProcess(dbBin, backupInfo);
                        }
                        else if (_componentType == EComponentType.Files)
                        {
                            var backupInfo = JsonConvert.DeserializeObject<BackupFilesInfo>(JsonConvert.SerializeObject(bkRequestObj.Specs));
                            _process = new BackupFilesProcess(backupInfo);
                        }
                    }
                    if(_process != null)
                    {
                        await _process.Process(cancellationToken, this.LogDebug, this.LogError);
                    }
                    
                }
                else
                {
                    try
                    {
                        signFile.Delete();
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                _ = this.LogErrorAsync(ex);
            }
            await Task.CompletedTask;
        }
    }
}
