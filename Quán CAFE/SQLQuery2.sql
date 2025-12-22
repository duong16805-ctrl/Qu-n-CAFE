CREATE PROC USP_GetTableList AS SELECT * FROM DiningTable;
GO

CREATE PROC USP_GetCategoryList AS SELECT * FROM Category;
GO

CREATE PROC USP_GetProductByCategoryID @id INT AS SELECT * FROM Product WHERE CategoryID = @id;
GO

CREATE PROC USP_InsertInvoice @tableID INT
AS 
BEGIN
    INSERT INTO Invoice (TableID, OrderDate, TotalAmount, IsPaid) VALUES (@tableID, GETDATE(), 0, 0);
    SELECT SCOPE_IDENTITY();
END;
GO

CREATE PROC USP_InsertInvoiceDetail @invoiceID INT, @productID NVARCHAR(20), @quantity INT, @price INT
AS
BEGIN
    INSERT INTO InvoiceDetail (InvoiceID, ProductID, Quantity, PriceAtTime) VALUES (@invoiceID, @productID, @quantity, @price);
    UPDATE Invoice SET TotalAmount = TotalAmount + (@quantity * @price) WHERE InvoiceID = @invoiceID;
END;
GO

CREATE PROC USP_PayInvoice @invoiceID INT
AS
BEGIN
    UPDATE Invoice SET IsPaid = 1 WHERE InvoiceID = @invoiceID;
    DECLARE @tableID INT = (SELECT TableID FROM Invoice WHERE InvoiceID = @invoiceID);
    UPDATE DiningTable SET Status = N'Chờ dọn' WHERE TableID = @tableID;
END;
GO

-- 4. Dữ liệu mẫu ban đầu
INSERT INTO Category (CategoryName) VALUES (N'Cà phê'), (N'Trà'), (N'Bánh'), (N'Khác');
INSERT INTO Product (ProductID, ProductName, Price, CategoryID) VALUES 
('M01', N'Espresso', 35000, 1), ('M02', N'Latte Hồng', 45000, 1), ('M03', N'Capuchino', 50000, 1),
('M04', N'Trà Đào Cam Sả', 40000, 2), ('M05', N'Bánh Croissant', 25000, 3);

DECLARE @i INT = 1;
WHILE @i <= 16
BEGIN
    INSERT INTO DiningTable (TableName) VALUES (N'Bàn ' + CAST(@i AS NVARCHAR(2)));
    SET @i = @i + 1;
END;
GO


