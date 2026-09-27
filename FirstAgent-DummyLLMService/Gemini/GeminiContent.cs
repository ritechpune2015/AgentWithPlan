using System.Text.Json.Serialization;

namespace FirstAgent_DummyLLMService.Gemini
{
    public class GeminiContent
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("parts")]
        public List<GeminiPart> Parts { get; set; } = [];

    }
}
