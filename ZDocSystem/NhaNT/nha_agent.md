# 🏨 Hotel Management System — Agent Task File
## Thành viên: Nguyễn Thanh Nhã (Leader)
> File này dùng cho AI Agent trong VS Code để sinh code tự động.
> Đọc toàn bộ file trước khi sinh code. Mỗi task có đủ UC ID, Actor, Flow, Validation, Business Rule, và gợi ý API endpoint để agent map chính xác.

---

## 📌 THÔNG TIN DỰ ÁN

| Mục | Nội dung |
|-----|----------|
| Project | Hotel Management System |
| Code | SE1902-G3 |
| Loại | Web Application |
| Roles có trong hệ thống | Admin, Hotel Manager, Receptionist, Room Staff |
| Auth | JWT Token, MD5 password hash |
| Audit | Mọi thao tác CRUD phải ghi vào `audit_logs` (AuditLogService) |

---

## 🗂️ DANH SÁCH USE CASE CỦA NHÃ

| UC ID | Tên Use Case | Actor | Module |
|-------|-------------|-------|--------|
| UC01 | Login | All Users | Auth |
| UC02 | Logout | All Users | Auth |
| UC05 | Manage Account (parent) | Admin | Account Management |
| UC5.1 | Create Account | Admin | Account Management |
| UC5.2 | Edit Account | Admin | Account Management |
| UC5.3 | Delete Account | Admin | Account Management |
| UC5.4 | View List Account | Admin | Account Management |
| UC5.5 | Search Account | Admin | Account Management |
| UC5.6 | View Detail Account | Admin | Account Management |
| UC15 | Manage Guest Profiles (parent) | Receptionist | Guest Management |
| UC15.1 | Create Guest Information | Receptionist | Guest Management |
| UC15.2 | View Guest Profile | Receptionist | Guest Management |
| UC15.3 | Edit Guest Profile | Receptionist | Guest Management |
| UC15.4 | View Guest History | Receptionist | Guest Management |

---

## 🔐 MODULE 1 — AUTH (UC01 & UC02)

### UC01 — Login

**Actor:** Admin, Hotel Manager, Receptionist, Room Staff  
**Trigger:** User nhấn nút "Login" trên trang đăng nhập  
**Priority:** High

#### Preconditions
- Hệ thống đang chạy và kết nối DB
- User chưa đăng nhập / không có session hợp lệ
- Tài khoản tồn tại với email, mật khẩu (MD5), và role

#### Postconditions
- Session được tạo thành công
- User được redirect tới dashboard theo role

#### Normal Flow
1. User mở trang Login
2. Hệ thống hiển thị form (email + password)
3. User nhập thông tin và nhấn Login
4. Hệ thống validate dữ liệu đầu vào
5. Hệ thống xác thực credentials (so sánh email + MD5(password))
6. Hệ thống xác định role của user
7. Hệ thống tạo JWT token (expire: 8 giờ)
8. Hệ thống redirect user tới dashboard tương ứng role

#### Alternative Flows
- **AF-1:** User nhấn "Forgot Password" → redirect sang UC03

#### Exception Flows
| Mã | Tình huống | Thông báo lỗi |
|----|-----------|---------------|
| E-1 | Sai email hoặc password | "Incorrect email or password. Please try again." |
| E-2 | Tài khoản bị khóa (Admin deactivate) | "Your account has been locked. Please contact the Administrator." |
| E-3 | Lỗi kết nối DB / timeout | "The system is experiencing issues. Please try again later." |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-01 | Mật khẩu phải được mã hóa MD5 trước khi lưu và so sánh |
| BR-02 | Email phải là duy nhất trong hệ thống |
| BR-03 | Phải cung cấp đúng email và password |
| BR-04 | Sau 5 lần đăng nhập thất bại liên tiếp → khóa tài khoản 15 phút |

#### API Endpoint gợi ý
```
POST /api/auth/login
Body: { email: string, password: string (plain, FE gửi lên, BE hash MD5) }
Response: { token: string, role: string, user: {...} }
```

---

### UC02 — Logout

**Actor:** Admin, Hotel Manager, Receptionist, Room Staff  
**Trigger:** User nhấn nút "Logout"  
**Priority:** High

#### Preconditions
- User đã đăng nhập thành công
- Session chưa hết hạn

#### Postconditions
- Session bị hủy
- User được redirect về trang Login

#### Normal Flow
1. User chọn Logout
2. Hệ thống yêu cầu xác nhận
3. User xác nhận
4. Hệ thống hủy session / invalidate JWT token
5. Redirect về trang Login

#### Alternative Flows
- **AF-1:** Session tự động hết hạn → redirect Login + thông báo "Your session has expired. Please log in again."

#### Exception Flows
| Mã | Tình huống | Hành động |
|----|-----------|-----------|
| E-1 | Session đã hết hạn | Tự redirect về Login |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-05 | Tất cả session token phải bị invalidate khi logout |
| BR-06 | Ngăn user quay lại xem dữ liệu cũ bằng nút Back (cache clearing) |

#### API Endpoint gợi ý
```
POST /api/auth/logout
Header: Authorization: Bearer <token>
Response: { message: "Logged out successfully" }
```

---

## 👤 MODULE 2 — ACCOUNT MANAGEMENT (UC05)

> **Quyền truy cập:** Chỉ **Admin**  
> **Lưu ý:** Xóa tài khoản dùng soft delete. Mọi thao tác phải ghi audit log.

---

### UC5.1 — Create Account

**Actor:** Admin  
**Trigger:** Admin nhấn "Create Account" trên dashboard quản lý tài khoản  
**Priority:** High

#### Preconditions
- Admin đã đăng nhập
- Admin có quyền quản lý tài khoản
- Kết nối DB khả dụng

#### Postconditions
- Tài khoản mới được tạo trong DB
- Tài khoản có role hợp lệ
- Ghi vào audit log

