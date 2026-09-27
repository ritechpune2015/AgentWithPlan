namespace FirstAgent_DummyLLMService.Tools
{
    public class ToolCall
    {
        public string Id { get; set; } = "";

        public string Name { get; set; } = "";

        public Dictionary<string, object?> Arguments { get; set; } = [];

    }
}
