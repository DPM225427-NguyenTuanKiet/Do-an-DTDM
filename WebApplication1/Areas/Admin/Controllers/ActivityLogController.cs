using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarShop.Models;
using CarShop.Services;

namespace CarShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ActivityLogController : Controller
    {
        private readonly ActivityLogService _service;
        public ActivityLogController(ActivityLogService service) => _service = service;

        // Hiển thị danh sách log (có tìm kiếm)
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 20)
        {
            var items = await _service.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int id))
                    items = items.Where(x => x.IDLOG == id || x.IDTK == id).ToList();
                else
                    items = items.Where(x => x.HANHDONG != null && x.HANHDONG.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                             (x.CHITIET != null && x.CHITIET.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            ViewBag.Search = search;
            var totalCount = items.Count;
            var logs = items.OrderByDescending(x => x.THOIGIAN).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return View(logs);
        }

        // Xem chi tiết log
        public async Task<IActionResult> Details(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // Xóa log (có thể giới hạn chỉ xóa log cũ)
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Xóa nhiều log (chọn hàng loạt)
        [HttpPost]
        public async Task<IActionResult> DeleteMultiple(int[] ids)
        {
            if (ids != null && ids.Any())
            {
                foreach (var id in ids)
                    await _service.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

        // Xóa tất cả log cũ hơn ngày chỉ định
        [HttpPost]
        public async Task<IActionResult> DeleteOldLogs(DateTime beforeDate)
        {
            var allLogs = await _service.GetAllAsync();
            var oldLogs = allLogs.Where(x => x.THOIGIAN < beforeDate).ToList();
            foreach (var log in oldLogs)
                await _service.DeleteAsync(log.IDLOG);
            return RedirectToAction(nameof(Index));
        }
    }
}