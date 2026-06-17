using Microsoft.AspNetCore.Mvc;

namespace HotelManageSystem.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// [Cấu hình hệ thống] Hàm khởi chạy trang chủ mặc định của ứng dụng.
        /// Thực hiện điều hướng tự động (Redirect) sang trang danh sách tài khoản của Admin (UC5.4) để tránh lỗi crash giao diện mẫu.
        /// </summary>
        public IActionResult Index()
        {
            // Tự động chuyển hướng toàn bộ request khởi động sang trang Index của AccountController
            return RedirectToAction("Index", "Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}