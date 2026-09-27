using FirstAgent_DummyLLMService.Agents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstAgent_DummyLLMService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly AgentOrchestrator _agent;
        public AgentController(AgentOrchestrator agent)
        {
            this._agent = agent;

        }

        [HttpPost("run")]
        public async Task<IActionResult> Run([FromBody] AgentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _agent.RunAsync(request.Goal,
                    cancellationToken);
            return Ok(result);
        }
    }
}
