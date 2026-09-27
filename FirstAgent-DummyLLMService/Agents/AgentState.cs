using FirstAgent_DummyLLMService.Agents.Planning;
using FirstAgent_DummyLLMService.Emums;
using FirstAgent_DummyLLMService.Tools;

namespace FirstAgent_DummyLLMService.Agents
{
    public class AgentState
    {
        public Guid RunId { get; set; } = Guid.NewGuid();
        public string Goal { get; set; } = "";
        public AgentStatus Status { get; set; } = AgentStatus.Created;
        public int Iteration { get; set; }
        public int MaxIterations { get; set; } = 10;
        public AgentPlan? Plan { get; set; }
        public List<AgentMessage> Messages { get; set; } = [];
        public List<AgentStep> Steps { get; set; } = [];
        public List<ToolCall> ToolCalls { get; set; } = [];
        public List<ToolExecutionResult> ToolResults { get; set; } = [];
        public int ReplanCount { get; set; }
        public int MaxReplans { get; set; } = 3;

        public string? FinalAnswer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
