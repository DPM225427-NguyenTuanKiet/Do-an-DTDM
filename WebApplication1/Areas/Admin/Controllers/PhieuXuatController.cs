using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class PhieuXuatController : Controller
    {
        private readonly PhieuXuatService _service;
        private readonly NhanVienService _nhanVienService;
        private readonly KhoService _khoService;
        private readonly DonHangService _donHangService;
        private readonly SanPhamService _sanPhamService;

        public PhieuXuatController(PhieuXuatService service, NhanVienService nhanVienService, KhoService khoService, DonHangService donHangService, SanPhamService sanPhamService)
        {
            _service = service;
            _nhanVienService = nhanVienService;
            _khoService = khoService;
            _donHangService = donHangService;
            _sanPhamService = sanPhamService;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // GET: /Admin/PhieuXuat
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDPX == id || x.IDNV == id || x.IDKHO == id || x.IDDH == id).ToList();
                else
                    items = items.Where(x => x.LOAIPHIEU.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var totalCount = items.Count;
            var phieuXuats = items.OrderByDescending(x => x.NGAYXUAT).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(phieuXuats);
        }

        // GET: /Admin/PhieuXuat/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.NhanVien = await _nhanVienService.GetByIdAsync(item.IDNV);
            ViewBag.Kho = await _khoService.GetByIdAsync(item.IDKHO);
            ViewBag.DonHang = item.IDDH.HasValue ? await _donHangService.GetByIdAsync(item.IDDH.Value) : null;
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(item);
        }

        // GET: /Admin/PhieuXuat/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuXuat model, List<int> idsp, List<int> soluong)
        {
            if (ModelState.IsValid)
            {
                var all = await _service.GetAllAsync();
                model.IDPX = all.Any() ? all.Max(x => x.IDPX) + 1 : 1;
                model.CHITIET = new List<ChiTietPhieuXuat>();
                if (idsp != null && soluong != null && idsp.Count == soluong.Count)
                {
                    for (int i = 0; i < idsp.Count; i++)
                    {
                        model.CHITIET.Add(new ChiTietPhieuXuat { IDSP = idsp[i], SOLUONG = soluong[i] });
                    }
                }
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/PhieuXuat/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PhieuXuat model, List<int> idsp, List<int> soluong)
        {
            if (id != model.IDPX) return BadRequest();
            if (ModelState.IsValid)
            {
                model.CHITIET = new List<ChiTietPhieuXuat>();
                if (idsp != null && soluong != null && idsp.Count == soluong.Count)
                {
                    for (int i = 0; i < idsp.Count; i++)
                    {
                        model.CHITIET.Add(new ChiTietPhieuXuat { IDSP = idsp[i], SOLUONG = soluong[i] });
                    }
                }
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.DonHangs = await _donHangService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/PhieuXuat/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.NhanVien = await _nhanVienService.GetByIdAsync(item.IDNV);
            ViewBag.Kho = await _khoService.GetByIdAsync(item.IDKHO);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Xuất Excel
        public async Task<IActionResult> ExportExcel()
        {
            var items = await _service.GetAllAsync();
            var nhanViens = await _nhanVienService.GetAllAsync();
            var khos = await _khoService.GetAllAsync();
            var donHangs = await _donHangService.GetAllAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("PhieuXuat");
                worksheet.Cells[1, 1].Value = "ID PX";
                worksheet.Cells[1, 2].Value = "Ngày xuất";
                worksheet.Cells[1, 3].Value = "Nhân viên";
                worksheet.Cells[1, 4].Value = "Kho";
                worksheet.Cells[1, 5].Value = "Đơn hàng";
                worksheet.Cells[1, 6].Value = "Loại phiếu";
                worksheet.Cells[1, 7].Value = "Chi tiết SP";
                worksheet.Cells[1, 8].Value = "Tổng số lượng";

                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                int row = 2;
                foreach (var item in items)
                {
                    var nv = nhanViens.FirstOrDefault(n => n.IDNV == item.IDNV);
                    var kho = khos.FirstOrDefault(k => k.IDKHO == item.IDKHO);
                    var dh = item.IDDH.HasValue ? donHangs.FirstOrDefault(d => d.IDDH == item.IDDH) : null;
                    string chiTiet = string.Join(", ", item.CHITIET.Select(c => $"SP{c.IDSP}:{c.SOLUONG}"));
                    int tongSL = item.CHITIET.Sum(c => c.SOLUONG);

                    worksheet.Cells[row, 1].Value = item.IDPX;
                    worksheet.Cells[row, 2].Value = item.NGAYXUAT.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 3].Value = nv?.HOTEN ?? "";
                    worksheet.Cells[row, 4].Value = kho?.TENKHO ?? "";
                    worksheet.Cells[row, 5].Value = dh != null ? $"#{dh.IDDH}" : "";
                    worksheet.Cells[row, 6].Value = item.LOAIPHIEU;
                    worksheet.Cells[row, 7].Value = chiTiet;
                    worksheet.Cells[row, 8].Value = tongSL;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string fileName = $"PhieuXuat_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}