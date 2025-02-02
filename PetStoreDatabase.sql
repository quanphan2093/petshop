
Create database PetStore
go

use PetStore

Go
-- Table: Role
CREATE TABLE Role (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(255)
);

-- Table: Account
CREATE TABLE Account (
    AccountID INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255),
    Password NVARCHAR(255),
    CreateAt DATETIME,
    UpdateAt DATETIME,
	[State] Nvarchar(255),
    RoleID INT FOREIGN KEY REFERENCES Role(RoleID)
);

-- Table: Address
CREATE TABLE Address (
    AddressID INT PRIMARY KEY IDENTITY(1,1),
    FullNameCustomer NVARCHAR(255),
    Phone NVARCHAR(50),
    Address NVARCHAR(255),
    Email NVARCHAR(255),
    Gender NVARCHAR(10)
);

-- Table: StatusOrder
CREATE TABLE StatusOrder (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(255)
);

-- Table: Category
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(255)
);

-- Table: Product
CREATE TABLE Product (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(255),
    Details NVARCHAR(MAX),
    Price DECIMAL(18, 2),
    Image NVARCHAR(255),
    Discount FLOAT,
	CreateAt DateTime,
	UpdateAt DateTime,
    Status NVARCHAR(50),
    CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID)
);

-- Table: Order
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    CreateAt DATETIME,
    UpdateAt DATETIME,
    Note NVARCHAR(MAX),
	NoteBySale NVARCHAR(MAX),
	SaleId Int,
	AccountId INT Foreign Key References Account(AccountId),
    AddressID INT FOREIGN KEY REFERENCES Address(AddressID),
    StatusID INT FOREIGN KEY REFERENCES StatusOrder(StatusID),
);

-- Table: OrderDetails
CREATE TABLE OrderDetails (
    OrderDetailID INT PRIMARY KEY IDENTITY(1,1),	
	Total Decimal,
	CreateAt Datetime,
	UnitPrice Decimal,
    OrderID INT FOREIGN KEY REFERENCES [Order](OrderID),
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
    Quantity INT
);

-- Table: ShoppingCart
CREATE TABLE ShoppingCart (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    AccountId INT Foreign Key References Account(AccountId),
	Quantity Int,
    CreateAt DATETIME,
    UpdateAt DATETIME,
	ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
);

-- Table: Forum
CREATE TABLE Forum (
    ForumID INT PRIMARY KEY IDENTITY(1,1),
    Content NVARCHAR(MAX),
	CommentId Int, 
    CreateAt DATETIME,
	UpdateAt Datetime,
	Status NVARCHAR(255),
    AccountID INT FOREIGN KEY REFERENCES Account(AccountID)
);

-- Table: Comment
CREATE TABLE Comment (
    CommentID INT PRIMARY KEY IDENTITY(1,1),
    ForumID INT FOREIGN KEY REFERENCES Forum(ForumID),
    ParentCommentID INT,
    Content NVARCHAR(MAX),
	CreateAt Datetime,
    AccountID INT FOREIGN KEY REFERENCES Account(AccountID)
);

-- Table: Feedback
CREATE TABLE Feedback (
    FeedbackID INT PRIMARY KEY IDENTITY(1,1),
    AccountID INT FOREIGN KEY REFERENCES Account(AccountID),
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
    Feedback NVARCHAR(MAX),
	Image NVARCHAR(MAX),
    CreateAt DATETIME,
	UpdateAt DATETIME,
);

-- Table: Messenger
CREATE TABLE Messenger (
    MessengerID INT PRIMARY KEY IDENTITY(1,1),
    AccountID INT FOREIGN KEY REFERENCES Account(AccountID),
    StaffID INT,
    MessageText NVARCHAR(MAX),
    SentTime DATETIME
);

CREATE TABLE StateInfor(
	StateId INT PRIMARY KEY IDENTITY(1,1),
	StateName NVARCHAR(255),
)

CREATE TABLE Infor (
	InforId INT PRIMARY KEY IDENTITY(1,1),
	Fullname Nvarchar(255),
	Phone Nvarchar(10),
	Address Nvarchar(255),
	Gender Nvarchar(255),
	Image Nvarchar(Max),
	AccountID INT FOREIGN KEY REFERENCES Account(AccountID),
	StateId INT FOREIGN KEY REFERENCES StateInfor(StateId)

)

CREATE TABLE ProductImage(
	ImgID int primary key identity(1,1),
	ImgUrl varchar(MAX),
	CreateAt DateTime,
	UpdateAt DateTime,
    Status NVARCHAR(50),
	ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
)

ALTER TABLE Product
ADD UnitOrdered INT DEFAULT 0;

ALTER TABLE Product
ADD UnitInStock INT DEFAULT 0;

ALTER TABLE Feedback
ADD love INT DEFAULT 0,
    [like] INT DEFAULT 0,
    dislike INT DEFAULT 0;


CREATE TABLE PaymentMethod (
	methodId int primary key identity(1,1),
	methodName nvarchar(50),
	createAt DateTime,
	updateAt Datetime,
	[status] nvarchar(10)
)

INSERT INTO PaymentMethod (methodName, createAt, updateAt, [status])  
VALUES  
(N'Chuyển khoản ngân hàng', GETDATE(), GETDATE(), N'active'),  
(N'Thanh toán khi nhận hàng (COD)', GETDATE(), GETDATE(), N'active');
ALTER TABLE [Order]
ADD paymentMethodId INT;

ALTER TABLE [Order]
ADD CONSTRAINT FK_Order_PaymentMethod
FOREIGN KEY (paymentMethodId)
REFERENCES PaymentMethod(methodId);

INSERT INTO Role (RoleName)
VALUES 
('Admin'),
('Customer'),
('Sales'),
('Support'),
('Manager');

INSERT INTO Account (Email, Password, CreateAt, UpdateAt, [State], RoleID)
VALUES 
('admin@example.com', 'admin123', GETDATE(), GETDATE(), 'Active', 1),
('sales@example.com', 'sales123', GETDATE(), GETDATE(), 'Active', 3),
('support@example.com', 'support123', GETDATE(), GETDATE(), 'Inactive', 4),
('manager@example.com', 'manager123', GETDATE(), GETDATE(), 'Active', 5);

INSERT INTO StatusOrder (StatusName)
VALUES 
(N'Chờ thanh toán'),
(N'Vận chuyển'),
(N'Chờ giao hàng'),
(N'Hoàn thành'),
(N'Ðã hủy'),
(N'Trả hàng/ hoàn tiền');

INSERT INTO Category (CategoryName)
VALUES 
(N'Thời trang'),
(N'Thức ăn'),
(N'Đồ chơi'),
(N'Vòng cổ');