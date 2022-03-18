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
    class SynchronizeWorker : PoolWorker
    {
        EProcessType _processType;
        EComponentType _componentType;
        IProcess _process;
        public SynchronizeWorker(EComponentType componentType, EProcessType processType, bool enabled, string serviceName, int interval, int numberOfThreads, ALogger logger)
        : base(new IntervalWorkerStartInfo("SynchronizeWorker_" + componentType + "_" + processType, enabled, serviceName, interval, numberOfThreads, EWorkerIntervalUnit.Second), logger)
        {
            _processType = processType;
            _componentType = componentType;
            _initPoolFolderAction = delegate
            {
                _poolFolder = new System.IO.DirectoryInfo(ServiceConfig.pools_synchronize_folder + $"/Execute/{_componentType}/{_processType}");
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
                var processRequestObj = JsonConvert.DeserializeObject<AddSynchronizeRequest>(json);
                if (processRequestObj != null)
                {
                    if (_processType == EProcessType.Schedule)
                    {
                        int hour, minute;
                        int.TryParse(processRequestObj.Schedule?.Hour, out hour);
                        int.TryParse(processRequestObj.Schedule?.Minute, out minute);
                        EPeriod period = EPeriod.Undefine;
                        Enum.TryParse(processRequestObj.Schedule?.Period, true, out period);
                        if (period != EPeriod.Undefine)
                        {
                            List<DayOfWeek> dayOfWeekObjs = new List<DayOfWeek>();
                            if (period == EPeriod.Daily)
                            {
                                dayOfWeekObjs = new List<DayOfWeek>() {
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
                                if (processRequestObj.Schedule.DayOfWeeks?.Count > 0)
                                {
                                    foreach (var sDayOfWeek in processRequestObj.Schedule.DayOfWeeks)
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
                                if (_componentType == EComponentType.Files)
                                {
                                    var processInfo = JsonConvert.DeserializeObject<SynchronizeFilesInfo>(JsonConvert.SerializeObject(processRequestObj.Specs));
                                    _process = new SynchronizeFilesProcess(processInfo);
                                }
                            }
                        }
                    }
                    else if (_processType == EProcessType.RealTime)
                    {
                        if (_componentType == EComponentType.Files)
                        {
                            var processInfo = JsonConvert.DeserializeObject<SynchronizeFilesInfo>(JsonConvert.SerializeObject(processRequestObj.Specs));
                            _process = new SynchronizeFilesProcess(processInfo);
                        }
                    }
                    if (_process != null)
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
