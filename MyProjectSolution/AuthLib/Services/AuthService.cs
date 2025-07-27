using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using AuthLib.Interfaces;
using AuthLib.Models;
using Microsoft.Extensions.Configuration;

namespace AuthLib.Services
{
    public class AuthService:IAuthService
    {
        private readonly string _connectionString;
        private readonly ITokenService _tokenService;

        public AuthService(IConfiguration config, ITokenService tokenService)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _tokenService = tokenService;
        }

        public async Task<AuthResult?> AuthenticateAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user);
            return new AuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user 
            };
        }


        private async Task<UserModel?> GetUserByUsernameAsync(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT Id, Username, PasswordHash, Role FROM Users WHERE Username = @Username", conn);
            cmd.Parameters.AddWithValue("@Username", username);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows)
                return null;

            await reader.ReadAsync();
            return new UserModel
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Role = reader.GetString(3)
            };
        }

        public async Task<RegistrationResult> RegisterAsync(string username, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return RegistrationResult.Fail("Username and password are required.");

            var passwordHash = ComputeSha256Hash(password);

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Check if user exists
            using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection);
            checkCmd.Parameters.AddWithValue("@Username", username);
            var exists = (int)(await checkCmd.ExecuteScalarAsync()) > 0;

            if (exists)
                return RegistrationResult.Fail("User already exists.");

            // Insert user
            using var insertCmd = new SqlCommand("INSERT INTO Users (Username, PasswordHash, Role) VALUES (@Username, @PasswordHash, @Role)", connection);
            insertCmd.Parameters.AddWithValue("@Username", username);
            insertCmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            insertCmd.Parameters.AddWithValue("@Role", role);

            await insertCmd.ExecuteNonQueryAsync();

            return RegistrationResult.Ok();
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var inputBytes = Encoding.UTF8.GetBytes(password);
            var inputHash = sha256.ComputeHash(inputBytes);
            var inputHashBase64 = Convert.ToBase64String(inputHash);

            return inputHashBase64 == storedHash;
        }
    }
}
