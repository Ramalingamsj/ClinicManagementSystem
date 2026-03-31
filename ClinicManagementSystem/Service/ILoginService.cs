using ClinicManagementSystem.Models;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Service
{
    public interface ILoginService
    {
        Task<User> ValidateUserService(string username, string password);
    }
}
