using FirstAgent_DummyLLMService.Agents;
using FirstAgent_DummyLLMService.Agents.Planning;
using FirstAgent_DummyLLMService.Gemini;
using FirstAgent_DummyLLMService.LLMs;
using FirstAgent_DummyLLMService.MCP;
using FirstAgent_DummyLLMService.Options;
using FirstAgent_DummyLLMService.Tools;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<McpServerOptions>(builder.Configuration.GetSection("MCP"));
builder.Services.Configure<GeminiOptions>(
    builder.Configuration
        .GetSection("Gemini"));

builder.Services.AddHttpClient<GeminiService>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                builder.Configuration[
                    "Gemini:BaseUrl"]!);
    });

builder.Services.AddScoped<GeminiMcpAgent>();

builder.Services.AddScoped<
    IToolExecutor,
    MCPToolExecutor>();

builder.Services.AddScoped<
    IMcpClientService,
    McpClientService>();


builder.Services.AddScoped<GeminiToolBuilder>();

builder.Services.AddScoped<ParallelToolExecutor>();
builder.Services.AddScoped<GeminiMcpAgent>();


builder.Services.AddScoped<PlanGenerator>();

builder.Services.AddScoped<PlanValidator>();

builder.Services.AddScoped<PlanExecutor>();

builder.Services.AddScoped<AgentOrchestrator>();


// -----------------------------
// Tools
// -----------------------------

//builder.Services.AddScoped<IToolExecutor,ToolExecutor>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
