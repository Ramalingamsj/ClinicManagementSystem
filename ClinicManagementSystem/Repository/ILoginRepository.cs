using ClinicManagementSystem.Models;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Repository
{
    public interface ILoginRepository
    {
        Task<User> ValidateUser(string username, string password);
    }
}