#### Normal Flow
1. Admin nhấn "Create Account"
2. Hệ thống hiển thị form tạo tài khoản
3. Admin nhập thông tin: full name, email, role (Hotel Manager / Receptionist / Room Staff), phone, temporary password
4. Admin submit form
5. Hệ thống validate thông tin
6. Hệ thống kiểm tra email chưa tồn tại trong DB
7. Hệ thống tạo tài khoản, mã hóa password MD5
8. Ghi vào audit log
9. Hiển thị thông báo thành công
10. Redirect về Account List

#### Alternative Flows
- **AF-1:** Admin nhấn Cancel → hủy, không lưu, về Account List

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Email đã tồn tại | "Email already exists. Please use another email." |
| E-2 | Thiếu thông tin bắt buộc | Highlight các field thiếu |
| E-3 | Format dữ liệu không hợp lệ | Hiển thị lỗi validation |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-02 | Email phải là duy nhất trong hệ thống |
| BR-08 | Chỉ Admin được tạo tài khoản |
| BR-09 | Ghi audit log với mọi thao tác quản lý tài khoản |
| (extra) | Temporary password chỉ có hiệu lực 24 giờ, user bắt buộc đổi password khi đăng nhập lần đầu |

#### Fields bắt buộc
| Field | Type | Ghi chú |
|-------|------|---------|
| full_name | string | Bắt buộc |
| email | string (email format) | Bắt buộc, unique |
| role | enum: HotelManager, Receptionist, RoomStaff | Bắt buộc |
| phone | string | Tùy chọn |
| temp_password | string | Bắt buộc, hash MD5 trước khi lưu |

#### API Endpoint gợi ý
```
POST /api/admin/accounts
Header: Authorization: Bearer <admin_token>
Body: { fullName, email, role, phone, tempPassword }
Response: { accountId, message: "Account created successfully" }
```

---

### UC5.2 — Edit Account

**Actor:** Admin  
**Trigger:** Admin nhấn "Edit" trên một tài khoản  
**Priority:** High

#### Preconditions
- Admin đã đăng nhập
- Tài khoản cần sửa tồn tại trong hệ thống

#### Postconditions
- Thông tin tài khoản được cập nhật trong DB
- Ghi vào audit log

#### Normal Flow
1. Admin chọn một tài khoản từ danh sách
2. Admin nhấn "Edit Account"
3. Hệ thống lấy và hiển thị thông tin tài khoản (pre-filled form)
4. Admin chỉnh sửa các field: full name, phone, role, status (active/inactive)
5. Hệ thống validate
6. Kiểm tra business rules (email không được thay đổi - BR-10)
7. Hệ thống cập nhật DB
8. Ghi audit log
9. Hiển thị thông báo thành công

#### Alternative Flows
- **AF-1:** Admin nhấn Cancel → hủy mọi thay đổi, về Account List

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Không tìm thấy tài khoản | "Account not found." |
| E-2 | Format dữ liệu không hợp lệ | Hiển thị lỗi validation |
| E-3 | Gán role không hợp lệ | "Invalid role assignment." |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-02 | Email là unique |
| BR-08 | Chỉ Admin được sửa tài khoản |
| BR-09 | Ghi audit log |
| BR-10 | **Email không được thay đổi sau khi tạo tài khoản** |

#### API Endpoint gợi ý
```
PUT /api/admin/accounts/{accountId}
Header: Authorization: Bearer <admin_token>
Body: { fullName, phone, role, status }
Response: { message: "Account updated successfully" }
```

---

### UC5.3 — Delete Account

**Actor:** Admin  
**Trigger:** Admin nhấn "Delete" trên một tài khoản  
**Priority:** Must Have

#### Preconditions
- Admin đã đăng nhập
- Tài khoản cần xóa tồn tại

#### Postconditions
- Tài khoản bị xóa khỏi DB (soft delete)
- Tài khoản không thể đăng nhập
- Ghi audit log

#### Normal Flow
1. Admin mở trang Account Management
2. Hệ thống hiển thị danh sách tài khoản
3. Admin chọn một tài khoản
4. Admin nhấn "Delete"
5. Hệ thống hiển thị popup xác nhận
6. Admin xác nhận xóa
7. Hệ thống soft delete tài khoản (đặt `is_deleted = true`, `status = inactive`)
8. Hiển thị thông báo thành công
9. Refresh danh sách tài khoản

#### Alternative Flows
- **AF-1:** Admin nhấn Cancel → không xóa, về danh sách

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Lỗi DB | "An error occurred. Account remains unchanged." |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-08 | Chỉ Admin được xóa tài khoản |
| BR-11 | **Admin không thể tự xóa tài khoản đang đăng nhập của mình** |
| (extra) | Dùng soft delete để giữ dữ liệu audit |

#### API Endpoint gợi ý
```
DELETE /api/admin/accounts/{accountId}
Header: Authorization: Bearer <admin_token>
Response: { message: "Account deleted successfully" }
```

---

### UC5.4 — View List Account

**Actor:** Admin  
**Trigger:** Admin vào trang Account Management  
**Priority:** High

#### Preconditions
- Admin đã đăng nhập

#### Postconditions
- Danh sách tài khoản hiển thị thành công

#### Normal Flow
1. Admin mở trang Account Management
2. Admin chọn "View List Account"
3. Hệ thống truy vấn DB
4. Hiển thị danh sách dạng bảng có phân trang

#### Alternative Flows
- **AF-1:** Không có tài khoản nào → "No accounts available."

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Lỗi kết nối DB | Hiển thị thông báo lỗi |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-08 | Chỉ Admin được xem danh sách |
| BR-12 | Ẩn thông tin nhạy cảm: password không trả về, phone/CCCD che dấu `*` |
| BR-13 | Hiển thị dạng bảng (table format) |

#### Thông tin hiển thị trong bảng
`Account ID | Full Name | Email | Role | Status | Created At`

#### API Endpoint gợi ý
```
GET /api/admin/accounts?page=1&size=20
Header: Authorization: Bearer <admin_token>
Response: { data: [...], total: number, page: number }
```

---

### UC5.5 — Search Account

**Actor:** Admin  
**Trigger:** Admin nhập keyword vào ô search và nhấn Search  
**Priority:** High

#### Preconditions
- Admin đã đăng nhập
- Có quyền quản lý tài khoản
- Kết nối DB khả dụng

