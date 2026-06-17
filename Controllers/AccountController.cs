using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelManageSystem.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HotelManageSystem.Controllers
{
    /// <summary>
    /// [Controller Xử Lý] Quản lý toàn bộ nghiệp vụ tài khoản thuộc cụm UC5.
    /// Chỉ dành riêng cho Actor có quyền Admin truy cập (Admin Rights / Root Access).
    /// </summary>
    public class AccountController : Controller
    {
        private readonly HotelDbContext _context;

        /// <summary>
        /// Khởi tạo Controller và tiêm bối cảnh Database (Dependency Injection) từ hệ thống.
        /// </summary>
        public AccountController(HotelDbContext context)
        {
            _context = context;
        }

        // ====================================================================================
        // MẮT XÍCH 1: XEM DANH SÁCH & TÌM KIẾM TÀI KHOẢN (MAPPED: UC5.4 & UC5.5)
        // ====================================================================================

        /// <summary>
        /// [Use Case: UC5.4 - View List Account] & [Use Case: UC5.5 - Search Account]
        /// Hiển thị danh sách toàn bộ tài khoản nhân viên dưới dạng bảng biểu (BR-13).
        /// Hỗ trợ tìm kiếm nâng cao theo từ khóa và phân trang dữ liệu.
        /// </summary>
        /// <param name="searchKeyword">Từ khóa tìm kiếm theo Username, Email, hoặc Số điện thoại.</param>
        /// <param name="roleId">Bộ lọc tài khoản theo phân quyền RoleId.</param>
        /// <returns>Giao diện danh sách tài khoản.</returns>
        public async Task<IActionResult> Index(string searchKeyword, int? roleId)
        {
            // Lưu từ khóa lại giao diện để không bị mất khi reload (UC5.5)
            ViewData["CurrentSearch"] = searchKeyword;
            ViewData["RolesList"] = new SelectList(await _context.Roles.ToListAsync(), "RoleId", "RoleName", roleId);

            // Khởi tạo câu truy vấn nạp kèm bảng liên kết Role
            var accountsQuery = _context.Accounts.Include(a => a.Role).AsQueryable();

            // Thực hiện bộ lọc tìm kiếm nâng cao nếu có từ khóa (UC5.5)
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                accountsQuery = accountsQuery.Where(a => a.Username.Contains(searchKeyword)
                                                      || a.Email!.Contains(searchKeyword)
                                                      || a.Phone!.Contains(searchKeyword));
            }

            // Thực hiện bộ lọc theo quyền hạn nếu được chọn
            if (roleId.HasValue)
            {
                accountsQuery = accountsQuery.Where(a => a.RoleId == roleId.Value);
            }

            var resultList = await accountsQuery.OrderByDescending(a => a.CreatedAt).ToListAsync();

            // Thực thi BR-12: Che giấu số điện thoại bằng dấu sao (*) trước khi đẩy lên View hiển thị
            foreach (var account in resultList)
            {
                if (!string.IsNullOrEmpty(account.Phone))
                {
                    account.Phone = MaskPhoneNumber(account.Phone);
                }
            }

            return View(resultList);
        }

        // ====================================================================================
        // MẮT XÍCH 2: XEM CHI TIẾT TÀI KHOẢN (MAPPED: UC5.6)
        // ====================================================================================

        /// <summary>
        /// [Use Case: UC5.6 - View Detail Account]
        /// Truy xuất và hiển thị thông tin chi tiết cấu hình của một tài khoản cụ thể.
        /// </summary>
        /// <param name="id">Mã định danh duy nhất của tài khoản cần xem (AccountId).</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);

            if (account == null) return NotFound();

            // Thực thi BR-12: Đảm bảo dữ liệu nhạy cảm hiển thị phải được che chắn cẩn thận
            if (!string.IsNullOrEmpty(account.Phone))
            {
                account.Phone = MaskPhoneNumber(account.Phone);
            }

            return View(account);
        }

        // ====================================================================================
        // MẮT XÍCH 3: TẠO MỚI TÀI KHOẢN NHÂN VIÊN (MAPPED: UC5.1)
        // ====================================================================================

        /// <summary>
        /// [Use Case: UC5.1 - Create Account] (Giao diện hiển thị)
        /// Trả về Form nhập liệu để Admin điền thông tin nhân viên mới.
        /// </summary>
        public async Task<IActionResult> Create()
        {
            ViewBag.RoleId = new SelectList(await _context.Roles.ToListAsync(), "RoleId", "RoleName");
            return View();
        }

        /// <summary>
        /// [Use Case: UC5.1 - Create Account] (Xử lý lưu trữ dữ liệu)
        /// Tiếp nhận thông tin, tiến hành kiểm tra ràng buộc nghiệp vụ và tiến hành ghi nhận vào DB.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,PasswordHash,Email,Phone,RoleId")] Account account)
        {
            // BR-02: Kiểm tra tính duy nhất của địa chỉ Email trên toàn hệ thống
            if (await _context.Accounts.AnyAsync(a => a.Email == account.Email))
            {
                ModelState.AddModelError("Email", "Địa chỉ Email này đã tồn tại trong hệ thống khách sạn!");
            }

            // Kiểm tra trùng lặp Username
            if (await _context.Accounts.AnyAsync(a => a.Username == account.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập này đã có người sử dụng!");
            }

            if (ModelState.IsValid)
            {
                // BR-01: Thực hiện mã hóa mật khẩu thô sang định dạng băm MD5
                account.PasswordHash = ComputeMD5Hash(account.PasswordHash);

                account.Status = "Active";
                account.CreatedAt = DateTime.Now;
                account.UpdatedAt = DateTime.Now;

                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoleId = new SelectList(await _context.Roles.ToListAsync(), "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // ====================================================================================
        // MẮT XÍCH 4: CHỈNH SỬA THÔNG TIN TÀI KHOẢN (MAPPED: UC5.2)
        // ====================================================================================

        /// <summary>
        /// [Use Case: UC5.2 - Edit Account] (Giao diện lấy dữ liệu cũ)
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound();

            ViewBag.RoleId = new SelectList(await _context.Roles.ToListAsync(), "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        /// <summary>
        /// [Use Case: UC5.2 - Edit Account] (Cập nhật dữ liệu mới)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Username,PasswordHash,Email,Phone,Status,RoleId,CreatedAt")] Account account)
        {
            if (id != account.AccountId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy bản ghi gốc từ Database để đối chiếu luật bảo vệ
                    var originalAccount = await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.AccountId == id);
                    if (originalAccount == null) return NotFound();

                    // BR-10: Email cannot be changed after account creation (Ngăn chặn cập nhật Email)
                    if (originalAccount.Email != account.Email)
                    {
                        ModelState.AddModelError("Email", "Quy tắc BR-10: Địa chỉ Email không được phép thay đổi sau khi tạo!");
                        ViewBag.RoleId = new SelectList(await _context.Roles.ToListAsync(), "RoleId", "RoleName", account.RoleId);
                        return View(account);
                    }

                    // Xử lý giữ nguyên hoặc cập nhật mật khẩu mới (Nếu có đổi thì mã hóa MD5 - BR-01)
                    if (account.PasswordHash != originalAccount.PasswordHash && !string.IsNullOrEmpty(account.PasswordHash))
                    {
                        account.PasswordHash = ComputeMD5Hash(account.PasswordHash);
                    }
                    else
                    {
                        account.PasswordHash = originalAccount.PasswordHash;
                    }

                    account.UpdatedAt = DateTime.Now;
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.RoleId = new SelectList(await _context.Roles.ToListAsync(), "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // ====================================================================================
        // MẮT XÍCH 5: VÔ HIỆU HÓA/XÓA TÀI KHOẢN (MAPPED: UC5.3)
        // ====================================================================================

        /// <summary>
        /// [Use Case: UC5.3 - Delete Account]
        /// Thực hiện khóa/Vô hiệu hóa (Soft Delete) tài khoản để lưu vết Audit Trail chứ không xóa cứng khỏi DB.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // GIẢ LẬP MOCK CONTEXT: Vì cơ chế Auth làm sau cùng, tạm thời giả lập ID của Admin đang login là số 1
            int currentLoggedInAdminId = 1;

            // BR-11: Admin cannot delete their own currently logged-in account (Chống tự sát tài khoản)
            if (id == currentLoggedInAdminId)
            {
                TempData["ErrorMessage"] = "Quy tắc BR-11: Quyền Quản Trị tối cao không được tự xóa tài khoản của chính mình khi đang đăng nhập!";
                return RedirectToAction(nameof(Index));
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                // Thực hiện Soft-delete theo tài liệu đặc tả hệ thống
                account.Status = "Inactive";
                account.UpdatedAt = DateTime.Now;

                _context.Update(account);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ====================================================================================
        // HỆ THỐNG TRỢ THỦ (HELPER METHODS)
        // ====================================================================================

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }

        /// <summary>
        /// Thực thi quy tắc mã hóa BR-01: Chuyển chuỗi mật khẩu thô thành chuỗi băm MD5.
        /// </summary>
        private string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Thực thi quy tắc bảo mật BR-12: Che giấu số điện thoại bằng ký tự hoa thị (*).
        /// Ví dụ: "0912345678" -> "0912***678".
        /// </summary>
        private string MaskPhoneNumber(string phone)
        {
            if (phone.Length < 7) return new string('*', phone.Length);
            return phone.Substring(0, 4) + "***" + phone.Substring(phone.Length - 3);
        }
    }
}