using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class TonKhoController : Controller
    {
        private readonly TonKhoService _service;
        private readonly KhoService _khoService;
        private readonly SanPhamService _sanPhamService;

        public TonKhoController(TonKhoService service, KhoService khoService, SanPhamService sanPhamService)
        {
            _service = service;
            _khoService = khoService;
            _sanPhamService = sanPhamService;
        }

        // GET: /Admin/TonKho
        public async Task<IActionResult> Index(string search, int? khoId, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDKHO == id || x.IDSP == id).ToList();
                else
                    items = items.Where(x => x.SOLUONGTON.ToString().Contains(search)).ToList();
            }
            if (khoId.HasValue && khoId.Value > 0)
                items = items.Where(x => x.IDKHO == khoId.Value).ToList();

            var totalCount = items.Count;
            var tonKhos = items.OrderBy(x => x.IDKHO).ThenBy(x => x.IDSP).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.KhoId = khoId;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(tonKhos);
        }

        // GET: /Admin/TonKho/Details/5? (dùng string id vì ObjectId)
        public async Task<IActionResult> Details(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.Kho = await _khoService.GetByIdAsync(item.IDKHO);
            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(item.IDSP);
            return View(item);
        }

        // GET: /Admin/TonKho/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TonKho model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp (cặp kho - sản phẩm)
                var existing = await _service.GetByKhoAndSanPhamAsync(model.IDKHO, model.IDSP);
                if (existing != null)
                {
                    ModelState.AddModelError("", "Tồn kho cho cặp kho - sản phẩm này đã tồn tại.");
                    ViewBag.Khos = await _khoService.GetAllAsync();
                    ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
                    return View(model);
                }
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/TonKho/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, TonKho model)
        {
            if (id != model.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/TonKho/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.Kho = await _khoService.GetByIdAsync(item.IDKHO);
            ViewBag.SanPham = await _sanPhamService.GetByIdAsync(item.IDSP);
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