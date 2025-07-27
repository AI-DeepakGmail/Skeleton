using AuthLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLib.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult?> AuthenticateAsync(string username, string password);

        Task<RegistrationResult> RegisterAsync(string username, string password, string role);

    }

}
