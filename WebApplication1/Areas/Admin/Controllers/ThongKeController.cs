using CarShop.Models;
using CarShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ThongKeController : Controller
    {
        private readonly DonHangService _donHangService;
        private readonly SanPhamService _sanPhamService;
        private readonly BaoCaoDoanhThuNgayService _baoCaoNgayService;
        private readonly BaoCaoDoanhThuThangService _baoCaoThangService;
        private readonly BaoCaoDoanhThuNamService _baoCaoNamService;

        public ThongKeController(
            DonHangService donHangService,
            SanPhamService sanPhamService,
            BaoCaoDoanhThuNgayService baoCaoNgayService,
            BaoCaoDoanhThuThangService baoCaoThangService,
            BaoCaoDoanhThuNamService baoCaoNamService)
        {
            _donHangService = donHangService;
            _sanPhamService = sanPhamService;
            _baoCaoNgayService = baoCaoNgayService;
            _baoCaoThangService = baoCaoThangService;
            _baoCaoNamService = baoCaoNamService;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy dữ liệu tổng quan
            var orders = await _donHangService.GetAllAsync();
            var totalRevenue = orders.Where(o => o.TRANGTHAI == "Đã giao").Sum(o => o.TONGTIEN);
            var totalOrders = orders.Count();
            var pendingOrders = orders.Count(o => o.TRANGTHAI == "Chờ xử lý");
            var deliveredOrders = orders.Count(o => o.TRANGTHAI == "Đã giao");

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.DeliveredOrders = deliveredOrders;

            // Báo cáo ngày (7 ngày gần nhất)
            var dailyReports = new List<BaoCaoDoanhThuNgay>();
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Now.AddDays(-i);
                var report = await _baoCaoNgayService.GetByDateAsync(date);
                dailyReports.Add(report ?? new BaoCaoDoanhThuNgay { NGAY = date, TONG_DOANHTHU = 0, TONG_DONHANG = 0 });
            }
            ViewBag.DailyReports = dailyReports;

            // Báo cáo tháng (12 tháng gần nhất)
            var monthlyReports = new List<BaoCaoDoanhThuThang>();
            for (int i = 11; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                var report = await _baoCaoThangService.GetByMonthYearAsync(date.Month, date.Year);
                monthlyReports.Add(report ?? new BaoCaoDoanhThuThang { THANG = date.Month, NAM = date.Year, TONG_DOANHTHU = 0, TONG_DONHANG = 0 });
            }
            ViewBag.MonthlyReports = monthlyReports;

            // Báo cáo năm (5 năm gần nhất)
            var yearlyReports = new List<BaoCaoDoanhThuNam>();
            int currentYear = DateTime.Now.Year;
            for (int i = 4; i >= 0; i--)
            {
                int year = currentYear - i;
                var report = await _baoCaoNamService.GetByYearAsync(year);
                yearlyReports.Add(report ?? new BaoCaoDoanhThuNam { NAM = year, TONG_DOANHTHU = 0, TONG_DONHANG = 0 });
            }
            ViewBag.YearlyReports = yearlyReports;

            // Top sản phẩm bán chạy
            var productSales = orders.SelectMany(o => o.CHITIET)
                                     .GroupBy(c => c.IDSP)
                                     .Select(g => new { IDSP = g.Key, TotalSold = g.Sum(c => c.SOLUONG) })
                                     .OrderByDescending(x => x.TotalSold)
                                     .Take(5)
                                     .ToList();
            var topProducts = new List<dynamic>();
            foreach (var item in productSales)
            {
                var product = await _sanPhamService.GetByIdAsync(item.IDSP);
                topProducts.Add(new { name = product?.TENSP ?? "Không xác định", sold = item.TotalSold });
            }
            ViewBag.TopProducts = topProducts;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStatsData(DateTime? startDate, DateTime? endDate, string type = "day")
        {
            var orders = await _donHangService.GetAllAsync();

            if (startDate.HasValue && endDate.HasValue)
            {
                orders = orders.Where(o => o.NGAYDAT >= startDate.Value && o.NGAYDAT <= endDate.Value).ToList();
            }

            var totalRevenue = orders.Where(o => o.TRANGTHAI == "Đã giao").Sum(o => o.TONGTIEN);
            var totalOrders = orders.Count();
            var pendingOrders = orders.Count(o => o.TRANGTHAI == "Chờ xử lý");
            var deliveredOrders = orders.Count(o => o.TRANGTHAI == "Đã giao");

            // Lấy dữ liệu biểu đồ theo type
            var monthlyRevenues = new List<decimal>();
            var monthLabels = new List<string>();

            if (type == "day")
            {
                // 30 ngày gần nhất
                for (int i = 29; i >= 0; i--)
                {
                    var date = DateTime.Now.AddDays(-i);
                    var dayRevenue = orders.Where(o => o.TRANGTHAI == "Đã giao" && o.NGAYDAT.Date == date.Date)
                                            .Sum(o => o.TONGTIEN);
                    monthlyRevenues.Add(dayRevenue);
                    monthLabels.Add(date.ToString("dd/MM"));
                }
            }
            else if (type == "month")
            {
                // 12 tháng gần nhất
                for (int i = 11; i >= 0; i--)
                {
                    var date = DateTime.Now.AddMonths(-i);
                    var monthStart = new DateTime(date.Year, date.Month, 1);
                    var monthEnd = monthStart.AddMonths(1);
                    var monthRevenue = orders.Where(o => o.TRANGTHAI == "Đã giao" && o.NGAYDAT >= monthStart && o.NGAYDAT < monthEnd)
                                              .Sum(o => o.TONGTIEN);
                    monthlyRevenues.Add(monthRevenue);
                    monthLabels.Add($"{date.Month}/{date.Year}");
                }
            }
            else // year
            {
                // 5 năm gần nhất
                int currentYear = DateTime.Now.Year;
                for (int i = 4; i >= 0; i--)
                {
                    int year = currentYear - i;
                    var yearStart = new DateTime(year, 1, 1);
                    var yearEnd = yearStart.AddYears(1);
                    var yearRevenue = orders.Where(o => o.TRANGTHAI == "Đã giao" && o.NGAYDAT >= yearStart && o.NGAYDAT < yearEnd)
                                             .Sum(o => o.TONGTIEN);
                    monthlyRevenues.Add(yearRevenue);
                    monthLabels.Add(year.ToString());
                }
            }

            // Top 5 sản phẩm bán chạy
            var productSales = orders.SelectMany(o => o.CHITIET)
                                     .GroupBy(c => c.IDSP)
                                     .Select(g => new { IDSP = g.Key, TotalSold = g.Sum(c => c.SOLUONG) })
                                     .OrderByDescending(x => x.TotalSold)
                                     .Take(5)
                                     .ToList();
            var topProducts = new List<object>();
            foreach (var item in productSales)
            {
                var product = await _sanPhamService.GetByIdAsync(item.IDSP);
                topProducts.Add(new { name = product?.TENSP ?? "Không xác định", sold = item.TotalSold });
            }

            return Ok(new
            {
                totalRevenue,
                totalOrders,
                pendingOrders,
                deliveredOrders,
                monthlyRevenues,
                monthLabels,
                topProducts
            });
        }
    }
}