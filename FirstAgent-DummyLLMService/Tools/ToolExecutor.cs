using System.Text.Json;

namespace FirstAgent_DummyLLMService.Tools
{
    public class ToolExecutor : IToolExecutor
    {
        private readonly IEnumerable<ITool> _tools;

        public ToolExecutor(IEnumerable<ITool> tools)
        {
            _tools = tools;
        }

        public async Task<object> ExecuteAsync(
            string toolName,
            JsonElement arguments,
            CancellationToken cancellationToken)
        {
            var tool = _tools.FirstOrDefault(
                x => x.Name.Equals(
                    toolName,
                    StringComparison.OrdinalIgnoreCase));

            if (tool == null)
            {
                throw new InvalidOperationException(
                    $"Tool '{toolName}' not found.");
            }

            return await tool.ExecuteAsync(
                arguments,
                cancellationToken);
        }

    }
}
