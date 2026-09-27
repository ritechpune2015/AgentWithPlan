using System.Text.Json;

namespace FirstAgent_DummyLLMService.Tools
{
    public interface ITool
    {
        string Name { get; }
        string Description { get; }
        Task<object> ExecuteAsync(JsonElement arguments, CancellationToken cancellationToken);
    }
}
