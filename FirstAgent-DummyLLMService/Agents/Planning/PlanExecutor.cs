using FirstAgent_DummyLLMService.Gemini;
using FirstAgent_DummyLLMService.Tools;
using System.Text.Json;

namespace FirstAgent_DummyLLMService.Agents.Planning
{
    public class PlanExecutor
    {
        private readonly IToolExecutor _toolExecutor;

        public PlanExecutor(IToolExecutor toolExecutor)
        {
            _toolExecutor = toolExecutor;
        }

        public List<PlanStep> GetReadySteps(AgentPlan plan)
        {
            var completedIds =
                plan.Steps
                    .Where(x =>
                        x.Status ==
                        PlanStepStatus.Completed)
                    .Select(x => x.Id)
                    .ToHashSet();

            return plan.Steps
                .Where(x =>
                    x.Status ==
                    PlanStepStatus.Pending)
                .Where(x =>
                    x.DependsOn.All(
                        completedIds.Contains))
                .ToList();
        }


        public async Task ExecuteAsync(AgentPlan plan, AgentState state,
    CancellationToken cancellationToken)
        {
            plan.Status = PlanStatus.Running;

            while (true)
            {
                cancellationToken
                    .ThrowIfCancellationRequested();

                var readySteps =
                    GetReadySteps(plan);

                if (readySteps.Count == 0)
                {
                    if (plan.Steps.All(
                        x =>
                            x.Status ==
                            PlanStepStatus.Completed ||
                            x.Status ==
                            PlanStepStatus.Skipped))
                    {
                        plan.Status =
                            PlanStatus.Completed;

                        return;
                    }

                    if (plan.Steps.Any(
                        x =>
                            x.Status ==
                            PlanStepStatus.Failed))
                    {
                        plan.Status =
                            PlanStatus.Failed;

                        return;
                    }

                    throw new InvalidOperationException(
                        "Plan cannot progress.");
                }

                var tasks =
                    readySteps.Select(
                        step =>
                            ExecuteStepAsync(
                                step,
                                state,
                                cancellationToken));

                await Task.WhenAll(tasks);
            }
        }


        private async Task ExecuteStepAsync(PlanStep step, AgentState state,
    CancellationToken cancellationToken)
        {
            step.Status =
                PlanStepStatus.Running;

            try
            {
                var arguments =
                    JsonSerializer.SerializeToElement(
                        step.Arguments);
               
                var result =
                    await   _toolExecutor.ExecuteAsync(
                        step.ToolName!,
                        arguments,
                        cancellationToken);

                step.Result = result;

                step.Status =
                    PlanStepStatus.Completed;

                state.ToolResults.Add(
                    new ToolExecutionResult
                    {
                        ToolCallId = step.Id,

                        ToolName =
                            step.ToolName!,

                        Success = true,

                        Result = result
                    });
            }
            catch (Exception ex)
            {
                step.Status =
                    PlanStepStatus.Failed;

                step.Error =
                    ex.Message;

                state.ToolResults.Add(
                    new ToolExecutionResult
                    {
                        ToolCallId = step.Id,

                        ToolName =
                            step.ToolName!,

                        Success = false,

                        Error = ex.Message
                    });
            }
        }

    }

}

