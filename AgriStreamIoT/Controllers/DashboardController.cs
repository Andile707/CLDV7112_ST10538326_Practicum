using AgriStreamIoT.Data;
using AgriStreamIoT.Models;
using AgriStreamIoT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace AgriStreamIoT.Controllers;

[Authorize]
public class DashboardController(AppDbContext db, ITelemetryService telemetry, ILogger<DashboardController> logger) : Controller
{
    public async Task<IActionResult> Index(string fieldId = "field-1", CancellationToken ct = default)
    {
        var model = new DashboardViewModel { Devices = await db.Devices.Where(x => x.FieldId == fieldId).ToListAsync(ct) };
        try { model.Readings = await telemetry.LatestAsync(fieldId, ct); }
        catch (Exception ex) { logger.LogError(ex, "Telemetry unavailable"); model.Error = "Telemetry is temporarily unavailable."; }
        ViewBag.FieldId = fieldId;
        return View(model);
    }
}
