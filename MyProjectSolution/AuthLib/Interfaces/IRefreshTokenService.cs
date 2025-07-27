using AuthLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLib.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<UserModel?> ValidateAsync(int userId, string refreshToken);
        Task<RefreshTokenResult> CreateAsync(int userId);
        Task RevokeAsync(Guid oldTokenId, Guid newTokenId);
    }


}
