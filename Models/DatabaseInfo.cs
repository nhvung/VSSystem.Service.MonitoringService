namespace VSSystem.Service.MonitoringService.Models
{
    public class DatabaseInfo
    {
        string _Server;
        public string Server { get { return _Server; } set { _Server = value; }}
        string _Username;
        public string Username { get { return _Username; } set { _Username = value; }}
        string _EncryptedPassword;
        public string EncryptedPassword { get { return _EncryptedPassword; } set { _EncryptedPassword = value; }}
        int _Port;
        public int Port { get { return _Port; } set { _Port = value; }}
        string _DatabaseName;
        public string DatabaseName { get { return _DatabaseName; } set { _DatabaseName = value; }}
    }
    
}