#### Postconditions
- Hiển thị các tài khoản khớp với điều kiện tìm kiếm

#### Normal Flow
1. Admin nhập một hoặc nhiều tiêu chí tìm kiếm
2. Hệ thống validate tiêu chí
3. Hệ thống tìm kiếm trong DB (partial match)
4. Hiển thị kết quả dạng bảng có phân trang
5. Admin có thể chọn một tài khoản để thao tác thêm

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Không tìm thấy | "Account not found. Please try again." |
| E-2 | Lỗi kết nối DB | Hiển thị thông báo lỗi |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-12 | Ẩn thông tin nhạy cảm trong kết quả |
| BR-14 | Phân quyền dữ liệu: Admin xem tất cả users |
| BR-16 | Hỗ trợ partial matching, kết quả phân trang |

#### Tiêu chí tìm kiếm hỗ trợ
- username / full name (partial match, case-insensitive)
- email
- role
- status (active / inactive)

#### API Endpoint gợi ý
```
GET /api/admin/accounts/search?keyword=nguyen&role=Receptionist&status=active&page=1&size=20
Header: Authorization: Bearer <admin_token>
Response: { data: [...], total: number }
```

---

### UC5.6 — View Detail Account

**Actor:** Admin  
**Trigger:** Admin click vào một tài khoản để xem chi tiết  
**Priority:** High

#### Preconditions
- Admin đã đăng nhập
- Tài khoản được chọn tồn tại

#### Postconditions
- Hiển thị thông tin chi tiết tài khoản (read-only)

#### Normal Flow
1. Admin chọn một tài khoản từ danh sách hoặc kết quả search
2. Hệ thống lấy thông tin từ DB
3. Hệ thống hiển thị trang chi tiết (read-only)
4. Admin xem xong → đóng hoặc về danh sách

#### Alternative Flows
- **AF-1:** Xem chi tiết từ kết quả search

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Không tìm thấy tài khoản | Hiển thị lỗi, về danh sách |
| E-2 | Lỗi kết nối DB | Hiển thị lỗi |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-08 | Chỉ Admin được xem chi tiết |
| (extra) | Password **không bao giờ** được hiển thị |
| (extra) | Thông tin phải phản ánh dữ liệu mới nhất trong DB |

#### Thông tin hiển thị
`Account ID | Full Name | Email | Role | Status | Phone (masked) | Created At | Last Login`

#### API Endpoint gợi ý
```
GET /api/admin/accounts/{accountId}
Header: Authorization: Bearer <admin_token>
Response: { accountId, fullName, email, role, status, phone (masked), createdAt }
```

---

## 🧑‍🤝‍🧑 MODULE 3 — GUEST PROFILES (UC15)

> **Quyền truy cập:** Receptionist có thể CRUD. Admin và Hotel Manager chỉ được xem (read-only).  
> **Lưu ý:** Mọi thao tác ghi audit log.

---

### UC15 — Manage Guest Profiles (Parent)

**Actor:** Receptionist  
**Trigger:** Receptionist chọn "Guest Management" từ dashboard  
**Priority:** High

#### Preconditions
- Receptionist đã đăng nhập
- Guest database khả dụng

#### Postconditions
- Thông tin guest được tạo, cập nhật, hoặc hiển thị theo yêu cầu
- Mọi thay đổi được lưu DB và ghi audit log

#### Normal Flow
1. Receptionist mở trang Guest Management
2. Hệ thống hiển thị danh sách guest hiện có
3. Receptionist chọn hành động: Create / View / Edit / View History
4. Hệ thống thực hiện sub-use case tương ứng (UC15.1 → UC15.4)
5. Sau khi xong, về Guest List hoặc dashboard

#### Alternative Flows
- **AF-1:** Tìm kiếm guest trước khi thao tác (search by name hoặc phone)

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Không có guest nào | "No guest records available." |
| E-2 | Lỗi kết nối DB | "Unable to load guest data. Please try again later." |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-22 | Full name + ít nhất 1 contact (phone hoặc email) là bắt buộc |
| BR-23 | Mỗi guest có Guest ID duy nhất do hệ thống tự sinh |
| BR-24 | Chỉ Receptionist mới được tạo/sửa. Admin & Manager chỉ xem |

---

### UC15.1 — Create Guest Information

**Actor:** Receptionist  
**Trigger:** Receptionist nhấn "Create New Guest"  
**Priority:** High

#### Preconditions
- Receptionist đã đăng nhập
- Đang ở trang Guest Management

#### Postconditions
- Guest profile mới được lưu vào DB
- Guest xuất hiện trong danh sách
- Ghi audit log

#### Normal Flow
1. Receptionist nhấn "Create New Guest"
2. Hệ thống hiển thị form trống: full name, phone, email, address, ID/passport (tùy chọn), notes
3. Receptionist nhập thông tin
4. Receptionist nhấn "Save"
5. Hệ thống kiểm tra: full name và ít nhất 1 trong phone/email phải có
6. Hệ thống lưu guest profile, tự sinh Guest ID (VD: GUEST-001)
7. Hiển thị "Guest profile created successfully."
8. Về Guest Management page, guest mới được highlight

#### Alternative Flows
- **AF-1:** Receptionist nhấn Cancel → hủy, không lưu
- **AF-2:** Phone/email đã tồn tại → hỏi "A guest with this phone/email already exists. View existing profile or continue?"

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Thiếu full name hoặc thiếu cả phone lẫn email | "Please provide full name and at least one contact method (phone or email)." |
| E-2 | Email format sai | "Please enter a valid email address." |
| E-2b | Phone chứa chữ cái | "Please enter a valid phone number." |
| E-3 | Lỗi DB | "Unable to save guest information. Please try again later." |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-22 | Full name + ít nhất 1 contact bắt buộc |
| BR-25 | Guest ID tự động sinh, Receptionist không được thay đổi |
| BR-26 | Phone/email trùng với guest khác → cảnh báo nhưng vẫn cho tạo (ví dụ: gia đình dùng chung số) |

