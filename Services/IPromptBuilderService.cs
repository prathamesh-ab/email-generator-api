using EmailGeneratorAPI.Models;

namespace EmailGeneratorAPI.Services
{
    public interface IPromptBuilderService
    {
        string BuildComposePrompt(EmailRequest request);
        string BuildReplyPrompt(EmailRequest request);
        string BuildFormalizePrompt(EmailRequest request);
        EmailValidationResult ValidateRequest(EmailRequest request);
    }
}
