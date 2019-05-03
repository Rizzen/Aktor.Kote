using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using OrleansKat.Core;
using OrleansKat.Domain;

namespace OrleansKat.Api.Controllers
{
    [Route("kote")]
    [ApiController]
    public class KoteController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;

        public KoteController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        [Route("{koteId}")]
        public async Task<IActionResult> GetKoteState(string koteId)
        {
            var grain = _clusterClient.GetGrain<IKoteGrain>(koteId);
            var state = await grain.GetState();
            return Ok(state);
        }
        
        [HttpPost]
        [Route("{koteId}/{state}")]
        public async Task<IActionResult> SetKoteState(string koteId, KoteStateEnum state)
        {
            var grain = _clusterClient.GetGrain<IKoteGrain>(koteId);
            await grain.ChangeState(state);
            return Ok();
        }
        
        [HttpPost]
        [Route("{koteId}")]
        public async Task<IActionResult> CreateKote(string koteId)
        {
            var grain = _clusterClient.GetGrain<IKoteGrain>(koteId);
            await grain.Initialize();
            return Ok();
        }
    }
}