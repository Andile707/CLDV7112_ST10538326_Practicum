using AgriStreamIoT.Models;
using AgriStreamIoT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AgriStreamIoT.Controllers;

[Authorize(Roles = "Admin,Farmer")]
public class IrrigationController(IIrrigationService irrigation, ILogger<IrrigationController> logger) : Controller
{
    [HttpGet] public IActionResult Index() => View(new IrrigationCommandModel());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IrrigationCommandModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        try { await irrigation.TriggerAsync(model, ct); TempData["Message"] = "Irrigation command queued."; return RedirectToAction(nameof(Index)); }
        catch (Exception ex) { logger.LogError(ex, "Irrigation queue request failed"); ModelState.AddModelError("", "Unable to queue irrigation command."); return View(model); }
    }
}
