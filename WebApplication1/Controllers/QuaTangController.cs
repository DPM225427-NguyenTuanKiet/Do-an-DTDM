using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Services;

namespace CarShop.Controllers
{
    public class QuaTangController : Controller
    {
        private readonly QuaTangService _quaTangService;

        public QuaTangController(QuaTangService quaTangService)
        {
            _quaTangService = quaTangService;
        }

        public async Task<IActionResult> Index()
        {
            var gifts = await _quaTangService.GetAllAsync();
            gifts = gifts.Where(g => g.TRANGTHAI && g.SOLUONG > 0).ToList();
            return View(gifts);
        }
    }
}