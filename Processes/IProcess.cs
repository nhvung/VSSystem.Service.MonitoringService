using System;
using System.Threading;
using System.Threading.Tasks;

namespace VSSystem.Service.MonitoringService.Processes
{
    public interface IProcess
    {
        Task Process(CancellationToken cancellationToken, Action<string> debugLogAction = default, Action<Exception> errorLogAction = default);
    }
}