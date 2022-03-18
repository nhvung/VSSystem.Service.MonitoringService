using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VSSystem.Service.MonitoringService.Models;

namespace VSSystem.Service.MonitoringService.Processes
{
    class BackupDatabaseProcess : AProcess
    {
        BackupDatabaseInfo _backupInfo;
        DirectoryInfo _dbBinFolder, _backupFolder;

        public BackupDatabaseProcess()
        {
        }

        public BackupDatabaseProcess(DirectoryInfo dbBinFolder, BackupDatabaseInfo backupInfo)
        {
            _backupInfo = backupInfo;
            _dbBinFolder = dbBinFolder;
            _backupFolder = new DirectoryInfo(backupInfo.BackupFolderPath);
        }

        protected override void _Process(Action<string> debugLogAction = default, Action<Exception> errorLogAction = default)
        {
            try
            {
                if ((_dbBinFolder?.Exists ?? false) && (_backupFolder?.Exists ?? false))
                {
                    FileInfo mysqldumpFile = new FileInfo(_dbBinFolder.FullName + "/mysqldump.exe");
                    if (mysqldumpFile.Exists)
                    {
                        DateTime now = DateTime.Now;                        
                        
                        FileInfo bkFile = new FileInfo(_backupFolder.FullName + $"/{_backupInfo.DatabaseName}.{now.ToString("yyyyMMdd.HHmm")}.sql");

                        string args = string.Format("\"{5}\" -h {0} -u {1} --password=\"{2}\" --routines=True {3} > \"{4}\"", _backupInfo.Server, _backupInfo.Username, _backupInfo.EncryptedPassword, _backupInfo.DatabaseName, bkFile.FullName, mysqldumpFile.FullName);
                        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
                        psi.Verb = "runas";
                        psi.UseShellExecute = false;
                        psi.RedirectStandardInput = true;
                        psi.CreateNoWindow = true;

                        debugLogAction?.Invoke($"Backup database {_backupInfo.DatabaseName} to {bkFile.FullName}");

                        Process p = System.Diagnostics.Process.Start(psi);
                        p.StandardInput.WriteLine(args);
                        p.StandardInput.Close();
                        p.WaitForExit();
                        p.Close();

                        debugLogAction?.Invoke($"Backup database {_backupInfo.DatabaseName} to {bkFile.FullName} done. Time: {(DateTime.Now - now).TotalMilliseconds}ms.");
                    }
                }
            }
            catch (Exception ex)
            {
                errorLogAction?.Invoke(ex);
            }
        }
    }
}