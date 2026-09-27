using FirstAgent_DummyLLMService.Gemini;
using ModelContextProtocol.Protocol;
using System.Text.Json;

namespace FirstAgent_DummyLLMService.Tools
{
    public interface IToolExecutor
    {
        // Task<CallToolResult> ExecuteAsync(string toolName,JsonElement arguments,CancellationToken cancellationToken);

        Task<AgentToolResult> ExecuteAsync(AgentToolCall toolCall,
        CancellationToken cancellationToken);


        Task<CallToolResult> ExecuteAsync(string toolname,
        JsonElement arguments, CancellationToken cancellationToken);


        Task<bool> ToolExistsAsync(
    string toolName,
    CancellationToken cancellationToken);

    }
}
