using FirstAgent_DummyLLMService.Tools;

namespace FirstAgent_DummyLLMService.Agents.Planning
{
    public class PlanValidator
    {
        private readonly IToolExecutor _toolExecutor;


        public PlanValidator(IToolExecutor toolExecutor)
        {
            _toolExecutor = toolExecutor;
        }

        public async Task ValidateAsync(
    AgentPlan plan,
    CancellationToken cancellationToken)
        {
            if (plan.Steps.Count == 0)
                throw new InvalidOperationException(
                    "Plan contains no steps.");

            ValidateStepIds(plan);

            ValidateDependencies(plan);

            await ValidateToolsAsync(
                plan,
                cancellationToken);

            ValidateCycles(plan);

            plan.Status =
                PlanStatus.Validated;
        }


        private static void ValidateCycles(AgentPlan plan)
        {
            var graph = plan.Steps.ToDictionary(x => x.Id,
                    x => x.DependsOn);

            var visiting =
                new HashSet<string>();

            var visited =
                new HashSet<string>();

            foreach (var step in plan.Steps)
            {
                if (HasCycle(
                    step.Id,
                    graph,
                    visiting,
                    visited))
                {
                    throw new InvalidOperationException(
                        "Plan contains a dependency cycle.");
                }
            }
        }

        private static bool HasCycle(
    string node,
    Dictionary<string, List<string>> graph,
    HashSet<string> visiting,
    HashSet<string> visited)
        {
            if (visiting.Contains(node))
                return true;

            if (visited.Contains(node))
                return false;

            visiting.Add(node);

            foreach (var dependency
                     in graph[node])
            {
                if (HasCycle(
                    dependency,
                    graph,
                    visiting,
                    visited))
                {
                    return true;
                }
            }

            visiting.Remove(node);

            visited.Add(node);

            return false;
        }


        //public void Validate(AgentPlan plan)
        //{
        //    if (plan.Steps.Count == 0)
        //    {
        //        throw new InvalidOperationException(
        //            "Plan contains no steps.");
        //    }

        //    ValidateStepIds(plan);

        //    ValidateDependencies(plan);

        //    ValidateTools(plan);

        //    ValidateCycles(plan);

        //    plan.Status = PlanStatus.Validated;
        //}

        private static void ValidateStepIds(AgentPlan plan)
        {
            var duplicateIds = plan.Steps.GroupBy(x => x.Id)
                    .Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (duplicateIds.Count > 0)
            {
                throw new InvalidOperationException("Duplicate plan step IDs: " +
                    string.Join(", ", duplicateIds));
            }
        }

        private static void ValidateDependencies(AgentPlan plan)
        {
            var stepIds = plan.Steps.Select(x => x.Id) .ToHashSet();

            foreach (var step in plan.Steps)
            {
                foreach (var dependency
                         in step.DependsOn)
                {
                    if (!stepIds.Contains(dependency))
                    {
                        throw new InvalidOperationException(
                            $"Step '{step.Id}' depends on " +
                            $"unknown step '{dependency}'.");
                    }

                    if (dependency == step.Id)
                    {
                        throw new InvalidOperationException(
                            $"Step '{step.Id}' cannot depend " +
                            "on itself.");
                    }
                }
            }
        }

        private async Task ValidateToolsAsync(AgentPlan plan, CancellationToken cancellationToken)
        {
            foreach (var step in plan.Steps)
            {
                if (string.IsNullOrWhiteSpace(
                    step.ToolName))
                {
                    throw new InvalidOperationException(
                        $"Step '{step.Id}' has no tool.");
                }

                var exists =
                    await _toolExecutor.ToolExistsAsync(
                        step.ToolName,
                        cancellationToken);

                if (!exists)
                {
                    throw new InvalidOperationException(
                        $"Unknown tool: {step.ToolName}");
                }
            }
        }


    }
}
