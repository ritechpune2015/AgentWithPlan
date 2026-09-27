using FirstAgent_DummyLLMService.Options;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace FirstAgent_DummyLLMService.MCP
{
    public class McpClientService: IMcpClientService
    {
        private readonly McpServerOptions _options;


        public McpClientService(IOptions<McpServerOptions> options)
        {
            _options =
            options.Value;
        }



        private async Task<McpClient> CreateClientAsync(
            CancellationToken cancellationToken){
            var serverUrl =_options.CustomerServerUrl;
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                throw new InvalidOperationException(
                    "MCP:CustomerServerUrl is not configured.");
            }

            var transport = new HttpClientTransport(
                new HttpClientTransportOptions
                {
                    Endpoint = new Uri(serverUrl),
                    ConnectionTimeout = TimeSpan.FromSeconds(30)
                });

            return await McpClient.CreateAsync(
                transport,
                cancellationToken: cancellationToken);
        }

      public async Task<IList<McpClientTool>> GetToolsAsync(CancellationToken cancellationToken = default)
        {
            await using var client = await CreateClientAsync(cancellationToken);
            return await client.ListToolsAsync(cancellationToken: cancellationToken);
        }

        public async Task<CallToolResult> CallToolAsync(string toolName,
        IReadOnlyDictionary<string, object?>? arguments = null,
        CancellationToken cancellationToken = default)
        {
            await using var client = await CreateClientAsync(cancellationToken);
            var result=await client.CallToolAsync(toolName,arguments,cancellationToken: cancellationToken);
           

            return result;
        }

        public async Task<object> GetServerCapablities(CancellationToken cancellationToken = default)
        {

            await using var client = await CreateClientAsync(cancellationToken);

            return new
            {
                tools =
            client.ServerCapabilities.Tools is not null,

                resources =
            client.ServerCapabilities.Resources is not null,

                prompts =
            client.ServerCapabilities.Prompts is not null
            };
        }
    }
}
