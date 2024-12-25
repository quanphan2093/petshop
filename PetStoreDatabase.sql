Create database PetStore

go

use PetStore

Go

-- Table: Address
CREATE TABLE Address (
    AddressID INT PRIMARY KEY,
    FullNameCustomer NVARCHAR(255),
    Phone NVARCHAR(50),
    Address NVARCHAR(255),
    Email NVARCHAR(255),
    Gender NVARCHAR(10)
);

-- Table: StatusOrder
CREATE TABLE StatusOrder (
    StatusID INT PRIMARY KEY,
    StatusName NVARCHAR(255)
);

-- Table: Category
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY,
    CategoryName NVARCHAR(255)
);

-- Table: Product
CREATE TABLE Product (
    ProductID INT PRIMARY KEY,
    ProductName NVARCHAR(255),
    Details NVARCHAR(MAX),
    Price DECIMAL(18, 2),
    Image NVARCHAR(255),
    Discount FLOAT,
    Status NVARCHAR(50),
    CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID)
);

-- Table: ProductDetail
CREATE TABLE ProductDetail (
    ProductDetailID INT PRIMARY KEY,
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID)
);

-- Table: Order
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY,
    CreateAt DATETIME,
    UpdateAt DATETIME,
    Note NVARCHAR(MAX),
    AddressID INT FOREIGN KEY REFERENCES Address(AddressID),
    StatusID INT FOREIGN KEY REFERENCES StatusOrder(StatusID)
);

-- Table: OrderDetails
CREATE TABLE OrderDetails (
    OrderDetailID INT PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES [Order](OrderID),
    ProductDetailID INT FOREIGN KEY REFERENCES ProductDetail(ProductDetailID),
    Quantity INT
);

-- Table: ShoppingCart
CREATE TABLE ShoppingCart (
    CartID INT PRIMARY KEY,
    AccountID INT,
    CreateAt DATETIME,
    UpdateAt DATETIME,
    Status NVARCHAR(50)
);

-- Table: Role
CREATE TABLE Role (
    RoleID INT PRIMARY KEY,
    RoleName NVARCHAR(255)
);

-- Table: Account
CREATE TABLE Account (
    AccountID INT PRIMARY KEY,
    Email NVARCHAR(255),
    Password NVARCHAR(255),
    CreateAt DATETIME,
    UpdateAt DATETIME,
    RoleID INT FOREIGN KEY REFERENCES Role(RoleID)
);

-- Table: Forum
CREATE TABLE Forum (
    ForumID INT PRIMARY KEY,
    Content NVARCHAR(MAX),
    CreateAt DATETIME,
    AccountID INT FOREIGN KEY REFERENCES Account(AccountID)
);

-- Table: Comment
CREATE TABLE Comment (
    CommentID INT PRIMARY KEY,
    ForumID INT FOREIGN KEY REFERENCES Forum(ForumID),
    ParentCommentID INT,
    Content NVARCHAR(MAX),
    AccountID INT FOREIGN KEY REFERENCES Account(AccountID)
);

-- Table: Feedback
CREATE TABLE Feedback (
    FeedbackID INT PRIMARY KEY,
    AccountID INT FOREIGN KEY REFERENCES Account(AccountID),
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID),
    Feedback NVARCHAR(MAX),
    CreateAt DATETIME
);

-- Table: Staff
CREATE TABLE Staff (
    StaffID INT PRIMARY KEY,
    FullName NVARCHAR(255),
    Address NVARCHAR(255),
    Phone NVARCHAR(50),
    Gender NVARCHAR(10)
);

-- Table: Messenger
CREATE TABLE Messenger (
    MessengerID INT PRIMARY KEY,
    AccountID INT FOREIGN KEY REFERENCES Account(AccountID),
    StaffID INT FOREIGN KEY REFERENCES Staff(StaffID),
    MessageText NVARCHAR(MAX),
    SentTime DATETIME
);