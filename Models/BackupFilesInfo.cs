using VSSystem.IO;

namespace VSSystem.Service.MonitoringService.Models
{
    public class BackupFilesInfo
    {
        string _FilesFolderPath;
        public string FilesFolderPath { get { return _FilesFolderPath; } set { _FilesFolderPath = value; }}
        string _BackupFolderPath;
        public string BackupFolderPath { get { return _BackupFolderPath; } set { _BackupFolderPath = value; }}
        ExcludeCondition _ExcludeCondition;
        public ExcludeCondition ExcludeCondition { get { return _ExcludeCondition; } set { _ExcludeCondition = value; }}
    }
    
}