using System;

namespace HotelManageSystem.Models
{
    /// <summary>
    /// [Entity Model] Đại diện cho bảng 'Guest' (Hồ sơ khách hàng lưu trú) dưới Database.
    /// Lưu trữ toàn bộ thông tin cá nhân và định danh pháp lý của khách hàng đến thuê phòng.
    /// Phục vụ trực tiếp cho cụm nghiệp vụ lễ tân UC15 (Manage Guest Profiles).
    /// </summary>
    public class Guest
    {
        /// <summary>
        /// Mã định danh duy nhất của hồ sơ khách hàng (Khóa chính - Tự tăng).
        /// Quy tắc BR-25: Mã này được hệ thống tự sinh và lễ tân không thể can thiệp chỉnh sửa.
        /// </summary>
        public int GuestId { get; set; }

        /// <summary>
        /// Họ và tên đầy đủ của khách lưu trú.
        /// Ràng buộc BR-22: Đây là trường thông tin bắt buộc phải nhập khi lập hồ sơ khách.
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Giới tính của khách hàng ('Male', 'Female', 'Other').
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// Ngày tháng năm sinh của khách.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Quốc tịch của khách hàng phục vụ cho việc khai báo tạm trú tạm vắng.
        /// </summary>
        public string? Nationality { get; set; }

        /// <summary>
        /// Số CMND, Căn cước công dân hoặc số Hộ chiếu (Passport) của khách.
        /// Lưu ý bảo mật BR-12: Thông tin này bắt buộc phải ẩn/che bằng dấu sao (*) khi hiển thị trên các giao diện chung.
        /// </summary>
        public string? IdentityNumber { get; set; }

        /// <summary>
        /// Số điện thoại liên lạc của khách hàng.
        /// Ràng buộc BR-22: Hệ thống yêu cầu phải có ít nhất Phone hoặc Email để tạo hồ sơ.
        /// Cảnh báo BR-26: Nếu số điện thoại bị trùng với khách cũ, hệ thống chỉ hiển thị cảnh báo chứ không chặn (cho phép dùng chung số gia đình).
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Địa chỉ thư điện tử cá nhân của khách hàng.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Địa chỉ thường trú hoặc tạm trú hiện tại của khách.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Thời điểm hồ sơ khách hàng được khởi tạo trên hệ thống trong lần lưu trú đầu tiên.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}