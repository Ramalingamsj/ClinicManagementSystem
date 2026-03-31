using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClinicManagementSystem.Models;
using Microsoft.Extensions.Options;

namespace ClinicManagementSystem.Service
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly SmsSettings _settings;

        public SmsService(HttpClient httpClient, IOptions<SmsSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<(bool Success, string Message)> SendSmsAsync(string phoneNumber, string message)
        {
            // Automatic formatting for Indian mobile numbers (+91)
            var formattedNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");
            if (formattedNumber.Length == 10 && !formattedNumber.StartsWith("+"))
            {
                formattedNumber = "+91" + formattedNumber;
                Console.WriteLine($"[SMS FORMAT] Auto-converted to: {formattedNumber}");
            }

            if (_settings.EnableSimulation)
            {
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"[SMS SIMULATION MODE]");
                Console.WriteLine($"To: {formattedNumber}");
                Console.WriteLine($"From: {_settings.SenderId}");
                Console.WriteLine($"Message: {message}");
                Console.WriteLine("--------------------------------------------------");
                await Task.Delay(200);
                return (true, "Simulation Successful");
            }

            try
            {
                // Twilio requires Basic Authentication: (AccountSid : AuthToken)
                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.AccountSid}:{_settings.ApiKey}"));
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

                // Twilio expects 'application/x-www-form-urlencoded' data
                var values = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("To", formattedNumber),
                    new KeyValuePair<string, string>("From", _settings.SenderId),
                    new KeyValuePair<string, string>("Body", message)
                };

                var content = new FormUrlEncodedContent(values);
                var response = await _httpClient.PostAsync(_settings.ApiUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[SMS TWILIO RESPONSE] {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    // Basic extraction of SID for tracking
                    string sid = "UNKNOWN";
                    if (responseBody.Contains("\"sid\":")) {
                        var p1 = responseBody.Split(new[] { "\"sid\": \"" }, StringSplitOptions.None);
                        if (p1.Length > 1) sid = p1[1].Split('\"')[0];
                    }

                    Console.WriteLine($"[SMS TWILIO SUCCESS] Dispatched to {formattedNumber}, SID: {sid}");
                    return (true, $"Dispatched Successfully! SID: {sid}");
                }

                Console.WriteLine($"[SMS TWILIO ERROR] Status: {response.StatusCode}, Details: {responseBody}");
                return (false, $"Twilio Error: {responseBody}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMS TWILIO EXCEPTION] {ex.Message}");
                return (false, $"Connection Error: {ex.Message}");
            }
        }
    }
}
