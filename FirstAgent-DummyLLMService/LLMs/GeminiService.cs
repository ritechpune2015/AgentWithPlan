using FirstAgent_DummyLLMService.Gemini;
using FirstAgent_DummyLLMService.Options;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using ModelContextProtocol.Protocol;
using System.Text.Json;

namespace FirstAgent_DummyLLMService.LLMs
{
    public class GeminiService
    {
        private readonly HttpClient _client;
        private readonly GeminiOptions _options;

        public GeminiService(HttpClient client, IOptions<GeminiOptions> options)
        {
            _client = client;
           _options = options.Value;
        }

        public async Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken)
        {
            var request = new
            {
                contents = new[]
                {
            new
            {
                role = "user",

                parts = new[]
                {
                    new
                    {
                        text = prompt
                    }
                }
            }
        }
            };

            var response = await _client.PostAsJsonAsync(
                    $"models/{_options.Model}:generateContent" +
                    $"?key={_options.ApiKey}",
                    request,
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            using var document = JsonDocument.Parse(json);

            return document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()
                ?? "";
        }


        public async Task<string> GeneratePlanAsync(
    string prompt,     CancellationToken cancellationToken)
        {
            var request = new
            {
                contents = new[]
                {
            new
            {
                role = "user",

                parts = new[]
                {
                    new
                    {
                        text = prompt
                    }
                }
            }
        },

                generationConfig = new
                {
                    responseMimeType =
                        "application/json"
                }
            };

            var response =
                await _client.PostAsJsonAsync(
                    $"models/{_options.Model}:generateContent" +
                    $"?key={_options.ApiKey}",
                    request,
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            var json =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            using var document =
                JsonDocument.Parse(json);

            var text =
                document.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

            return text ?? "";
        }


        public async Task<JsonDocument> GenerateAsync(List<GeminiContent> contents, List<GeminiTool> tools, CancellationToken cancellationToken)
        {
            var request = new GeminiToolRequest
            {
                Contents = contents,
                Tools = tools
            };

            var response =
                await _client.PostAsJsonAsync(
                    $"models/{_options.Model}:generateContent" +
                    $"?key={_options.ApiKey}",
                    request,
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            var json =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken);

            return JsonDocument.Parse(json);
        }


        public static GeminiContent   ExtractModelContent(
        JsonDocument document)
        {
            var content =
                document.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content");

            return JsonSerializer.Deserialize<GeminiContent>(
                content.GetRawText(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
        }


        public static List<AgentToolCall> ExtractFunctionCalls(JsonDocument document)
        {
            var result =
                new List<AgentToolCall>();

            var parts =
                document.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts");

            foreach (var part in parts.EnumerateArray())
            {
                if (!part.TryGetProperty(
                        "functionCall",
                        out var functionCall))
                {
                    continue;
                }

                var id =
                    functionCall.TryGetProperty(
                        "id",
                        out var idElement)
                        ? idElement.GetString()
                        : Guid.NewGuid().ToString();

                var name =
                    functionCall
                        .GetProperty("name")
                        .GetString()!;

                var args =
                    functionCall.TryGetProperty(
                        "args",
                        out var argsElement)
                        ? argsElement.Clone()
                        : JsonDocument.Parse("{}")
                            .RootElement;

                result.Add(
                    new AgentToolCall
                    {
                        Id = id!,
                        Name = name,
                        Arguments = args
                    });
                }
                return result;
            }

        

            //public async Task<GeminiAgentResponse> GenerateAsync(
            //List<GeminiContent> contents,
            //CancellationToken cancellationToken)
            //{
            //    var request = new GeminiToolRequest
            //        {
            //            Contents = contents,
            //            Tools = BuildTools()  //modification 
            //        };

            //    var response =
            //        await _client.PostAsJsonAsync(
            //            $"models/{_options.Model}:generateContent" +
            //            $"?key={_options.ApiKey}",
            //            request,
            //            cancellationToken);

            //    response.EnsureSuccessStatusCode();

            //    var json =
            //        await response.Content
            //            .ReadAsStringAsync(
            //                cancellationToken);

            //    return ParseResponse(json);
            //}

            //private static GeminiAgentResponse ParseResponse(string json)
            //{
            //    using var document =
            //        JsonDocument.Parse(json);

            //    var root =
            //        document.RootElement;

            //    var content =
            //        root
            //            .GetProperty("candidates")[0]
            //            .GetProperty("content");

            //    var modelContent =
            //        JsonSerializer.Deserialize<GeminiContent>(
            //            content.GetRawText());

            //    if (modelContent == null)
            //    {
            //        throw new InvalidOperationException(
            //            "Gemini returned no model content.");
            //    }

            //    var functionCalls =
            //        modelContent.Parts
            //            .Where(x => x.FunctionCall != null)
            //            .Select(x => x.FunctionCall!)
            //            .ToList();

            //    if (functionCalls.Count > 0)
            //    {
            //        return new GeminiAgentResponse
            //        {
            //            IsFinal = false,

            //            ModelContent = modelContent,

            //            FunctionCalls = functionCalls
            //        };
            //    }

            //    var text =
            //        string.Join(
            //            Environment.NewLine,
            //            modelContent.Parts
            //                .Where(x =>
            //                    !string.IsNullOrWhiteSpace(x.Text))
            //                .Select(x => x.Text));

            //    return new GeminiAgentResponse
            //    {
            //        IsFinal = true,

            //        Text = text,

            //        ModelContent = modelContent
            //    };
            //}


            //private List<GeminiTool> BuildTools()
            //{
            //    return
            //    [
            //        new GeminiTool
            //{
            //    FunctionDeclarations =
            //    [
            //        BuildCustomerTool(),

            //        BuildCustomerBalanceTool(),

            //        BuildCustomerOrdersTool()
            //    ]
            //}
            //    ];
            //}


            //    private GeminiFunctionDeclaration   BuildCustomerBalanceTool()
            //    {
            //        return new GeminiFunctionDeclaration
            //        {
            //            Name = "get_customer_balance",

            //            Description =
            //                "Gets the current account balance " +
            //                "for a customer.",

            //            Parameters = new
            //            {
            //                type = "OBJECT",

            //                properties = new
            //                {
            //                    customerId = new
            //                    {
            //                        type = "STRING",

            //                        description =
            //                            "Unique customer ID."
            //                    }
            //                },

            //                required = new[]
            //                {
            //                   "customerId"
            //                }
            //            }
            //        };
            //    }

            //    private GeminiFunctionDeclaration BuildCustomerTool()
            //    {
            //        return new GeminiFunctionDeclaration
            //        {
            //            Name = "get_customer",

            //            Description =
            //                "Gets customer information by customer ID.",

            //            Parameters = new
            //            {
            //                type = "OBJECT",

            //                properties = new
            //                {
            //                    customerId = new
            //                    {
            //                        type = "STRING",

            //                        description =
            //                            "Unique customer ID."
            //                    }
            //                },

            //                required = new[]
            //                {
            //            "customerId"
            //        }
            //            }
            //        };
            //    }


            //private GeminiFunctionDeclaration     BuildCustomerOrdersTool()
            //    {
            //        return new GeminiFunctionDeclaration
            //        {
            //            Name = "get_customer_orders",

            //            Description =
            //                "Gets all orders for a customer.",

            //            Parameters = new
            //            {
            //                type = "OBJECT",

            //                properties = new
            //                {
            //                    customerId = new
            //                    {
            //                        type = "STRING",

            //                        description =
            //                            "Unique customer ID."
            //                    }
            //                },

            //                required = new[]
            //                {
            //            "customerId"
            //        }
            //            }
            //        };
            //    }

        }
    }
