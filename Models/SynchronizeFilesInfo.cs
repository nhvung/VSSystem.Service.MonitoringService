using VSSystem.IO;

namespace VSSystem.Service.MonitoringService.Models
{
    public class SynchronizeFilesInfo
    {
        string _SourceFilesFolderPath;
        public string SourceFilesFolderPath { get { return _SourceFilesFolderPath; } set { _SourceFilesFolderPath = value; }}
        string _DestinationFilesFolderPath;
        public string DestinationFilesFolderPath { get { return _DestinationFilesFolderPath; } set { _DestinationFilesFolderPath = value; }}
        ExcludeCondition _ExcludeCondition;
        public ExcludeCondition ExcludeCondition { get { return _ExcludeCondition; } set { _ExcludeCondition = value; }}
        bool _CheckFileMd5;
        public bool CheckFileMd5 { get { return _CheckFileMd5; } set { _CheckFileMd5 = value; }}
    }
    
}