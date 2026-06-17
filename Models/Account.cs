using System;

namespace HotelManageSystem.Models
{
    /// <summary>
    /// [Entity Model] Đại diện cho bảng 'Account' (Tài khoản người dùng) dưới Database.
    /// Lưu trữ thông tin định danh, thông tin liên hệ và trạng thái hoạt động của nhân viên/quản lý khách sạn.
    /// Thuộc phạm vi xử lý của cụm tính năng UC5 (Manage Account) thuộc quyền Admin.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Mã định danh duy nhất của tài khoản (Khóa chính - Tự tăng).
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Tên đăng nhập của tài khoản dùng để truy cập hệ thống.
        /// Ràng buộc: Bắt buộc điền và không được trùng lặp (UNIQUE).
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// Chuỗi mật khẩu đã được mã hóa bằng thuật toán MD5 (Quy tắc BR-01).
        /// Tuyệt đối không lưu mật khẩu dạng Plaintext để đảm bảo an toàn.
        /// </summary>
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// Thư điện tử liên hệ của nhân viên.
        /// Ràng buộc BR-02: Địa chỉ Email phải là duy nhất trên toàn hệ thống.
        /// Ràng buộc BR-10: Không cho phép thay đổi Email sau khi tài khoản đã được tạo thành công.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Số điện thoại liên lạc của chủ tài khoản.
        /// Lưu ý bảo mật BR-12: Khi hiển thị ở danh sách API công khai, thông tin này phải được che bằng dấu sao (*).
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Trạng thái hoạt động của tài khoản (Mặc định: 'Active', hoặc cấu hình 'Inactive' khi soft-delete).
        /// </summary>
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Thời điểm tài khoản được khởi tạo lần đầu trên hệ thống.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Thời điểm tài khoản có sự thay đổi hoặc cập nhật thông tin gần nhất.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Mã phân quyền liên kết (Khóa ngoại trỏ đến bảng Role).
        /// Xác định nhóm chức năng được phép truy cập theo ma trận phân quyền.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Đối tượng liên kết phân quyền chi tiết (Lazy Loading / Eager Loading).
        /// </summary>
        public virtual Role Role { get; set; } = null!;
    }
}