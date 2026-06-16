---- ==========================================
---- CREATE DATABASE
---- ==========================================
--CREATE DATABASE HotelManagement;
--GO

--USE HotelManagement;
--GO

---- ==========================================
---- ROLE
---- ==========================================
--CREATE TABLE Role (
--    RoleID INT IDENTITY(1,1) PRIMARY KEY,
--    RoleName NVARCHAR(50) NOT NULL UNIQUE,
--    Description NVARCHAR(255)
--);
--GO

---- ==========================================
---- ACCOUNT
---- ==========================================
--CREATE TABLE Account (
--    AccountID INT IDENTITY(1,1) PRIMARY KEY,
--    Username NVARCHAR(50) NOT NULL UNIQUE,
--    PasswordHash NVARCHAR(255) NOT NULL,
--    Email NVARCHAR(100),
--    Phone NVARCHAR(20),

--    Status NVARCHAR(20) NOT NULL
--        CHECK (Status IN ('Active','Inactive'))
--        DEFAULT 'Active',

--    CreatedAt DATETIME2 DEFAULT GETDATE(),
--    UpdatedAt DATETIME2 DEFAULT GETDATE(),

--    RoleID INT NOT NULL,

--    CONSTRAINT FK_Account_Role
--        FOREIGN KEY(RoleID)
--        REFERENCES Role(RoleID)
--);
--GO

---- ==========================================
---- HOTEL
---- ==========================================
--CREATE TABLE Hotel (
--    HotelID INT IDENTITY(1,1) PRIMARY KEY,
--    HotelName NVARCHAR(150) NOT NULL,
--    Address NVARCHAR(255),
--    Phone NVARCHAR(20),
--    Email NVARCHAR(100),
--    Description NVARCHAR(MAX),
--    Logo NVARCHAR(255),
--    CheckInTime TIME,
--    CheckOutTime TIME
--);
--GO

---- ==========================================
---- ROOM TYPE
---- ==========================================
--CREATE TABLE RoomType (
--    RoomTypeID INT IDENTITY(1,1) PRIMARY KEY,
--    TypeName NVARCHAR(100) NOT NULL,

--    Description NVARCHAR(MAX),

--    Capacity INT NOT NULL,

--    BasePrice DECIMAL(18,2) NOT NULL,

--    Size DECIMAL(10,2),

--    Status NVARCHAR(20)
--        CHECK (Status IN ('Active','Inactive'))
--        DEFAULT 'Active'
--);
--GO

---- ==========================================
---- ROOM
---- ==========================================
--CREATE TABLE Room (
--    RoomID INT IDENTITY(1,1) PRIMARY KEY,

--    RoomNumber NVARCHAR(20) NOT NULL UNIQUE,

--    FloorNumber INT,

--    Status NVARCHAR(30)
--        CHECK (
--            Status IN (
--                'Available',
--                'Occupied',
--                'Dirty',
--                'Cleaning',
--                'Maintenance'
--            )
--        )
--        DEFAULT 'Available',

--    Description NVARCHAR(MAX),

--    RoomTypeID INT NOT NULL,

--    CONSTRAINT FK_Room_RoomType
--        FOREIGN KEY(RoomTypeID)
--        REFERENCES RoomType(RoomTypeID)
--);
--GO

---- ==========================================
---- GUEST
---- ==========================================
--CREATE TABLE Guest (
--    GuestID INT IDENTITY(1,1) PRIMARY KEY,

--    FullName NVARCHAR(150) NOT NULL,

--    Gender NVARCHAR(20)
--        CHECK (Gender IN ('Male','Female','Other')),

--    DateOfBirth DATE,

--    Nationality NVARCHAR(100),

--    IdentityNumber NVARCHAR(50),

--    Phone NVARCHAR(20),

--    Email NVARCHAR(100),

--    Address NVARCHAR(255),

--    CreatedAt DATETIME2 DEFAULT GETDATE()
--);
--GO

