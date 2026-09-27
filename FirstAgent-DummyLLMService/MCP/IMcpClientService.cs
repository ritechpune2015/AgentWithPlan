
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace FirstAgent_DummyLLMService.MCP
{
    public interface IMcpClientService
    {
        Task<IList<McpClientTool>> GetToolsAsync(CancellationToken cancellationToken = default);

    Task<CallToolResult> CallToolAsync(string toolName,IReadOnlyDictionary<string,object?>?arguments = null,CancellationToken cancellationToken = default);

        Task<object> GetServerCapablities(CancellationToken cancellationToken = default);

    }

}