#### Fields
| Field | Type | Bắt buộc | Ghi chú |
|-------|------|----------|---------|
| full_name | string | ✅ | Hỗ trợ tiếng Việt |
| phone | string | Ít nhất 1 trong 2 | Có hoặc không có country code |
| email | string | Ít nhất 1 trong 2 | Validate format |
| address | string | ❌ | |
| id_passport | string | ❌ | Khuyến nghị cho mục đích pháp lý |
| notes | text | ❌ | |

#### API Endpoint gợi ý
```
POST /api/receptionist/guests
Header: Authorization: Bearer <token>
Body: { fullName, phone, email, address, idPassport, notes }
Response: { guestId: "GUEST-001", message: "Guest profile created successfully" }
```

---

### UC15.2 — View Guest Profile

**Actor:** Receptionist (Admin, Hotel Manager: read-only)  
**Trigger:** Receptionist click vào tên guest từ danh sách  
**Priority:** High

#### Preconditions
- Đã đăng nhập
- Ít nhất 1 guest tồn tại trong hệ thống
- Đã chọn guest cần xem

#### Postconditions
- Hiển thị đầy đủ thông tin guest (read-only)
- Không có dữ liệu nào bị thay đổi

#### Normal Flow
1. Receptionist tìm guest qua danh sách hoặc search
2. Click vào tên hoặc nút "View"
3. Hệ thống lấy thông tin từ DB
4. Hiển thị trang guest profile: full name, phone, email, address, ID/passport, notes, Guest ID
5. Receptionist xem xong → đóng hoặc Back về danh sách

#### Alternative Flows
- **AF-1:** Xem profile từ màn hình reservation / check-in (popup hoặc trang mới)

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Guest không tồn tại (đã bị xóa) | "Guest profile not found. It may have been removed." |
| E-2 | Lỗi DB | "Unable to load guest details. Please try again later." |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-24 | Admin/Manager chỉ xem, không sửa được từ trang này |

#### API Endpoint gợi ý
```
GET /api/receptionist/guests/{guestId}
Header: Authorization: Bearer <token>
Response: { guestId, fullName, phone, email, address, idPassport, notes, createdAt }
```

---

### UC15.3 — Edit Guest Profile

**Actor:** Receptionist  
**Trigger:** Receptionist nhấn "Edit" trên trang guest profile  
**Priority:** High

#### Preconditions
- Receptionist đã đăng nhập
- Guest profile tồn tại
- Đang ở trang guest profile hoặc danh sách

#### Postconditions
- Thông tin guest được cập nhật trong DB
- Ghi audit log (ai sửa, sửa lúc nào)
- Hiển thị thông tin đã cập nhật

#### Normal Flow
1. Receptionist tìm guest và mở profile
2. Nhấn "Edit"
3. Hệ thống chuyển sang edit mode (tất cả field trở nên chỉnh sửa được, trừ Guest ID)
4. Receptionist sửa các field cần thiết
5. Nhấn "Save Changes"
6. Hệ thống kiểm tra: full name và ít nhất 1 contact vẫn còn
7. Nếu phone/email mới trùng với guest khác → cảnh báo nhưng vẫn cho lưu
8. Hệ thống cập nhật DB
9. Ghi audit log
10. Hiển thị "Guest profile updated successfully."
11. Trở về view mode với thông tin mới

#### Alternative Flows
- **AF-1:** Cancel → hủy, giữ thông tin cũ
- **AF-2:** Sửa từ màn hình reservation / check-in → sau khi save về màn hình đó

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Xóa full name hoặc xóa cả phone lẫn email | "Full name and at least one contact method (phone or email) are required." |
| E-2 | Email format sai | "Please enter a valid email address." |
| E-2b | Phone chứa chữ cái | "Please enter a valid phone number." |
| E-3 | Guest bị xóa bởi user khác trong lúc đang sửa | "Guest profile no longer exists. Changes cannot be saved." |
| E-4 | Lỗi DB | "Unable to save changes. Please try again later." |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-22 | Vẫn bắt buộc full name + 1 contact sau khi sửa |
| BR-26 | Phone/email trùng → cảnh báo nhưng cho lưu |
| BR-28 | **Chỉ Receptionist được sửa. Admin/Manager không được sửa.** |
| (extra) | Guest ID **không bao giờ** được thay đổi |

#### API Endpoint gợi ý
```
PUT /api/receptionist/guests/{guestId}
Header: Authorization: Bearer <token>
Body: { fullName, phone, email, address, idPassport, notes }
Response: { message: "Guest profile updated successfully" }
```

---

### UC15.4 — View Guest History

**Actor:** Receptionist (Admin, Hotel Manager: xem được)  
**Trigger:** Receptionist nhấn "View History" / "Booking History" trên trang guest profile  
**Priority:** High

#### Preconditions
- Đã đăng nhập
- Guest profile tồn tại
- Guest có ít nhất 1 reservation (nếu không → hiển thị empty state)

#### Postconditions
- Danh sách lịch sử đặt phòng của guest được hiển thị
- Không có dữ liệu nào bị thay đổi

#### Normal Flow
1. Receptionist mở guest profile (UC15.2)
2. Nhấn "View History"
3. Hệ thống lấy tất cả reservation liên kết với Guest ID từ DB
4. Hiển thị danh sách sắp xếp theo ngày check-in (gần nhất trước)
5. Mỗi dòng hiển thị: Reservation ID, Check-in Date, Check-out Date, Room Number, Room Type, Total Amount, Payment Status, Reservation Status
6. Receptionist có thể click vào từng reservation để xem chi tiết (invoice, dịch vụ đã dùng)
7. Đóng hoặc Back về guest profile

#### Alternative Flows
- **AF-1:** Lọc theo date range (ví dụ: 3 tháng gần nhất, năm nay)
- **AF-2:** Tab "Upcoming" → chỉ hiện reservation tương lai hoặc đang diễn ra

#### Exception Flows
| Mã | Tình huống | Thông báo |
|----|-----------|-----------|
| E-1 | Guest chưa có reservation nào | "This guest has no booking history." |
| E-2 | Lỗi DB | "Unable to load booking history. Please try again later." |

