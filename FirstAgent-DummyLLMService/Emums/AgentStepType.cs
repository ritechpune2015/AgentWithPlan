namespace FirstAgent_DummyLLMService.Emums
{
    public enum AgentStepType
    {
        LlmDecision,
        ToolCall,
        ToolResult,
        HumanApproval,
        FinalAnswer,
        Error

    }
}
