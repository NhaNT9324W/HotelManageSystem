using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManageSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManageSystem.Controllers
{
    /// <summary>
    /// [Controller Xử Lý] Quản lý toàn bộ nghiệp vụ hồ sơ khách hàng lưu trú thuộc cụm UC15.
    /// Phục vụ trực tiếp cho Actor bộ phận Lễ tân (Receptionist) xử lý thao tác.
    /// </summary>
    public class GuestController : Controller
    {
        private readonly HotelDbContext _context;

        /// <summary>
        /// Tiêm bối cảnh trung tâm Database kết nối vào Controller phục vụ truy vấn.
        /// </summary>
        public GuestController(HotelDbContext context)
        {
            _context = context;
        }

        // ====================================================================================
        // MẮT XÍCH 1: XEM DANH SÁCH & TÌM KIẾM HỒ SƠ KHÁCH (MAPPED: UC15.0)
        // ====================================================================================

        /// <summary>
        /// [Use Case: UC15.0 - Manage Guest Profiles]
        /// Hiển thị danh sách khách hàng và hỗ trợ tìm kiếm bộ lọc nhanh theo Tên hoặc Số điện thoại.
        /// </summary>
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var guestsQuery = _context.Guests.AsQueryable();

            // Thực hiện lọc dữ liệu hồ sơ nếu Lễ tân có nhập từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                guestsQuery = guestsQuery.Where(g => g.FullName.Contains(searchString)
                                                  || g.Phone!.Contains(searchString));
            }

            var guestList = await guestsQuery.OrderByDescending(g => g.CreatedAt).ToListAsync();

            // Thực thi BR-12: Ẩn thông tin số định danh CMND/CCCD/Passport bằng dấu sao (*) bảo mật
            foreach (var guest in guestList)
            {
                if (!string.IsNullOrEmpty(guest.IdentityNumber))
                {
                    guest.IdentityNumber = MaskIdentityNumber(guest.IdentityNumber);
                }
            }

            return View(guestList);
        }

        // ====================================================================================
        // MẮT XÍCH 2: XEM CHI TIẾT HỒ SƠ KHÁCH HÀNG (MAPPED: UC15.2)
        // ====================================================================================

        /// <summary>
        /// [Use Case: UC15.2 - View Guest Profile]
        /// Hiển thị toàn bộ thông tin chi tiết (Read-only) của một hồ sơ khách hàng được chọn.
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var guest = await _context.Guests.FirstOrDefaultAsync(m => m.GuestId == id);
            if (guest == null) return NotFound();

            // Thực thi BR-12: Khi xem chi tiết, số CMND/CCCD cũng phải được che chắn kỹ càng
            if (!string.IsNullOrEmpty(guest.IdentityNumber))
            {
                guest.IdentityNumber = MaskIdentityNumber(guest.IdentityNumber);
            }

            return View(guest);
        }

        // ====================================================================================
        // MẮT XÍCH 3: LẬP HỒ SƠ KHÁCH HÀNG MỚI (MAPPED: UC15.1)
        // ====================================================================================

        /// <summary>
        /// [Use Case: UC15.1 - Create Guest Information] (Giao diện điền thông tin)
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// [Use Case: UC15.1 - Create Guest Information] (Xử lý ghi nhận lưu trữ)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Gender,DateOfBirth,Nationality,IdentityNumber,Phone,Email,Address")] Guest guest)
        {
            // BR-22: Kiểm tra bắt buộc điền FullName và ít nhất phải có 1 phương thức liên lạc (Phone hoặc Email)
            if (string.IsNullOrEmpty(guest.Phone) && string.IsNullOrEmpty(guest.Email))
            {
                ModelState.AddModelError("", "Quy tắc BR-22: Bắt buộc phải nhập ít nhất Số điện thoại hoặc Địa chỉ Email để tạo phương thức liên lạc với khách!");
            }

            if (ModelState.IsValid)
            {
                // BR-26: Kiểm tra trùng lặp thông tin liên hệ để đưa ra cảnh báo (Warning) hệ thống nhưng vẫn cho phép lưu dữ liệu
                bool isDuplicateContact = await _context.Guests.AnyAsync(g => g.Phone == guest.Phone || g.Email == guest.Email);
                if (isDuplicateContact)
                {
                    // Đẩy thông báo cảnh báo qua TempData để giao diện hiển thị cho Lễ tân biết
                    TempData["WarningMessage"] = "Cảnh báo BR-26: Số điện thoại hoặc Email này đã tồn tại trên một hồ sơ khách khác (Hệ thống vẫn ghi nhận vì có thể thuê theo hộ gia đình).";
                }

                guest.CreatedAt = DateTime.Now;

                // Quy tắc BR-25: GuestId tự tăng từ hệ thống, Lễ tân không được can thiệp điền mã thủ công
                _context.Add(guest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        // ====================================================================================
        // MẮT XÍCH 4: CHỈNH SỬA HỒ SƠ KHÁCH HÀNG (MAPPED: UC15.3)
        // ====================================================================================

        /// <summary>
        /// [Use Case: UC15.3 - Edit Guest Profile] (Lấy dữ liệu cũ phục vụ chỉnh sửa)
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var guest = await _context.Guests.FindAsync(id);
            if (guest == null) return NotFound();
            return View(guest);
        }

        /// <summary>
        /// [Use Case: UC15.3 - Edit Guest Profile] (Cập nhật thông tin sửa đổi)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GuestId,FullName,Gender,DateOfBirth,Nationality,IdentityNumber,Phone,Email,Address,CreatedAt")] Guest guest)
        {
            if (id != guest.GuestId) return NotFound();

            // BR-22: Đảm bảo khi sửa vẫn phải tuân thủ luật điền ít nhất 1 thông tin liên hệ
            if (string.IsNullOrEmpty(guest.Phone) && string.IsNullOrEmpty(guest.Email))
            {
                ModelState.AddModelError("", "Quy tắc BR-22: Bắt buộc phải giữ lại ít nhất Số điện thoại hoặc Địa chỉ Email liên lạc!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(guest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GuestExists(guest.GuestId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(guest);
        }

        // ====================================================================================
        // MẮT XÍCH 5: XEM LỊCH SỬ ĐẶT PHÒNG CỦA KHÁCH (MAPPED: UC15.4)
        // ====================================================================================

        /// <summary>
        /// [Use Case: UC15.4 - View Guest History]
        /// Hiển thị toàn bộ danh sách phòng khách đã từng thuê trong quá khứ dưới dạng Read-only (BR-30).
        /// </summary>
        public async Task<IActionResult> History(int? id)
        {
            if (id == null) return NotFound();

            var guest = await _context.Guests.FirstOrDefaultAsync(g => g.GuestId == id);
            if (guest == null) return NotFound();

            ViewBag.GuestName = guest.FullName;
            return View();
        }

        // ====================================================================================
        // HELPER METHODS
        // ====================================================================================

        private bool GuestExists(int id)
        {
            return _context.Guests.Any(e => e.GuestId == id);
        }

        /// <summary>
        /// Thực thi quy tắc bảo mật BR-12: Mã hóa/Che giấu số căn cước hoặc Passport của khách hàng.
        /// Ví dụ: "012345678912" -> "0123******12".
        /// </summary>
        private string MaskIdentityNumber(string identity)
        {
            if (identity.Length < 6) return new string('*', identity.Length);
            return identity.Substring(0, 4) + new string('*', identity.Length - 6) + identity.Substring(identity.Length - 2);
        }
    }
}