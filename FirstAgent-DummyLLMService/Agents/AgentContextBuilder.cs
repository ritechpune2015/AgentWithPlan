using System.Text;

namespace FirstAgent_DummyLLMService.Agents
{
    public class AgentContextBuilder : IAgentContextBuilder
    {
        //public Task<string> BuildAsync(AgentState state, CancellationToken cancellationToken)
        //{
        //    var context = new StringBuilder();

        //    context.AppendLine("AGENT GOAL:");
        //    context.AppendLine(state.Goal);

        //    context.AppendLine();

        //    context.AppendLine("CURRENT ITERATION:");
        //    context.AppendLine(
        //        state.Iteration.ToString());

        //    context.AppendLine();

        //    context.AppendLine("CONVERSATION:");

        //    foreach (var message in state.Messages)
        //    {
        //        context.AppendLine(
        //            $"{message.Role}: {message.Content}");
        //    }

        //    context.AppendLine();

        //    context.AppendLine("TOOL RESULTS:");

        //    foreach (var result in state.ToolResults)
        //    {
        //        context.AppendLine(
        //            $"Tool: {result.ToolName}");

        //        context.AppendLine(
        //            $"Success: {result.Success}");

        //        context.AppendLine(
        //            $"Result: {result.Result}");

        //        if (!string.IsNullOrWhiteSpace(result.Error))
        //        {
        //            context.AppendLine(
        //                $"Error: {result.Error}");
        //        }
        //    }

        //    return Task.FromResult(
        //        context.ToString());
        //}


        public Task<string> BuildAsync(AgentState state,
     CancellationToken cancellationToken)
        {
            var builder = new StringBuilder();

            builder.AppendLine(
                "You are an enterprise customer service agent.");

            builder.AppendLine();

            builder.AppendLine("GOAL:");
            builder.AppendLine(state.Goal);

            builder.AppendLine();

            builder.AppendLine(
                $"CURRENT ITERATION: {state.Iteration}");

            builder.AppendLine();

            builder.AppendLine("PREVIOUS TOOL RESULTS:");

            if (state.ToolResults.Count == 0)
            {
                builder.AppendLine("None");
            }
            else
            {
                foreach (var result   in state.ToolResults)
                {
                    builder.AppendLine(
                        $"Tool: {result.ToolName}");

                    builder.AppendLine(
                        $"Success: {result.Success}");

                    builder.AppendLine(
                        $"Result: {result.Result.Content.FirstOrDefault().ToString()}");

                    if (!string.IsNullOrWhiteSpace(
                        result.Error))
                    {
                        builder.AppendLine(
                            $"Error: {result.Error}");
                    }

                    builder.AppendLine();
                }
            }

            builder.AppendLine(
                "If more information is required, " +
                "request the appropriate tool.");

            builder.AppendLine(
                "If enough information is available, " +
                "return the final answer.");

            return Task.FromResult(
                builder.ToString());
        }


    }
}

