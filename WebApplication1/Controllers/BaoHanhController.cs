using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace CarShop.Controllers
{
    [Authorize]
    public class BaoHanhController : Controller
    {
        private readonly BaoHanhService _baoHanhService;
        private readonly KhachHangService _khachHangService;
        private readonly SanPhamService _sanPhamService; // THÊM DỊCH VỤ NÀY

        public BaoHanhController(BaoHanhService baoHanhService, KhachHangService khachHangService, SanPhamService sanPhamService)
        {
            _baoHanhService = baoHanhService;
            _khachHangService = khachHangService;
            _sanPhamService = sanPhamService;
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

            // Lấy danh sách sản phẩm để hiển thị tên xe
            var productIds = list.Select(b => b.IDSP).Distinct().ToList();
            var products = await _sanPhamService.GetAllAsync();
            ViewBag.Products = products.Where(p => productIds.Contains(p.IDSP)).ToDictionary(p => p.IDSP);

            return View(list.OrderByDescending(b => b.NGAYBATDAU).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var idkh = await GetKhachHangId();
            if (idkh == null) return RedirectToAction("Login", "Account");

            var baoHanh = await _baoHanhService.GetByIdAsync(id);
            if (baoHanh == null || baoHanh.IDKH != idkh.Value) return NotFound();

            var sanPham = await _sanPhamService.GetByIdAsync(baoHanh.IDSP);
            ViewBag.SanPham = sanPham;

            return View(baoHanh);
        }
    }
}