using FirstAgent_DummyLLMService.Gemini;
using FirstAgent_DummyLLMService.LLMs;
using FirstAgent_DummyLLMService.Tools;
using System.Text.Json;

namespace FirstAgent_DummyLLMService.Agents
{
    public class GeminiMcpAgent
    {
        private readonly GeminiService _gemini;

        private readonly GeminiToolBuilder _toolBuilder;

        private readonly ParallelToolExecutor _executor;


        public GeminiMcpAgent(GeminiService gemini, GeminiToolBuilder toolBuilder,
            ParallelToolExecutor executor)
        {
            _gemini = gemini;
            _toolBuilder = toolBuilder;
            _executor = executor;
        }


        public async Task<string> RunAsync(string userMessage, CancellationToken cancellationToken)
        {
            var tools = await _toolBuilder.BuildAsync(cancellationToken);

            var contents =
                new List<GeminiContent>
                {
            new GeminiContent
            {
                Role = "user",

                Parts =
                [
                    new GeminiPart
                    {
                        Text = userMessage
                    }
                ]
            }
                };

            for (int iteration = 1;  iteration <= 10;iteration++)
            {
                var response =
                    await _gemini.GenerateAsync(contents, tools, cancellationToken);
                
                var toolCalls = GeminiService.ExtractFunctionCalls(response);

                if (toolCalls.Count == 0)
                {
                    return ExtractText(response);
                }

                var modelContent =
                    GeminiService.ExtractModelContent(
                        response);

                contents.Add(modelContent);

                //var results =
                //    await _executor.ExecuteAsync(
                //        toolCalls,
                //        ToolExecutionMode.Parallel,
                //        cancellationToken);

                var results =
                  await _executor.ExecuteParallelAsync(
                      toolCalls,
                      ToolExecutionMode.Parallel,
                      cancellationToken);
                AddToolResults(
                    contents,
                    results);
            }

            throw new InvalidOperationException(
                "Maximum agent iterations exceeded.");
        }


        private static string ExtractText(
    JsonDocument document)
        {
            var parts =
                document.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts");

            foreach (var part in parts.EnumerateArray())
            {
                if (part.TryGetProperty(
                        "text",
                        out var text))
                {
                    return text.GetString() ?? "";
                }
            }

            return "";
        }


        private static void AddToolResults(
  List<GeminiContent> contents,
  List<AgentToolResult> results)
        {
            var parts = new List<GeminiPart>();

            foreach (var result in results)
            {
                parts.Add(
                    new GeminiPart
                    {
                        FunctionResponse =
                            new GeminiFunctionResponse
                            {
                                Id = result.Id,

                                Name = result.ToolName,

                                Response =
                                    result.Success
                                        ? new
                                        {
                                            success = true,
                                            result = result.Result
                                        }
                                        : new
                                        {
                                            success = false,
                                            error = result.Error
                                        }
                            }
                    });
            }

            // VERY IMPORTANT
            contents.Add(
                new GeminiContent
                {
                    Role = "user",
                    Parts = parts
                });
        }

    }

}
