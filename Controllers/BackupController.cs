using System.Threading.Tasks;
using VSSystem.Hosting.Webs.Controllers;
using VSSystem.Hosting;
using System;
using System.Text;
using VSSystem.Hosting.Webs.Response;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using VSSystem.Service.MonitoringService.Models;

namespace VSSystem.Service.MonitoringService.Controllers
{
    public class BackupController : AController
    {
        public BackupController() : base("BackupController", VSHost.SERVICE_NAME, VSHost.StaticLogger)
        {
        }
        protected override Task _ProcessApiContext(string path, string queryString)
        {
            if (path.Equals($"{_ServicePath}api/backup/add/", StringComparison.InvariantCultureIgnoreCase))
            {
                return Add();
            }
            else if (path.Equals($"{_ServicePath}api/backup/list/", StringComparison.InvariantCultureIgnoreCase))
            {
                return List();
            }
            else if (path.Equals($"{_ServicePath}api/backup/delete/", StringComparison.InvariantCultureIgnoreCase))
            {
                return Delete();
            }
            return base._ProcessApiContext(path, queryString);
        }

        async Task Add()
        {
            try
            {
                var requestObj = await this.GetRequestObject<AddBackupRequest>(Encoding.UTF8);
                if (requestObj != null)
                {
                    EProcessType processType = EProcessType.Undefine;
                    Enum.TryParse(requestObj.Type, true, out processType);
                    EComponentType componentType = EComponentType.Undefine;
                    Enum.TryParse(requestObj.ComponentType, true, out componentType);
                    if (processType != EProcessType.Undefine && componentType != EComponentType.Undefine)
                    {
                        var processFolder = new DirectoryInfo(ServiceConfig.pools_backup_folder + "/Request/" + componentType + "/" + processType);
                        if (!processFolder.Exists)
                        {
                            processFolder.Create();
                        }

                        string jsonSpecs = JsonConvert.SerializeObject(requestObj);
                        string guid = Guid.NewGuid().ToString();
                        FileInfo jsonFile = new FileInfo(processFolder.FullName + "/" + guid + ".json");
                        await System.IO.File.WriteAllTextAsync(jsonFile.FullName, jsonSpecs, Encoding.UTF8);

                        await this.ResponseJsonAsync(DefaultResponse.Success, System.Net.HttpStatusCode.OK);
                    }
                    else
                    {
                        await this.ResponseJsonAsync(DefaultResponse.InvalidParameters, System.Net.HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    await this.ResponseJsonAsync(DefaultResponse.InvalidParameters, System.Net.HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                await this.ResponseJsonAsync(DefaultResponse.ServerError, System.Net.HttpStatusCode.InternalServerError);
                this.LogError(ex);
            }
        }

        async Task List()
        {
            try
            {
                var processFolder = new DirectoryInfo(ServiceConfig.pools_backup_folder);
                if (processFolder?.Exists ?? false)
                {
                    var processFiles = processFolder.GetFiles("*.json", SearchOption.AllDirectories);
                    if (processFiles?.Length > 0)
                    {
                        List<object> processRequestObjs = new List<object>();
                        foreach (var bkFile in processFiles)
                        {
                            var jsonSpecs = System.IO.File.ReadAllText(bkFile.FullName, Encoding.UTF8);
                            var processRequestObj = JsonConvert.DeserializeObject<AddBackupRequest>(jsonSpecs);                            
                            if (processRequestObj != null)
                            {
                                processRequestObj.Path = bkFile.FullName.Substring(processFolder.FullName.Length).Replace("\\", "/");
                                processRequestObjs.Add(processRequestObj);
                            }
                        }

                        await this.ResponseJsonAsync(processRequestObjs, System.Net.HttpStatusCode.OK);
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ResponseJsonAsync(DefaultResponse.ServerError, System.Net.HttpStatusCode.InternalServerError);
                this.LogError(ex);
            }
        }

        async Task Delete()
        {
            try
            {
                List<string> deletePaths = await this.GetRequestObject<List<string>>(Encoding.UTF8);
                if (deletePaths?.Count > 0)
                {
                    var processFolder = new DirectoryInfo(ServiceConfig.pools_backup_folder);
                    if (processFolder?.Exists ?? false)
                    {
                        foreach (var path in deletePaths)
                        {
                            var processFile = new FileInfo(processFolder.FullName + "/" + path);
                            if(processFile.Exists)
                            {
                                try
                                {
                                    processFile.Attributes = FileAttributes.Archive;
                                    processFile.Delete();
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ResponseJsonAsync(DefaultResponse.ServerError, System.Net.HttpStatusCode.InternalServerError);
                this.LogError(ex);
            }
        }
    }
}