#### Business Rules
| BR ID | Mô tả |
|-------|-------|
| BR-29 | Hiển thị tất cả reservation của guest (mọi trạng thái: completed, cancelled, no-show) |
| BR-30 | **Màn hình này read-only.** Để sửa reservation → phải sang Reservation Management module |

#### API Endpoint gợi ý
```
GET /api/receptionist/guests/{guestId}/history?status=all&startDate=&endDate=&page=1&size=20
Header: Authorization: Bearer <token>
Response: {
  data: [
    {
      reservationId, checkIn, checkOut, roomNumber, roomType,
      totalAmount, paymentStatus, reservationStatus
    }
  ],
  total: number
}
```

---

## 📋 TÓM TẮT BUSINESS RULES CỦA NHÃ

| BR ID | Áp dụng cho | Mô tả |
|-------|------------|-------|
| BR-01 | UC01, UC02, UC5.1 | Password phải hash MD5 |
| BR-02 | UC01, UC5.1, UC5.2 | Email phải unique trong hệ thống |
| BR-03 | UC01 | Phải cung cấp đúng credentials |
| BR-04 | UC01 | Khóa 15 phút sau 5 lần đăng nhập thất bại |
| BR-05 | UC02 | Invalidate tất cả token khi logout |
| BR-06 | UC02 | Ngăn xem cache cũ sau logout |
| BR-08 | UC5.x | Chỉ Admin mới được quản lý tài khoản |
| BR-09 | UC5.x | Ghi audit log với mọi thao tác account |
| BR-10 | UC5.2 | Email không được thay đổi sau khi tạo |
| BR-11 | UC5.3 | Admin không được tự xóa tài khoản đang dùng |
| BR-12 | UC5.4, UC5.5 | Ẩn password; che phone/CCCD bằng `*` |
| BR-13 | UC5.4 | Hiển thị dạng bảng |
| BR-14 | UC5.5 | Phân quyền dữ liệu theo role |
| BR-16 | UC5.5 | Partial matching + phân trang |
| BR-22 | UC15.1, UC15.3 | Full name + ít nhất 1 contact bắt buộc |
| BR-23 | UC15 | Mỗi guest có Guest ID duy nhất |
| BR-24 | UC15.2, UC15.3 | Chỉ Receptionist CRUD guest; Admin/Manager read-only |
| BR-25 | UC15.1 | Guest ID tự sinh, không thay đổi được |
| BR-26 | UC15.1, UC15.3 | Trùng contact → cảnh báo nhưng vẫn cho tạo/lưu |
| BR-28 | UC15.3 | Chỉ Receptionist được sửa guest profile |
| BR-29 | UC15.4 | Xem tất cả trạng thái reservation trong history |
| BR-30 | UC15.4 | History screen là read-only |

---

## 🛠️ GỢI Ý KIẾN TRÚC & LƯU Ý KỸ THUẬT

### Authentication & Authorization
- Dùng JWT Token (expire: 8 giờ)
- Mỗi request cần `Authorization: Bearer <token>` header
- AuthService kiểm tra token và role trước khi vào controller
- Role-based access: middleware kiểm tra `req.user.role`

### Password
- Hash MD5 trước khi lưu DB
- Không bao giờ trả password về API response
- Temp password hết hạn sau 24h, bắt đổi lần đầu đăng nhập

### Soft Delete (Account)
- Thêm field `is_deleted = true` và `deleted_at = timestamp`
- Query mặc định luôn filter `WHERE is_deleted = false`

### Audit Log
- Bảng `audit_logs`: `(id, user_id, action, entity_type, entity_id, timestamp, ip_address)`
- Ghi log tại: login, logout, create/edit/delete account, create/edit guest, view guest

### Duplicate Warning (Guest)
- Khi phone hoặc email đã tồn tại ở guest khác → trả về warning trong response nhưng HTTP 200
- FE hiển thị dialog cho Receptionist chọn tiếp tục hoặc xem profile cũ

### Phân trang
- Default: 20 records/page
- Params: `?page=1&size=20`

---

*File được tạo từ tài liệu SRS: SWD392_Group3_NhaNT_NganNHK_PhongNT_HauNP_BaoLG_ThinhDT.docx và Backlog_Code_HotelManagement.xlsx*







MÔ TẢ TASH CHI TIẾT
UC01 - Login
Functional Description
UC ID and Name:
UC01 – Login 
Created By:
Nguyễn Thanh Nhã
Date Created:
31/05/2026 
Primary Actor:
Admin, Hotel Manager, Receptionist, Room Staff
Secondary Actors:
Authentication Service 
Trigger:
User selects the "Login" button from the system login page.  
Description:
This use case allows users to log in using their assigned email and password to access the Hotel Management System according to their assigned role. 
Preconditions:
PRE-1: The system is active and connected to the database.
PRE-2: The user is not logged in and has no valid session.
PRE-3: The user account exists and has been granted access to the system, including email, MD5-encrypted password, and role.
Postconditions:
POST-1: The user has successfully logged in, and a session is created.
POST-2: User is redirected to the dashboard corresponding to their role. 
Normal Flow:
User opens the Login page.
System displays the Login form.
User enters email and password.
User clicks the Login button.
System validates input data.
System verifies credentials.
System identifies the user's role.
System creates a login session.
System redirects user to the corresponding dashboard.
Alternative Flows:
AF-1: Forgot password
- The user clicks the “Forgot password” link in Login form.
- The system redirects to UC3 – Forgot Password. After completing the password change, the user returns to Login form to log in. 
Exceptions:
E-1: Incorrect email or password
- The system displays a message: “Incorrect email or password. Please try again.”
- No session is created.
E-2: Account disabled
- Occurs when the Admin has deactivated the account. The system checks the account’s active status.
- The system display a message: “Your account has been locked. Please contact the Administrator.”
- Login is not allowed .
E-3: Database connection lost or timeout
- The system displays a message: “The system is experiencing issues. Please try again later.”
- System error log is recorded.
Priority:
High
Frequency of Use:
Each user logs in at least 1–2 times per work shift, averaging 4–6 times per day per user. Total frequency depends on the number of employees. 
Business Rules:
BR-01, BR-02, BR-03, BR-04
Other Information:
- The system uses JWT tokens to maintain login sessions; the token has an expiration time, for example 8 hours.
- Every request to other APIs must include the token, and it is verified by AuthService.
- If a login fails due to unforeseen issues such as power outage or crash, the system state remains unchanged, meaning no transaction is affected 
- Related requirements: NF-03 for AuthService and NF-07 for AuditLogService.
Assumptions:
- Each user has a unique email in the system, which is used as the login identifier.
- The user's account has been verified as valid by the Admin.
- The system has a stable network connection.
- The browser supports JavaScript and localStorage/cookies. 

