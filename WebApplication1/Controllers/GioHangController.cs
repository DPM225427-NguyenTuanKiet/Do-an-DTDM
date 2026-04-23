using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarShop.Models;
using CarShop.Services;
using CarShop.Extensions;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CarShop.Controllers
{
    public class GioHangController : Controller
    {
        private readonly SanPhamService _sanPhamService;
        private readonly DonHangService _donHangService;
        private readonly KhachHangService _khachHangService;
        private readonly VoucherService _voucherService;
        private readonly ThanhToanService _thanhToanService;
        private readonly DiemTichLuyService _diemTichLuyService;
        private readonly LichSuDiemService _lichSuDiemService;
        private readonly BaoHanhService _baoHanhService;
        private readonly TonKhoService _tonKhoService;

        public GioHangController(
            SanPhamService sanPhamService,
            DonHangService donHangService,
            KhachHangService khachHangService,
            VoucherService voucherService,
            ThanhToanService thanhToanService,
            DiemTichLuyService diemTichLuyService,
            LichSuDiemService lichSuDiemService,
            BaoHanhService baoHanhService,
            TonKhoService tonKhoService)
        {
            _sanPhamService = sanPhamService;
            _donHangService = donHangService;
            _khachHangService = khachHangService;
            _voucherService = voucherService;
            _thanhToanService = thanhToanService;
            _diemTichLuyService = diemTichLuyService;
            _lichSuDiemService = lichSuDiemService;
            _baoHanhService = baoHanhService;
            _tonKhoService = tonKhoService;
        }

        private Cart GetCart() => HttpContext.Session.GetObject<Cart>("Cart") ?? new Cart();
        private void SaveCart(Cart cart) => HttpContext.Session.SetObject("Cart", cart);

        public IActionResult Index() => View(GetCart());

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var product = await _sanPhamService.GetByIdAsync(id);
            if (product == null || !product.TRANGTHAI) return Json(new { success = false, message = "Sản phẩm không tồn tại" });

            var cart = GetCart();
            cart.AddItem(new CartItem { ProductId = product.IDSP, ProductName = product.TENSP, Price = product.GIA, Quantity = quantity });
            SaveCart(cart);
            return Json(new { success = true, cartCount = cart.TotalQuantity });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0) return Json(new { success = false, message = "Số lượng không hợp lệ" });
            var cart = GetCart();
            cart.UpdateQuantity(productId, quantity);
            SaveCart(cart);
            return Json(new { success = true, cartCount = cart.TotalQuantity, totalAmount = cart.TotalAmount });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.RemoveItem(productId);
            SaveCart(cart);
            return Json(new { success = true, cartCount = cart.TotalQuantity, totalAmount = cart.TotalAmount });
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            SaveCart(new Cart());
            return Json(new { success = true, cartCount = 0, totalAmount = 0 });
        }

        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart == null || !cart.Items.Any()) return RedirectToAction("Index", "GioHang");
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string diaChiGiao, string ghiChu, string phuongThucThanhToan)
        {
            var cart = GetCart();
            if (cart == null || !cart.Items.Any()) return RedirectToAction("Index", "GioHang");

            // Xác thực khách hàng
            var userIdStr = User.FindFirstValue("IDTK");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            var khachHang = await _khachHangService.GetByTaiKhoanIdAsync(int.Parse(userIdStr));
            if (khachHang == null) return RedirectToAction("Login", "Account");

            decimal discount = HttpContext.Session.GetDecimal("AppliedDiscount") ?? 0;
            decimal finalTotalAmount = cart.TotalAmount - discount;
            if (finalTotalAmount < 0) finalTotalAmount = 0;

            var lastOrder = await _donHangService.GetLastOrderAsync();
            int newIdDh = (lastOrder?.IDDH ?? 0) + 1;

            // 1. Tạo đơn hàng
            var donHang = new DonHang
            {
                IDDH = newIdDh,
                IDKH = khachHang.IDKH,
                NGAYDAT = DateTime.Now,
                TONGTIEN = finalTotalAmount,
                TRANGTHAI = "Chờ xử lý",
                DIACHIGIAO = diaChiGiao,
                CHITIET = cart.Items.Select(item => new ChiTietDonHang
                {
                    IDSP = item.ProductId,
                    SOLUONG = item.Quantity,
                    DONGIA = item.Price
                }).ToList()
            };
            await _donHangService.CreateAsync(donHang);

            // 2. Xử lý Thanh Toán
            var allThanhToan = await _thanhToanService.GetAllAsync();
            int newIdTt = allThanhToan.Any() ? (allThanhToan.Max(x => (int?)x.IDTT) ?? 0) + 1 : 1;

            await _thanhToanService.CreateAsync(new ThanhToan
            {
                IDTT = newIdTt,
                IDDH = donHang.IDDH,
                SOTIEN = finalTotalAmount,
                HINHTHUC = phuongThucThanhToan == "COD" ? "Tiền mặt khi nhận hàng (COD)" : "Chuyển khoản",
                TRANGTHAI = "Chưa thanh toán",
                NGAYTHANHTOAN = null
            });

            // 3. Tích điểm thưởng
            int diemThuong = (int)(finalTotalAmount / 1000000);
            if (diemThuong > 0)
            {
                var diem = await _diemTichLuyService.GetByKhachHangIdAsync(khachHang.IDKH);
                if (diem == null)
                {
                    await _diemTichLuyService.CreateAsync(new DiemTichLuy { IDKH = khachHang.IDKH, DIEMHIENTAI = diemThuong, TONGDIEMDADUNG = 0, NGAYCAPNHAT = DateTime.Now });
                }
                else
                {
                    diem.DIEMHIENTAI += diemThuong;
                    diem.NGAYCAPNHAT = DateTime.Now;
                    await _diemTichLuyService.UpdateAsync(khachHang.IDKH, diem);
                }

                var allLichSu = await _lichSuDiemService.GetAllAsync();
                int newLsId = allLichSu.Any() ? (allLichSu.Max(x => (int?)x.ID) ?? 0) + 1 : 1;

                await _lichSuDiemService.CreateAsync(new LichSuDiem
                {
                    ID = newLsId,
                    IDKH = khachHang.IDKH,
                    IDDH = donHang.IDDH,
                    SODIEM = diemThuong,
                    LOAI = "Cộng",
                    GHICHU = $"Mua xe (Đơn #{donHang.IDDH})",
                    NGAY = DateTime.Now
                });
            }

            var allBaoHanh = await _baoHanhService.GetAllAsync();
            int newIdBh = allBaoHanh.Any() ? (allBaoHanh.Max(x => (int?)x.IDBH) ?? 0) + 1 : 1;

            var allTonKho = await _tonKhoService.GetAllAsync();

            // 4. TRỪ TỒN KHO VÀ KÍCH HOẠT BẢO HÀNH TỰ ĐỘNG
            foreach (var item in cart.Items)
            {
                // Trừ kho ở bảng Sản Phẩm
                var sp = await _sanPhamService.GetByIdAsync(item.ProductId);
                if (sp != null)
                {
                    sp.SOLUONG -= item.Quantity;
                    if (sp.SOLUONG < 0) sp.SOLUONG = 0;
                    await _sanPhamService.UpdateAsync(sp.IDSP, sp);
                }

                // Trừ kho ở bảng Tồn Kho
                int qtyToDeduct = item.Quantity;
                var tks = allTonKho.Where(x => x.IDSP == item.ProductId && x.SOLUONGTON > 0).ToList();
                foreach (var tk in tks)
                {
                    if (qtyToDeduct <= 0) break;
                    if (tk.SOLUONGTON >= qtyToDeduct)
                    {
                        tk.SOLUONGTON -= qtyToDeduct;
                        await _tonKhoService.UpdateAsync(tk.Id, tk);
                        qtyToDeduct = 0;
                    }
                    else
                    {
                        qtyToDeduct -= tk.SOLUONGTON;
                        tk.SOLUONGTON = 0;
                        await _tonKhoService.UpdateAsync(tk.Id, tk);
                    }
                }

                // Kích hoạt bảo hành
                for (int i = 0; i < item.Quantity; i++)
                {
                    var baoHanh = new BaoHanh
                    {
                        IDBH = newIdBh++,
                        IDSP = item.ProductId,
                        IDKH = khachHang.IDKH,
                        IDDH = donHang.IDDH,
                        NGAYBATDAU = DateTime.Now,
                        NGAYKETTHUC = DateTime.Now.AddYears(3),
                        TRANGTHAI = "Đang bảo hành",
                        MOTA = $"Bảo hành điện tử chính hãng. Kích hoạt từ đơn hàng #{donHang.IDDH}"
                    };
                    await _baoHanhService.CreateAsync(baoHanh);
                }
            }

            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("AppliedVoucherId");
            HttpContext.Session.Remove("AppliedDiscount");

            return RedirectToAction("OrderSuccess", new { orderId = donHang.IDDH });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyVoucher(string code)
        {
            var cart = GetCart();
            if (cart == null || !cart.Items.Any()) return Json(new { success = false, message = "Giỏ hàng trống" });

            var voucher = await _voucherService.GetByCodeAsync(code);
            if (voucher == null) return Json(new { success = false, message = "Mã giảm giá không tồn tại" });
            if (!voucher.TRANGTHAI || voucher.SOLUONG <= 0) return Json(new { success = false, message = "Mã giảm giá đã hết hạn hoặc không khả dụng" });
            if (voucher.NGAYKETTHUC < DateTime.Now) return Json(new { success = false, message = "Mã giảm giá đã hết hạn" });
            if (voucher.SOTIEN_TOITHIEU.HasValue && cart.TotalAmount < voucher.SOTIEN_TOITHIEU.Value) return Json(new { success = false, message = $"Đơn hàng tối thiểu {voucher.SOTIEN_TOITHIEU.Value:N0}đ" });

            decimal discount = voucher.LOAI == "Tiền" ? voucher.GIATRI : (cart.TotalAmount * voucher.GIATRI / 100);
            if (discount > cart.TotalAmount) discount = cart.TotalAmount;

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