using System.Text.Json;

namespace EmailGeneratorAPI.Models
{
    public static class ResponseParser
    {
        public static string ExtractTextFromGeminiResponse(string jsonResponse)
        {
            try
            {
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse);

                if (geminiResponse?.Candidates?.Count > 0)
                {
                    var firstCandidate = geminiResponse.Candidates[0];
                    if (firstCandidate.Content?.Parts?.Count > 0)
                    {
                        var firstPart = firstCandidate.Content.Parts[0];
                        return firstPart.Text?.Trim() ?? "No content generated.";
                    }
                }

                return "No content generated.";
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse Gemini API response: {ex.Message}");
            }
        }

        public static bool IsValidGeminiResponse(string jsonResponse)
        {
            try
            {
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse);
                return geminiResponse?.Candidates?.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public static string ExtractErrorFromGeminiResponse(string jsonResponse)
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<GeminiApiError>(jsonResponse);
                return errorResponse?.Error?.Message ?? "Unknown API error occurred.";
            }
            catch
            {
                return "Failed to parse error response.";
            }
        }

        public static string CleanGeneratedEmail(string rawEmail)
        {
            if (string.IsNullOrWhiteSpace(rawEmail))
                return string.Empty;

            // Remove common prefixes that Gemini might add
            var cleaned = rawEmail.Trim();

            // Remove "Here's the email:" or similar phrases
            string[] prefixesToRemove = {
                "here's the email:",
                "here is the email:",
                "email:",
                "here's your email:",
                "generated email:"
            };

            foreach (var prefix in prefixesToRemove)
            {
                if (cleaned.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    cleaned = cleaned.Substring(prefix.Length).Trim();
                    break;
                }
            }

            return cleaned;
        }
    }
}
