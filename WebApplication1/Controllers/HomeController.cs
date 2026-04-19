using CarShop.Models;
using CarShop.Services;
using CarShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

namespace CarShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly SanPhamService _sanPhamService;
        private readonly DanhMucService _danhMucService;
        private readonly ThuongHieuService _thuongHieuService;
        private readonly KhuyenMaiService _khuyenMaiService;
        private readonly GiaKhuyenMaiService _giaKhuyenMaiService;
        private readonly HinhAnhService _hinhAnhService;
        private readonly ThongSoKyThuatService _thongSoKyThuatService;

        public HomeController(SanPhamService sanPhamService, DanhMucService danhMucService,
                              ThuongHieuService thuongHieuService, KhuyenMaiService khuyenMaiService,
                              GiaKhuyenMaiService giaKhuyenMaiService, HinhAnhService hinhAnhService,
                              ThongSoKyThuatService thongSoKyThuatService)
        {
            _sanPhamService = sanPhamService;
            _danhMucService = danhMucService;
            _thuongHieuService = thuongHieuService;
            _khuyenMaiService = khuyenMaiService;
            _giaKhuyenMaiService = giaKhuyenMaiService;
            _hinhAnhService = hinhAnhService;
            _thongSoKyThuatService = thongSoKyThuatService;
        }

        // Trang chủ (Index)
        // Trang chủ (Index)
        public async Task<IActionResult> Index()
        {
            var newProducts = await _sanPhamService.GetNewProductsAsync(8);
            var saleProducts = await _sanPhamService.GetSaleProductsAsync(8);
            var allImages = await _hinhAnhService.GetAllAsync();

            foreach (var sp in newProducts)
                sp.HinhAnh = allImages.Where(img => img.IDSP == sp.IDSP).ToList();
            foreach (var sp in saleProducts)
                sp.HinhAnh = allImages.Where(img => img.IDSP == sp.IDSP).ToList();

            var categories = await _danhMucService.GetAllAsync();
            var viewModel = new HomeViewModel
            {
                NewProducts = newProducts,
                SaleProducts = saleProducts,
                Categories = categories
            };
            return View(viewModel);
        }


        // Tìm kiếm nâng cao
        public async Task<IActionResult> Search(string search, int? danhMucId, int? thuongHieuId,
                                                decimal? minPrice, decimal? maxPrice, string sortBy = "moi")
        {
            var allProducts = await _sanPhamService.GetAllAsync();
            var query = allProducts.Where(p => p.TRANGTHAI);
            var allImages = await _hinhAnhService.GetAllAsync();
            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.TENSP.Contains(search, StringComparison.OrdinalIgnoreCase));

            // Lọc danh mục
            if (danhMucId.HasValue)
                query = query.Where(p => p.IDDM == danhMucId.Value);

            // Lọc thương hiệu
            if (thuongHieuId.HasValue)
                query = query.Where(p => p.IDTH == thuongHieuId.Value);

            // Lọc giá
            if (minPrice.HasValue)
                query = query.Where(p => p.GIA >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.GIA <= maxPrice.Value);

            var sanPhams = query.ToList();

            // Lấy danh mục, thương hiệu
            var allDanhMucs = await _danhMucService.GetAllAsync();
            var allThuongHieus = await _thuongHieuService.GetAllAsync();

            foreach (var sp in sanPhams)
            {
                sp.HinhAnh = allImages.Where(ha => ha.IDSP == sp.IDSP).ToList();
                sp.DanhMuc = allDanhMucs.FirstOrDefault(dm => dm.IDDM == sp.IDDM);
                sp.ThuongHieu = allThuongHieus.FirstOrDefault(th => th.IDTH == sp.IDTH);
            }

            // Sắp xếp
            switch (sortBy)
            {
                case "gia_tang": sanPhams = sanPhams.OrderBy(x => x.GIA).ToList(); break;
                case "gia_giam": sanPhams = sanPhams.OrderByDescending(x => x.GIA).ToList(); break;
                case "ten_tang": sanPhams = sanPhams.OrderBy(x => x.TENSP).ToList(); break;
                default: sanPhams = sanPhams.OrderByDescending(x => x.NGAYTAO).ToList(); break;
            }

            ViewBag.DanhMucs = allDanhMucs;
            ViewBag.ThuongHieus = allThuongHieus;
            ViewBag.SearchKeyword = search;
            ViewBag.SelectedDanhMuc = danhMucId;
            ViewBag.SelectedThuongHieu = thuongHieuId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortBy = sortBy;

            return View(sanPhams);
        }

        // Danh sách sản phẩm (có lọc, phân trang)
        public async Task<IActionResult> ProductList(int? category, int? brand, string search,
            decimal? minPrice, decimal? maxPrice, int? year, string fuel, string transmission,
            int page = 1, int pageSize = 12)
        {
            var allProducts = await _sanPhamService.GetAllAsync();
            var query = allProducts.Where(p => p.TRANGTHAI);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.TENSP.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         (p.MOTA != null && p.MOTA.Contains(search, StringComparison.OrdinalIgnoreCase)));

            if (category.HasValue && category.Value > 0)
                query = query.Where(p => p.IDDM == category.Value);
            if (brand.HasValue && brand.Value > 0)
                query = query.Where(p => p.IDTH == brand.Value);
            if (minPrice.HasValue)
                query = query.Where(p => p.GIA >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(p => p.GIA <= maxPrice.Value);

            // Lọc theo năm sản xuất (nếu có trong thông số kỹ thuật)
            // Tạm bỏ qua vì model SanPham không có trường năm

            var totalCount = query.Count();
            var products = query.OrderBy(p => p.IDSP).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var allImages = await _hinhAnhService.GetAllAsync();
            ViewBag.Images = allImages.GroupBy(i => i.IDSP).ToDictionary(g => g.Key, g => g.FirstOrDefault()?.DUONGDAN ?? "/images/no-car.png");

            ViewBag.Categories = await _danhMucService.GetAllAsync();
            ViewBag.Brands = await _thuongHieuService.GetAllAsync();
            ViewBag.Years = Enumerable.Range(2015, DateTime.Now.Year - 2015 + 1).OrderByDescending(y => y);
            ViewBag.FuelTypes = new List<string> { "Xăng", "Diesel", "Điện", "Hybrid", "Xăng + Điện" };
            ViewBag.Transmissions = new List<string> { "Số sàn", "Số tự động", "CVT", "DCT", "AT" };

            ViewBag.CurrentCategory = category;
            ViewBag.CurrentBrand = brand;
            ViewBag.Search = search;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Year = year;
            ViewBag.Fuel = fuel;
            ViewBag.Transmission = transmission;
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.PageSize = pageSize;

            return View(products);
        }

        // Chi tiết sản phẩm
        public async Task<IActionResult> ProductDetail(int id)
        {
            var product = await _sanPhamService.GetByIdAsync(id);
            if (product == null || !product.TRANGTHAI)
                return NotFound();

            var category = await _danhMucService.GetByIdAsync(product.IDDM);
            var brand = await _thuongHieuService.GetByIdAsync(product.IDTH);
            var images = await _hinhAnhService.GetBySanPhamIdAsync(id);
            var promotionPrice = (await _giaKhuyenMaiService.GetAllAsync())
                .FirstOrDefault(g => g.IDSP == id && g.NGAYBATDAU <= DateTime.Today && g.NGAYKETTHUC >= DateTime.Today);
            var technicalSpecs = await _thongSoKyThuatService.GetBySanPhamIdAsync(id);

            ViewBag.Images = images.Any() ? images.Select(i => i.DUONGDAN).ToList() : new List<string> { "/images/no-car.png" };
            ViewBag.Category = category;
            ViewBag.Brand = brand;
            ViewBag.PromotionPrice = promotionPrice?.GIAKHUYENMAI;
            ViewBag.TechnicalSpecs = technicalSpecs;

            return View(product);
        }

        // Sản phẩm liên quan
        public async Task<IActionResult> RelatedProducts(int productId, int take = 4)
        {
            var product = await _sanPhamService.GetByIdAsync(productId);
            if (product == null) return PartialView("_RelatedProducts", new List<SanPham>());

            var allProducts = await _sanPhamService.GetAllAsync();
            var related = allProducts.Where(p => p.TRANGTHAI && p.IDSP != productId &&
                                                 (p.IDDM == product.IDDM || p.IDTH == product.IDTH))
                                     .Take(take).ToList();

            // Lấy hình ảnh cho sản phẩm liên quan
            var allImages = await _hinhAnhService.GetAllAsync();
            ViewBag.Images = allImages.GroupBy(i => i.IDSP).ToDictionary(g => g.Key, g => g.FirstOrDefault()?.DUONGDAN ?? "/images/no-car.png");

            return PartialView("_RelatedProducts", related);
        }

        // Chuyển hướng lọc theo danh mục
        public IActionResult Category(int id) => RedirectToAction("ProductList", new { category = id });

        // Chuyển hướng lọc theo thương hiệu
        public IActionResult Brand(int id) => RedirectToAction("ProductList", new { brand = id });

        // Tìm kiếm đơn giản
        public IActionResult SearchSimple(string search) => RedirectToAction("ProductList", new { search });

        // Liên hệ
        [HttpGet]
        public IActionResult Contact() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Gửi email (có thể dùng service)
                TempData["Message"] = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm nhất!";
                return RedirectToAction("Contact");
            }
            return View(model);
        }

        // Lỗi
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}