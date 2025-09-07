using EmailGeneratorAPI.Models;

namespace EmailGeneratorAPI.Services
{
    public class PromptBuilderService : IPromptBuilderService
    {
        public string BuildComposePrompt(EmailRequest request)
        {
            return PromptBuilder.BuildComposePrompt(request);
        }

        public string BuildReplyPrompt(EmailRequest request)
        {
            return PromptBuilder.BuildReplyPrompt(request);
        }

        public string BuildFormalizePrompt(EmailRequest request)
        {
            return PromptBuilder.BuildFormalizePrompt(request);
        }

        public EmailValidationResult ValidateRequest(EmailRequest request)
        {
            var result = new EmailValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(request.ActionType))
            {
                result.IsValid = false;
                result.ErrorMessage = "Action type is required.";
                return result;
            }

            switch (request.ActionType.ToLower())
            {
                case "compose":
                    if (string.IsNullOrWhiteSpace(request.Subject))
                    {
                        result.IsValid = false;
                        result.ErrorMessage = "Subject is required for compose action.";
                    }
                    break;

                case "reply":
                case "formalize":
                    if (string.IsNullOrWhiteSpace(request.EmailContent))
                    {
                        result.IsValid = false;
                        result.ErrorMessage = "Email content is required for this action.";
                    }
                    break;

                default:
                    result.IsValid = false;
                    result.ErrorMessage = "Invalid action type. Allowed: compose, reply, formalize.";
                    break;
            }

            return result;
        }
    }
}
