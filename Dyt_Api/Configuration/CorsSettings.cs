namespace Dyt_Api.Configuration
{
    public sealed class CorsSettings
    {
        public const string SectionName = "Cors";
        public const string FrontendPolicyName = "Frontend";

        public string[] AllowedOrigins { get; set; } = [];
    }
}
