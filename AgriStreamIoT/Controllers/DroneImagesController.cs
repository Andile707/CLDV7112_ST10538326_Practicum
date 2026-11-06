using AgriStreamIoT.Models;
using AgriStreamIoT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace AgriStreamIoT.Controllers;

[Authorize(Roles = "Admin,Agronomist")]
public class DroneImagesController(IBlobStorageService storage, ILogger<DroneImagesController> logger) : Controller
{
    [HttpGet] public IActionResult Index() => View(new DroneImageModel());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(DroneImageModel model, CancellationToken ct)
    {
        if (model.Image == null) ModelState.AddModelError(nameof(model.Image), "Select an image.");
        if (string.IsNullOrWhiteSpace(model.FieldId)) ModelState.AddModelError(nameof(model.FieldId), "Field ID is required.");
        if (!ModelState.IsValid) return View(model);
        try { var name = await storage.UploadAsync(model.Image!, model.FieldId, ct); TempData["Message"] = $"Uploaded {name}."; return RedirectToAction(nameof(Index)); }
        catch (ArgumentException ex) { ModelState.AddModelError("", ex.Message); }
        catch (Exception ex) { logger.LogError(ex, "Upload failed"); ModelState.AddModelError("", "Upload failed."); }
        return View(model);
    }
}
