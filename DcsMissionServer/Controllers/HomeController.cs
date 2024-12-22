using DcsMissionServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UnitManager;

namespace DcsMissionServer.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        readonly Manager _unitManager;

        public HomeController(ILogger<HomeController> logger, Manager unitManager) {
            _logger = logger;
            _unitManager = unitManager;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        public async Task<IActionResult> Units() => View(new UnitsViewModel {
            Units = await _unitManager.GetAllUnits()
        });

        public async Task<IActionResult> Map() => View(new UnitsViewModel {
            Units = await _unitManager.GetAllUnits()
        });

        [ResponseCache(Duration = 0,Location = ResponseCacheLocation.None,NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
