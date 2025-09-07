namespace EmailGeneratorAPI.Services
{
    public interface IGeminiService
    {
        Task<string> GenerateContentAsync(string prompt);
        Task<bool> ValidateApiKeyAsync();
    }
}
