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
    public class SanPhamController : Controller
    {
        private readonly SanPhamService _sanPhamService;
        private readonly DanhMucService _danhMucService;
        private readonly ThuongHieuService _thuongHieuService;

        public SanPhamController(SanPhamService sanPhamService, DanhMucService danhMucService, ThuongHieuService thuongHieuService)
        {
            _sanPhamService = sanPhamService;
            _danhMucService = danhMucService;
            _thuongHieuService = thuongHieuService;
        }

        // GET: /Admin/SanPham
        public async Task<IActionResult> Index(string search, int? category, int? brand, int page = 1, int pageSize = 10)
        {
            var items = await _sanPhamService.GetAllAsync();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(search))
                items = items.Where(x => x.TENSP.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            // Lọc theo danh mục
            if (category.HasValue && category.Value > 0)
                items = items.Where(x => x.IDDM == category.Value).ToList();

            // Lọc theo thương hiệu
            if (brand.HasValue && brand.Value > 0)
                items = items.Where(x => x.IDTH == brand.Value).ToList();

            var totalCount = items.Count;
            var sanPhams = items.OrderBy(x => x.IDSP).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.DanhMucs = await _danhMucService.GetAllAsync();
            ViewBag.ThuongHieus = await _thuongHieuService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.Brand = brand;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(sanPhams);
        }

        // GET: /Admin/SanPham/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _sanPhamService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.DanhMuc = await _danhMucService.GetByIdAsync(item.IDDM);
            ViewBag.ThuongHieu = await _thuongHieuService.GetByIdAsync(item.IDTH);
            return View(item);
        }

        // GET: /Admin/SanPham/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.DanhMucs = await _danhMucService.GetAllAsync();
            ViewBag.ThuongHieus = await _thuongHieuService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham model)
        {
            if (ModelState.IsValid)
            {
                var all = await _sanPhamService.GetAllAsync();
                model.IDSP = all.Any() ? all.Max(x => x.IDSP) + 1 : 1;
                model.NGAYTAO = DateTime.Now;
                model.TRANGTHAI = true;
                await _sanPhamService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DanhMucs = await _danhMucService.GetAllAsync();
            ViewBag.ThuongHieus = await _thuongHieuService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/SanPham/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _sanPhamService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.DanhMucs = await _danhMucService.GetAllAsync();
            ViewBag.ThuongHieus = await _thuongHieuService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanPham model)
        {
            if (id != model.IDSP) return BadRequest();
            if (ModelState.IsValid)
            {
                var existing = await _sanPhamService.GetByIdAsync(id);
                if (existing == null) return NotFound();
                model.NGAYTAO = existing.NGAYTAO;
                model.TRANGTHAI = existing.TRANGTHAI;
                await _sanPhamService.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DanhMucs = await _danhMucService.GetAllAsync();
            ViewBag.ThuongHieus = await _thuongHieuService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/SanPham/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _sanPhamService.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.DanhMuc = await _danhMucService.GetByIdAsync(item.IDDM);
            ViewBag.ThuongHieu = await _thuongHieuService.GetByIdAsync(item.IDTH);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _sanPhamService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Xuất Excel
        public async Task<IActionResult> ExportExcel()
        {
            var items = await _sanPhamService.GetAllAsync();
            var danhMucs = await _danhMucService.GetAllAsync();
            var thuongHieus = await _thuongHieuService.GetAllAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SanPham");
                worksheet.Cells[1, 1].Value = "ID SP";
                worksheet.Cells[1, 2].Value = "Tên sản phẩm";
                worksheet.Cells[1, 3].Value = "Giá (VNĐ)";
                worksheet.Cells[1, 4].Value = "Số lượng";
                worksheet.Cells[1, 5].Value = "Mô tả";
                worksheet.Cells[1, 6].Value = "Danh mục";
                worksheet.Cells[1, 7].Value = "Thương hiệu";
                worksheet.Cells[1, 8].Value = "Ngày tạo";
                worksheet.Cells[1, 9].Value = "Trạng thái";
                worksheet.Cells[1, 10].Value = "Tồn kho tối thiểu";

                using (var range = worksheet.Cells[1, 1, 1, 10])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                int row = 2;
                foreach (var item in items)
                {
                    var dm = danhMucs.FirstOrDefault(d => d.IDDM == item.IDDM);
                    var th = thuongHieus.FirstOrDefault(t => t.IDTH == item.IDTH);

                    worksheet.Cells[row, 1].Value = item.IDSP;
                    worksheet.Cells[row, 2].Value = item.TENSP;
                    worksheet.Cells[row, 3].Value = item.GIA;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row, 4].Value = item.SOLUONG;
                    worksheet.Cells[row, 5].Value = item.MOTA ?? "";
                    worksheet.Cells[row, 6].Value = dm?.TENDANHMUC ?? "";
                    worksheet.Cells[row, 7].Value = th?.TENTHUONGHIEU ?? "";
                    worksheet.Cells[row, 8].Value = item.NGAYTAO.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 9].Value = item.TRANGTHAI ? "Đang bán" : "Ngừng bán";
                    worksheet.Cells[row, 10].Value = item.TONKHO_TOITHIEU;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string fileName = $"SanPham_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
