using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using System.Security.Claims;

namespace CarShop.Controllers
{
    [Authorize]
    public class YeuThichController : Controller
    {
        private readonly YeuThichService _yeuThichService;
        private readonly SanPhamService _sanPhamService;
        private readonly KhachHangService _khachHangService;
        private readonly HinhAnhService _hinhAnhService;
        public YeuThichController(YeuThichService yeuThichService, SanPhamService sanPhamService, KhachHangService khachHangService, HinhAnhService hinhAnhService)
        {
            _yeuThichService = yeuThichService;
            _sanPhamService = sanPhamService;
            _khachHangService = khachHangService;
            _hinhAnhService = hinhAnhService;
        }

        private async Task<int> GetKhachHangId()
        {
            var userId = int.Parse(User.FindFirstValue("IDTK"));
            var kh = await _khachHangService.GetByTaiKhoanIdAsync(userId);
            return kh?.IDKH ?? 0;
        }

        // Danh sách yêu thích
        public async Task<IActionResult> Index()
        {
            var khachHangId = await GetKhachHangId();
            if (khachHangId == 0)
                return RedirectToAction("Profile", "Account");

            var favorites = await _yeuThichService.GetByKhachHangIdAsync(khachHangId);
            var productIds = favorites.Select(f => f.IDSP).Distinct();
            var allProducts = await _sanPhamService.GetAllAsync();
            var favoriteProducts = allProducts.Where(p => productIds.Contains(p.IDSP) && p.TRANGTHAI).ToList();

            // Lấy hình ảnh cho sản phẩm
            var allImages = await _hinhAnhService.GetAllAsync();
            ViewBag.Images = allImages.GroupBy(i => i.IDSP).ToDictionary(g => g.Key, g => g.FirstOrDefault()?.DUONGDAN ?? "/images/no-car.png");

            return View(favoriteProducts);
        }

        // Thêm vào yêu thích (AJAX)
        [HttpPost]
        public async Task<IActionResult> AddToFavorite(int productId)
        {
            var khachHangId = await GetKhachHangId();
            if (khachHangId == 0)
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            if (await _yeuThichService.IsFavouriteAsync(khachHangId, productId))
                return Json(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích" });

            // Tạo mới yêu thích...
            return Json(new { success = true, message = "Đã thêm vào yêu thích" });
        }

        // Xóa khỏi yêu thích (AJAX)
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorite(int productId)
        {
            var khachHangId = await GetKhachHangId();
            if (khachHangId == 0)
                return Json(new { success = false, message = "Không tìm thấy khách hàng" });

            await _yeuThichService.DeleteByKhachHangAndSanPhamAsync(khachHangId, productId);
            return Json(new { success = true, message = "Đã xóa khỏi danh sách yêu thích" });
        }
    }
}