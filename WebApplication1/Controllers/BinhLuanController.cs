using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using System.Security.Claims;

namespace CarShop.Controllers
{
    [Authorize]
    public class BinhLuanController : Controller
    {
        private readonly BinhLuanService _binhLuanService;
        private readonly SanPhamService _sanPhamService;
        private readonly DonHangService _donHangService;
        private readonly KhachHangService _khachHangService;

        public BinhLuanController(BinhLuanService binhLuanService, SanPhamService sanPhamService, DonHangService donHangService, KhachHangService khachHangService)
        {
            _binhLuanService = binhLuanService;
            _sanPhamService = sanPhamService;
            _donHangService = donHangService;
            _khachHangService = khachHangService;
        }

        private async Task<int> GetKhachHangId()
        {
            var userId = int.Parse(User.FindFirstValue("IDTK"));
            var kh = await _khachHangService.GetByTaiKhoanIdAsync(userId);
            return kh?.IDKH ?? 0;
        }

        // Hiển thị form bình luận cho sản phẩm (có thể gọi bằng partial view trong ProductDetail)
        public async Task<IActionResult> Create(int productId)
        {
            var khachHangId = await GetKhachHangId();
            // Kiểm tra xem khách hàng đã mua sản phẩm này chưa (có đơn hàng đã giao)
            var orders = await _donHangService.GetByKhachHangIdAsync(khachHangId);
            var hasPurchased = orders.Any(o => o.TRANGTHAI == "Đã giao" && o.CHITIET.Any(c => c.IDSP == productId));
            if (!hasPurchased)
                return Forbid("Bạn chỉ có thể bình luận sau khi đã mua sản phẩm.");

            ViewBag.ProductId = productId;
            return PartialView("_CreateComment");
        }

        [HttpPost]
        public async Task<IActionResult> Create(int productId, int rating, string content)
        {
            var khachHangId = await GetKhachHangId();
            var taiKhoanId = int.Parse(User.FindFirstValue("IDTK"));
            var all = await _binhLuanService.GetAllAsync();
            var newId = all.Any() ? all.Max(b => b.IDBL) + 1 : 1;

            var comment = new BinhLuan
            {
                IDBL = newId,
                IDSP = productId,
                IDTK = taiKhoanId,
                NOIDUNG = content,
                NGAYTAO = DateTime.Now,
                SOSAO = rating
            };
            await _binhLuanService.CreateAsync(comment);

            TempData["Success"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
            return RedirectToAction("ProductDetail", "Home", new { id = productId });
        }

        // Lấy danh sách bình luận cho sản phẩm (dùng partial view)
        public async Task<IActionResult> GetComments(int productId, int page = 1, int pageSize = 5)
        {
            var allComments = await _binhLuanService.GetBySanPhamIdAsync(productId);
            var comments = allComments.OrderByDescending(c => c.NGAYTAO).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var total = allComments.Count;
            ViewBag.TotalPages = (int)Math.Ceiling((double)total / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.ProductId = productId;
            return PartialView("_CommentsList", comments);
        }
    }
}