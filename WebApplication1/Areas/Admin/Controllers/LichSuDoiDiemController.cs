using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class LichSuDoiDiemController : Controller
    {
        private readonly LichSuDoiDiemService _service;
        private readonly KhachHangService _khachHangService;
        private readonly VoucherService _voucherService;
        private readonly QuaTangService _quaTangService;
        private readonly DonHangService _donHangService;

        public LichSuDoiDiemController(LichSuDoiDiemService service,
                                        KhachHangService khachHangService,
                                        VoucherService voucherService,
                                        QuaTangService quaTangService,
                                        DonHangService donHangService)
        {
            _service = service;
            _khachHangService = khachHangService;
            _voucherService = voucherService;
            _quaTangService = quaTangService;
            _donHangService = donHangService;
        }

        // GET: /Admin/LichSuDoiDiem
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.ID == id || x.IDKH == id || x.IDDONHANG == id || x.IDV == id || x.IDQT == id).ToList();
                else
                    items = items.Where(x => x.TRANGTHAI.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var totalCount = items.Count;
            var histories = items.OrderByDescending(x => x.NGAYDOI).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.Vouchers = await _voucherService.GetAllAsync();
            ViewBag.QuaTangs = await _quaTangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(histories);
        }

        // GET: /Admin/LichSuDoiDiem/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(item.IDKH);
            ViewBag.Voucher = item.IDV.HasValue ? await _voucherService.GetByIdAsync(item.IDV.Value) : null;
            ViewBag.QuaTang = item.IDQT.HasValue ? await _quaTangService.GetByIdAsync(item.IDQT.Value) : null;
            ViewBag.DonHang = item.IDDONHANG.HasValue ? await _donHangService.GetByIdAsync(item.IDDONHANG.Value) : null;
            return View(item);
        }

        // GET: /Admin/LichSuDoiDiem/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.Vouchers = await _voucherService.GetAllAsync();
            ViewBag.QuaTangs = await _quaTangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LichSuDoiDiem model)
        {
            if (ModelState.IsValid)
            {
                var all = await _service.GetAllAsync();
                model.ID = all.Any() ? all.Max(x => x.ID) + 1 : 1;
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.Vouchers = await _voucherService.GetAllAsync();
            ViewBag.QuaTangs = await _quaTangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/LichSuDoiDiem/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.Vouchers = await _voucherService.GetAllAsync();
            ViewBag.QuaTangs = await _quaTangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LichSuDoiDiem model)
        {
            if (id != model.ID) return BadRequest();
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.KhachHangs = await _khachHangService.GetAllAsync();
            ViewBag.Vouchers = await _voucherService.GetAllAsync();
            ViewBag.QuaTangs = await _quaTangService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/LichSuDoiDiem/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.KhachHang = await _khachHangService.GetByIdAsync(item.IDKH);
            ViewBag.Voucher = item.IDV.HasValue ? await _voucherService.GetByIdAsync(item.IDV.Value) : null;
            ViewBag.QuaTang = item.IDQT.HasValue ? await _quaTangService.GetByIdAsync(item.IDQT.Value) : null;
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