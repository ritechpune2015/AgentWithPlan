using System.Text.Json.Serialization;

namespace FirstAgent_DummyLLMService.Gemini
{
    public class GeminiFunctionDeclaration
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("parameters")]
        public object Parameters { get; set; } = new();

    }
}
