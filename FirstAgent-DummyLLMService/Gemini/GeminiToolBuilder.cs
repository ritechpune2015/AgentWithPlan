using FirstAgent_DummyLLMService.MCP;

namespace FirstAgent_DummyLLMService.Gemini
{
    public class GeminiToolBuilder
    {
        private readonly IMcpClientService _mcp;
        public GeminiToolBuilder(
            IMcpClientService mcp)
        {
            _mcp = mcp;
        }

        public async Task<List<GeminiTool>>
            BuildAsync(
                CancellationToken cancellationToken)
        {
            var mcpTools =
                await _mcp.GetToolsAsync(
                    cancellationToken);

            var declarations =
                new List<GeminiFunctionDeclaration>();

            foreach (var tool in mcpTools)
            {
                declarations.Add(
                    new GeminiFunctionDeclaration
                    {
                        Name = tool.Name,

                        Description =
                            tool.Description ?? "",
                        Parameters = tool.ProtocolTool.InputSchema
                    });
            }

            return
            [
                new GeminiTool
            {
                FunctionDeclarations =
                    declarations
            }
            ];

        }
    }
}