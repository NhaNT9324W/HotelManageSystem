using System.Collections.Generic;

namespace HotelManageSystem.Models
{
    /// <summary>
    /// [Entity Model] Đại diện cho bảng 'Role' (Phân quyền hệ thống) dưới Database.
    /// Quản lý danh sách các vai trò/quyền hạn truy cập cốt lõi của nền tảng bao gồm: Admin, Hotel Manager, Receptionist, và Room Staff.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Mã định danh duy nhất của phân quyền (Khóa chính - Tự tăng).
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Tên của quyền hạn truy cập (Ví dụ: Admin, Hotel Manager,...).
        /// Ràng buộc: Bắt buộc điền (NOT NULL) và không được trùng lặp (UNIQUE).
        /// </summary>
        public string RoleName { get; set; } = null!;

        /// <summary>
        /// Mô tả chi tiết về phạm vi và trách nhiệm của quyền hạn này.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Thuộc tính điều hướng (Navigation Property) thể hiện quan hệ một-nhiều.
        /// Một quyền hạn có thể được gán cho nhiều tài khoản nhân viên khác nhau.
        /// </summary>
        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}