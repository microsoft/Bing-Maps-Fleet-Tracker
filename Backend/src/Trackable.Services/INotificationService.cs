using System.Threading.Tasks;

namespace Trackable.Services
{
    public interface INotificationService
    {
        Task<bool> NotifyViaEmail(string email, string subject, string textContent, string htmlContent);
    }
}
