using FirstAgent_DummyLLMService.Agents.Planning;
using FirstAgent_DummyLLMService.Emums;
using FirstAgent_DummyLLMService.Gemini;
using FirstAgent_DummyLLMService.LLMs;
using FirstAgent_DummyLLMService.Tools;
using System.Text;
using System.Text.Json;

namespace FirstAgent_DummyLLMService.Agents
{
    public class AgentOrchestrator
    {
        private readonly PlanGenerator _planGenerator;

        private readonly PlanValidator _planValidator;

        private readonly PlanExecutor _planExecutor;

        private readonly GeminiService _geminiService;
        public AgentOrchestrator(
            PlanGenerator planGenerator,
            PlanValidator planValidator,
            PlanExecutor planExecutor,GeminiService geminiService)
        {
            _planGenerator =
                planGenerator;

            _planValidator =
                planValidator;

            _planExecutor =
                planExecutor;

            _geminiService = geminiService;
        }


private async Task<string> GenerateFinalAnswerAsync(
        AgentState state,
        CancellationToken cancellationToken)
        {
            var prompt = $"""
You are an enterprise customer service agent.

USER GOAL:
{state.Goal}

The following tools have been executed.

TOOL RESULTS:

{BuildToolResults(state)}

Prepare a concise and accurate answer.

Do not invent information.
Use only the tool results.

Return normal text.
""";

            return await _geminiService.GenerateTextAsync(
                prompt,
                cancellationToken);
}

        private static string BuildToolResults(AgentState state)
        {
            var builder = new StringBuilder();

            foreach (var result
                     in state.ToolResults)
            {
                builder.AppendLine(
                    $"Tool: {result.ToolName}");

                builder.AppendLine(
                    $"Success: {result.Success}");

                if (result.Result != null)
                {
                    builder.AppendLine(
                        $"Result: {result.Result}");
                }

                if (!string.IsNullOrWhiteSpace(
                    result.Error))
                {
                    builder.AppendLine(
                        $"Error: {result.Error}");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }


        public async Task<AgentState> RunAsync(string goal, CancellationToken cancellationToken)
        {
            var state = new AgentState
            {
                Goal = goal,
                Status = AgentStatus.Running
            };

            // 1. PLAN

            var plan =
                await _planGenerator.GenerateAsync(
                    state,
                    cancellationToken);

            state.Plan = plan;

            // 2. VALIDATE

            await _planValidator.ValidateAsync(
                plan,
                cancellationToken);

            // 3. EXECUTE

            await _planExecutor.ExecuteAsync(
                plan,
                state,
                cancellationToken);

            if (plan.Status ==
                PlanStatus.Failed)
            {
                state.Status =
                    AgentStatus.Failed;

                return state;
            }

            // 4. FINAL ANSWER

            state.FinalAnswer =
                await GenerateFinalAnswerAsync(
                    state,
                    cancellationToken);

            state.Status =
                AgentStatus.Completed;

            return state;
        }

    }

}