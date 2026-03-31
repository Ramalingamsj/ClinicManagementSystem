using System.Threading.Tasks;

namespace ClinicManagementSystem.Service
{
    public interface ISmsService
    {
        Task<(bool Success, string Message)> SendSmsAsync(string phoneNumber, string message);
    }
}
