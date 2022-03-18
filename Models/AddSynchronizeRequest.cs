using VSSystem.Hosting.Webs.Request;

namespace VSSystem.Service.MonitoringService.Models
{
    public class AddSynchronizeRequest : DefaultRequest
    { 
        string _Name;
        public string Name { get { return _Name; } set { _Name = value; }}
        string _Type;
        public string Type { get { return _Type; } set { _Type = value; }}
        string _ComponentType;
        public string ComponentType { get { return _ComponentType; } set { _ComponentType = value; }}
        ScheduleInfo _Schedule;
        public ScheduleInfo Schedule { get { return _Schedule; } set { _Schedule = value; }}
        object _Specs;
        public object Specs { get { return _Specs; } set { _Specs = value; }}
        string _Path;
        public string Path { get { return _Path ?? string.Empty; } set { _Path = value; }}
    }


}