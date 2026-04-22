using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting; // Thêm thư viện để xác định thư mục lưu ảnh
using Microsoft.AspNetCore.Http;    // Thêm thư viện để nhận file từ HTML Form
using System.IO;
using System;

namespace CarShop.Controllers
{
    [Authorize]
    public class YeuCauTraHangController : Controller
    {
        private readonly YeuCauTraHangService _yeuCauService;
        private readonly DonHangService _donHangService;
        private readonly KhachHangService _khachHangService;
        private readonly SanPhamService _sanPhamService;
        private readonly IWebHostEnvironment _env; // Thêm biến môi trường

        // Tiêm IWebHostEnvironment vào constructor
        public YeuCauTraHangController(YeuCauTraHangService yeuCauService, DonHangService donHangService,
                                        KhachHangService khachHangService, SanPhamService sanPhamService,
                                        IWebHostEnvironment env)
        {
            _yeuCauService = yeuCauService;
            _donHangService = donHangService;
            _khachHangService = khachHangService;
            _sanPhamService = sanPhamService;
            _env = env;
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
            var khachHangId = await GetKhachHangId();
            if (khachHangId == null) return RedirectToAction("Index", "YeuCauTraHang");

            var requests = await _yeuCauService.GetByKhachHangIdAsync(khachHangId.Value);
            return View(requests.OrderByDescending(x => x.NGAYYEUCAU).ToList());
        }

        public async Task<IActionResult> Create()
        {
            var khachHangId = await GetKhachHangId();
            if (khachHangId == null) return RedirectToAction("Create", "YeuCauTraHang");

            var orders = await _donHangService.GetByKhachHangIdAsync(khachHangId.Value);
            orders = orders.Where(o => o.TRANGTHAI == "Đã giao").ToList();
            ViewBag.Orders = orders;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bổ sung thêm tham số IFormFile minhChung để nhận ảnh từ người dùng
        public async Task<IActionResult> Create(int orderId, string reason, List<int> productIds, List<int> quantities, IFormFile minhChung)
        {
            var khachHangId = await GetKhachHangId();
            if (khachHangId == null) return RedirectToAction("Create", "YeuCauTraHang");

            var order = await _donHangService.GetByIdAsync(orderId);
            if (order == null || order.IDKH != khachHangId || order.TRANGTHAI != "Đã giao")
                return BadRequest("Đơn hàng không hợp lệ");

            string imagePath = null;

            // XỬ LÝ LƯU ẢNH VÀO SERVER
            if (minhChung != null && minhChung.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "returns");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + minhChung.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await minhChung.CopyToAsync(fileStream);
                }
                imagePath = "/uploads/returns/" + uniqueFileName;
            }
            else
            {
                // Bắt lỗi nếu khách hàng không chịu tải ảnh lên
                ModelState.AddModelError("", "Vui lòng đính kèm hình ảnh minh chứng tình trạng xe/linh kiện.");
                var orders = await _donHangService.GetByKhachHangIdAsync(khachHangId.Value);
                ViewBag.Orders = orders.Where(o => o.TRANGTHAI == "Đã giao").ToList();
                return View();
            }

            var all = await _yeuCauService.GetAllAsync();
            var newId = all.Any() ? all.Max(r => r.IDYCTH) + 1 : 1;

            var request = new YeuCauTraHang
            {
                IDYCTH = newId,
                IDDH = orderId,
                IDKH = khachHangId.Value,
                NGAYYEUCAU = DateTime.Now,
                LYDO = reason,
                HINHANH_MINHCHUNG = imagePath, // Gán đường dẫn ảnh vào database
                TRANGTHAI = "Đang xử lý",
                CHITIET = new List<ChiTietTraHang>()
            };

            for (int i = 0; i < productIds.Count; i++)
            {
                var detail = order.CHITIET.FirstOrDefault(c => c.IDSP == productIds[i]);
                if (detail != null && quantities[i] > 0 && quantities[i] <= detail.SOLUONG)
                {
                    request.CHITIET.Add(new ChiTietTraHang
                    {
                        IDSP = productIds[i],
                        SOLUONG = quantities[i],
                        DONGIA = detail.DONGIA,
                        TIENHOAN = detail.DONGIA * quantities[i]
                    });
                }
            }

            if (!request.CHITIET.Any())
                return BadRequest("Chưa chọn sản phẩm hợp lệ");

            await _yeuCauService.CreateAsync(request);

            // Chuyển hướng sang trang Success
            return RedirectToAction(nameof(Success), new { id = request.IDYCTH });
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _yeuCauService.GetByIdAsync(id);
            if (request == null) return NotFound();

            var khachHangId = await GetKhachHangId();
            if (khachHangId == null || request.IDKH != khachHangId) return Forbid();

            ViewBag.Order = await _donHangService.GetByIdAsync(request.IDDH);
            var productIds = request.CHITIET.Select(c => c.IDSP).Distinct();
            var products = await _sanPhamService.GetAllAsync();
            ViewBag.Products = products.Where(p => productIds.Contains(p.IDSP)).ToDictionary(p => p.IDSP);

            return View(request);
        }

        // Trang thông báo thành công
        public IActionResult Success(int id)
        {
            ViewBag.RequestId = id;
            return View();
        }
    }
}