# TÀI LIỆU HỆ THỐNG – HOTEL MANAGEMENT SYSTEM (HMS)

**Dự án:** Hotel Management System  
**Mã dự án:** SE1902-G3  
**Môn học:** SWD392  
**Nhóm:** Group 3  
**Loại phần mềm:** Web Application  
**Phiên bản tài liệu:** v1.0  
**Ngày cập nhật:** Tháng 6, 2026

---

## MỤC LỤC

1. [Mô tả tổng quan hệ thống](#1-mô-tả-tổng-quan-hệ-thống)
2. [Phân quyền hệ thống](#2-phân-quyền-hệ-thống)
3. [Vấn đề Login / Logout – Làm sau](#3-vấn-đề-login--logout--làm-sau)
4. [Danh sách Use Case và phân quyền](#4-danh-sách-use-case-và-phân-quyền)
5. [Mô tả chi tiết từng nhóm UC](#5-mô-tả-chi-tiết-từng-nhóm-uc)
6. [Context giữa các Use Case](#6-context-giữa-các-use-case)
7. [Background Services & Scheduled Jobs](#7-background-services--scheduled-jobs)
8. [Trạng thái phòng – State Machine](#8-trạng-thái-phòng--state-machine)
9. [Database chính cần có](#9-database-chính-cần-có)
10. [Phân công thực hiện](#10-phân-công-thực-hiện)

---

## 1. Mô tả tổng quan hệ thống

### 1.1 Mục tiêu

Hệ thống **Hotel Management System (HMS)** là ứng dụng web giúp số hóa và tập trung hóa toàn bộ vận hành khách sạn, thay thế quy trình thủ công trên giấy tờ và điện thoại. Hệ thống phục vụ các bộ phận: lễ tân, buồng phòng, quản lý và quản trị hệ thống.

### 1.2 Các tính năng chính

| Mã tính năng | Tên tính năng | Mô tả ngắn |
|---|---|---|
| F01 | Manage Reservations | Tạo, xem, chỉnh sửa, hủy đặt phòng; check-in/check-out |
| F02 | Manage Guests | Xem, chỉnh sửa thông tin khách; lịch sử đặt phòng |
| F03 | Manage Rooms | Tìm kiếm, xem trạng thái, cập nhật, tạo mới, quản lý loại phòng |
| F04 | Manage Invoices | Tạo hóa đơn, tính phí, cập nhật trạng thái thanh toán |
| F05 | Manage Room Tasks | Xem nhiệm vụ, báo cáo hư hỏng, gửi yêu cầu bảo trì |
| F06 | View Reports & Statistics | Báo cáo công suất, thống kê đặt phòng, dashboard vận hành |
| F07 | Manage Staff & Permissions | Tạo tài khoản, phân quyền, vô hiệu hóa tài khoản |
| F08 | View Dashboard | Xem tổng quan hiệu suất khách sạn, log hoạt động hệ thống |

### 1.3 Các actor (người dùng hệ thống)

| # | Actor | Mô tả |
|---|---|---|
| 1 | **Admin** | Toàn quyền hệ thống: quản lý tài khoản, cấu hình, phân quyền, thông tin khách sạn |
| 2 | **Hotel Manager** | Giám sát vận hành hằng ngày, xem dashboard, quản lý phòng/loại phòng/dịch vụ, tạo báo cáo |
| 3 | **Receptionist** | Quản lý đặt phòng, check-in/check-out, hồ sơ khách, hóa đơn, dịch vụ |
| 4 | **Room Staff** | Xem nhiệm vụ housekeeping được giao, cập nhật trạng thái phòng, báo cáo sự cố |

### 1.4 Hệ thống ngoài (External Systems – tương lai)

| Hệ thống | Mục đích |
|---|---|
| Payment Gateway | Xử lý thanh toán online |
| Online Booking Platforms | Nhận đặt phòng từ Agoda, Booking.com |
| E-invoicing Service | Tạo và gửi hóa đơn điện tử |
| Automated Notification System | Gửi email/SMS xác nhận, nhắc nhở cho khách và nhân viên |

---

## 2. Phân quyền hệ thống

### 2.1 Ma trận phân quyền tổng quát

| Chức năng | Admin | Hotel Manager | Receptionist | Room Staff |
|---|:---:|:---:|:---:|:---:|
| **Xác thực (Auth)** | | | | |
| Login / Logout | ✅ | ✅ | ✅ | ✅ |
| Forgot Password | ✅ | ✅ | ✅ | ✅ |
| Change Password | ✅ | ✅ | ✅ | ✅ |
| **Quản lý tài khoản** | | | | |
| Create / Edit / Delete Account | ✅ | ❌ | ❌ | ❌ |
| View List / Detail Account | ✅ | ❌ | ❌ | ❌ |
| Search Account | ✅ | ❌ | ❌ | ❌ |
| **Thông tin khách sạn** | | | | |
| Edit Hotel Information | ✅ | ❌ | ❌ | ❌ |
| **Quản lý phòng** | | | | |
| Create / Edit / Delete Room | ✅ | ✅ | ❌ | ❌ |
| View List / Detail / Search Room | ✅ | ✅ | ❌ | ❌ |
| **Quản lý loại phòng** | | | | |
| Create / Edit / Delete Room Type | ✅ | ✅ | ❌ | ❌ |
| View / Search Room Type | ✅ | ✅ | ❌ | ❌ |
| **Quản lý dịch vụ** | | | | |
| Create / Edit / Delete Service | ✅ | ✅ | ❌ | ❌ |
| View / Search Service | ✅ | ✅ | ❌ | ❌ |
| **Dashboard & Báo cáo** | | | | |
| View Dashboard | ❌ | ✅ | ❌ | ❌ |
| Generate / Export Reports | ❌ | ✅ | ❌ | ❌ |
| View System Logs | ✅ | ❌ | ❌ | ❌ |
| **Đặt phòng** | | | | |
| Create / Edit / Cancel Reservation | ❌ | ❌ | ✅ | ❌ |
| View List / Detail / Search Reservation | ❌ | ❌ | ✅ | ❌ |
| Check Room Availability | ❌ | ❌ | ✅ | ❌ |
| **Hồ sơ khách** | | | | |
| Create / Edit Guest Profile | ❌ | ❌ | ✅ | ❌ |
| View Guest Profile / History | ❌ | ❌ | ✅ | ❌ |
| **Check-in / Check-out** | | | | |
| Process Check-in (Verify + Assign Room) | ❌ | ❌ | ✅ | ❌ |
| Process Check-out (Invoice + Payment) | ❌ | ❌ | ✅ | ❌ |
| Record Service Usage | ❌ | ❌ | ✅ | ❌ |
| **Housekeeping** | | | | |
| View Assigned Tasks | ❌ | ❌ | ❌ | ✅ |
| Update Room Status (Clean/Dirty/Ready) | ❌ | ❌ | ❌ | ✅ |
| Report Maintenance Issues | ❌ | ❌ | ❌ | ✅ |

> **Ghi chú quan trọng:**
> - Admin **không trực tiếp** thực hiện nghiệp vụ đặt phòng hay housekeeping – chỉ cấu hình hệ thống.
> - Room Staff **chỉ** được thay đổi trạng thái vật lý (Dirty → Clean → Ready). **Không được** đụng đến trạng thái booking (Reserved, Occupied, Checked-out).
> - Hotel Manager **không** quản lý tài khoản nhân viên – đó là việc của Admin.

### 2.2 Mô tả chi tiết quyền từng role

#### 🔴 Admin
- Toàn quyền hệ thống.
- Tạo, chỉnh sửa, xóa tài khoản cho tất cả các role.
- Cấu hình thông tin khách sạn (tên, địa chỉ, chính sách, liên hệ).
- Quản lý phòng và loại phòng (tạo, sửa, xóa).
- Quản lý dịch vụ khách sạn.
- Xem system logs / audit trail.
- **Không** xem dashboard hoạt động hằng ngày (đó là của Manager).

#### 🟠 Hotel Manager
- Giám sát toàn bộ hoạt động hằng ngày qua Dashboard.
- Quản lý phòng, loại phòng, dịch vụ (nhưng không thể tạo tài khoản).
- Xem và xuất các báo cáo: công suất, doanh thu, tài chính, hiệu suất nhân viên.
- Theo dõi trạng thái phòng real-time.
- Nhận cảnh báo khi Room Staff báo cáo sự cố nghiêm trọng.

#### 🟡 Receptionist
- Quản lý toàn bộ quy trình đặt phòng (tạo, sửa, hủy).
- Kiểm tra phòng trống trước khi đặt.
- Thực hiện check-in: xác nhận đặt phòng, gán phòng.
- Thực hiện check-out: tạo hóa đơn, tính phí, xử lý thanh toán.
- Quản lý hồ sơ và lịch sử khách hàng.
- Ghi nhận dịch vụ khách đã sử dụng trong kỳ lưu trú.

#### 🟢 Room Staff
- Giao diện đơn giản, tập trung vào nhiệm vụ được giao.
- Xem danh sách phòng được phân công.
- Cập nhật trạng thái dọn phòng: `Dirty → Cleaning → Clean → Ready`.
- Báo cáo sự cố: chọn danh mục, mô tả, ảnh (tùy chọn), mức độ nghiêm trọng.
- Khi báo cáo mức Critical → hệ thống tự động khóa phòng (`Under Maintenance`).

---

## 3. Vấn đề Login / Logout – Làm sau

### 3.1 Lý do làm sau

> **Quyết định kỹ thuật:** Login/Logout và toàn bộ cơ chế Authentication được cố tình **để lại làm sau cùng**, sau khi các UC nghiệp vụ chính đã hoàn thành. Lý do:

1. **Dễ test hơn trong giai đoạn phát triển:** Khi chưa có auth, dev có thể gọi thẳng API hoặc mở trang mà không cần token. Test nhanh hơn, không bị block.
2. **Tách biệt concern:** Các UC nghiệp vụ (đặt phòng, quản lý phòng, housekeeping...) cần được ổn định trước. Auth là lớp bảo vệ bên ngoài – có thể plug vào sau.
3. **Mock role khi test:** Trong quá trình phát triển, có thể dùng một biến config đơn giản (ví dụ `CURRENT_ROLE=receptionist`) để giả lập role mà không cần đăng nhập thực.
4. **Tránh bị chặn sớm:** Nếu làm auth trước, mọi UC đều phải có token mới test được → chậm tiến độ không cần thiết.

### 3.2 Kế hoạch triển khai Auth

| Thứ tự | Hạng mục | Mô tả |
|---|---|---|
| Bước 1 | Hoàn thành tất cả UC nghiệp vụ (UC5 → UC21) | Code xong, test logic hoạt động với mock role |
| Bước 2 | Xây dựng `AuthService` (JWT) | Validate token, kiểm tra role, trả về user context |
| Bước 3 | Implement UC1 – Login | API `/api/auth/login` → trả về JWT token |
| Bước 4 | Implement UC2 – Logout | Invalidate token (blacklist hoặc refresh token rotation) |
| Bước 5 | Implement UC3 – Forgot Password | Gửi email reset, validate token reset, đặt mật khẩu mới |
| Bước 6 | Implement UC4 – Change Password | User đăng nhập → đổi mật khẩu hiện tại |
| Bước 7 | Plug AuthService vào toàn bộ controller | Mọi request đều đi qua middleware kiểm tra JWT + role |
| Bước 8 | Integration test toàn hệ thống với auth | Test đủ 4 role, kiểm tra từ chối truy cập đúng chỗ |

### 3.3 Thiết kế tạm thời khi chưa có Auth

```
// Dev mode: bypass auth, mock user context
const mockUser = {
  id: "dev-user-001",
  role: "receptionist", // thay đổi để test role khác
  name: "Dev Tester"
};
```

Khi auth hoàn chỉnh: thay `mockUser` bằng kết quả decode từ JWT token thực.

### 3.4 Thông tin kỹ thuật Auth (khi làm)

- **Cơ chế:** JWT (JSON Web Token)
- **Lưu trữ token:** HttpOnly Cookie (bảo mật hơn localStorage)
- **Token expiry:** Access Token 15 phút, Refresh Token 7 ngày
- **Endpoint chính:**
  - `POST /api/auth/login`
  - `POST /api/auth/logout`
  - `POST /api/auth/refresh`
  - `POST /api/auth/forgot-password`
  - `POST /api/auth/reset-password`
  - `PUT /api/auth/change-password`
- **Middleware:** Mọi request tới `/api/*` đều qua `AuthMiddleware` → validate JWT → inject `req.user` → controller kiểm tra role.

---

## 4. Danh sách Use Case và phân quyền

> **Chú ý thứ tự ưu tiên code:** UC được đánh dấu 🔴 = làm trước; Login/Logout = làm sau cùng.

### 4.1 Authentication (⚠️ Làm sau)

| UC ID | Use Case | Actor | Ưu tiên |
|---|---|---|---|
| UC1 | Login | All (Admin, Manager, Receptionist, Room Staff) | ⏳ Sau |
| UC2 | Logout | All | ⏳ Sau |
| UC3 | Forgot Password | All | ⏳ Sau |
| UC4 | Change Password | All | ⏳ Sau |

### 4.2 Admin – Quản lý tài khoản & hệ thống

| UC ID | Use Case | Actor | Ưu tiên |
|---|---|---|---|
| UC5.0 | Manage Account | Admin | 🔴 Cao |
| UC5.1 | Create Account | Admin | 🔴 Cao |
| UC5.2 | Edit Account | Admin | 🔴 Cao |
| UC5.3 | Delete Account | Admin | 🔴 Cao |
| UC5.4 | View List Account | Admin | 🔴 Cao |
| UC5.5 | Search Account | Admin | 🔴 Cao |
| UC5.6 | View Detail Account | Admin | 🔴 Cao |
| UC6 | Edit Hotel Information | Admin | 🟡 Trung bình |
| UC8 | View System Logs | Admin | 🟡 Trung bình |

### 4.3 Admin + Hotel Manager – Quản lý phòng, loại phòng, dịch vụ

| UC ID | Use Case | Actor | Ưu tiên |
|---|---|---|---|
| UC7.0 | Manage Room | Admin, Hotel Manager | 🔴 Cao |
| UC7.1 | Create Room | Admin, Hotel Manager | 🔴 Cao |
| UC7.2 | Edit Room | Admin, Hotel Manager | 🔴 Cao |
| UC7.3 | View List Room | Admin, Hotel Manager | 🔴 Cao |
| UC7.4 | Search Room | Admin, Hotel Manager | 🔴 Cao |
| UC7.5 | View Detail Room | Admin, Hotel Manager | 🔴 Cao |
| UC7.6 | Delete Room | Admin, Hotel Manager | 🔴 Cao |
| UC9.0 | Manage Room Type | Admin, Hotel Manager | 🔴 Cao |
| UC9.1 | Create Room Type | Admin, Hotel Manager | 🔴 Cao |
| UC9.2 | Edit Room Type | Admin, Hotel Manager | 🔴 Cao |
| UC9.3 | Delete Room Type | Admin, Hotel Manager | 🔴 Cao |
| UC9.4 | View List Room Type | Admin, Hotel Manager | 🔴 Cao |
| UC9.5 | View Detail Room Type | Admin, Hotel Manager | 🔴 Cao |
| UC9.6 | Search Room Type | Admin, Hotel Manager | 🔴 Cao |
| UC10.0 | Manage Hotel Services | Admin, Hotel Manager | 🟡 Trung bình |
| UC10.1 | View List Services | Admin, Hotel Manager | 🟡 Trung bình |
| UC10.2 | View Detail Service | Admin, Hotel Manager | 🟡 Trung bình |
| UC10.3 | Create Service | Admin, Hotel Manager | 🟡 Trung bình |
| UC10.4 | Edit Service | Admin, Hotel Manager | 🟡 Trung bình |
| UC10.5 | Search Service | Admin, Hotel Manager | 🟡 Trung bình |
| UC10.6 | Delete Service | Admin, Hotel Manager | 🟡 Trung bình |

### 4.4 Hotel Manager – Dashboard & Báo cáo

| UC ID | Use Case | Actor | Ưu tiên |
|---|---|---|---|
| UC11 | View Dashboard | Hotel Manager | 🔴 Cao |
| UC12.0 | Manage Hotel Report | Hotel Manager | 🟡 Trung bình |
| UC12.1 | Generate Occupancy Report | Hotel Manager | 🟡 Trung bình |
| UC12.2 | Generate Revenue Report | Hotel Manager | 🟡 Trung bình |
| UC12.3 | Generate Financial Report | Hotel Manager | 🟡 Trung bình |
| UC12.4 | Generate Staff Performance Report | Hotel Manager | 🟡 Trung bình |
| UC12.5 | Export Reports | Hotel Manager | 🟡 Trung bình |

### 4.5 Receptionist – Đặt phòng & Khách hàng

| UC ID | Use Case | Actor | Ưu tiên |
|---|---|---|---|
| UC13.0 | Manage Reservations | Receptionist | 🔴 Cao |
| UC13.1 | View List Reservations | Receptionist | 🔴 Cao |
| UC13.2 | View Detail Reservation | Receptionist | 🔴 Cao |
| UC13.3 | Create Reservation | Receptionist | 🔴 Cao |
| UC13.4 | Search Reservation | Receptionist | 🔴 Cao |
| UC13.5 | Edit Reservation | Receptionist | 🔴 Cao |
| UC13.6 | Cancel Reservation | Receptionist | 🔴 Cao |
| UC14 | Check Room Availability | Receptionist | 🔴 Cao |
| UC15.0 | Manage Guest Profiles | Receptionist | 🔴 Cao |
| UC15.1 | Create Guest Information | Receptionist | 🔴 Cao |
| UC15.2 | View Guest Profile | Receptionist | 🔴 Cao |
| UC15.3 | Edit Guest Profile | Receptionist | 🔴 Cao |
| UC15.4 | View Guest History | Receptionist | 🟡 Trung bình |

### 4.6 Receptionist – Check-in / Check-out

| UC ID | Use Case | Actor | Ưu tiên |
|---|---|---|---|
| UC16.0 | Process Check-in | Receptionist | 🔴 Cao |
| UC16.1 | Verify Reservation | Receptionist | 🔴 Cao |
| UC16.2 | Assign Room | Receptionist | 🔴 Cao |
| UC17 | Record Service Usage | Receptionist | 🟡 Trung bình |
| UC18.0 | Process Check-out | Receptionist | 🔴 Cao |
| UC18.1 | Generate Invoice | Receptionist | 🔴 Cao |
| UC18.2 | Calculate Charges | Receptionist | 🔴 Cao |
| UC18.3 | Process Payment | Receptionist | 🔴 Cao |

### 4.7 Room Staff – Housekeeping

| UC ID | Use Case | Actor | Ưu tiên |
|---|---|---|---|
| UC19.0 | Manage Housekeeping Tasks | Room Staff | 🔴 Cao |
| UC19.1 | View Assigned Tasks | Room Staff | 🔴 Cao |
| UC20 | Update Room Status | Room Staff | 🔴 Cao |
| UC21 | Report Room Maintenance Issues | Room Staff | 🟡 Trung bình |

---

## 5. Mô tả chi tiết từng nhóm UC

### 5.1 UC5 – Manage Account (Admin)

**Mục đích:** Admin quản lý tất cả tài khoản nhân viên trong hệ thống.

**Các sub-UC:**

- **UC5.1 Create Account:** Admin nhập thông tin (họ tên, email, role, mật khẩu tạm). Hệ thống validate và tạo tài khoản. Gửi email thông báo cho nhân viên mới.
- **UC5.2 Edit Account:** Sửa thông tin cá nhân, đổi role, thay đổi trạng thái tài khoản (Active/Inactive).
- **UC5.3 Delete Account:** Vô hiệu hóa (soft delete) tài khoản. Không xóa cứng để giữ audit trail.
- **UC5.4 View List Account:** Danh sách tất cả tài khoản, có phân trang.
- **UC5.5 Search Account:** Tìm kiếm theo tên, email, role, trạng thái.
- **UC5.6 View Detail Account:** Xem thông tin chi tiết một tài khoản.

**Business Rules:**
- Email phải là duy nhất trong hệ thống.
- Không thể xóa tài khoản đang có reservation hoạt động.
- Admin không thể tự xóa tài khoản của chính mình.

---

### 5.2 UC7 – Manage Room (Admin, Hotel Manager)

**Mục đích:** Quản lý thông tin phòng trong khách sạn.

**Thông tin phòng:** số phòng, tầng, loại phòng, giá, sức chứa, mô tả, trạng thái, ảnh.

**Các sub-UC:**
- **UC7.1 Create Room:** Nhập thông tin phòng mới. Phải chọn Room Type đã có.
- **UC7.2 Edit Room:** Chỉnh sửa thông tin phòng (giá, mô tả, ảnh,...). Không đổi số phòng.
- **UC7.3 View List Room:** Danh sách phòng, lọc theo tầng, loại phòng, trạng thái.
- **UC7.4 Search Room:** Tìm theo số phòng, loại, tầng.
- **UC7.5 View Detail Room:** Xem chi tiết 1 phòng.
- **UC7.6 Delete Room:** Vô hiệu hóa phòng (không xóa nếu có reservation đang hoạt động).

**Business Rules:**
- Số phòng phải là duy nhất.
- Không thể xóa phòng đang có khách (trạng thái Occupied/Reserved).
- Phải thuộc một Room Type.

---

### 5.3 UC9 – Manage Room Type (Admin, Hotel Manager)

**Mục đích:** Quản lý danh mục loại phòng (Standard, Deluxe, Family, VIP Suite,...).

**Thông tin Room Type:** tên, mô tả, giá cơ bản, tiện nghi, sức chứa tối đa, ảnh đại diện.

**Business Rules:**
- Không thể xóa Room Type nếu còn Room thuộc loại đó.
- Tên loại phòng phải là duy nhất.

---

### 5.4 UC10 – Manage Hotel Services (Admin, Hotel Manager)

**Mục đích:** Quản lý các dịch vụ bổ sung (spa, giặt ủi, room service, xe đưa đón,...).

**Thông tin Service:** tên dịch vụ, mô tả, đơn giá, đơn vị tính, trạng thái (Active/Inactive).

**Business Rules:**
- Không thể xóa service nếu đang được dùng trong reservation đang hoạt động.

---

### 5.5 UC11 – View Dashboard (Hotel Manager)

**Mục đích:** Xem tổng quan hoạt động khách sạn theo thời gian thực.

**Nội dung dashboard:**
- Tỷ lệ phòng đang có khách (Occupancy Rate) hôm nay
- Số đặt phòng mới trong ngày
- Doanh thu hôm nay / tuần này / tháng này
- Danh sách check-in / check-out hôm nay
- Phòng đang cần dọn dẹp / bảo trì
- Cảnh báo sự cố từ Room Staff (nếu có)

---

### 5.6 UC12 – Manage Hotel Report (Hotel Manager)

**Các báo cáo:**
- **Occupancy Report:** Tỷ lệ lấp đầy theo ngày/tuần/tháng.
- **Revenue Report:** Doanh thu theo ngày/tháng, theo loại phòng.
- **Financial Report:** Tổng thu, phân tích chi tiết.
- **Staff Performance Report:** Theo dõi hoạt động nhân viên.
- **Export:** Xuất file PDF hoặc Excel. Báo cáo lớn xử lý bất đồng bộ, trả về `jobId`, thông báo khi hoàn thành.

---

### 5.7 UC13 – Manage Reservations (Receptionist)

**Quy trình tạo đặt phòng:**
1. Receptionist kiểm tra phòng trống (UC14).
2. Nhập thông tin khách (hoặc tìm khách đã có trong hệ thống).
3. Chọn phòng, ngày check-in/check-out.
4. Ghi nhận yêu cầu đặc biệt và dịch vụ muốn dùng.
5. Xác nhận và lưu đặt phòng. Trạng thái: `Reserved`.

**Trạng thái Reservation:**
```
Reserved → [Check-in] → Checked-in → [Check-out] → Checked-out
Reserved → [Hủy] → Cancelled
Reserved → [Hết hạn check-in] → Expired (auto bởi cron job)
```

**Business Rules:**
- Không thể đặt phòng đang Occupied hoặc Under Maintenance.
- Check-out date phải sau check-in date.
- Không thể hủy reservation khi khách đã check-in.

---

### 5.8 UC14 – Check Room Availability (Receptionist)

**Mục đích:** Kiểm tra phòng trống trước khi tạo đặt phòng.

**Đầu vào:** ngày check-in, ngày check-out, loại phòng (tùy chọn), sức chứa (tùy chọn).

**Đầu ra:** Danh sách phòng Available trong khoảng thời gian đó.

**Liên kết:** Gọi `RoomAvailabilityService` (API nội bộ `/api/internal/room-status`).

---

### 5.9 UC15 – Manage Guest Profiles (Receptionist)

**Thông tin khách:** họ tên, CMND/Passport, quốc tịch, ngày sinh, email, số điện thoại, địa chỉ.

**UC15.4 View Guest History:** Xem toàn bộ lịch sử lưu trú của khách trong quá khứ.

**Business Rules:**
- Số CMND/Passport là duy nhất (dùng để nhận diện khách tái lưu trú).
- Không xóa hồ sơ khách – chỉ lưu lịch sử.

---

### 5.10 UC16 – Process Check-in (Receptionist)

**Quy trình:**
1. Receptionist tìm reservation (theo tên, mã đặt phòng).
2. **UC16.1 Verify Reservation:** Xác nhận thông tin khách, ngày giờ, phòng.
3. **UC16.2 Assign Room:** Gán phòng cụ thể (nếu chưa gán lúc đặt). Kiểm tra phòng đang `Available` và `Clean/Ready`.
4. Cập nhật trạng thái reservation → `Checked-in`.
5. Cập nhật trạng thái phòng → `Occupied`.

---

### 5.11 UC17 – Record Service Usage (Receptionist)

**Mục đích:** Ghi nhận dịch vụ khách sử dụng trong thời gian lưu trú (room service, spa,...).

**Thông tin:** dịch vụ, số lượng, thời gian sử dụng, phòng. Dữ liệu này sẽ được tính vào hóa đơn lúc check-out.

---

### 5.12 UC18 – Process Check-out (Receptionist)

**Quy trình:**
1. Receptionist mở reservation đang `Checked-in`.
2. **UC18.1 Generate Invoice:** Hệ thống tạo hóa đơn tổng hợp.
3. **UC18.2 Calculate Charges:** Tính phí phòng theo số đêm + dịch vụ đã dùng + thuế/phí.
4. **UC18.3 Process Payment:** Khách thanh toán (tiền mặt hoặc thẻ). Hệ thống ghi nhận, tạo receipt.
5. Cập nhật trạng thái reservation → `Checked-out`.
6. Cập nhật trạng thái phòng → `Dirty` (chờ housekeeping).

**Business Rules:**
- Check-out chỉ hoàn tất khi thanh toán thành công.
- Phí = (số đêm × giá phòng) + tổng dịch vụ + thuế.

---

### 5.13 UC19 – Manage Housekeeping Tasks (Room Staff)

**Quy trình:**
1. Room Staff mở trang Housekeeping Tasks.
2. Hệ thống hiển thị danh sách phòng được giao.
3. Room Staff chọn phòng → xem chi tiết trạng thái.
4. Cập nhật trạng thái (UC20) hoặc báo cáo sự cố (UC21).

---

### 5.14 UC20 – Update Room Status (Room Staff)

**Chuyển trạng thái được phép (Room Staff):**
```
Dirty → Cleaning → Clean → Ready (for Check-in)
Clean → Under Maintenance (nếu phát hiện sự cố)
```

**Chuyển trạng thái bị CẤM với Room Staff:**
```
❌ Không được đụng: Reserved, Occupied, Checked-out
```

**Business Rules:**
- Phòng không thể chuyển từ Dirty thẳng sang Ready (phải qua Cleaning và Clean).
- Mọi thay đổi trạng thái đều được log vào audit trail.
- UI phải thân thiện mobile (tablet/điện thoại): nút to, dễ bấm.

---

### 5.15 UC21 – Report Room Maintenance Issues (Room Staff)

**Form báo cáo:**
- Danh mục sự cố: Plumbing, Electrical, Furniture, AC, Other.
- Mô tả chi tiết (bắt buộc).
- Ảnh minh chứng (tùy chọn, hỗ trợ camera).
- Mức độ: Minor / Critical.

**Khi severity = Critical:**
- Hệ thống **tự động** đổi trạng thái phòng → `Under Maintenance`.
- Receptionist **không thể** gán phòng này cho khách mới.
- Gửi cảnh báo ngay lên Dashboard của Hotel Manager và Receptionist.

---

## 6. Context giữa các Use Case

### 6.1 Sơ đồ luồng nghiệp vụ chính

```
┌─────────────────────────────────────────────────────────────────┐
│                    LUỒNG ĐẶT PHÒNG → CHECK-OUT                 │
│                                                                   │
│  [Receptionist]                    [Room Staff]                  │
│                                                                   │
│  UC14: Check Room Availability                                    │
│       ↓                                                          │
│  UC15.1: Create Guest Profile (nếu khách mới)                    │
│       ↓                                                          │
│  UC13.3: Create Reservation                                       │
│       ↓                            Sau check-out:                │
│  UC16.1: Verify Reservation        UC20: Update Room Status       │
│       ↓                            (Occupied → Dirty → Clean     │
│  UC16.2: Assign Room               → Ready)                       │
│       ↓                                                          │
│  UC17: Record Service Usage (nếu có)                             │
│       ↓                                                          │
│  UC18.1: Generate Invoice                                        │
│       ↓                                                          │
│  UC18.2: Calculate Charges                                       │
│       ↓                                                          │
│  UC18.3: Process Payment                                         │
│       ↓                                                          │
│  [Kết thúc: phòng → Dirty, Room Staff nhận task]                │
└─────────────────────────────────────────────────────────────────┘
```

### 6.2 Quan hệ phụ thuộc giữa các UC

| UC cần có trước | UC phụ thuộc | Lý do |
|---|---|---|
| UC9 (Room Type phải có) | UC7 (Create Room) | Phòng phải thuộc 1 Room Type |
| UC7 (Room phải có) | UC14 (Check Availability) | Phải có phòng mới check được |
| UC14 (phòng trống) | UC13.3 (Create Reservation) | Phải biết phòng trống mới đặt |
| UC15.1 (Guest Profile) | UC13.3 (Create Reservation) | Phải có thông tin khách |
| UC13.3 (Reservation = Reserved) | UC16 (Check-in) | Phải có đặt phòng trước |
| UC16 (Checked-in) | UC17 (Record Service) | Phải check-in rồi mới dùng service |
| UC17 (Service recorded) | UC18.2 (Calculate Charges) | Phí service đưa vào hóa đơn |
| UC18.2 (Tính xong phí) | UC18.3 (Process Payment) | Phải biết tổng tiền trước khi thu |
| UC18.3 (Paid) | UC18.1 (Invoice final) | Hóa đơn chốt sau khi thanh toán |
| UC10 (Services có) | UC17 (Record Service) | Phải có dịch vụ mới ghi nhận |
| UC7 (phòng có) | UC20 (Update Room Status) | Room Staff cập nhật phòng đã tồn tại |

### 6.3 Quan hệ giữa trạng thái phòng và các UC

```
[Admin/Manager: UC7.1 Create Room]
        ↓
    Available
        ↓ UC13.3 Create Reservation
    Reserved
        ↓ UC16 Process Check-in
    Occupied ←────────────────────────┐
        ↓ UC18 Process Check-out      │ (nếu reservation expired)
      Dirty                           │
        ↓ UC20 Update (Room Staff)    │
    Cleaning                          │
        ↓                             │
      Clean                           │
        ↓                             │
      Ready ─────────────────────────►┘
        
    [Nhánh đặc biệt]
    Bất kỳ trạng thái nào
        ↓ UC21 Report Critical Issue
    Under Maintenance
        ↓ [Sửa xong - Manager reset]
    Available
```

### 6.4 Sự kiện trigger giữa các UC

| Sự kiện | UC trigger | UC được kích hoạt | Ghi chú |
|---|---|---|---|
| Guest check-out thành công | UC18.3 | Phòng chuyển → Dirty, Room Staff nhận task | Tự động |
| Room Staff báo Critical issue | UC21 | Phòng khóa → Under Maintenance, cảnh báo Dashboard | Tự động |
| Reservation quá giờ check-in | Cron job | Reservation → Expired, phòng → Available | Tự động (mỗi 30 phút) |
| 11:00 AM mỗi ngày | Cron job | Occupied rooms → Cleaning | Tự động |
| Room Staff mark Ready | UC20 | Phòng → Available, Receptionist có thể gán | Tự động |

---

## 7. Background Services & Scheduled Jobs

### 7.1 Các cron jobs

| Tên | Lịch chạy | Mô tả |
|---|---|---|
| `autoUpdateRoomStatus` | Midnight & Noon | Tự động đổi Occupied → Cleaning sau 11:00 AM checkout |
| `calculateDailyRevenue` | 23:55 mỗi ngày | Tổng hợp doanh thu ngày vào bảng `daily_revenue` |
| `cancelExpiredReservations` | Mỗi 30 phút | Hủy các reservation quá giờ check-in, trả phòng về Available |
| `backupDatabase` | 02:00 AM mỗi ngày | Backup toàn bộ DB, giữ 30 ngày, xóa backup cũ hơn |

### 7.2 Các internal services

| Service | Endpoint | Mô tả |
|---|---|---|
| `AuthService` | Middleware (internal) | Validate JWT token, kiểm tra role permission |
| `RoomAvailabilityService` | `GET /api/internal/room-status` | Trạng thái phòng real-time, dùng cho UC14 |
| `generateReportAsync` | `POST /api/reports/generate` | Tạo báo cáo async, trả `jobId`, notify khi xong |
| `AuditLogService` | Internal (event-driven) | Ghi log mọi thao tác quan trọng vào `audit_logs` |

### 7.3 Audit Log – những gì được ghi

| Hành động | Actor |
|---|---|
| Login / Logout | Tất cả |
| Create / Update / Cancel Reservation | Receptionist |
| Room Status Update | Room Staff, System |
| Grant / Change / Revoke Permission | Admin |
| Create / Edit / Delete Account | Admin |
| Process Payment | Receptionist |
| Report Maintenance Issue | Room Staff |

---

## 8. Trạng thái phòng – State Machine

### 8.1 Các trạng thái

| Trạng thái | Mô tả | Ai có thể set |
|---|---|---|
| `Available` | Phòng trống, sạch, sẵn sàng nhận khách | System (auto), Manager (reset) |
| `Reserved` | Đã có đặt phòng nhưng khách chưa đến | System (khi tạo reservation) |
| `Occupied` | Khách đang lưu trú | System (khi check-in) |
| `Dirty` | Khách vừa check-out, chờ dọn | System (khi check-out) |
| `Cleaning` | Đang được dọn dẹp | Room Staff |
| `Clean` | Đã dọn xong | Room Staff |
| `Ready` | Sạch và sẵn sàng, chờ xác nhận | Room Staff |
| `Under Maintenance` | Đang bảo trì, không thể nhận khách | System (khi Critical issue), Manager |

### 8.2 Các chuyển trạng thái hợp lệ

```
Available → Reserved        (UC13.3: Receptionist tạo đặt phòng)
Reserved  → Occupied        (UC16: Receptionist check-in)
Reserved  → Available       (UC13.6: Hủy đặt phòng / Cron: expired)
Occupied  → Dirty           (UC18: Check-out hoàn thành)
Dirty     → Cleaning        (UC20: Room Staff bắt đầu dọn)
Cleaning  → Clean           (UC20: Room Staff dọn xong)
Clean     → Ready           (UC20: Room Staff xác nhận sẵn sàng)
Ready     → Available       (System: tự động sau verify / Manager confirm)
* → Under Maintenance       (UC21: Critical issue được báo cáo)
Under Maintenance → Available (Manager reset sau khi sửa xong)
```

---

## 9. Database chính cần có

### 9.1 Bảng chính

| Bảng | Mô tả | UC liên quan |
|---|---|---|
| `users` | Tài khoản nhân viên, role | UC1, UC5 |
| `room_types` | Loại phòng | UC9 |
| `rooms` | Thông tin phòng, trạng thái | UC7, UC20 |
| `guests` | Hồ sơ khách hàng | UC15 |
| `reservations` | Đặt phòng | UC13, UC16, UC18 |
| `services` | Danh mục dịch vụ | UC10 |
| `service_usages` | Dịch vụ khách đã dùng | UC17 |
| `invoices` | Hóa đơn | UC18.1 |
| `payments` | Thanh toán | UC18.3 |
| `housekeeping_tasks` | Nhiệm vụ dọn phòng | UC19 |
| `maintenance_reports` | Báo cáo sự cố | UC21 |
| `audit_logs` | Log toàn bộ hoạt động | AuditLogService |
| `daily_revenue` | Doanh thu tổng hợp theo ngày | Cron job |
| `hotel_info` | Thông tin khách sạn | UC6 |

### 9.2 Trường quan trọng bảng `reservations`

```
reservation_id, guest_id, room_id, receptionist_id
check_in_date, check_out_date, actual_check_in, actual_check_out
status: Reserved | Checked-in | Checked-out | Cancelled | Expired
special_requests, notes
created_at, updated_at
```

### 9.3 Trường quan trọng bảng `rooms`

```
room_id, room_number, floor, room_type_id
price_per_night, capacity, description
status: Available | Reserved | Occupied | Dirty | Cleaning | Clean | Ready | Under Maintenance
is_active, created_at, updated_at
```

---

## 10. Phân công thực hiện

| UC | Use Case | Actor | Người thực hiện |
|---|---|---|---|
| 5.0–5.6 | Manage Account | Admin | Nhã |
| 6 | Edit Hotel Information | Admin | Phong |
| 7.0–7.6 | Manage Room | Admin, Manager | Phong |
| 8 | View System Logs | Admin | Phong |
| 9.0–9.6 | Manage Room Type | Admin, Manager | Thịnh |
| 10.0–10.6 | Manage Hotel Services | Admin, Manager | Ngân |
| 11 | View Dashboard | Hotel Manager | Hậu |
| 12.0–12.5 | Manage Hotel Report | Hotel Manager | Hậu |
| 13.0–13.6 | Manage Reservations | Receptionist | Bảo |
| 14 | Check Room Availability | Receptionist | Bảo |
| 15.0–15.4 | Manage Guest Profiles | Receptionist | Nhã |
| 16.0–16.2 | Process Check-in | Receptionist | Phong |
| 17 | Record Service Usage | Receptionist | Phong |
| 18.0–18.3 | Process Check-out | Receptionist | Hậu |
| 19.0–19.1 | Manage Housekeeping Tasks | Room Staff | Thịnh |
| 20 | Update Room Status | Room Staff | Bảo |
| 21 | Report Room Maintenance Issues | Room Staff | Bảo |
| 4 | Change Password | All | Thịnh |
| 1 | Login | All | Ngân (**làm sau**) |
| 2 | Logout | All | Ngân (**làm sau**) |
| 3 | Forgot Password | All | Thịnh (**làm sau**) |

---

*Tài liệu này được tạo từ dữ liệu trong SWD392_Group3 Report và Backlog_Code_HotelManagement.*  
*Cần cập nhật khi có thay đổi về phân quyền hoặc UC mới.*