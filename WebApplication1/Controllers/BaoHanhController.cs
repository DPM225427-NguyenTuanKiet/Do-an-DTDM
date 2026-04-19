using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarShop.Controllers
{
    [Authorize]
    public class BaoHanhController : Controller
    {
        private readonly BaoHanhService _baoHanhService;
        private readonly KhachHangService _khachHangService;

        public BaoHanhController(BaoHanhService baoHanhService, KhachHangService khachHangService)
        {
            _baoHanhService = baoHanhService;
            _khachHangService = khachHangService;
        }

        private async Task<int?> GetKhachHangId()
        {
            var userId = User.FindFirstValue("IDTK");
            if (string.IsNullOrEmpty(userId)) return null;
            var kh = await _khachHangService.GetByTaiKhoanIdAsync(int.Parse(userId));
            return kh?.IDKH;
        }

        public async Task<IActionResult> Index()
        {
            var idkh = await GetKhachHangId();
            if (idkh == null) return RedirectToAction("Login", "Account");

            var list = await _baoHanhService.GetByKhachHangIdAsync(idkh.Value);
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var idkh = await GetKhachHangId();
            if (idkh == null) return RedirectToAction("Login", "Account");

            var baoHanh = await _baoHanhService.GetByIdAsync(id);
            if (baoHanh == null || baoHanh.IDKH != idkh) return NotFound();
            return View(baoHanh);
        }
    }
}