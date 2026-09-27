namespace FirstAgent_DummyLLMService.Gemini
{
    public class GeminiAgentResponse
    {
        public bool IsFinal { get; set; }
        public string? Text { get; set; }
        public GeminiContent? ModelContent { get; set; }
        public List<GeminiFunctionCall> FunctionCalls { get; set; } = [];

    }
}
