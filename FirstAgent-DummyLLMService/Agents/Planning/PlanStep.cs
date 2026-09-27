namespace FirstAgent_DummyLLMService.Agents.Planning
{
    public class PlanStep
    {
        public string Id { get; set; } = "";

        public int Order { get; set; }

        public string Description { get; set; } = "";

        public string? ToolName { get; set; }

        public Dictionary<string, object?> Arguments
        { get; set; } = [];

        public List<string> DependsOn { get; set; }
            = [];

        public PlanStepStatus Status { get; set; }
            = PlanStepStatus.Pending;

        public object? Result { get; set; }

        public string? Error { get; set; }

    }
}
