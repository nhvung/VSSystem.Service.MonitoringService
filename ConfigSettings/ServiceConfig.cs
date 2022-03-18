namespace VSSystem.Service.MonitoringService
{
    public class ServiceConfig
    {
        #region pools
        static string _pools_backup_folder;
        static public string pools_backup_folder { get { return _pools_backup_folder; } set { _pools_backup_folder = value; }}
        static string _pools_synchronize_folder;
        static public string pools_synchronize_folder { get { return _pools_synchronize_folder; } set { _pools_synchronize_folder = value; }}
        #endregion

        #region backup_worker
        static bool _backup_worker_enable = true;
        static public bool backup_worker_enable { get { return _backup_worker_enable; } set { _backup_worker_enable = value; }} 
        static int _backup_worker_interval = 5;
        static public int backup_worker_interval { get { return _backup_worker_interval; } set { _backup_worker_interval = value; }}
        static int _backup_worker_number_of_threads = 1;
        static public int backup_worker_number_of_threads { get { return _backup_worker_number_of_threads; } set { _backup_worker_number_of_threads = value; }} 
        #endregion

        #region check_backup_worker
        static bool _check_backup_worker_enable = true;
        static public bool check_backup_worker_enable { get { return _check_backup_worker_enable; } set { _check_backup_worker_enable = value; }}
        static int _check_backup_worker_interval = 5;
        static public int check_backup_worker_interval { get { return _check_backup_worker_interval; } set { _check_backup_worker_interval = value; }}
        static int _check_backup_worker_number_of_threads = 1;
        static public int check_backup_worker_number_of_threads { get { return _check_backup_worker_number_of_threads; } set { _check_backup_worker_number_of_threads = value; }}
        
        #endregion

        #region synchronize_worker
        static bool _synchronize_worker_enable = true;
        static public bool synchronize_worker_enable { get { return _synchronize_worker_enable; } set { _synchronize_worker_enable = value; }}
        static int _synchronize_worker_interval = 5;
        static public int synchronize_worker_interval { get { return _synchronize_worker_interval; } set { _synchronize_worker_interval = value; }}
        static int _synchronize_worker_number_of_threads = 1;
        static public int synchronize_worker_number_of_threads { get { return _synchronize_worker_number_of_threads; } set { _synchronize_worker_number_of_threads = value; }}
        #endregion
    }
}
