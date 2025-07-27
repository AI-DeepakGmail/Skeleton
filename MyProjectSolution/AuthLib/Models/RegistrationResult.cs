namespace AuthLib.Models
{
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public static RegistrationResult Ok() => new() { Success = true };
        public static RegistrationResult Fail(string error) => new() { Success = false, ErrorMessage = error };
    }
}