---- ==========================================
---- RESERVATION
---- ==========================================
--CREATE TABLE Reservation (
--    ReservationID INT IDENTITY(1,1) PRIMARY KEY,

--    ReservationCode NVARCHAR(50) UNIQUE,

--    CheckInDate DATE NOT NULL,

--    CheckOutDate DATE NOT NULL,

--    NumberOfGuests INT DEFAULT 1,

--    BookingDate DATETIME2 DEFAULT GETDATE(),

--    Status NVARCHAR(30)
--        CHECK (
--            Status IN (
--                'Pending',
--                'Confirmed',
--                'CheckedIn',
--                'Completed',
--                'Cancelled'
--            )
--        )
--        DEFAULT 'Pending',

--    Notes NVARCHAR(MAX),

--    GuestID INT NOT NULL,

--    CreatedBy INT NOT NULL,

--    CONSTRAINT FK_Reservation_Guest
--        FOREIGN KEY(GuestID)
--        REFERENCES Guest(GuestID),

--    CONSTRAINT FK_Reservation_Account
--        FOREIGN KEY(CreatedBy)
--        REFERENCES Account(AccountID)
--);
--GO

---- ==========================================
---- RESERVATION ROOM
---- ==========================================
--CREATE TABLE ReservationRoom (
--    ReservationRoomID INT IDENTITY(1,1) PRIMARY KEY,

--    ReservationID INT NOT NULL,

--    RoomID INT NOT NULL,

--    PricePerNight DECIMAL(18,2) NOT NULL,

--    CONSTRAINT FK_ReservationRoom_Reservation
--        FOREIGN KEY(ReservationID)
--        REFERENCES Reservation(ReservationID),

--    CONSTRAINT FK_ReservationRoom_Room
--        FOREIGN KEY(RoomID)
--        REFERENCES Room(RoomID)
--);
--GO

---- ==========================================
---- SERVICE
---- ==========================================
--CREATE TABLE Service (
--    ServiceID INT IDENTITY(1,1) PRIMARY KEY,

--    ServiceName NVARCHAR(150) NOT NULL,

--    Description NVARCHAR(MAX),

--    Price DECIMAL(18,2) NOT NULL,

--    Status NVARCHAR(20)
--        CHECK (Status IN ('Active','Inactive'))
--        DEFAULT 'Active'
--);
--GO

---- ==========================================
---- SERVICE USAGE
---- ==========================================
--CREATE TABLE ServiceUsage (
--    UsageID INT IDENTITY(1,1) PRIMARY KEY,

--    ReservationID INT NOT NULL,

--    ServiceID INT NOT NULL,

--    Quantity INT NOT NULL DEFAULT 1,

--    UnitPrice DECIMAL(18,2) NOT NULL,

--    UsageDate DATETIME2 DEFAULT GETDATE(),

--    RecordedBy INT NOT NULL,

--    CONSTRAINT FK_ServiceUsage_Reservation
--        FOREIGN KEY(ReservationID)
--        REFERENCES Reservation(ReservationID),

--    CONSTRAINT FK_ServiceUsage_Service
--        FOREIGN KEY(ServiceID)
--        REFERENCES Service(ServiceID),

--    CONSTRAINT FK_ServiceUsage_Account
--        FOREIGN KEY(RecordedBy)
--        REFERENCES Account(AccountID)
--);
--GO

---- ==========================================
---- INVOICE
---- ==========================================
--CREATE TABLE Invoice (
--    InvoiceID INT IDENTITY(1,1) PRIMARY KEY,

--    ReservationID INT NOT NULL,

--    InvoiceDate DATETIME2 DEFAULT GETDATE(),

--    RoomCharge DECIMAL(18,2) DEFAULT 0,

--    ServiceCharge DECIMAL(18,2) DEFAULT 0,

--    Tax DECIMAL(18,2) DEFAULT 0,

--    Discount DECIMAL(18,2) DEFAULT 0,

--    TotalAmount DECIMAL(18,2) NOT NULL,

--    Status NVARCHAR(20)
--        CHECK (Status IN ('Pending','Paid','Cancelled'))
--        DEFAULT 'Pending',

--    CONSTRAINT FK_Invoice_Reservation
--        FOREIGN KEY(ReservationID)
--        REFERENCES Reservation(ReservationID)
--);
--GO