b. Business Rules
Provide the business rules those are applied only to the use case
ID
Business Rule
Business Rule Description
BR-01
Password Encoding
User’s password must be encoded with MD5 hashing
BR-02
Email uniqueness
Email address must be unique within the system.
BR-03
Login credential verification 
User must provide valid username and password. 
BR-04
Account lockout policy 
After 5 consecutive failed login attempts, account is locked for 15 minutes. 


UC02 - Logout
Functional Description
UC ID and Name:
UC02 – Logout
Created By:
Nguyễn Thanh Nhã
Date Created:
31/05/2026 
Primary Actor:
Admin, Hotel Manager, Receptionist, Room Staff 
Secondary Actors:
Authentication Service 
Trigger:
The user clicks the “Logout” button on the system interface 
Description:
This use case allows logged-in users to log out of their account. 
Preconditions:
PRE-1: The user has successfully logged.
PRE-2: The session has not expired. 
Postconditions:
POST-1: User session is terminated. 
POST-2: User is redirected to Login page. 
Normal Flow:
1. User selects Logout.
2. System requests confirmation.
3. User confirms logout.
4. System destroys current session.
5. System redirects to Login page.
Alternative Flows:
AF-1: Logout due to session timeout
- The system automatically redirects the user to the login page and displays a message: “Your session has expired. Please log in again.”
Exceptions:
E-1: Session Expired
- System automatically redirects to Login page.
Priority:
High
Frequency of Use:
Equivalent to login frequency, typically one logout per work shift or when ending a shift.
Business Rules:
BR-05, BR-06 
Other Information:
- Logout does not discard unsaved edited data; it is recommended that users save their work before logging out.
- If the system has a token blacklist, adding the token to the blacklist ensures it cannot be reused even if it has not yet expired.
- Related requirements: NF-03 (AuthService), NF-07 (AuditLogService). 
Assumptions:
- The user understands the purpose of the logout button.
- The browser allows deletion of cookies/storage.
- A token blacklist mechanism is optional and not required for Release 1.0 

b. Business Rules
Provide the business rules those are applied only to the use case
ID
Business Rule
Business Rule Description
BR-05
Session termination 
All session tokens must be invalidated. 
BR-06
Cache clearing enforcement 
Pressing the Back button to review old data is not allowed.



UC15 – Manage Guest Profiles
Functional Description
UC ID and Name:
UC15 – Manage Guest Profiles
Created By:
Nguyễn Thanh Nhã
Date Created:
05/06/2026
Primary Actor:
Receptionist
Secondary Actors:
Admin, Hotel Manager (can view only) 
Trigger:
Receptionist selects “Guest Management” from the dashboard.
Description:
This use case allows the receptionist to manage guest information, including creating a new guest profile, viewing profile details, editing profile information, and viewing the guest’s booking history
Preconditions:
PRE-1: Receptionist is logged into the system.
PRE-2: Guest database is accessible.
Postconditions:
POST-1: Guest information is either created, updated, or displayed as requested.
POST-2: Changes (if any) are saved to the database.
POST-3: All actions are logged in the audit trail. 
Normal Flow:
1. Receptionist opens Guest Management page.
2. System displays a list of existing guests (if any).
3. Receptionist chooses one of the following actions:
 - Create new guest profile
 - View a guest profile
 - Edit a guest profile
 - View guest history
4. System performs the selected sub-use case (UC15.1 to UC15.4).
5. After finishing, system returns to the guest list or dashboard.
Alternative Flows:
AF-1: Search guest before action
- Receptionist types a name or phone number in the search box.
- System filters and shows matching guests.
- Receptionist picks a guest and proceeds with view, edit, or history. 
Exceptions:
E-1: No guests found
- If the guest list is empty, system shows “No guest records available.”
- Receptionist can choose to create a new guest.
E-2: Database connection lost
- System shows “Unable to load guest data. Please try again later.”
- No guest data is modified. 
Priority:
High
Frequency of Use:
Used many times per shift – every time a guest checks in, checks out, or requests a change to their information   
Business Rules:
BR-22, BR-23, BR-24 
Other Information:
- Guest profiles are linked to reservations, invoices, and room assignments.
- Only receptionists can create and edit guest profiles. Admin and Hotel Manager can only view.
- Related to UC01 (Login), NF-07 (AuditLogService).
Assumptions:
- Each guest has at least a full name and phone number or email.
- Guest ID is automatically generated by the system.
- The system stores guest information securely.

b. Business Rules
Provide the business rules those are applied only to the use case
ID
Business Rule
Business Rule Description
BR-22
Guest data entry
Full name and at least one contact method (phone or email) are required to create a guest profile.
BR-23 
Unique guest identification 
Each guest is identified by a unique system-generated Guest ID. Duplicate names or phones are allowed but flagged for manual check.
BR-24 
Profile modification permission
Only receptionists can create or edit guest profiles. Admin and Hotel Manager have read-only access.



