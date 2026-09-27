using System.Text.Json;

namespace FirstAgent_DummyLLMService.Gemini
{
    public class AgentToolCall
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public JsonElement Arguments { get; set; }

    }
}
