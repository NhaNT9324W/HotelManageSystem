using HotelManageSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ cơ bản của mô hình MVC vào DI Container
builder.Services.AddControllersWithViews();

// ============================================================================
// ĐĂNG KÝ KẾT NỐI DATABASE CHUẨN ĐÚNG THEO YÊU CẦU MÔN PRN222
// ============================================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HotelDbContext>(options =>
    options.UseSqlServer(connectionString)); // Trình cung cấp dữ liệu MS SQL Server

// ============================================================================
var app = builder.Build();

// ====================================================================================
// CẤU HÌNH TỰ ĐỘNG CẬP NHẬT DATABASE KHI KHỞI CHẠY (DDL-AUTO=UPDATE STYLE)
// ====================================================================================
using (var scope = app.Services.CreateScope())
{
    try
    {
        /// <summary>
        /// Khởi tạo một Scope tạm thời để triệu gọi bối cảnh Database Context trung tâm.
        /// </summary>
        var dbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();

        /// <summary>
        /// Hàm Migrate() tự động đối chiếu các file bản vẽ trong thư mục Migrations.
        /// Nếu phát hiện có bảng mới (như bảng Guests) hoặc cột mới chưa có dưới SQL Server, app sẽ tự động cập nhật ngầm.
        /// </summary>
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Ghi nhận nhật ký lỗi hệ thống nếu quá trình kết nối hoặc đồng bộ SQL Server thất bại
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi nghiêm trọng trong quá trình tự động đồng bộ hóa cơ sở dữ liệu vật lý!");
    }
}
// ====================================================================================

// Cấu hình các đường ống xử lý HTTP Request Pipeline của ứng dụng Web
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Kích hoạt nạp các file giao diện tĩnh (CSS/JS) trong wwwroot

app.UseRouting();

app.UseAuthorization();

// Định tuyến mặc định của hệ thống MVC: Tự động trỏ về HomeController khi khởi động
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();