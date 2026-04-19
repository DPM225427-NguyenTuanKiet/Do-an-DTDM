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

        // GET: /Admin/YeuCauTraHang/Create
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

        // GET: /Admin/YeuCauTraHang/Edit/5
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

        // GET: /Admin/YeuCauTraHang/Delete/5
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