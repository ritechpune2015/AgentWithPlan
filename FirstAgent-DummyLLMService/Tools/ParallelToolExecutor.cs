using FirstAgent_DummyLLMService.Gemini;
using FirstAgent_DummyLLMService.MCP;

namespace FirstAgent_DummyLLMService.Tools
{
    public class ParallelToolExecutor
    {
        private readonly IToolExecutor _executor;

        private readonly SemaphoreSlim _semaphore = new(5);


        public ParallelToolExecutor(
            IToolExecutor executor)
        {
            _executor = executor;
        }

        private async Task<AgentToolResult> ExecuteWithLimitAsync(AgentToolCall toolCall,
        CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(
                cancellationToken);

            using var timeoutCts =
    CancellationTokenSource
        .CreateLinkedTokenSource(
            cancellationToken);

            timeoutCts.CancelAfter(
                TimeSpan.FromSeconds(30));


            try
            {
                return await _executor.ExecuteAsync(
                    toolCall,
                    timeoutCts.Token);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<AgentToolResult>>  ExecuteParallelAsync(
                List<AgentToolCall> toolCalls, ToolExecutionMode mode,
                CancellationToken cancellationToken)
        {

            if (mode == ToolExecutionMode.Sequential)
            {
                var results = new List<AgentToolResult>();

                foreach (var toolCall in toolCalls)
                {
                    var result =
                        await _executor.ExecuteAsync(
                            toolCall,
                            cancellationToken);

                    results.Add(result);
                }

                return results;
            }

            var tasks =
                toolCalls.Select(
                    tool => ExecuteWithLimitAsync(tool, cancellationToken));
                        //_executor.ExecuteAsync(
                        //    tool,
                        //    cancellationToken)); ==> without limit 

            return
                (await Task.WhenAll(tasks))
                .ToList();

        }

    }
}