UC15.1 – Manage Guest Profiles
a. Functional Description
UC ID and Name:
UC15.1 – Create Guest Information
Created By:
Nguyễn Thanh Nhã
Date Created:
05/06/2026
Primary Actor:
Receptionist
Secondary Actors:
None 
Trigger:
Receptionist clicks the “Create New Guest” button on the Guest Management page.
Description:
This use case allows the receptionist to enter a new guest’s personal information into the system. The system saves the guest profile so it can be used for future reservations and check-in.
Preconditions:
PRE-1: Receptionist is logged into the system.
PRE-2: Receptionist is on the Guest Management page.
Postconditions:
POST-1: A new guest profile is created and stored in the database.
POST-2: The new guest appears in the guest list.
POST-3: The action is recorded in the audit log. 
Normal Flow:
1. Receptionist clicks “Create New Guest”.
2. System displays an empty form with fields: full name, phone number, email, address, ID/passport number (optional), and notes.
3. Receptionist enters the guest’s information.
4. Receptionist clicks “Save”.
5. System checks that full name and at least one contact method (phone or email) are provided.
6. System saves the guest profile and auto-generates a unique Guest ID.
7. System shows a success message: “Guest profile created successfully.”
8. System returns to the Guest Management page with the new guest highlighted in the list.
Alternative Flows:
AF-1: Cancel creating guest
- Receptionist clicks “Cancel” instead of “Save”.
- System discards all entered information and returns to the Guest Management page without saving anything.
AF-2: Guest already exists (duplicate check)
- If the receptionist enters a phone number or email that already exists in the system, the system asks: “A guest with this phone/email already exists. Do you want to view the existing profile?”
- Receptionist can choose “View” to see the existing profile, or “Continue” to create a new one anyway.
Exceptions:
E-1: Missing required fields
- If the receptionist leaves full name blank and also leaves both phone and email blank, the system displays: “Please provide full name and at least one contact method (phone or email).”
- The system does not save the profile.
E-2: Invalid data format
- If the email format is wrong (e.g., missing @), the system shows: “Please enter a valid email address.”
- If the phone number contains letters, the system shows: “Please enter a valid phone number.”
E-3: Database connection lost
- System displays: “Unable to save guest information. Please try again later.”
- No guest profile is created.
Priority:
High
Frequency of Use:
Used many times per day, each time a new guest arrives who has never stayed at the hotel before
Business Rules:
BR-22, BR-25, BR-26 
Other Information:
- The guest ID is automatically generated (e.g., GUEST-001, GUEST-002).
- ID/passport number is optional but recommended for security and legal purposes.
- The system does not check for duplicate names because many guests can have the same name.
- Related to NF-07 (AuditLogService).
Assumptions:
- The receptionist has the guest’s consent to store their personal information.
- The system supports Vietnamese characters in guest names and addresses.
- Phone numbers can be entered with or without country code.

b. Business Rules
Provide the business rules those are applied only to the use case
ID
Business Rule
Business Rule Description
BR-22
Guest data entry
Full name and at least one contact method (phone or email) are required to create a guest profile.
BR-25
Auto-generated Guest ID
Each guest profile is assigned a unique, system-generated Guest ID. The receptionist cannot change it.
BR-26
Duplicate contact warning
If a phone number or email already exists in another guest profile, the system shows a warning but still allows creation (because two guests could share a phone number, e.g., family members).



UC15.2 – View Guest Profile
a. Functional Description
UC ID and Name:
UC15.2 – View Guest Profile
Created By:
Nguyễn Thanh Nhã
Date Created:
05/06/2026
Primary Actor:
Receptionist
Secondary Actors:
Admin, Hotel Manager (view only)
Trigger:
Receptionist clicks on a guest’s name from the guest list or search results.
Description:
This use case allows the receptionist to see all stored information of a specific guest, including personal details, contact information, and any notes.
Preconditions:
PRE-1: Receptionist is logged into the system.
PRE-2: At least one guest profile exists in the system.
PRE-3: The receptionist has selected a guest to view.
Postconditions:
POST-1: The guest’s full profile is displayed on the screen.
POST-2: No data is changed.
Normal Flow:
1. Receptionist finds a guest using the guest list or search bar.
2. Receptionist clicks on the guest’s name or a “View” button next to the guest record.
3. System retrieves the guest’s information from the database.
4. System displays the guest profile page with all fields: full name, phone number, email, address, ID/passport number, notes, and system-generated Guest ID.
5. Receptionist reviews the information.
6. Receptionist closes the profile or clicks “Back” to return to the guest list.
Alternative Flows:
AF-1: View profile from a reservation
- While viewing a reservation or check-in screen, receptionist clicks on the guest’s name link.
- System opens the guest profile in a popup or new page.
- After closing, receptionist returns to the previous screen.
Exceptions:
E-1: Guest profile not found
- If the selected guest was deleted or not found, system displays: “Guest profile not found. It may have been removed.”
- Receptionist returns to the guest list.
E-2: Database connection lost
- System shows: “Unable to load guest details. Please try again later.”
- No information is displayed.
Priority:
High
Frequency of Use:
Used very often – every time a guest checks in, checks out, or asks to change something, receptionist needs to view the profile.
Business Rules:
BR-24,
Other Information:
- ID/passport number and address are displayed in full because receptionist needs to verify them with the guest at check-in.
- Viewing a guest profile does not change any data.
- Related to NF-07 (AuditLogService) – viewing may be logged for security purposes.
Assumptions:
- The guest list can be sorted and searched by name, phone, or email.
- The profile page is read-only; to edit, receptionist must use UC15.3.

b. Business Rules
Provide the business rules those are applied only to the use case
ID
Business Rule
Business Rule Description
BR-24
Profile modification permission
Only receptionists can create or edit guest profiles. Admin and Hotel Manager have read-only access.




