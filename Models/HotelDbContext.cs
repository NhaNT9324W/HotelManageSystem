using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace HotelManageSystem.Models
{
    /// <summary>
    /// Lớp bối cảnh cơ sở dữ liệu trung tâm (Database Context Hub) của hệ thống.
    /// Quản lý tập trung kết nối, cấu hình Fluent API và thực hiện nạp dữ liệu nền tảng ban đầu (Data Seeding).
    /// </summary>
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Guest> Guests { get; set; } // Quản lý tập hợp thực thể khách hàng lưu trú

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Unique Index bảo đảm tính duy nhất của dữ liệu gốc
            modelBuilder.Entity<Role>().HasIndex(r => r.RoleName).IsUnique();
            modelBuilder.Entity<Account>().HasIndex(a => a.Username).IsUnique();
            modelBuilder.Entity<Account>().HasIndex(a => a.Email).IsUnique(); // BR-02

            // ====================================================================================
            // 1. DATA SEEDING: BẢNG PHÂN QUYỀN (ROLE)
            // ====================================================================================
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", Description = "System Administrator (Quản trị viên tối cao)" },
                new Role { RoleId = 2, RoleName = "Hotel Manager", Description = "Hotel Manager (Quản lý khách sạn)" },
                new Role { RoleId = 3, RoleName = "Receptionist", Description = "Receptionist (Nhân viên lễ tân)" },
                new Role { RoleId = 4, RoleName = "Room Staff", Description = "Room Staff (Nhân viên buồng phòng)" }
            );

            // ====================================================================================
            // 2. DATA SEEDING: BẢNG TÀI KHOẢN NHÂN VIÊN (ACCOUNT)
            // Tuân thủ quy tắc mật khẩu mã hóa định dạng băm MD5 (BR-01)
            // ====================================================================================
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    AccountId = 1,
                    Username = "admin",
                    PasswordHash = "0192023a7bbd73250516f069df18b500", // Chuỗi gốc: admin123
                    Email = "admin@fpt.edu.vn",
                    Phone = "0912345678",
                    Status = "Active",
                    RoleId = 1,
                    CreatedAt = DateTime.Parse("2026-06-17"),
                    UpdatedAt = DateTime.Parse("2026-06-17")
                },
                new Account
                {
                    AccountId = 2,
                    Username = "thanhnha_reception",
                    PasswordHash = "8323c2bd6189ef0627e38ebcf55941da", // Chuỗi gốc: receptionist123
                    Email = "nhantse1902@fpt.edu.vn",
                    Phone = "0987654321",
                    Status = "Active",
                    RoleId = 3,
                    CreatedAt = DateTime.Parse("2026-06-17"),
                    UpdatedAt = DateTime.Parse("2026-06-17")
                }
            );

            // ====================================================================================
            // 3. DATA SEEDING: BẢNG HỒ SƠ KHÁCH HÀNG LƯU TRÚ (GUEST)
            // Phục vụ chạy thử nghiệm luồng nghiệp vụ của phân hệ lễ tân UC15
            // ====================================================================================
            modelBuilder.Entity<Guest>().HasData(
                new Guest
                {
                    GuestId = 1,
                    FullName = "Nguyễn Hoàng Nam",
                    Gender = "Male",
                    DateOfBirth = DateTime.Parse("1995-08-20"),
                    Nationality = "Việt Nam",
                    IdentityNumber = "079205001234", // Sẽ tự động che hoa thị ở danh sách hiển thị (BR-12)
                    Phone = "0909123456",
                    Email = "hoangnam95@gmail.com",
                    Address = "123 Nguyễn Trãi, Phường 2, Quận 5, TP. Hồ Chí Minh",
                    CreatedAt = DateTime.Parse("2026-06-10")
                },
                new Guest
                {
                    GuestId = 2,
                    FullName = "Trần Thị Mai Chi",
                    Gender = "Female",
                    DateOfBirth = DateTime.Parse("1999-11-05"),
                    Nationality = "Việt Nam",
                    IdentityNumber = "082201005678",
                    Phone = "0938987654",
                    Email = "maichi.tran@fpt.edu.vn",
                    Address = "456 Đường Nguyễn Văn Cừ, Quận Ninh Kiều, TP. Cần Thơ",
                    CreatedAt = DateTime.Parse("2026-06-12")
                },
                new Guest
                {
                    GuestId = 3,
                    FullName = "David Chopper",
                    Gender = "Male",
                    DateOfBirth = DateTime.Parse("1991-03-15"),
                    Nationality = "United Kingdom",
                    IdentityNumber = "9876543210", // Số Hộ chiếu quốc tế (Passport)
                    Phone = "0855222333",
                    Email = "david.chopper@uk-mail.com",
                    Address = "London, United Kingdom",
                    CreatedAt = DateTime.Parse("2026-06-16")
                }
            );
        }
    }
}