using FirstAgent_DummyLLMService.Agents;

namespace FirstAgent_DummyLLMService.LLMs
{
    public interface ILLMService
    {
     //   Task<LLMResponse> GenerateAsync(AgentState state,string context,CancellationToken cancellationToken);


        Task<LLMResponse> GenerateWithToolsAsync(AgentState state,
        string context,
        CancellationToken cancellationToken);


    }
}
