using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class PhieuNhapController : Controller
    {
        private readonly PhieuNhapService _service;
        private readonly NhaCungCapService _nhaCungCapService;
        private readonly NhanVienService _nhanVienService;
        private readonly KhoService _khoService;
        private readonly SanPhamService _sanPhamService;
        private readonly TonKhoService _tonKhoService; // ✅ THÊM SERVICE TỒN KHO

        public PhieuNhapController(PhieuNhapService service, NhaCungCapService nhaCungCapService,
                                    NhanVienService nhanVienService, KhoService khoService,
                                    SanPhamService sanPhamService, TonKhoService tonKhoService) // ✅ INJECT VÀO CONSTRUCTOR
        {
            _service = service;
            _nhaCungCapService = nhaCungCapService;
            _nhanVienService = nhanVienService;
            _khoService = khoService;
            _sanPhamService = sanPhamService;
            _tonKhoService = tonKhoService;
        }

        // GET: /Admin/PhieuNhap
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDPN == id || x.IDNCC == id || x.IDNV == id || x.IDKHO == id).ToList();
                else
                    items = items.Where(x => x.CHITIET.Any(c => _sanPhamService.GetByIdAsync(c.IDSP).Result?.TENSP?.Contains(search) == true)).ToList();
            }
            var totalCount = items.Count;
            var phieuNhaps = items.OrderByDescending(x => x.NGAYNHAP).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.NhaCungCaps = await _nhaCungCapService.GetAllAsync();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(phieuNhaps);
        }

        // GET: /Admin/PhieuNhap/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.NhaCungCap = await _nhaCungCapService.GetByIdAsync(item.IDNCC);
            ViewBag.NhanVien = await _nhanVienService.GetByIdAsync(item.IDNV);
            ViewBag.Kho = await _khoService.GetByIdAsync(item.IDKHO);
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(item);
        }

        // GET: /Admin/PhieuNhap/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.NhaCungCaps = await _nhaCungCapService.GetAllAsync();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuNhap model, List<int> IDSP, List<int> SOLUONG, List<decimal> DONGIA)
        {
            if (ModelState.IsValid)
            {
                var all = await _service.GetAllAsync();
                model.IDPN = all.Any() ? all.Max(x => x.IDPN) + 1 : 1;
                model.CHITIET = new List<ChiTietPhieuNhap>();

                for (int i = 0; i < IDSP.Count; i++)
                {
                    if (IDSP[i] > 0 && SOLUONG[i] > 0)
                    {
                        model.CHITIET.Add(new ChiTietPhieuNhap
                        {
                            IDSP = IDSP[i],
                            SOLUONG = SOLUONG[i],
                            DONGIA = DONGIA[i]
                        });

                        // ==============================================================
                        // ✅ XỬ LÝ TỰ ĐỘNG CỘNG VÀO TỒN KHO VÀ SẢN PHẨM KHI NHẬP HÀNG
                        // ==============================================================

                        // 1. Cập nhật bảng Tồn Kho
                        var tk = await _tonKhoService.GetByKhoAndSanPhamAsync(model.IDKHO, IDSP[i]);
                        if (tk != null)
                        {
                            tk.SOLUONGTON += SOLUONG[i];
                            await _tonKhoService.UpdateAsync(tk.Id, tk);
                        }
                        else
                        {
                            await _tonKhoService.CreateAsync(new TonKho
                            {
                                IDKHO = model.IDKHO,
                                IDSP = IDSP[i],
                                SOLUONGTON = SOLUONG[i]
                            });
                        }

                        // 2. Cập nhật số lượng hiển thị ở bảng Sản Phẩm
                        var sp = await _sanPhamService.GetByIdAsync(IDSP[i]);
                        if (sp != null)
                        {
                            sp.SOLUONG += SOLUONG[i];
                            await _sanPhamService.UpdateAsync(sp.IDSP, sp);
                        }
                    }
                }
                await _service.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NhaCungCaps = await _nhaCungCapService.GetAllAsync();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/PhieuNhap/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.NhaCungCaps = await _nhaCungCapService.GetAllAsync();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PhieuNhap model, List<int> IDSP, List<int> SOLUONG, List<decimal> DONGIA)
        {
            if (id != model.IDPN) return BadRequest();
            if (ModelState.IsValid)
            {
                model.CHITIET = new List<ChiTietPhieuNhap>();
                for (int i = 0; i < IDSP.Count; i++)
                {
                    if (IDSP[i] > 0 && SOLUONG[i] > 0)
                    {
                        model.CHITIET.Add(new ChiTietPhieuNhap
                        {
                            IDSP = IDSP[i],
                            SOLUONG = SOLUONG[i],
                            DONGIA = DONGIA[i]
                        });
                    }
                }
                await _service.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.NhaCungCaps = await _nhaCungCapService.GetAllAsync();
            ViewBag.NhanViens = await _nhanVienService.GetAllAsync();
            ViewBag.Khos = await _khoService.GetAllAsync();
            ViewBag.SanPhams = await _sanPhamService.GetAllAsync();
            return View(model);
        }

        // GET: /Admin/PhieuNhap/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            ViewBag.NhaCungCap = await _nhaCungCapService.GetByIdAsync(item.IDNCC);
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

        // GET: /Admin/PhieuNhap/ExportExcel
        public async Task<IActionResult> ExportExcel(string search)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDPN == id || x.IDNCC == id || x.IDNV == id || x.IDKHO == id).ToList();
                else
                    items = items.Where(x => x.CHITIET.Any(c => _sanPhamService.GetByIdAsync(c.IDSP).Result?.TENSP?.Contains(search) == true)).ToList();
            }
            var nhaCungCaps = await _nhaCungCapService.GetAllAsync();
            var nhanViens = await _nhanVienService.GetAllAsync();
            var khos = await _khoService.GetAllAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("PhieuNhap");
                worksheet.Cells.LoadFromCollection(items, true);
                worksheet.Cells[1, 1, 1, 8].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1, 1, 8].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string fileName = $"PhieuNhap_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}