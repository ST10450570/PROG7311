namespace GLMS.Api.Observers
{
    public class AuditLogObserver : IContractObserver
    {
        private readonly string _logPath;

        public AuditLogObserver(string logPath)
        {
            _logPath = logPath;
        }

        public void OnStatusChanged(int contractId, string newStatus)
        {
            var entry = $"[{DateTime.UtcNow:u}] Contract #{contractId} status changed to {newStatus}{Environment.NewLine}";
            File.AppendAllText(_logPath, entry);
        }
    }
}