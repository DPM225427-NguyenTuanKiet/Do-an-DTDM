using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using BCrypt.Net;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class TaiKhoanController : Controller
    {
        private readonly TaiKhoanService _service;

        public TaiKhoanController(TaiKhoanService service)
        {
            _service = service;
        }

        // GET: /Admin/TaiKhoan
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(x => x.TENDANGNHAP.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         x.EMAIL.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         x.VAITRO.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var totalCount = items.Count;
            var taiKhoans = items.OrderBy(x => x.IDTK).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(taiKhoans);
        }

        // GET: /Admin/TaiKhoan/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Admin/TaiKhoan/Create
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoan model, string password)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tên đăng nhập hoặc email
                var existing = await _service.GetByUsernameAsync(model.TENDANGNHAP);
                if (existing != null)
                {
                    ModelState.AddModelError("TENDANGNHAP", "Tên đăng nhập đã tồn tại");
                    return View(model);
                }
                existing = await _service.GetByEmailAsync(model.EMAIL);
                if (existing != null)
                {
                    ModelState.AddModelError("EMAIL", "Email đã được sử dụng");
                    return View(model);
                }

                var all = await _service.GetAllAsync();
                model.IDTK = all.Any() ? all.Max(x => x.IDTK) + 1 : 1;
                model.MATKHAU = BCrypt.Net.BCrypt.HashPassword(password);
                model.NGAYTAO = DateTime.Now;
                model.TRANGTHAI = true;
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/TaiKhoan/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiKhoan model, string password)
        {
            if (id != model.IDTK) return BadRequest();
            if (ModelState.IsValid)
            {
                var existing = await _service.GetByIdAsync(id);
                if (existing == null) return NotFound();

                // Kiểm tra trùng tên đăng nhập (trừ chính nó)
                var conflict = await _service.GetByUsernameAsync(model.TENDANGNHAP);
                if (conflict != null && conflict.IDTK != id)
                {
                    ModelState.AddModelError("TENDANGNHAP", "Tên đăng nhập đã tồn tại");
                    return View(model);
                }
                conflict = await _service.GetByEmailAsync(model.EMAIL);
                if (conflict != null && conflict.IDTK != id)
                {
                    ModelState.AddModelError("EMAIL", "Email đã được sử dụng");
                    return View(model);
                }

                // Giữ nguyên mật khẩu nếu không nhập mới
                if (!string.IsNullOrEmpty(password))
                {
                    model.MATKHAU = BCrypt.Net.BCrypt.HashPassword(password);
                }
                else
                {
                    model.MATKHAU = existing.MATKHAU;
                }
                model.NGAYTAO = existing.NGAYTAO;
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/TaiKhoan/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
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