namespace VSSystem.Service.MonitoringService.Models
{
    public class BackupDatabaseInfo : DatabaseInfo
    {
        string _BackupFolderPath;
        public string BackupFolderPath { get { return _BackupFolderPath; } set { _BackupFolderPath = value; }}
    }
    
}