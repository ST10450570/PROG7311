namespace GLMS.Web.Observers
{
    public class EmailNotificationObserver : IContractObserver
    {
        private readonly string _smtpHost;

        public EmailNotificationObserver(string smtpHost)
        {
            _smtpHost = smtpHost;
        }

        public void OnStatusChanged(int contractId, string newStatus)
        {
            
            Console.WriteLine($"[EMAIL] Contract #{contractId} moved to '{newStatus}'. SMTP Host: {_smtpHost}");
        }
    }
}
