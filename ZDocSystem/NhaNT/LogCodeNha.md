## [2026-06-16 00:00]

### Task
Khoi tao luong dang nhap/dang xuat va dashboard co ban cho he thong.

### Files Changed
- Program.cs
- Controllers/HomeController.cs
- Controllers/AuthController.cs
- Helpers/PasswordHashHelper.cs
- ViewModels/LoginViewModel.cs
- Views/Shared/_Layout.cshtml
- Views/Home/Index.cshtml
- Views/Home/Dashboard.cshtml
- Views/Auth/Login.cshtml

### Implementation
- Cau hinh cookie authentication va middleware dang nhap/dang xuat.
- Them AuthController voi xu ly login, logout, xac thuc MD5 va redirect dashboard.
- Bo sung view model dang nhap, helper hash MD5, va giao dien login.
- Cap nhat layout, trang chu, va dashboard de phuc vu flow UC01/UC02.

### Notes
- Login dang hoat dong voi cookie auth thay vi JWT de phu hop MVC server-side hien tai.
- Chua co seeding account mau, nen can du lieu tai khoan trong DB de kiem thu dang nhap.

### Status
Completed

## [2026-06-16 00:02]

### Task
Hoan thien phan con lai cua UC Nha: account CRUD va guest profile CRUD/history.

### Files Changed
- Controllers/AccountsController.cs
- Controllers/GuestsController.cs
- Models/HotelDbContext.cs
- Program.cs
- Migrations/20260616000100_ExtendAccountAndGuestManagement.cs
- ViewModels/AccountCreateViewModel.cs
- ViewModels/AccountEditViewModel.cs
- ViewModels/AccountDeleteViewModel.cs
- ViewModels/GuestCreateEditViewModel.cs
- ViewModels/GuestListItemViewModel.cs
- ViewModels/GuestListViewModel.cs
- ViewModels/GuestDetailViewModel.cs
- ViewModels/GuestHistoryItemViewModel.cs
- ViewModels/GuestHistoryViewModel.cs
- Views/Accounts/Create.cshtml
- Views/Accounts/Edit.cshtml
- Views/Accounts/Delete.cshtml
- Views/Accounts/Index.cshtml
- Views/Guests/Index.cshtml
- Views/Guests/Create.cshtml
- Views/Guests/Edit.cshtml
- Views/Guests/Details.cshtml
- Views/Guests/History.cshtml
- Views/Shared/_Layout.cshtml

### Implementation
- Bo sung soft delete, audit log, seeding role/account/guest/reservation, va migration schema moi.
- Hoan thanh UC5.1-UC5.6 voi create, edit, delete, list, search, va detail cho account.
- Hoan thanh UC15.1-UC15.4 voi create, view, edit, va booking history cho guest profile.
- Cap nhat navigation, dashboard, va startup de app co the chay voi schema moi.

### Notes
- App da build thanh cong sau khi them migration va tai cau truc EF Core.
- So lieu seed ban dau duoc them de test login, account management, va guest history.

### Status
Completed

## [2026-06-16 00:01]

### Task
Tiep tuc xay dung chuc nang quan ly tai khoan cho Admin.

### Files Changed
- Controllers/AccountsController.cs
- ViewModels/AccountListItemViewModel.cs
- ViewModels/AccountListViewModel.cs
- ViewModels/AccountDetailViewModel.cs
- Views/Accounts/Index.cshtml
- Views/Accounts/Details.cshtml
- Views/Shared/_Layout.cshtml
- Views/Home/Dashboard.cshtml

### Implementation
- Them AccountsController voi danh sach, tim kiem, phan trang va xem chi tiet tai khoan.
- Bo sung cac view model de hien thi du lieu co mask phone va thong tin read-only.
- Tao giao dien danh sach, tim kiem, pagination va trang chi tiet tai khoan.
- Cap nhat navigation va dashboard de dieu huong nhanh toi Account Management cho Admin.

### Notes
- Luong hien tai chi doc du lieu tai khoan; chua co create/edit/delete de tranh dong cham den schema hien co.
- Validation build da duoc kiem tra thanh cong sau khi sua loi Razor pagination.

### Status
Completed
