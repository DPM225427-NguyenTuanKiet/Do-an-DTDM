using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class YeuCauTraHangController : Controller
    {
        private readonly YeuCauTraHangService _service;
        private readonly DonHangService _donHangService;
        private readonly KhachHangService _khachHangService;
        private readonly SanPhamService _sanPhamService;

        public YeuCauTraHangController(YeuCauTraHangService service, DonHangService donHangService, KhachHangService khachHangService, SanPhamService sanPhamService)
        {
            _service = service;
            _donHangService = donHangService;
            _khachHangService = khachHangService;
            _sanPhamService = sanPhamService;
        }

        // GET: /Admin/YeuCauTraHang
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDYCTH == id || x.IDDH == id || x.IDKH == id).ToList();
                else
                    items = items.Where(x => (x.TRANGTHAI != null && x.TRANGTHAI.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                             (x.LYDO != null && x.LYDO.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            var totalCount = items.Count;
            // Sắp xếp yêu cầu mới nhất lên đầu
            var yeuCaus = items.OrderByDescending(x => x.NGAYYEUCAU).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(yeuCaus);
        }

        // GET: /Admin/YeuCauTraHang/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.DonHang = await _donHangService.GetByIdAsync(item.IDDH);
            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(item.IDKH);
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(item);
        }

        // =========================================================================
        // THÊM MỚI: HÀM XỬ LÝ CẬP NHẬT TRẠNG THÁI TỪ VIEW DETAILS
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var request = await _service.GetByIdAsync(id);
            if (request == null) return NotFound();

            request.TRANGTHAI = status;
            await _service.UpdateAsync(id, request);

            // Bật thông báo thành công (Bạn có thể hiển thị nó ngoài file Layout)
            TempData["Success"] = "Đã cập nhật trạng thái yêu cầu trả hàng!";

            // Quay lại trang danh sách sau khi duyệt xong
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/YeuCauTraHang/Create (Giữ nguyên của bạn)
        public async Task<IActionResult> Create()
        {
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(YeuCauTraHang model, List<int> idsp, List<int> soluong, List<decimal> dongia, List<decimal> tienhoan)
        {
            if (ModelState.IsValid)
            {
                var all = await _service.GetAllAsync();
                model.IDYCTH = all.Any() ? all.Max(x => x.IDYCTH) + 1 : 1;
                model.NGAYYEUCAU = DateTime.Now;
                model.CHITIET = new List<ChiTietTraHang>();
                if (idsp != null && idsp.Count > 0)
                {
                    for (int i = 0; i < idsp.Count; i++)
                    {
                        model.CHITIET.Add(new ChiTietTraHang
                        {
                            IDSP = idsp[i],
                            SOLUONG = soluong[i],
                            DONGIA = dongia[i],
                            TIENHOAN = tienhoan[i]
                        });
                    }
                }
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/YeuCauTraHang/Edit/5 (Giữ nguyên của bạn)
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, YeuCauTraHang model, List<int> idsp, List<int> soluong, List<decimal> dongia, List<decimal> tienhoan)
        {
            if (id != model.IDYCTH) return BadRequest();
            if (ModelState.IsValid)
            {
                model.CHITIET = new List<ChiTietTraHang>();
                if (idsp != null && idsp.Count > 0)
                {
                    for (int i = 0; i < idsp.Count; i++)
                    {
                        model.CHITIET.Add(new ChiTietTraHang
                        {
                            IDSP = idsp[i],
                            SOLUONG = soluong[i],
                            DONGIA = dongia[i],
                            TIENHOAN = tienhoan[i]
                        });
                    }
                }
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/YeuCauTraHang/Delete/5 (Giữ nguyên của bạn)
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.DonHang = await _donHangService.GetByIdAsync(item.IDDH);
            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(item.IDKH);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}