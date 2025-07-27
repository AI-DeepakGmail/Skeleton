using AuthLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLib.Interfaces
{

    public interface ITokenService
    {
        string GenerateAccessToken(UserModel user); 
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(UserModel user);
    }



}
