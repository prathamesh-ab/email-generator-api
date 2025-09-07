using EmailGeneratorAPI.Models;
using System.Text;
using System.Text.Json;

namespace EmailGeneratorAPI.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiKey = _configuration["GeminiApi:ApiKey"] ?? throw new ArgumentNullException("Gemini API Key not found");
            _apiUrl = _configuration["GeminiApi:BaseUrl"] ?? "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        }

        public async Task<string> GenerateContentAsync(string prompt)
        {
            try
            {
                var request = new GeminiRequest
                {
                    Contents = new List<GeminiContent>
                    {
                        new GeminiContent
                        {
                            Parts = new List<GeminiPart>
                            {
                                new GeminiPart { Text = prompt }
                            }
                        }
                    }
                };

                var jsonRequest = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", _apiKey);

                var response = await _httpClient.PostAsync(_apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = ResponseParser.ExtractErrorFromGeminiResponse(responseContent);
                    throw new HttpRequestException($"Gemini API error: {errorMessage}");
                }

                var generatedText = ResponseParser.ExtractTextFromGeminiResponse(responseContent);
                return ResponseParser.CleanGeneratedEmail(generatedText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating content with Gemini API");
                throw;
            }
        }

        public async Task<bool> ValidateApiKeyAsync()
        {
            try
            {
                var testPrompt = "Test";
                await GenerateContentAsync(testPrompt);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
