namespace Demo.Services.Services
{
    public interface IAuthService
    {
        public string GenerateToken();
        public DateTime CalculateExpirationTime();
    }
}