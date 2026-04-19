using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class NhanVienController : Controller
    {
        private readonly NhanVienService _service;
        private readonly TaiKhoanService _taiKhoanService;
        private readonly ChucVuService _chucVuService;

        public NhanVienController(NhanVienService service, TaiKhoanService taiKhoanService, ChucVuService chucVuService)
        {
            _service = service;
            _taiKhoanService = taiKhoanService;
            _chucVuService = chucVuService;
        }

        // GET: /Admin/NhanVien
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(x => (x.HOTEN != null && x.HOTEN.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                         (x.DIENTHOAI != null && x.DIENTHOAI.Contains(search)) ||
                                         (x.CHUCVU != null && x.CHUCVU.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                                         (x.DIACHI != null && x.DIACHI.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            var totalCount = items.Count;
            var nhanViens = items.OrderBy(x => x.IDNV).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.ChucVus = await _chucVuService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(nhanViens);
        }

        // GET: /Admin/NhanVien/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.TaiKhoan = await _taiKhoanService.GetByIdAsync(item.IDTK ?? 0);
            ViewBag.ChucVu = await _chucVuService.GetByIdAsync(item.IDCV ?? 0);
            return View(item);
        }

        // GET: /Admin/NhanVien/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.ChucVus = await _chucVuService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVien model)
        {
            if (ModelState.IsValid)
            {
                var all = await _service.GetAllAsync();
                model.IDNV = all.Any() ? all.Max(x => x.IDNV) + 1 : 1;
                model.TONGLUONG = (model.LUONGCB ?? 0) + (model.GIONGHI ?? 0) * 50000; // ví dụ tính lương
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.ChucVus = await _chucVuService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/NhanVien/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.ChucVus = await _chucVuService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhanVien model)
        {
            if (id != model.IDNV) return BadRequest();
            if (ModelState.IsValid)
            {
                model.TONGLUONG = (model.LUONGCB ?? 0) + (model.GIONGHI ?? 0) * 50000;
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.ChucVus = await _chucVuService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/NhanVien/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.TaiKhoan = await _taiKhoanService.GetByIdAsync(item.IDTK ?? 0);
            ViewBag.ChucVu = await _chucVuService.GetByIdAsync(item.IDCV ?? 0);
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