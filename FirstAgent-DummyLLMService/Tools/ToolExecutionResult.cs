using ModelContextProtocol.Protocol;

namespace FirstAgent_DummyLLMService.Tools
{
    public class ToolExecutionResult
    {
        public string ToolCallId { get; set; } = "";

        public string ToolName { get; set; } = "";

        public bool Success { get; set; }

        public CallToolResult? Result { get; set; }

        public string? Error { get; set; }

        public long DurationMs { get; set; }

    }
}
