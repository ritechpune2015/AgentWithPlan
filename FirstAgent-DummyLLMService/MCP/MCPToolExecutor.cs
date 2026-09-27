using FirstAgent_DummyLLMService.Gemini;
using FirstAgent_DummyLLMService.Tools;
using ModelContextProtocol.Protocol;
using System.Text;
using System.Text.Json;

namespace FirstAgent_DummyLLMService.MCP
{
    public class MCPToolExecutor : IToolExecutor
    {
        private readonly IMcpClientService _mcpClient;
        


        public MCPToolExecutor(IMcpClientService mcpClient)
        {
            _mcpClient = mcpClient;
        }

        


        public async Task<AgentToolResult>   ExecuteAsync( AgentToolCall toolCall,
        CancellationToken cancellationToken)
        {
            var stopwatch =
                System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var arguments =
                    JsonSerializer.Deserialize<
                        Dictionary<string, object?>>(
                        toolCall.Arguments.GetRawText())
                    ?? [];

                var result =
                    await _mcpClient.CallToolAsync(
                        toolCall.Name,
                        arguments,
                        cancellationToken);

                stopwatch.Stop();

                return new AgentToolResult
                {
                    Id = toolCall.Id,

                    ToolName =
                        toolCall.Name,

                    Success = true,

                    Result = result,

                    DurationMs =
                        stopwatch.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                return new AgentToolResult
                {
                    Id = toolCall.Id,

                    ToolName =
                        toolCall.Name,

                    Success = false,

                    Error = ex.Message,

                    DurationMs =
                        stopwatch.ElapsedMilliseconds
                };
            }
        }




        public async Task<CallToolResult> ExecuteAsync(string toolName,
     JsonElement arguments,
     CancellationToken cancellationToken)
        {
            var dictionary = new Dictionary<string, object?>();

            if (arguments.ValueKind == JsonValueKind.Object)
            {
                foreach (var property
                         in arguments.EnumerateObject())
                {
                    dictionary[property.Name] =
                        ConvertJsonValue(property.Value);
                }
            }

            var result =
                await _mcpClient.CallToolAsync(toolName, dictionary, cancellationToken);

            return result;
        }

        private static object? ConvertJsonValue(JsonElement value)
        {
            return value.ValueKind switch
            {
                JsonValueKind.String =>
                    value.GetString(),

                JsonValueKind.Number =>
                    value.TryGetInt64(out var l)
                        ? l
                        : value.GetDouble(),

                JsonValueKind.True =>
                    true,

                JsonValueKind.False =>
                    false,

                JsonValueKind.Null =>
                    null,

                _ =>
                    value.ToString()
            };
        }


        public async Task<string> GetToolDescriptionsAsync(CancellationToken cancellationToken)
        {
            var tools =
                await _mcpClient.GetToolsAsync(
                    cancellationToken);

            var builder = new StringBuilder();

            foreach (var tool in tools)
            {
                builder.AppendLine(
                    $"Tool: {tool.Name}");

                builder.AppendLine(
                    $"Description: {tool.Description}");

                builder.AppendLine();
            }

            return builder.ToString();
        }

        public async Task<bool> ToolExistsAsync(
    string toolName,
    CancellationToken cancellationToken)
        {
            var tools =
                await _mcpClient.GetToolsAsync(
                    cancellationToken);

            return tools.Any(
                x =>
                    string.Equals(
                        x.Name,
                        toolName,
                        StringComparison.OrdinalIgnoreCase));
        }

        
    }

}
