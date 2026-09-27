namespace FirstAgent_DummyLLMService.Gemini
{
    public class AgentToolResult
    {
        public string Id { get; set; } = "";

        public string ToolName { get; set; } = "";

        public bool Success { get; set; }

        public object? Result { get; set; }

        public string? Error { get; set; }

        public long DurationMs { get; set; }

    }
}
