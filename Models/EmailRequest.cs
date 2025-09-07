namespace EmailGeneratorAPI.Models
{
    public class EmailRequest
    {
        public string ActionType { get; set; } = string.Empty; // "compose", "reply", "formalize"
        public string? RecipientName { get; set; }
        public string? Subject { get; set; }
        public string? EmailContent { get; set; } // For reply/formalize
        public string? Keywords { get; set; }
        public string Tone { get; set; } = "formal"; // formal, casual, professional, friendly
        public string Language { get; set; } = "English";
        public string Complexity { get; set; } = "simple"; // simple, intermediate, advanced
    }

    public class EmailValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
