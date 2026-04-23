using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using CarShop.Models;
using CarShop.Services;
using Microsoft.AspNetCore.Hosting;

namespace CarShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly TaiKhoanService _taiKhoanService;
        private readonly KhachHangService _khachHangService;
        private readonly IWebHostEnvironment _webHostEnvironment;  

        public AccountController(TaiKhoanService taiKhoanService, KhachHangService khachHangService, IWebHostEnvironment webHostEnvironment)
        {
            _taiKhoanService = taiKhoanService;
            _khachHangService = khachHangService;
            _webHostEnvironment = webHostEnvironment; 
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var account = await _taiKhoanService.GetByUsernameAsync(username);
            if (account == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                return View();
            }

            bool passwordValid = false;

            // Trường hợp 1: Mật khẩu trong DB đã được hash (BCrypt)
            if (account.MATKHAU.Length >= 60 && account.MATKHAU.StartsWith("$2"))
            {
                passwordValid = BCrypt.Net.BCrypt.Verify(password, account.MATKHAU);
            }
            else
            {
                // Trường hợp 2: Mật khẩu trong DB là plain text (chưa hash)
                if (account.MATKHAU == password)
                {
                    // Hash mật khẩu và cập nhật lại DB
                    account.MATKHAU = BCrypt.Net.BCrypt.HashPassword(password);
                    await _taiKhoanService.UpdateAsync(account.IDTK, account);
                    passwordValid = true;
                }
            }

            if (!passwordValid)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu";
                return View();
            }

            if (!account.TRANGTHAI)
            {
                ViewBag.Error = "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên.";
                return View();
            }

            // Tạo claims và đăng nhập
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.TENDANGNHAP),
                new Claim(ClaimTypes.Email, account.EMAIL),
                new Claim(ClaimTypes.Role, account.VAITRO),
                new Claim("IDTK", account.IDTK.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            });

            // Chuyển hướng dựa trên vai trò
            if (account.VAITRO.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                account.VAITRO.Equals("Quản trị", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            else
                return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string tendangnhap, string matkhau, string email, string hoten, string dienthoai, string diachi)
        {
            // Kiểm tra tên đăng nhập đã tồn tại
            var existing = await _taiKhoanService.GetByUsernameAsync(tendangnhap);
            if (existing != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại";
                return View();
            }

            // Kiểm tra email đã tồn tại
            existing = await _taiKhoanService.GetByEmailAsync(email);
            if (existing != null)
            {
                ViewBag.Error = "Email đã được sử dụng";
                return View();
            }

            // Tạo tài khoản mới
            var allAccounts = await _taiKhoanService.GetAllAsync();
            var newId = allAccounts.Any() ? allAccounts.Max(tk => tk.IDTK) + 1 : 1;

            var taiKhoan = new TaiKhoan
            {
                IDTK = newId,
                TENDANGNHAP = tendangnhap,
                MATKHAU = BCrypt.Net.BCrypt.HashPassword(matkhau),
                EMAIL = email,
                VAITRO = "customer",
                TRANGTHAI = true,
                NGAYTAO = DateTime.Now
            };
            await _taiKhoanService.CreateAsync(taiKhoan);

            // Tạo khách hàng tương ứng
            var allCustomers = await _khachHangService.GetAllAsync();
            var khachHang = new KhachHang
            {
                IDKH = allCustomers.Any() ? allCustomers.Max(kh => kh.IDKH) + 1 : 1,
                IDTK = taiKhoan.IDTK,
                HOTEN = hoten,
                DIENTHOAI = dienthoai,
                DIACHI = diachi,
                EMAIL = email,
                IDLG = 1 // Mặc định hạng Thường
            };
            await _khachHangService.CreateAsync(khachHang);

            // Tự động đăng nhập sau khi đăng ký
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, taiKhoan.TENDANGNHAP),
                new Claim(ClaimTypes.Email, taiKhoan.EMAIL),
                new Claim(ClaimTypes.Role, taiKhoan.VAITRO),
                new Claim("IDTK", taiKhoan.IDTK.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Vui lòng nhập email.";
                return View();
            }

            var account = await _taiKhoanService.GetByEmailAsync(email);
            if (account == null)
            {
                ViewBag.Message = "Nếu email tồn tại trong hệ thống, bạn sẽ nhận được hướng dẫn đặt lại mật khẩu.";
                return View("ForgotPasswordConfirmation");
            }

            // Tạo reset token
            var token = Guid.NewGuid().ToString();
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            account.ResetToken = token;
            account.ResetTokenExpiry = DateTime.Now.AddHours(24);
            await _taiKhoanService.UpdateAsync(account.IDTK, account);

            // Tạo link reset password (demo)
            var resetLink = Url.Action("ResetPassword", "Account", new { userId = account.IDTK, token = encodedToken }, Request.Scheme);

            ViewBag.Link = resetLink;
            ViewBag.Message = "Một link đặt lại mật khẩu đã được tạo (demo). Nhấn vào link bên dưới để tiếp tục.";
            return View("ForgotPasswordConfirmation");
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(int userId, string token)
        {
            if (userId == 0 || string.IsNullOrEmpty(token))
                return BadRequest();

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            ViewBag.UserId = userId;
            ViewBag.Token = decodedToken;
            return View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int userId, string token, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(newPassword) || newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu không khớp hoặc để trống.";
                return View();
            }

            var account = await _taiKhoanService.GetByIdAsync(userId);
            if (account == null || account.ResetToken != token || account.ResetTokenExpiry < DateTime.Now)
            {
                ViewBag.Error = "Link đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
                return View();
            }

            account.MATKHAU = BCrypt.Net.BCrypt.HashPassword(newPassword);
            account.ResetToken = null;
            account.ResetTokenExpiry = null;
            await _taiKhoanService.UpdateAsync(account.IDTK, account);

            TempData["Message"] = "Mật khẩu đã được đặt lại thành công. Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        // GET: /Account/Profile
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirstValue("IDTK");
            if (string.IsNullOrEmpty(userIdClaim)) return NotFound();

            var userId = int.Parse(userIdClaim);
            var taiKhoan = await _taiKhoanService.GetByIdAsync(userId);
            if (taiKhoan == null) return NotFound();

            // Lấy thông tin khách hàng theo IDTK
            var khachHang = await _khachHangService.GetByTaiKhoanIdAsync(userId);
            ViewBag.KhachHang = khachHang;

            return View(taiKhoan);
        }

        // POST: /Account/Profile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(TaiKhoan model, string hoten, string dienthoai, string diachi)
        {
            var userIdClaim = User.FindFirstValue("IDTK");
            if (string.IsNullOrEmpty(userIdClaim)) return NotFound();

            var userId = int.Parse(userIdClaim);
            var taiKhoan = await _taiKhoanService.GetByIdAsync(userId);
            if (taiKhoan == null) return NotFound();

            // Cập nhật thông tin tài khoản
            taiKhoan.EMAIL = model.EMAIL;
            await _taiKhoanService.UpdateAsync(userId, taiKhoan);

            // Cập nhật thông tin khách hàng
            var khachHang = await _khachHangService.GetByTaiKhoanIdAsync(userId);
            if (khachHang != null)
            {
                khachHang.HOTEN = hoten;
                khachHang.DIENTHOAI = dienthoai;
                khachHang.DIACHI = diachi;
                await _khachHangService.UpdateAsync(khachHang.IDKH, khachHang);
            }

            ViewBag.Message = "Cập nhật thông tin thành công!";
            ViewBag.KhachHang = khachHang;
            return View(taiKhoan);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
        {
            if (avatarFile == null || avatarFile.Length == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn một file ảnh hợp lệ." });
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue("IDTK"));
                var khachHang = await _khachHangService.GetByTaiKhoanIdAsync(userId);

                // 1. Kiểm tra và tạo thư mục uploads/avatars nếu chưa có
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 2. Đổi tên file để không bị trùng lặp
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + avatarFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // 3. Lưu file ảnh vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(fileStream);
                }

                string imageUrl = "/uploads/avatars/" + uniqueFileName;

                // 4. Cập nhật Database
                if (khachHang != null)
                {
                    // Xóa ảnh cũ trên Server (tránh rác máy chủ), trừ ảnh mặc định
                    if (!string.IsNullOrEmpty(khachHang.AVATAR) && !khachHang.AVATAR.Contains("avatar-default"))
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, khachHang.AVATAR.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    khachHang.AVATAR = imageUrl;
                    await _khachHangService.UpdateAsync(khachHang.IDKH, khachHang);
                }
                else
                {
                    // Nếu khách hàng chưa có profile thì tạo mới
                    var all = await _khachHangService.GetAllAsync();
                    khachHang = new KhachHang
                    {
                        IDKH = all.Any() ? all.Max(k => k.IDKH) + 1 : 1,
                        IDTK = userId,
                        AVATAR = imageUrl,
                        HOTEN = User.Identity.Name ?? "Khách hàng",
                        DIENTHOAI = "",
                        DIACHI = "",
                        EMAIL = User.FindFirstValue(ClaimTypes.Email) ?? "",
                        IDLG = 1 // Hạng thẻ mặc định
                    };
                    await _khachHangService.CreateAsync(khachHang);
                }

                // Trả về JSON thành công kèm link ảnh mới để JS cập nhật giao diện
                return Json(new { success = true, imageUrl = imageUrl, message = "Cập nhật ảnh đại diện thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}