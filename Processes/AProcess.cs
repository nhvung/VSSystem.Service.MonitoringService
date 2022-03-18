using System;
using System.Threading;
using System.Threading.Tasks;

namespace VSSystem.Service.MonitoringService.Processes
{
    abstract class AProcess : IProcess
    {
        public Task Process(CancellationToken cancellationToken, Action<string> debugLogAction = default, Action<Exception> errorLogAction = default)
        {
            return Task.Run(()=>_Process(debugLogAction, errorLogAction), cancellationToken);
        }

        protected abstract void _Process(Action<string> debugLogAction = default, Action<Exception> errorLogAction = default);
    }
}