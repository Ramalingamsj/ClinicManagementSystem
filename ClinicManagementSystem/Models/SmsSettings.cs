namespace ClinicManagementSystem.Models
{
    public class SmsSettings
    {
        public string Provider { get; set; } = "Generic";
        public string AccountSid { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public string SenderId { get; set; } = "CLINICSYS";
        public string ApiUrl { get; set; } = "";
        public bool EnableSimulation { get; set; } = true;
    }
}
