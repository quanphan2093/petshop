
Create database PetStoreTest
go

use PetStoreTest

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

ALTER TABLE Product
ADD Size INT DEFAULT 0;

ALTER TABLE Forum
ADD [Image] NVARCHAR(255),
	[Age] INT DEFAULT 0,
	[GENE] BIT DEFAULT 0;

ALTER TABLE Forum
ADD views INT,
	likes int,
	comments INT,
	title NVARCHAR(255);

CREATE TABLE Hashtags(
	HashtagID INT PRIMARY KEY IDENTITY,
    Tag VARCHAR(100) UNIQUE NOT NULL
)

CREATE TABLE PostHashtags (
	PostHashtagsId INT PRIMARY KEY IDENTITY,
    ForumID INT NOT NULL,
    HashtagID INT NOT NULL,
    FOREIGN KEY (ForumID) REFERENCES Forum(ForumId),
    FOREIGN KEY (HashtagID) REFERENCES Hashtags(HashtagID)
);

CREATE TABLE ForumType(
	TypeId INT PRIMARY KEY IDENTITY,
	TypeName NVARCHAR(255)
)

ALTER TABLE Forum
ADD TypeId INT FOREIGN KEY (TypeId) REFERENCES ForumType(TypeId);

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
--admin123
('adminfurfriends@gmail.com', '$2a$11$mTqrUFm2l58Yf.8rZuoXRetDlqtZPGe2T2IN7p9OjjjG4SuBtdVGe', GETDATE(), GETDATE(), 'Active', 1),
--sales123
('salesfurfriends@gmail.com', '$2a$11$L5M8UODpM6bbsbDJRUiEoOWpfggiJD8CiZhbz9qj2rpFgWw0iPXyi', GETDATE(), GETDATE(), 'Active', 3),
--support123
('supportfurfriends@gmail.com', '$2a$11$Tq5zSpdnI38J8J3qtuzP6Ogs6.BTOwJ4JDl3.mp0R40pvTX8vnqWm', GETDATE(), GETDATE(), 'Inactive', 4),
--manager123
('managerfurfriends@gmail.com', '$2a$11$F344X3nn60LQbidNnp0NeuZc0q.QbjrWbTj1qY2p/PFEzV.CODo4u', GETDATE(), GETDATE(), 'Active', 5);

INSERT INTO Infor (Fullname, Phone, Address, Gender, Image, AccountID) values
('Admin','0396925536','Thuong Tin, Ha Noi','1',null,1)

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

INSERT INTO ForumType VALUES (N'Câu chuyện thú cưng') ,(N'Thắc mắc & tư vấn')

create table DiscountCode(
	codeId int primary key identity(1,1),
	Code varchar(50),
	discountPercent int,
	[status] varchar(50)
)


ALTER TABLE [Order]
ADD DiscountID INT;

ALTER TABLE [Order]
ADD CONSTRAINT FK_Order_Discount FOREIGN KEY (DiscountID)
REFERENCES DiscountCode(codeId);

update StatusOrder set StatusName = N'Chờ xác nhận' where StatusID = 1;

create table Shop(
	shopId int primary key identity(1,1) not null,
	shopName nvarchar(255),
	[address] nvarchar(255),
	phone varchar(10),
	website nvarchar(255),
	facebookUrl nvarchar(255),
	createAt Datetime,
	[status] varchar(50)
)

ALTER TABLE [Product]
ADD shopId INT;

ALTER TABLE [Product]
ADD CONSTRAINT FK_Product_Shop FOREIGN KEY (shopId)
REFERENCES Shop(shopId);

create table banner (
	bannerId int primary key identity (1,1),
	bannerImage varchar(255),
	bannerUrl varchar(255),
	clickCount int,
	createAt datetime,
	[status] varchar(50) 
)

insert into banner (bannerImage, bannerUrl, clickCount, createAt, [status]) values
('/images/bannerforum.jpg','/home',0, GETDATE(),'Active'),
('/images/bannerfacebook.png','https://www.facebook.com/nhungnguoibanlamlongg',0, GETDATE(),'Active'),
('/images/bannerproduct.png','/product',0, GETDATE(),'Active')