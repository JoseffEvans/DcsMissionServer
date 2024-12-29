using Microsoft.AspNetCore.Mvc;
using UnitManager;
using System.Text.Json;
using DcsModels;
using DcsTcp;
using SharedUtil;

namespace DcsMissionServer.Controllers {
    public class UnitController(Manager manager, DcsTcpSender sender) : Controller {

        readonly Manager _manager = manager;
        readonly DcsTcpSender _sender = sender;

        public async Task<string> UnitDetails(int id) => 
            JsonSerializer.Serialize(await _manager.GetUnit(id));

        [HttpPost]
        public async Task<IActionResult> CreateGroundUnit([FromBody]GroundGroup group) {
            try {
                Console.Write(JsonSerializer.Serialize(group));
                //await _sender.SendMessage($"CreateGroundGroup\n{JsonSerializer.Serialize(group)}");
                return Ok();
            }catch(Exception ex) {
                return StatusCode(500, $"Error occurred: {ex.CombinedStackTrace()}");
            }
        }
    }
}
