namespace EmailGeneratorAPI.Models
{
    public static class PromptBuilder
    {
        public static string BuildComposePrompt(EmailRequest request)
        {
            var prompt = $@"Write a {request.Tone} email in {request.Language} language to {request.RecipientName ?? "the recipient"} with the subject: '{request.Subject}'.

        Key requirements:
        - Include these keywords naturally: {request.Keywords}
        - Use {request.Complexity} language complexity
        - Maintain a {request.Tone} tone throughout
        - Make it professional and well-structured
        - Include appropriate greeting and closing

        Generate only the email content without any additional explanations.";

            return prompt;
        }

        public static string BuildReplyPrompt(EmailRequest request)
        {
            var prompt = $@"Generate a {request.Tone} reply in {request.Language} language to the following email:

                Original Email:
                {request.EmailContent}

                Key requirements:
                - Use {request.Complexity} language complexity
                - Maintain a {request.Tone} tone
                - Address the main points from the original email
                - Include these keywords if relevant: {request.Keywords ?? "professional communication"}
                - Make it concise and appropriate

                Generate only the reply email content without any additional explanations.";

            return prompt;
        }

        public static string BuildFormalizePrompt(EmailRequest request)
        {
            var prompt = $@"Formalize and improve the following email in {request.Language} language:

                Original Email:
                {request.EmailContent}

                Key requirements:
                - Make it more {request.Tone} and professional
                - Use {request.Complexity} language complexity
                - Correct any grammar and spelling errors
                - Improve sentence structure and flow
                - Include these keywords naturally: {request.Keywords ?? "professional communication"}
                - Maintain the original intent and meaning

                Generate only the improved email content without any additional explanations.";

            return prompt;
        }

        public static string GetLanguageCode(string language)
        {
            return language.ToLower() switch
            {
                "english" => "English",
                "hindi" => "Hindi",
                "bengali" => "Bengali",
                "tamil" => "Tamil",
                "telugu" => "Telugu",
                "marathi" => "Marathi",
                "gujarati" => "Gujarati",
                "kannada" => "Kannada",
                "malayalam" => "Malayalam",
                "punjabi" => "Punjabi",
                _ => "English"
            };
        }
    }
}
