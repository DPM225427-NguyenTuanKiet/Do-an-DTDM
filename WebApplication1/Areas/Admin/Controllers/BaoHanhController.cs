using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class BaoHanhController : Controller
    {
        private readonly BaoHanhService _baoHanhService;
        private readonly SanPhamService _sanPhamService;
        private readonly KhachHangService _khachHangService;
        private readonly DonHangService _donHangService;

        public BaoHanhController(BaoHanhService baoHanhService, SanPhamService sanPhamService, KhachHangService khachHangService, DonHangService donHangService)
        {
            _baoHanhService = baoHanhService;
            _sanPhamService = sanPhamService;
            _khachHangService = khachHangService;
            _donHangService = donHangService;
        }
        public async Task<IActionResult> Index(string search, string trangThai, int page = 1, int pageSize = 10)
        {
            var items = await _baoHanhService.GetAllAsync();

            // Tìm kiếm theo IDSP, IDKH, IDDH
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDSP == id || x.IDKH == id || x.IDDH == id).ToList();
                else
                    items = items.Where(x => x.TRANGTHAI != null && x.TRANGTHAI.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(trangThai) && trangThai != "all")
                items = items.Where(x => x.TRANGTHAI == trangThai).ToList();

            var totalCount = items.Count;
            var baoHanhs = items.OrderByDescending(x => x.NGAYBATDAU).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Lấy thông tin sản phẩm, khách hàng, đơn hàng để hiển thị (tối ưu có thể dùng Dictionary)
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();

            ViewBag.Search = search;
            ViewBag.TrangThai = trangThai;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(baoHanhs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var baoHanh = await _baoHanhService.GetByIdAsync(id);
            if (baoHanh == null) return NotFound();

            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(baoHanh.IDSP);
            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(baoHanh.IDKH);
            ViewBag.DonHang = await _donHangService.GetByIdAsync(baoHanh.IDDH);
            return View(baoHanh);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BaoHanh model)
        {
            if (ModelState.IsValid)
            {
                var all = await _baoHanhService.GetAllAsync();
                model.IDBH = all.Any() ? all.Max(x => x.IDBH) + 1 : 1;
                model.LICHSU = new List<LichSuBaoHanh>();
                await _baoHanhService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            return View(model);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var baoHanh = await _baoHanhService.GetByIdAsync(id);
            if (baoHanh == null) return NotFound();

            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            return View(baoHanh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BaoHanh model)
        {
            if (id != model.IDBH) return BadRequest();
            if (ModelState.IsValid)
            {
                var existing = await _baoHanhService.GetByIdAsync(id);
                if (existing == null) return NotFound();
                // Giữ lại lịch sử cũ
                model.LICHSU = existing.LICHSU;
                await _baoHanhService.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            return View(model);
        }
        // GET: /Admin/BaoHanh/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var baoHanh = await _baoHanhService.GetByIdAsync(id);
            if (baoHanh == null) return NotFound();

            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(baoHanh.IDSP);
            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(baoHanh.IDKH);
            ViewBag.DonHang = await _donHangService.GetByIdAsync(baoHanh.IDDH);
            return View(baoHanh);
        }

        // POST: /Admin/BaoHanh/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _baoHanhService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> AddHistory(int id, LichSuBaoHanh history)
        {
            var baoHanh = await _baoHanhService.GetByIdAsync(id);
            if (baoHanh == null) return NotFound();

            var allHistory = baoHanh.LICHSU;
            history.IDLS = allHistory.Any() ? allHistory.Max(x => x.IDLS) + 1 : 1;
            baoHanh.LICHSU.Add(history);
            await _baoHanhService.UpdateAsync(id, baoHanh);
            return RedirectToAction("Details", new { id });
        }

        // Sửa lịch sử bảo hành
        [HttpPost]
        public async Task<IActionResult> EditHistory(int id, int historyId, LichSuBaoHanh updatedHistory)
        {
            var baoHanh = await _baoHanhService.GetByIdAsync(id);
            if (baoHanh == null) return NotFound();

            var history = baoHanh.LICHSU.FirstOrDefault(x => x.IDLS == historyId);
            if (history == null) return NotFound();

            history.NGAYTIEPNHAN = updatedHistory.NGAYTIEPNHAN;
            history.NGAYTRA = updatedHistory.NGAYTRA;
            history.NOIDUNG = updatedHistory.NOIDUNG;
            history.TINHTRANG = updatedHistory.TINHTRANG;
            history.CHIPHI = updatedHistory.CHIPHI;

            await _baoHanhService.UpdateAsync(id, baoHanh);
            return RedirectToAction("Details", new { id });
        }

        // Xóa lịch sử bảo hành
        [HttpPost]
        public async Task<IActionResult> DeleteHistory(int id, int historyId)
        {
            var baoHanh = await _baoHanhService.GetByIdAsync(id);
            if (baoHanh == null) return NotFound();

            var history = baoHanh.LICHSU.FirstOrDefault(x => x.IDLS == historyId);
            if (history != null)
                baoHanh.LICHSU.Remove(history);

            await _baoHanhService.UpdateAsync(id, baoHanh);
            return RedirectToAction("Details", new { id });
        }
    }
}