using FirstAgent_DummyLLMService.Agents;
using FirstAgent_DummyLLMService.Tools;
using System.Text.Json;

namespace FirstAgent_DummyLLMService.LLMs
{
    public class OllamaService : ILLMService
    {
        private readonly HttpClient _client;

        private readonly string _model;

        public OllamaService( HttpClient client,IConfiguration configuration)
        {
            _client = client;

            _model =configuration["Ollama:Model"]
                ?? "phi4:latest";
        }

        public async Task<LLMResponse>
            GenerateWithToolsAsync(AgentState state,string context,         CancellationToken cancellationToken)
        {
            var prompt = $$$"""
You are an enterprise customer service agent.

You must decide whether to call a tool
or return a final answer.

Available tool names may include:

get_customer
get_customer_balance
get_customer_orders
Return ONLY valid JSON.

{{
  "type": "tool_call",
  "tool": "tool_name",
  "arguments": {{
      "customerId": "CUST001"
  }}
}}

For a final answer:

{{
  "type": "final",
  "answer": "your answer"
}}

Current context: {{{context}}}
""";

            var request = new
            {
                model = _model,
                prompt = prompt,
                stream = false,
                format = "json"
            };

            var response =await _client.PostAsJsonAsync(
                    "/api/generate",
                    request,
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            var result =
                await response.Content
                    .ReadFromJsonAsync<OllamaResponse>(
                        cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException(
                    "Empty Ollama response.");
            }

            return ParseResponse(result.Response);
        }

        private static LLMResponse ParseResponse(string response)
        {
            using var document =JsonDocument.Parse(response);

            var root = document.RootElement;

            var type =
                root.GetProperty("type")
                    .GetString();

            if (type == "final")
            {
                return new LLMResponse
                {
                    IsFinal = true,

                    Text =
                        root.GetProperty("answer")
                            .GetString()
                };
            }

            if (type == "tool_call")
            {
                var toolName =
                    root.GetProperty("tool")
                        .GetString()
                    ?? throw new InvalidOperationException(
                        "Tool name missing.");

                var arguments =
                    root.GetProperty("arguments");

                var dictionary =
                    JsonSerializer
                        .Deserialize<
                            Dictionary<string, object?>>(
                            arguments.GetRawText())
                    ?? [];

                return new LLMResponse
                {
                    IsFinal = false,

                    ToolCalls =
                    [
                        new ToolCall
                    {
                        Id =
                            Guid.NewGuid()
                                .ToString(),

                        Name = toolName,

                        Arguments = dictionary
                    }
                    ]
                };
            }

            throw new InvalidOperationException(
                "Invalid LLM response.");
        }
    }

}

