using CarShop.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CarShop.Models; 
using OfficeOpenXml;
using Microsoft.AspNetCore.HttpOverrides; 

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình MongoDB settings
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));

// 2. Đăng ký IMongoClient 
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddScoped<MongoDbContext>();

builder.Services.AddScoped<SanPhamService>();
builder.Services.AddScoped<DonHangService>();
builder.Services.AddScoped<GioHangService>();
builder.Services.AddScoped<KhachHangService>();
builder.Services.AddScoped<TaiKhoanService>();
builder.Services.AddScoped<DanhMucService>();
builder.Services.AddScoped<ThuongHieuService>();
builder.Services.AddScoped<NhanVienService>();
builder.Services.AddScoped<PhieuNhapService>();
builder.Services.AddScoped<PhieuXuatService>();
builder.Services.AddScoped<KhuyenMaiService>();
builder.Services.AddScoped<BinhLuanService>();
builder.Services.AddScoped<YeuThichService>();
builder.Services.AddScoped<ChucVuService>();
builder.Services.AddScoped<LoaiGiaService>();
builder.Services.AddScoped<KhoService>();
builder.Services.AddScoped<NhaCungCapService>();
builder.Services.AddScoped<ThuocTinhService>();
builder.Services.AddScoped<QuaTangService>();
builder.Services.AddScoped<VoucherService>();
builder.Services.AddScoped<GiaKhuyenMaiService>();
builder.Services.AddScoped<SanPhamKhuyenMaiService>();
builder.Services.AddScoped<HoaDonService>();
builder.Services.AddScoped<ThanhToanService>();
builder.Services.AddScoped<VanChuyenService>();
builder.Services.AddScoped<BaoHanhService>();
builder.Services.AddScoped<HinhAnhService>();
builder.Services.AddScoped<DiemTichLuyService>();
builder.Services.AddScoped<LichSuDiemService>();
builder.Services.AddScoped<LichSuDoiDiemService>();
builder.Services.AddScoped<DiaChiGiaoService>();
builder.Services.AddScoped<ActivityLogService>();
builder.Services.AddScoped<LichSuDangNhapService>();
builder.Services.AddScoped<LichSuDonHangService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<PhieuLuongService>();
builder.Services.AddScoped<BaoCaoDoanhThuNgayService>();
builder.Services.AddScoped<BaoCaoDoanhThuThangService>();
builder.Services.AddScoped<BaoCaoDoanhThuNamService>();
builder.Services.AddScoped<GiaiThuocTinhService>();
builder.Services.AddScoped<TonKhoService>();
builder.Services.AddScoped<ThongSoKyThuatService>();
builder.Services.AddScoped<YeuCauTraHangService>();

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();