using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VSSystem.IO.Extensions;
using VSSystem.Service.MonitoringService.Models;

namespace VSSystem.Service.MonitoringService.Processes
{
    class SynchronizeFilesProcess : AProcess
    {
        SynchronizeFilesInfo _processInfo;
        DirectoryInfo _sourceFilesFolder, _destinationFilesFolder;

        public SynchronizeFilesProcess(SynchronizeFilesInfo processInfo)
        {
            _processInfo = processInfo;
            _sourceFilesFolder = new DirectoryInfo(processInfo.SourceFilesFolderPath);
            _destinationFilesFolder = new DirectoryInfo(processInfo.DestinationFilesFolderPath);
        }

        protected override void _Process(Action<string> debugLogAction = default, Action<Exception> errorLogAction = default)
        {
            try
            {
                if (_sourceFilesFolder?.Exists ?? false)
                {
                    DateTime now = DateTime.Now;
                    debugLogAction?.Invoke($"Synchronize folder {_sourceFilesFolder.FullName} to {_destinationFilesFolder.FullName}");
                    int total = _sourceFilesFolder.SynchronizeFolder(_destinationFilesFolder, _processInfo.CheckFileMd5, _processInfo.ExcludeCondition);
                    debugLogAction?.Invoke($"Synchronize folder {_sourceFilesFolder.FullName} to {_destinationFilesFolder.FullName} done. Total: {total}. Time: {(DateTime.Now - now).TotalMilliseconds}ms.");
                }
            }
            catch (Exception ex)
            {
                errorLogAction?.Invoke(ex);
            }
        }
    }
}