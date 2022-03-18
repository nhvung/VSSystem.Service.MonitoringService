using System.Collections.Generic;

namespace VSSystem.Service.MonitoringService.Models
{
    public class ScheduleInfo
    {
        string _Period;
        public string Period { get { return _Period; } set { _Period = value; }}
        List<string> _DayOfWeeks;
        public List<string> DayOfWeeks { get { return _DayOfWeeks; } set { _DayOfWeeks = value; }}
        string _Hour;
        public string Hour { get { return _Hour; } set { _Hour = value; }}
        string _Minute;
        public string Minute { get { return _Minute; } set { _Minute = value; }}
    }
}