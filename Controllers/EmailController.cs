using EmailGeneratorAPI.Models;
using EmailGeneratorAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailGeneratorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private readonly IPromptBuilderService _promptBuilderService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            IGeminiService geminiService,
            IPromptBuilderService promptBuilderService,
            ILogger<EmailController> logger)
        {
            _geminiService = geminiService;
            _promptBuilderService = promptBuilderService;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<ActionResult<ApiResponse<EmailResponse>>> GenerateEmail([FromBody] EmailRequest request)
        {
            try
            {
                // Validate request
                var validation = _promptBuilderService.ValidateRequest(request);
                if (!validation.IsValid)
                {
                    return BadRequest(new ApiResponse<EmailResponse>
                    {
                        Success = false,
                        Message = validation.ErrorMessage,
                        ErrorCode = "VALIDATION_ERROR"
                    });
                }

                // Build prompt based on action type
                string prompt = request.ActionType.ToLower() switch
                {
                    "compose" => _promptBuilderService.BuildComposePrompt(request),
                    "reply" => _promptBuilderService.BuildReplyPrompt(request),
                    "formalize" => _promptBuilderService.BuildFormalizePrompt(request),
                    _ => throw new ArgumentException("Invalid action type")
                };

                // Generate email using Gemini API
                var generatedContent = await _geminiService.GenerateContentAsync(prompt);

                var response = new EmailResponse
                {
                    Success = true,
                    GeneratedEmail = generatedContent,
                    Timestamp = DateTime.UtcNow,
                    RequestId = Guid.NewGuid().ToString()
                };

                _logger.LogInformation("Email generated successfully for action: {ActionType}", request.ActionType);

                return Ok(new ApiResponse<EmailResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "Email generated successfully"
                });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Invalid request parameters");
                return BadRequest(new ApiResponse<EmailResponse>
                {
                    Success = false,
                    Message = argEx.Message,
                    ErrorCode = "INVALID_PARAMETERS"
                });
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Error calling Gemini API");
                return StatusCode(502, new ApiResponse<EmailResponse>
                {
                    Success = false,
                    Message = "External API error occurred",
                    ErrorCode = "EXTERNAL_API_ERROR"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error generating email");
                return StatusCode(500, new ApiResponse<EmailResponse>
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    ErrorCode = "INTERNAL_SERVER_ERROR"
                });
            }
        }

        [HttpGet("health")]
        public async Task<ActionResult<ApiResponse<object>>> HealthCheck()
        {
            try
            {
                var isApiKeyValid = await _geminiService.ValidateApiKeyAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = isApiKeyValid,
                    Data = new
                    {
                        status = isApiKeyValid ? "healthy" : "unhealthy",
                        geminiApi = isApiKeyValid ? "connected" : "disconnected",
                        timestamp = DateTime.UtcNow
                    },
                    Message = isApiKeyValid ? "Service is healthy" : "Gemini API connection failed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Health check failed",
                    ErrorCode = "HEALTH_CHECK_ERROR"
                });
            }
        }

        [HttpGet("supported-languages")]
        public ActionResult<ApiResponse<object>> GetSupportedLanguages()
        {
            var languages = new[]
            {
                new { code = "english", name = "English" },
                new { code = "hindi", name = "Hindi" },
                new { code = "bengali", name = "Bengali" },
                new { code = "tamil", name = "Tamil" },
                new { code = "telugu", name = "Telugu" },
                new { code = "marathi", name = "Marathi" },
                new { code = "gujarati", name = "Gujarati" },
                new { code = "kannada", name = "Kannada" },
                new { code = "malayalam", name = "Malayalam" },
                new { code = "punjabi", name = "Punjabi" }
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { languages },
                Message = "Supported languages retrieved successfully"
            });
        }

        [HttpGet("supported-tones")]
        public ActionResult<ApiResponse<object>> GetSupportedTones()
        {
            var tones = new[]
            {
                new { code = "formal", name = "Formal", description = "Professional and structured" },
                new { code = "casual", name = "Casual", description = "Relaxed and conversational" },
                new { code = "professional", name = "Professional", description = "Business-appropriate" },
                new { code = "friendly", name = "Friendly", description = "Warm and approachable" }
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { tones },
                Message = "Supported tones retrieved successfully"
            });
        }
    }
}
