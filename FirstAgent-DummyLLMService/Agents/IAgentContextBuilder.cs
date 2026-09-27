namespace FirstAgent_DummyLLMService.Agents
{
    public interface IAgentContextBuilder
    {
        Task<string> BuildAsync(AgentState state,CancellationToken cancellationToken);

    }
}
