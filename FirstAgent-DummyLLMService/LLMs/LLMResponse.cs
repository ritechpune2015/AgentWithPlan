using FirstAgent_DummyLLMService.Tools;

namespace FirstAgent_DummyLLMService.LLMs
{
    public class LLMResponse
    {
        public bool IsFinal { get; set; }

        public string? Text { get; set; }

        public List<ToolCall> ToolCalls { get; set; } = [];

    }
}
