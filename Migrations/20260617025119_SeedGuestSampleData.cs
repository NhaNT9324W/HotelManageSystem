using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelManageSystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedGuestSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "GuestId", "Address", "CreatedAt", "DateOfBirth", "Email", "FullName", "Gender", "IdentityNumber", "Nationality", "Phone" },
                values: new object[,]
                {
                    { 1, "123 Nguyễn Trãi, Phường 2, Quận 5, TP. Hồ Chí Minh", new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1995, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "hoangnam95@gmail.com", "Nguyễn Hoàng Nam", "Male", "079205001234", "Việt Nam", "0909123456" },
                    { 2, "456 Đường Nguyễn Văn Cừ, Quận Ninh Kiều, TP. Cần Thơ", new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1999, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "maichi.tran@fpt.edu.vn", "Trần Thị Mai Chi", "Female", "082201005678", "Việt Nam", "0938987654" },
                    { 3, "London, United Kingdom", new DateTime(2026, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1991, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "david.chopper@uk-mail.com", "David Chopper", "Male", "9876543210", "United Kingdom", "0855222333" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Guests",
                keyColumn: "GuestId",
                keyValue: 3);
        }
    }
}
