using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class DiemTichLuyController : Controller
    {
        private readonly DiemTichLuyService _diemService;
        private readonly KhachHangService _khachHangService;

        public DiemTichLuyController(DiemTichLuyService diemService, KhachHangService khachHangService)
        {
            _diemService = diemService;
            _khachHangService = khachHangService;
        }

        // GET: /Admin/DiemTichLuy
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _diemService.GetAllAsync();
            var khachHangs = await _khachHangService.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDKH == id).ToList();
                else
                {
                    var matchingKh = khachHangs.Where(k => k.HOTEN.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                                           k.EMAIL.Contains(search, StringComparison.OrdinalIgnoreCase))
                                                .Select(k => k.IDKH).ToList();
                    items = items.Where(x => matchingKh.Contains(x.IDKH)).ToList();
                }
            }

            // Join với khách hàng để hiển thị tên
            var viewModel = items.Select(d => new DiemTichLuyViewModel
            {
                Diem = d,
                KhachHang = khachHangs.FirstOrDefault(k => k.IDKH == d.IDKH)
            }).OrderBy(x => x.KhachHang?.HOTEN).ToList();

            var totalCount = viewModel.Count;
            var paged = viewModel.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(paged);
        }

        // GET: /Admin/DiemTichLuy/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var diem = await _diemService.GetByKhachHangIdAsync(id);
            if (diem == null) return NotFound();

            var khachHang = await _khachHangService.GetByIdAsync(id);
            ViewBag.KhachHang = khachHang;
            return View(diem);
        }

        // GET: /Admin/DiemTichLuy/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var diem = await _diemService.GetByKhachHangIdAsync(id);
            if (diem == null) return NotFound();

            var khachHang = await _khachHangService.GetByIdAsync(id);
            ViewBag.KhachHang = khachHang;
            return View(diem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiemTichLuy model)
        {
            if (id != model.IDKH) return BadRequest();
            if (ModelState.IsValid)
            {
                model.NGAYCAPNHAT = DateTime.Now;
                await _diemService.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            var khachHang = await _khachHangService.GetByIdAsync(id);
            ViewBag.KhachHang = khachHang;
            return View(model);
        }

        // GET: /Admin/DiemTichLuy/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var diem = await _diemService.GetByKhachHangIdAsync(id);
            if (diem == null) return NotFound();

            var khachHang = await _khachHangService.GetByIdAsync(id);
            ViewBag.KhachHang = khachHang;
            return View(diem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _diemService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

    public class DiemTichLuyViewModel
    {
        public DiemTichLuy Diem { get; set; }
        public KhachHang KhachHang { get; set; }
    }
}