using System;
using System.Threading.Tasks;
using VSSystem.Logger;
using VSSystem.Service.MonitoringService.Workers;
using VSSystem.ServiceProcess;
using VSSystem.ServiceProcess.Extensions;

namespace VSSystem.Service.MonitoringService.Service
{
    public class VSService : AService
    {
        public VSService(string name, int server_ID, string rootComponentName, string privateKey, ALogger logger)
            : base(name, server_ID, rootComponentName, privateKey, new string[]
            {
                "pools",
                "backup_worker",
                "synchronize_worker"
            }, logger)
        {
        }

        protected override void _InitializeWorkers()
        {
            try
            {
                #region Backup
                AddWorker(new BackupWorker(Models.EComponentType.Database, Models.EProcessType.Schedule, ServiceConfig.backup_worker_enable, _name, 59, ServiceConfig.backup_worker_number_of_threads, _logger));
                AddWorker(new BackupWorker(Models.EComponentType.Database, Models.EProcessType.RealTime, ServiceConfig.backup_worker_enable, _name, ServiceConfig.backup_worker_interval, ServiceConfig.backup_worker_number_of_threads, _logger));

                AddWorker(new BackupWorker(Models.EComponentType.Files, Models.EProcessType.Schedule, ServiceConfig.backup_worker_enable, _name, 59, ServiceConfig.backup_worker_number_of_threads, _logger));
                AddWorker(new BackupWorker(Models.EComponentType.Files, Models.EProcessType.RealTime, ServiceConfig.backup_worker_enable, _name, ServiceConfig.backup_worker_interval, ServiceConfig.backup_worker_number_of_threads, _logger));    
                #endregion
                
                #region Synchronize
                AddWorker(new SynchronizeWorker(Models.EComponentType.Files, Models.EProcessType.Schedule, ServiceConfig.synchronize_worker_enable, _name, 59, ServiceConfig.synchronize_worker_number_of_threads, _logger));
                AddWorker(new SynchronizeWorker(Models.EComponentType.Files, Models.EProcessType.RealTime, ServiceConfig.synchronize_worker_enable, _name, ServiceConfig.synchronize_worker_interval, ServiceConfig.synchronize_worker_number_of_threads, _logger));
                #endregion
            }
            catch (Exception ex)
            {
                _ = this.LogErrorAsync(ex);
            }
        }

        async protected override Task _InitConfiguration()
        {
            await base._InitConfiguration();

            try
            {
                var dbIni = _ini;
                _ini.ReadAllStaticConfigs<ServiceConfig>(_defaultSections);

                if (string.IsNullOrWhiteSpace(ServiceConfig.pools_backup_folder))
                {
                    ServiceConfig.pools_backup_folder = WorkingFolder.FullName + "/Pools/Backup";
                }
                if (string.IsNullOrWhiteSpace(ServiceConfig.pools_synchronize_folder))
                {
                    ServiceConfig.pools_synchronize_folder = WorkingFolder.FullName + "/Pools/Synchronize";
                }
            }
            catch (Exception ex)
            {
                _ = this.LogErrorAsync(ex);
            }
        }
    }
}
