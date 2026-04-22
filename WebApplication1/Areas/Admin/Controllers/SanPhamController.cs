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

        // GET: Xuất Excel (Đã nâng cấp)
        public async Task<IActionResult> ExportExcel(string search, int? category, int? brand)
        {
            // Bắt buộc cấu hình License cho EPPlus bản mới
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var items = await _sanPhamService.GetAllAsync();

            // Lọc dữ liệu giống y hệt như trên giao diện Index
            if (!string.IsNullOrEmpty(search))
                items = items.Where(x => x.TENSP.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (category.HasValue && category.Value > 0)
                items = items.Where(x => x.IDDM == category.Value).ToList();

            if (brand.HasValue && brand.Value > 0)
                items = items.Where(x => x.IDTH == brand.Value).ToList();

            var danhMucs = await _danhMucService.GetAllAsync();
            var thuongHieus = await _thuongHieuService.GetAllAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachSanPham");

                // 1. Tạo Tiêu đề lớn cho bảng Excel
                worksheet.Cells["A1:J1"].Merge = true;
                worksheet.Cells["A1"].Value = "DANH SÁCH SẢN PHẨM Ô TÔ";
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // 2. Tạo Header (Dòng 2)
                var headers = new string[] { "ID", "Tên sản phẩm", "Giá (VNĐ)", "Số lượng", "Mô tả", "Danh mục", "Thương hiệu", "Ngày tạo", "Trạng thái", "Tồn tối thiểu" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[2, i + 1].Value = headers[i];
                }

                // Định dạng Header
                using (var range = worksheet.Cells[2, 1, 2, 10])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(52, 152, 219)); // Màu xanh dương đẹp mắt
                    range.Style.Font.Color.SetColor(Color.White);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                // 3. Đổ dữ liệu (Từ dòng 3)
                int row = 3;
                foreach (var item in items)
                {
                    var dm = danhMucs.FirstOrDefault(d => d.IDDM == item.IDDM);
                    var th = thuongHieus.FirstOrDefault(t => t.IDTH == item.IDTH);

                    worksheet.Cells[row, 1].Value = item.IDSP;
                    worksheet.Cells[row, 2].Value = item.TENSP;

                    worksheet.Cells[row, 3].Value = item.GIA;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0"; // Format tiền tệ

                    worksheet.Cells[row, 4].Value = item.SOLUONG;
                    worksheet.Cells[row, 5].Value = item.MOTA ?? "";
                    worksheet.Cells[row, 6].Value = dm?.TENDANHMUC ?? "";
                    worksheet.Cells[row, 7].Value = th?.TENTHUONGHIEU ?? "";

                    worksheet.Cells[row, 8].Value = item.NGAYTAO.Date; // Chỉ lấy ngày
                    worksheet.Cells[row, 8].Style.Numberformat.Format = "dd/MM/yyyy";

                    worksheet.Cells[row, 9].Value = item.TRANGTHAI ? "Đang bán" : "Ngừng bán";
                    worksheet.Cells[row, 10].Value = item.TONKHO_TOITHIEU;

                    // Căn giữa một số cột cho đẹp
                    worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    row++;
                }

                // 4. Kẻ khung (Borders) cho toàn bộ bảng dữ liệu
                if (items.Count > 0)
                {
                    using (var range = worksheet.Cells[2, 1, row - 1, 10])
                    {
                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                }

                // 5. Tự động giãn cột cho vừa text
                worksheet.Cells.AutoFitColumns();

                // 6. Trả về file
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"DanhSachSanPham_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}