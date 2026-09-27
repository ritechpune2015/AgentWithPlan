using FirstAgent_DummyLLMService.LLMs;
using System.Text.Json;

namespace FirstAgent_DummyLLMService.Agents.Planning
{
    public class PlanGenerator
    {
        private readonly GeminiService _gemini;

        public PlanGenerator(GeminiService gemini)
        {
            _gemini = gemini;
        }

    public async Task<AgentPlan> GenerateAsync(AgentState state,
            CancellationToken cancellationToken)
        {
            var prompt = BuildPrompt(state);

            var json = await _gemini.GeneratePlanAsync(prompt,cancellationToken);

            var plan =JsonSerializer.Deserialize<AgentPlan>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (plan == null)
            {
                throw new InvalidOperationException(
                    "Gemini returned an empty plan.");
            }

            return plan;
        }

        private static string BuildPrompt(AgentState state)
        {
            return $$"""
You are an enterprise task planner.

Create a simple executable plan for
the user's goal.

USER GOAL:

{{state.Goal}}

Available tools:

1. get_customer

Description:

Gets customer information.

Parameters:

customerId : string

2. get_customer_balance

Description:

Gets the current customer balance.

Parameters:

customerId : string

3. get_customer_orders

Description:

Gets all orders for a customer.

Parameters:

customerId : string

Rules:

- Use only the tools listed above.
- Do not invent tools.
- Use exact tool names.
- Use exact parameter names.
- Every step must have a unique id.
- Specify dependencies using dependsOn.
- Independent steps may have the same dependency.
- Keep the plan as small as possible.
- Do not add unnecessary steps.

Return only JSON in this format:

{
  "goal": "string",
  "steps": [
    {
      "id": "step1",
      "order": 1,
      "description": "string",
      "toolName": "string",
      "arguments": {
        "customerId": "string"
      },
      "dependsOn": []
    }
  ]
}
""";
        }
    }
}