UC15.3 – Edit Guest Profile
a. Functional Description
UC ID and Name:
UC15.3 – Edit Guest Profile
Created By:
Nguyễn Thanh Nhã
Date Created:
05/06/2026
Primary Actor:
Receptionist
Secondary Actors:
None
Trigger:
Receptionist clicks the “Edit” button on the guest profile page or next to a guest in the lis
Description:
This use case allows the receptionist to update a guest’s personal information, such as phone number, email, address, ID/passport number, or notes. The system saves the changes and records the update.
Preconditions:
PRE-1: Receptionist is logged into the system.
PRE-2: The guest profile exists in the system.
PRE-3: Receptionist is viewing the guest profile or the guest list.
Postconditions:
POST-1: Guest information is updated in the database.
POST-2: The edit action is recorded in the audit log.
POST-3: The updated information is shown on the guest profile.
Normal Flow:
1. Receptionist finds the guest (by searching or browsing the list).
2. Receptionist opens the guest profile.
3. Receptionist clicks the “Edit” button.
4. System switches the profile page to edit mode – all fields become changeable: full name, phone number, email, address, ID/passport number, notes.
5. Receptionist modifies the needed fields.
6. Receptionist clicks “Save Changes”.
7. System validates that full name and at least one contact method (phone or email) are still provided.
8. System checks if the new phone number or email already exists in another guest profile. If yes, it shows a warning but still allows saving.
9. System updates the guest profile in the database.
10. System records the edit in the audit log (who changed what, and when).
11. System displays a success message: “Guest profile updated successfully.”
12. System returns to view mode, showing the updated information..
Alternative Flows:
AF-1: Cancel editing
- After clicking “Edit”, receptionist clicks “Cancel” instead of “Save”.
- System discards all changes and returns to view mode with the original information unchanged.
AF-2: Edit from reservation screen
- While processing a reservation or check-in, receptionist clicks a “Edit Guest Info” link next to the guest’s name.
- System opens the edit form directly. After saving, receptionist returns to the reservation screen.
Exceptions:
E-1: Missing required fields
- If receptionist removes the full name or removes both phone and email, then clicks “Save”.
- System shows: “Full name and at least one contact method (phone or email) are required.”
- System does not save and stays in edit mode.
E-2: Invalid data format
- If email format is wrong (missing @, etc.), system shows: “Please enter a valid email address.”
- If phone contains letters, system shows: “Please enter a valid phone number.”
E-3: Guest profile not found (deleted by another user in the meantime)
- System shows: “Guest profile no longer exists. Changes cannot be saved.”
- Receptionist returns to guest list.
E-4: Database connection lost
- System shows: “Unable to save changes. Please try again later.”
- No update is made
Priority:
High
Frequency of Use:
Used several times per day – when a guest changes phone number, corrects a spelling mistake, updates address, or provides additional ID information.
Business Rules:
BR-22, BR-26, BR-28
Other Information:
- All fields except Guest ID (which is auto-generated and cannot be changed) are editable.
- If a phone or email is changed to one that already exists for another guest, the system only warns but still allows the change, because two different people might use the same contact (e.g., family sharing a phone).
- The edit history is not stored in detail (only that an edit happened). Full version control is not required for Release 1.0.
- Related to NF-07 (AuditLogService).
Assumptions:
- The receptionist has the guest’s permission to update the information.
- The receptionist is careful not to accidentally create duplicate profiles (the warning helps).
- The system does not require approval from a manager for minor edits..

b. Business Rules
Provide the business rules those are applied only to the use case
ID
Business Rule
Business Rule Description
BR-22
Guest data entry
Full name and at least one contact method (phone or email) are required to save a guest profile, both when creating and editing.
BR-26
Duplicate contact warning
If the new phone number or email already exists in another guest profile, the system shows a warning but still allows saving.
BR-28
Edit permission
Only receptionists can edit guest profiles. Admin and Hotel Manager cannot edit (view only).



UC15.4 – View Guest History
a. Functional Description
UC ID and Name:
UC15.4 – View Guest History
Created By:
Nguyễn Thanh Nhã
Date Created:
05/06/2026
Primary Actor:
Receptionist
Secondary Actors:
Admin, Hotel Manager (view only)
Trigger:
Receptionist clicks the “View History” or “Booking History” button on the guest profile page.
Description:
This use case allows the receptionist to see all past and upcoming reservations of a specific guest, including check-in/out dates, room assigned, total paid, and any special requests.
Preconditions:
PRE-1: Receptionist is logged into the system.
PRE-2: The guest profile exists.
PRE-3: The guest has at least one reservation (if none, the system shows an empty list).
Postconditions:
POST-1: The guest’s reservation history is displayed.
POST-2: No data is changed.
Normal Flow:
1. Receptionist opens the guest profile (using UC15.2).
2. Receptionist clicks the “View History” button.
3. System retrieves all reservations linked to this guest’s ID from the database.
4. System displays the list of reservations sorted by check-in date (most recent first or upcoming first).
5. For each reservation, the system shows: reservation ID, check-in date, check-out date, room number, room type, total amount, payment status, and status (e.g., checked-in, checked-out, cancelled, no-show).
6. Receptionist can scroll through the list and click on any reservation to see more details (e.g., services used, invoice).
7. Receptionist closes the history view to return to the guest profile.
Alternative Flows:
AF-1: Filter history by date range
- Receptionist selects a date range filter (e.g., last 3 months, this year).
- System shows only reservations within that range.
AF-2: Show only upcoming or active reservations
- Receptionist clicks “Upcoming” tab.
- System filters to show only future or in-progress reservations.
Exceptions:
E-1: No reservations found
- If the guest has no past or future reservations, system displays: “This guest has no booking history.”
E-2: Database connection lost
- System shows: “Unable to load booking history. Please try again later.”
Priority:
High
Frequency of Use:
Used many times per day – when a returning guest arrives, receptionist checks past stays to see room preferences, special requests, or any issues. Also used for loyalty purposes.
Business Rules:
BR-29, BR-30
Other Information:
- History includes both completed (checked-out) and cancelled reservations, but cancelled ones are marked clearly.
- Staff can use this information to personalize service (e.g., “Welcome back, last time you stayed in room 205”).
- No modification is allowed from this screen; to change a reservation, receptionist uses reservation management use cases.
- Related to NF-07 (AuditLogService) – viewing may be logged for audit.
Assumptions:
- The system stores all reservations permanently (soft delete only).
- A guest may have multiple reservations, each with a unique reservation ID.
- The history list can be paginated if there are many records

b. Business Rules
Provide the business rules those are applied only to the use case
ID
Business Rule
Business Rule Description
BR-29
History visibility
All reservations linked to the same guest ID are visible to receptionists, regardless of status (completed, cancelled, no-show).
BR-30
No modification in history
The guest history screen is read-only. To modify a reservation, receptionist must go to the reservation management module.