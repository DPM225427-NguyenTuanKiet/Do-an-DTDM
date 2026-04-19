using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class SanPhamKhuyenMaiController : Controller
    {
        private readonly SanPhamKhuyenMaiService _service;
        private readonly SanPhamService _sanPhamService;
        private readonly KhuyenMaiService _khuyenMaiService;

        public SanPhamKhuyenMaiController(SanPhamKhuyenMaiService service, SanPhamService sanPhamService, KhuyenMaiService khuyenMaiService)
        {
            _service = service;
            _sanPhamService = sanPhamService;
            _khuyenMaiService = khuyenMaiService;
        }

        // GET: /Admin/SanPhamKhuyenMai
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDSP == id || x.IDKM == id).ToList();
                else
                    items = items.ToList(); // không tìm theo chuỗi
            }
            var totalCount = items.Count;
            var spkmList = items.OrderBy(x => x.IDSP).ThenBy(x => x.IDKM).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.KhuyenMais = await _khuyenMaiService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(spkmList);
        }

        // GET: /Admin/SanPhamKhuyenMai/Details/5? (dùng string id vì ObjectId)
        public async Task<IActionResult> Details(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(item.IDSP);
            ViewBag.KhuyenMai = await _khuyenMaiService.GetByIdAsync(item.IDKM);
            return View(item);
        }

        // GET: /Admin/SanPhamKhuyenMai/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.KhuyenMais = await _khuyenMaiService.GetAllAsync();
            return View();
        }

        // GET: /Admin/SanPhamKhuyenMai/Create
        public async Task<IActionResult> Create(int? kmId)
        {
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.KhuyenMais = await _khuyenMaiService.GetAllAsync();
            var model = new SanPhamKhuyenMai();
            if (kmId.HasValue)
            {
                model.IDKM = kmId.Value;
            }
            return View(model);
        }

        // GET: /Admin/SanPhamKhuyenMai/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.KhuyenMais = await _khuyenMaiService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SanPhamKhuyenMai model)
        {
            if (id != model.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.KhuyenMais = await _khuyenMaiService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/SanPhamKhuyenMai/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(item.IDSP);
            ViewBag.KhuyenMai = await _khuyenMaiService.GetByIdAsync(item.IDKM);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}