using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class KhachHangController : Controller
    {
        private readonly KhachHangService _khachHangService;
        private readonly TaiKhoanService _taiKhoanService;
        private readonly LoaiGiaService _loaiGiaService;

        public KhachHangController(KhachHangService khachHangService, TaiKhoanService taiKhoanService, LoaiGiaService loaiGiaService)
        {
            _khachHangService = khachHangService;
            _taiKhoanService = taiKhoanService;
            _loaiGiaService = loaiGiaService;
        }

        // GET: /Admin/KhachHang
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _khachHangService.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(x => x.HOTEN.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         x.EMAIL.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         x.DIENTHOAI.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         x.DIACHI.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var totalCount = items.Count;
            var khachHangs = items.OrderBy(x => x.IDKH).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.LoaiGias = await _loaiGiaService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(khachHangs);
        }

        // GET: /Admin/KhachHang/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _khachHangService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.TaiKhoan = await _taiKhoanService.GetByIdAsync(item.IDTK ?? 0);
            ViewBag.LoaiGia = await _loaiGiaService.GetByIdAsync(item.IDLG);
            return View(item);
        }

        // GET: /Admin/KhachHang/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.LoaiGias = await _loaiGiaService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang model)
        {
            if (ModelState.IsValid)
            {
                var all = await _khachHangService.GetAllAsync();
                model.IDKH = all.Any() ? all.Max(x => x.IDKH) + 1 : 1;
                await _khachHangService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.LoaiGias = await _loaiGiaService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/KhachHang/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _khachHangService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.LoaiGias = await _loaiGiaService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhachHang model)
        {
            if (id != model.IDKH) return BadRequest();
            if (ModelState.IsValid)
            {
                await _khachHangService.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TaiKhoans = await _taiKhoanService.GetAllAsync();
            ViewBag.LoaiGias = await _loaiGiaService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/KhachHang/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _khachHangService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.TaiKhoan = await _taiKhoanService.GetByIdAsync(item.IDTK ?? 0);
            ViewBag.LoaiGia = await _loaiGiaService.GetByIdAsync(item.IDLG);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _khachHangService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}