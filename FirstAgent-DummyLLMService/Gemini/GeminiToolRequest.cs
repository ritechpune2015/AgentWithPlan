using System.Text.Json.Serialization;

namespace FirstAgent_DummyLLMService.Gemini
{
    public class GeminiToolRequest
    {
        [JsonPropertyName("contents")]
        public List<GeminiContent> Contents { get; set; } = [];

        [JsonPropertyName("tools")]
        public List<GeminiTool> Tools { get; set; } = [];

    }
}
