using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VSSystem.IO.Extensions;
using VSSystem.Service.MonitoringService.Models;

namespace VSSystem.Service.MonitoringService.Processes
{
    class BackupFilesProcess : AProcess
    {
        BackupFilesInfo _backupInfo;
        DirectoryInfo _filesFolder, _backupFolder;

        public BackupFilesProcess(BackupFilesInfo backupInfo)
        {
            _backupInfo = backupInfo;
            _filesFolder = new DirectoryInfo(backupInfo.FilesFolderPath);
            _backupFolder = new DirectoryInfo(backupInfo.BackupFolderPath);
        }

        protected override void _Process(Action<string> debugLogAction = default, Action<Exception> errorLogAction = default)
        {
            try
            {
                if (_filesFolder?.Exists ?? false)
                {
                    DateTime now = DateTime.Now;
                    DirectoryInfo bkFolder = new DirectoryInfo(_backupFolder.FullName + $"/{_filesFolder.Name}.{now.ToString("yyyyMMdd.HHmm")}");                    
                    debugLogAction?.Invoke($"Backup folder {_filesFolder.FullName} to {bkFolder.FullName}");
                    _filesFolder.CopyTo(bkFolder, _backupInfo.ExcludeCondition);
                    debugLogAction?.Invoke($"Backup folder {_filesFolder.FullName} to {bkFolder.FullName} done. Time: {(DateTime.Now - now).TotalMilliseconds}ms.");
                }
            }
            catch (Exception ex)
            {
                errorLogAction?.Invoke(ex);
            }
        }
    }
}