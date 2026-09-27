namespace FirstAgent_DummyLLMService.Emums
{
    public enum AgentStatus
    {
        Created,
        Running,
        WaitingForTool,
        WaitingForApproval,
        Completed,
        Failed,
        Cancelled

    }
}
