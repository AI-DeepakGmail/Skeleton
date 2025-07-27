using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using AuthLib.Interfaces;
using AuthLib.Models;
using Microsoft.Extensions.Configuration;

namespace AuthLib.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly string _connectionString;
        private readonly TimeSpan _tokenLifetime = TimeSpan.FromDays(7);

        public RefreshTokenService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<UserModel?> ValidateAsync(int userId, string rawToken)
        {
            var tokenHash = ComputeSha256Hash(rawToken);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(@"
                SELECT RT.Id, RT.UserId, RT.ExpiresAt, RT.RevokedAt,
                       U.Username, U.Role
                FROM RefreshTokens RT
                JOIN Users U ON U.Id = RT.UserId
                WHERE RT.UserId = @UserId AND RT.TokenHash = @TokenHash", connection);

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@TokenHash", tokenHash);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows) return null;
            await reader.ReadAsync();

            var expiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt"));
            var revokedAtOrdinal = reader.GetOrdinal("RevokedAt");
            var isRevoked = !reader.IsDBNull(revokedAtOrdinal);

            if (expiresAt < DateTime.UtcNow || isRevoked)
                return null;

            return new UserModel
            {
                Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Role = reader.GetString(reader.GetOrdinal("Role"))
            };
        }

        public async Task<RefreshTokenResult> CreateAsync(int userId)
        {
            var rawToken = GenerateSecureToken();
            var tokenHash = ComputeSha256Hash(rawToken);
            var expiresAt = DateTime.UtcNow.Add(_tokenLifetime);
            var newTokenId = Guid.NewGuid();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(@"
                INSERT INTO RefreshTokens (Id, UserId, TokenHash, ExpiresAt, CreatedAt)
                VALUES (@Id, @UserId, @TokenHash, @ExpiresAt, @CreatedAt)", connection);

            command.Parameters.AddWithValue("@Id", newTokenId);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@TokenHash", tokenHash);
            command.Parameters.AddWithValue("@ExpiresAt", expiresAt);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return new RefreshTokenResult
            {
                RawToken = rawToken,
                StoredToken = new RefreshToken
                {
                    Id = newTokenId,
                    UserId = userId,
                    TokenHash = tokenHash,
                    ExpiresAt = expiresAt,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public async Task RevokeAsync(Guid oldTokenId, Guid newTokenId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(@"
                UPDATE RefreshTokens
                SET RevokedAt = @RevokedAt,
                    ReplacedByTokenId = @NewTokenId
                WHERE Id = @OldTokenId", connection);

            command.Parameters.AddWithValue("@RevokedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@NewTokenId", newTokenId);
            command.Parameters.AddWithValue("@OldTokenId", oldTokenId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        private string GenerateSecureToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private string ComputeSha256Hash(string rawToken)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(rawToken);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    
    }
}
