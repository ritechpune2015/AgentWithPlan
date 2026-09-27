namespace FirstAgent_DummyLLMService.Agents
{
    public class AgentMessage
    {
        public string Role { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
}
