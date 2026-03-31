using ClinicManagementSystem.Models;
using ClinicManagementSystem.Repository;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Service
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _repository;

        public LoginService(ILoginRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> ValidateUserService(string username, string password)
        {
            return await _repository.ValidateUser(username, password);
        }
    }
}
