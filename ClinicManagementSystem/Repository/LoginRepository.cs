using ClinicManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Repository
{
    public class LoginRepository : ILoginRepository
    {
        private readonly HospitalManagementDbContext _context;

        public LoginRepository(HospitalManagementDbContext context)
        {
            _context = context;
        }

        public async Task<User> ValidateUser(string username, string password)
        {
            if (_context != null)
            {
                User? dbUser = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

                return dbUser;
            }

            return null;
        }
    }
}
