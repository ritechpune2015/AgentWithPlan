namespace FirstAgent_DummyLLMService.Agents.Planning
{
    public class AgentPlan
    {
        public Guid PlanId { get; set; }  = Guid.NewGuid();

        public string Goal { get; set; } = "";

        public PlanStatus Status { get; set; }  = PlanStatus.Created;

        public List<PlanStep> Steps { get; set; }       = [];

    }
}
