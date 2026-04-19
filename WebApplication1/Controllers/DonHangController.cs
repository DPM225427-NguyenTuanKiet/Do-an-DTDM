using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using CarShop.Extensions;
using System.Security.Claims;

namespace CarShop.Controllers
{
    [Authorize]
    public class DonHangController : Controller
    {
        private readonly DonHangService _donHangService;
        private readonly KhachHangService _khachHangService;
        private readonly SanPhamService _sanPhamService;

        public DonHangController(DonHangService donHangService, KhachHangService khachHangService, SanPhamService sanPhamService)
        {
            _donHangService = donHangService;
            _khachHangService = khachHangService;
            _sanPhamService = sanPhamService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue("IDTK"));
        }

        private async Task<int?> GetKhachHangId()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return null;
            var kh = await _khachHangService.GetByEmailAsync(userEmail);
            return kh?.IDKH;
        }

        public async Task<IActionResult> Index()
        {
            var idkh = await GetKhachHangId();
            if (idkh == null) return RedirectToAction("Login", "Account");

            var allOrders = await _donHangService.GetByKhachHangIdAsync(idkh.Value);
            var orders = allOrders.Where(o => o.DA_DUYET == true).OrderByDescending(o => o.NGAYDAT).ToList();
            return View(orders);
        }

        // Trang thanh toán
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObject<Cart>("Cart");
            if (cart == null || !cart.Items.Any())
                return RedirectToAction("Index", "GioHang");

            var khachHangId = await GetKhachHangId();
            if (khachHangId == null || khachHangId.Value == 0)
                return RedirectToAction("Profile", "Account");

            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(khachHangId.Value);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string shippingAddress, string note)
        {
            var cart = HttpContext.Session.GetObject<Cart>("Cart");
            if (cart == null || !cart.Items.Any())
                return RedirectToAction("Index", "GioHang");

            var khachHangId = await GetKhachHangId();
            if (khachHangId == null || khachHangId.Value == 0)
                return RedirectToAction("Profile", "Account");

            var order = new DonHang
            {
                IDDH = new Random().Next(1000, 9999),
                IDKH = khachHangId.Value,
                NGAYDAT = DateTime.Now,
                TONGTIEN = cart.TotalAmount,
                TRANGTHAI = "Chờ xử lý",
                DIACHIGIAO = shippingAddress,
                CHITIET = cart.Items.Select(i => new ChiTietDonHang
                {
                    IDSP = i.ProductId,
                    SOLUONG = i.Quantity,
                    DONGIA = i.Price
                }).ToList()
            };

            await _donHangService.CreateAsync(order);
            HttpContext.Session.Remove("Cart");
            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("History");
        }

        // Lịch sử đơn hàng
        public async Task<IActionResult> History()
        {
            var khachHangId = await GetKhachHangId();
            if (khachHangId == null || khachHangId.Value == 0)
                return RedirectToAction("Profile", "Account");

            var orders = await _donHangService.GetByKhachHangIdAsync(khachHangId.Value);
            orders = orders.OrderByDescending(o => o.NGAYDAT).ToList();
            return View(orders);
        }

        // Chi tiết đơn hàng
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _donHangService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            var khachHangId = await GetKhachHangId();
            if (khachHangId == null || order.IDKH != khachHangId.Value)
                return Forbid();

            var productIds = order.CHITIET.Select(c => c.IDSP).Distinct();
            var products = await _sanPhamService.GetAllAsync();
            ViewBag.Products = products.Where(p => productIds.Contains(p.IDSP)).ToDictionary(p => p.IDSP);

            return View(order);
        }
    }
}