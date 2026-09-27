using FirstAgent_DummyLLMService.Emums;

namespace FirstAgent_DummyLLMService.Agents
{
    public class AgentStep
    {
        public int StepNumber { get; set; }
        public AgentStepType Type { get; set; }
        public string Action { get; set; } = "";
        public object? Input { get; set; }
        public object? Observation { get; set; }
        public bool Success { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
}