---- ==========================================
---- PAYMENT
---- ==========================================
--CREATE TABLE Payment (
--    PaymentID INT IDENTITY(1,1) PRIMARY KEY,

--    InvoiceID INT NOT NULL,

--    Amount DECIMAL(18,2) NOT NULL,

--    PaymentMethod NVARCHAR(30)
--        CHECK (
--            PaymentMethod IN (
--                'Cash',
--                'CreditCard',
--                'BankTransfer',
--                'EWallet'
--            )
--        ),

--    PaymentDate DATETIME2 DEFAULT GETDATE(),

--    Status NVARCHAR(20)
--        CHECK (
--            Status IN (
--                'Pending',
--                'Completed',
--                'Failed'
--            )
--        )
--        DEFAULT 'Completed',

--    CONSTRAINT FK_Payment_Invoice
--        FOREIGN KEY(InvoiceID)
--        REFERENCES Invoice(InvoiceID)
--);
--GO

---- ==========================================
---- HOUSEKEEPING TASK
---- ==========================================
--CREATE TABLE HousekeepingTask (
--    TaskID INT IDENTITY(1,1) PRIMARY KEY,

--    RoomID INT NOT NULL,

--    AssignedTo INT NOT NULL,

--    TaskDate DATE NOT NULL,

--    Status NVARCHAR(20)
--        CHECK (
--            Status IN (
--                'Pending',
--                'InProgress',
--                'Completed'
--            )
--        )
--        DEFAULT 'Pending',

--    Notes NVARCHAR(MAX),

--    CONSTRAINT FK_HousekeepingTask_Room
--        FOREIGN KEY(RoomID)
--        REFERENCES Room(RoomID),

--    CONSTRAINT FK_HousekeepingTask_Account
--        FOREIGN KEY(AssignedTo)
--        REFERENCES Account(AccountID)
--);
--GO

---- ==========================================
---- MAINTENANCE REPORT
---- ==========================================
--CREATE TABLE MaintenanceReport (
--    ReportID INT IDENTITY(1,1) PRIMARY KEY,

--    RoomID INT NOT NULL,

--    ReportedBy INT NOT NULL,

--    IssueDescription NVARCHAR(MAX) NOT NULL,

--    Priority NVARCHAR(20)
--        CHECK (
--            Priority IN (
--                'Low',
--                'Medium',
--                'High',
--                'Critical'
--            )
--        )
--        DEFAULT 'Medium',

--    Status NVARCHAR(20)
--        CHECK (
--            Status IN (
--                'Open',
--                'InProgress',
--                'Resolved'
--            )
--        )
--        DEFAULT 'Open',

--    ReportDate DATETIME2 DEFAULT GETDATE(),

--    ResolvedDate DATETIME2 NULL,

--    CONSTRAINT FK_MaintenanceReport_Room
--        FOREIGN KEY(RoomID)
--        REFERENCES Room(RoomID),

--    CONSTRAINT FK_MaintenanceReport_Account
--        FOREIGN KEY(ReportedBy)
--        REFERENCES Account(AccountID)
--);
--GO

---- ==========================================
---- AUDIT LOG
---- ==========================================
--CREATE TABLE AuditLog (
--    LogID INT IDENTITY(1,1) PRIMARY KEY,

--    AccountID INT NOT NULL,

--    Action NVARCHAR(255) NOT NULL,

--    EntityName NVARCHAR(100),

--    EntityID INT,

--    CreatedAt DATETIME2 DEFAULT GETDATE(),

--    CONSTRAINT FK_AuditLog_Account
--        FOREIGN KEY(AccountID)
--        REFERENCES Account(AccountID)
--);
--GO

---- ==========================================
---- DEFAULT ROLES
---- ==========================================
--INSERT INTO Role(RoleName, Description)
--VALUES
--('Admin','System Administrator'),
--('Hotel Manager','Hotel Manager'),
--('Receptionist','Receptionist'),
--('Room Staff','Room Staff');
--GO