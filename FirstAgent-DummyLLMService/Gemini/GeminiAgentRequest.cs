namespace FirstAgent_DummyLLMService.Gemini
{
    public class GeminiAgentRequest
    {
        public List<GeminiContent> Contents { get; set; }
    = [];

        public List<GeminiTool> Tools { get; set; }
            = [];

    }
}
