using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarShop.Models;
using CarShop.Services;
using CarShop.Extensions;
using System.Security.Claims;

namespace CarShop.Controllers
{
    public class GioHangController : Controller
    {
        private readonly SanPhamService _sanPhamService;
        private readonly DonHangService _donHangService;
        private readonly KhachHangService _khachHangService;
        private readonly VoucherService _voucherService;
        public GioHangController(SanPhamService sanPhamService, DonHangService donHangService, KhachHangService khachHangService, VoucherService voucherService)
        {
            _sanPhamService = sanPhamService;
            _donHangService = donHangService;
            _khachHangService = khachHangService;
            _voucherService = voucherService;
        }

        // Lấy giỏ hàng từ session
        private Cart GetCart()
        {
            return HttpContext.Session.GetObject<Cart>("Cart") ?? new Cart();
        }

        private void SaveCart(Cart cart)
        {
            HttpContext.Session.SetObject("Cart", cart);
        }

        // Xem giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Thêm vào giỏ
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await _sanPhamService.GetByIdAsync(id);
            if (product == null || !product.TRANGTHAI)
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });

            var cart = GetCart();
            cart.AddItem(new CartItem
            {
                ProductId = product.IDSP,
                ProductName = product.TENSP,
                Price = product.GIA,
                Quantity = quantity
            });
            SaveCart(cart);
            return Json(new { success = true, cartCount = cart.TotalQuantity });
        }

        // Cập nhật số lượng (trả về JSON)
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
                return Json(new { success = false, message = "Số lượng không hợp lệ" });

            var cart = GetCart();
            cart.UpdateQuantity(productId, quantity);
            SaveCart(cart);
            return Json(new { success = true, cartCount = cart.TotalQuantity, totalAmount = cart.TotalAmount });
        }

        // Xóa sản phẩm khỏi giỏ (trả về JSON)
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.RemoveItem(productId);
            SaveCart(cart);
            return Json(new { success = true, cartCount = cart.TotalQuantity, totalAmount = cart.TotalAmount });
        }

        // Xóa toàn bộ giỏ (trả về JSON)
        [HttpPost]
        public IActionResult ClearCart()
        {
            SaveCart(new Cart());
            return Json(new { success = true, cartCount = 0, totalAmount = 0 });
        }
        // GET: Hiển thị trang thanh toán
        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index", "GioHang");
            }
            return View(cart);
        }

        // POST: Xử lý đặt hàng
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string diaChiGiao, string ghiChu)
        {
            var cart = GetCart();
            if (cart == null || !cart.Items.Any())
                return RedirectToAction("Index", "GioHang");

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return RedirectToAction("Login", "Account");

            var khachHang = await _khachHangService.GetByEmailAsync(userEmail);
            if (khachHang == null)
                return RedirectToAction("Login", "Account");

            var lastOrder = await _donHangService.GetLastOrderAsync();
            int newId = (lastOrder?.IDDH ?? 0) + 1;

            var donHang = new DonHang
            {
                IDDH = newId,
                IDKH = khachHang.IDKH,
                NGAYDAT = DateTime.Now,
                TONGTIEN = cart.TotalAmount,
                TRANGTHAI = "Chờ duyệt",
                DIACHIGIAO = diaChiGiao,
                CHITIET = cart.Items.Select(item => new ChiTietDonHang
                {
                    IDSP = item.ProductId,
                    SOLUONG = item.Quantity,
                    DONGIA = item.Price
                }).ToList()
            };

            await _donHangService.CreateAsync(donHang);
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("OrderSuccess", new { orderId = donHang.IDDH });
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyVoucher(string code)
        {
            var cart = GetCart();
            if (cart == null || !cart.Items.Any())
                return Json(new { success = false, message = "Giỏ hàng trống" });

            // Tìm voucher theo mã (còn hiệu lực, còn số lượng)
            var voucher = await _voucherService.GetByCodeAsync(code);
            if (voucher == null)
                return Json(new { success = false, message = "Mã giảm giá không tồn tại" });
            if (!voucher.TRANGTHAI || voucher.SOLUONG <= 0)
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn hoặc không khả dụng" });
            if (voucher.NGAYKETTHUC < DateTime.Now)
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn" });
            if (voucher.SOTIEN_TOITHIEU.HasValue && cart.TotalAmount < voucher.SOTIEN_TOITHIEU.Value)
                return Json(new { success = false, message = $"Đơn hàng tối thiểu {voucher.SOTIEN_TOITHIEU.Value:N0}đ để áp dụng" });

            decimal discount = 0;
            if (voucher.LOAI == "Tiền")
                discount = voucher.GIATRI;
            else if (voucher.LOAI == "Phần trăm")
                discount = cart.TotalAmount * voucher.GIATRI / 100;
            if (discount > cart.TotalAmount) discount = cart.TotalAmount;

            // Lưu voucher vào session để dùng trong PlaceOrder
            HttpContext.Session.SetInt32("AppliedVoucherId", voucher.IDV);
            HttpContext.Session.SetDecimal("AppliedDiscount", discount);

            return Json(new { success = true, message = "Áp dụng thành công!", voucherId = voucher.IDV, discount = discount });
        }
        public IActionResult OrderSuccess(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}