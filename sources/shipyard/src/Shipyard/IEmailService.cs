using System.Threading.Tasks;
using Shipyard.Models;

namespace Shipyard
{
    public interface IEmailService
    {
        Task SendEmail(EmailModel message);
    }
}
