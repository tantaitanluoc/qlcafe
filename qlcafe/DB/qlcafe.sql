CREATE DATABASE QuanLyQuanCafe
GO

USE QuanLyQuanCafe
GO

-- Food
-- Table
-- FoodCategory
-- Account
-- Bill
-- BillInfo

CREATE TABLE TableFood
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Bàn chưa có tên',
	status NVARCHAR(100) NOT NULL DEFAULT N'Trống'	-- Trống || Có người
)
GO

CREATE TABLE Account
(
	usname NVARCHAR(100) PRIMARY KEY,	
	dplname NVARCHAR(100) NOT NULL DEFAULT N'user',
	passwd NVARCHAR(1000) NOT NULL DEFAULT 0,
	Type INT NOT NULL  DEFAULT 0 -- 1: admin && 0: staff
)
GO

CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên'
)
GO

CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
	idCategory INT NOT NULL,
	price FLOAT NOT NULL DEFAULT 0,
	onservice int default 1 -- 1: còn phục vụ, 0: ngưng phục vụ
	
	FOREIGN KEY (idCategory) REFERENCES dbo.FoodCategory(id)
)
GO

CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATE,
	idTable INT NOT NULL,
	status INT NOT NULL DEFAULT 0, -- 1: đã thanh toán && 0: chưa thanh toán,
	discount float default 0,
	totalPrice float default 0,
	paid float default 0
	
	FOREIGN KEY (idTable) REFERENCES dbo.TableFood(id)
)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0
	
	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)
)
GO



--- Insert data

insert into Account values (
N'admin',
N'Administator',
N'5feceb66ffc86f38d952786c6d696c79c2dbc239dd4e91b46729d73a27fb57e9',
1
)
go
insert into Account values (
N'staff',
N'Staff',
N'5feceb66ffc86f38d952786c6d696c79c2dbc239dd4e91b46729d73a27fb57e9',
0
)
go


create proc USP_GetAccountByUserName
@usName nvarchar(100)
as
begin
	select * from Account where usname = @usName;
end
go

create proc USP_Login
@usName nvarchar(100), @passwd nvarchar(1000)as
begin
	select COUNT(*) from Account where usname = @usName and passwd = @passwd;
end
go


declare @i int = 11
while @i <= 20
begin
	insert into tablefood(name) values (N'Bàn ' + cast(@i as nvarchar(100)));
	set @i = @i + 1;
end
go


create proc USP_GetTablesList as select * from TableFood;
go

insert into FoodCategory(name) values(N'hải sản')
insert into FoodCategory(name) values(N'thịt')
insert into FoodCategory(name) values(N'cá')
insert into FoodCategory(name) values(N'tráng miệng')
insert into FoodCategory(name) values(N'nước uống')
go

insert into Food(name, idCategory, price)
values(N'Mực một nắng nướng sa tế',1,120000)
insert into Food(name, idCategory, price)
values(N'Nghêu hấp sả',1,50000)
insert into Food(name, idCategory, price)
values(N'Vú dê nướng sữa',2,150000)
insert into Food(name, idCategory, price)
values(N'Cá lóc nướng trui',3,100000)
insert into Food(name, idCategory, price)
values(N'Sữa chua',4,10000)
insert into Food(name, idCategory, price)
values(N'Cafe',5,12000)
insert into Food(name, idCategory, price)
values(N'Cafe sữa',5,17000)
insert into Food(name, idCategory, price)
values(N'Trà đá',1,10000)
insert into Food(name, idCategory, price)
values(N'Đá chanh',1,12000)
go


create proc USP_InsertBill
@idTable int
as
begin
insert into Bill(DateCheckOut,idTable, discount) values(null,@idTable, 0); 
end
go

create proc USP_InsertBillInfo
@idBill int, @idFood int, @count int
as
begin
	declare @i int;
	declare @c int = 1;
	select @i = id, @c = count from BillInfo where idBill = @idBill and idFood = @idFood;
	if(@i > 0)
		begin
			declare @t int = @c + @count;
			if(@t > 0)
				update BillInfo set count = @t where idFood = @idFood;
			else
				delete BillInfo where idBill = @idBill and idFood = @idFood;
		end
	else
		begin
			insert into BillInfo(idBill,idFood,count) values(@idBill,@idFood,@count);
		end
	
	
end
go

create trigger UTG_UpdateBillInfo
on BillInfo for update, insert
as
begin 
	declare @idBill int;
	declare @idTable int;
	select @idBill = idBill from inserted;
	select @idTable = idTable from Bill where id = @idBill and status = 0;
	update TableFood set status = N'Có người' where id = @idTable;
end
go

create trigger UTG_UpdateBill
on Bill for update
as
begin
	declare @idBill int;
	declare @idTable int;
	select @idBill = id from inserted;
	select @idTable = idTable from Bill where id = @idBill;
	declare @count int = 0;
	select @count = COUNT(*) from Bill where idTable = @idTable and status = 0
	if(@count > 0)
	--	update TableFood set status = N'Trống' where id = @idTable; 
	--else
		update TableFood set status = N'Có người' where id = @idTable; 
		
	update TableFood set status = N'Trống' where id not in(select idTable from BillInfo as BI, Bill as B where BI.idBill = B.id and B.status = 0)
end
go


create proc USP_SwitchTables
@id1 int, @id2 int
as 
begin
	declare @firstBill int
	declare @secondBill int
	select  @firstBill = id from Bill where idTable = @id1 and status = 0
	select  @secondBill = id from Bill where idTable = @id2 and status = 0
	if(@firstBill is null)
		begin
			insert into Bill(DateCheckIn, DateCheckOut, idTable, status)
			values (GETDATE(), null, @id1, 0)
			select @firstBill = MAX(id) from Bill where idTable = @id1 and status = 0
		end
	if(@secondBill is null)
		begin
			insert into Bill(DateCheckIn, DateCheckOut, idTable, status)
			values (GETDATE(), null, @id2, 0)
			select @secondBill = MAX(id) from Bill where idTable = @id2 and status = 0
		end
	update Bill set idTable = @id2 where id = @firstBill
	update Bill set idTable = @id1 where id = @secondBill
end
go


create proc USP_GetListBillByDateRange
@from date, @to date as
begin
	select tf.name as [Bàn], b.DateCheckIn as [Checkin], b.DateCheckOut as [Checkout], b.totalPrice as [Tổng cộng], b.discount as [Giảm giá (%)], b.paid as [Thành tiền]
	from Bill as b, TableFood as tf
	where tf.id = b.idTable and b.status = 1
	and b.DateCheckOut >= @from and b.DateCheckOut <= @to
end
go

create proc USP_UpdateAccount
@usname nvarchar(100), @dplname nvarchar(100), @passwd nvarchar(1000), @new_passwd nvarchar(1000)
as
begin
	declare @auth int
	select @auth = COUNT(*) from Account where usname = @usname and passwd = @passwd
	if(@auth = 1)
		begin
			if(@new_passwd is null or @new_passwd = '')
				update Account set dplname = @dplname where usname = @usname
			else
				update Account set dplname = @dplname, passwd = @new_passwd where usname = @usname
		end
end
go
 

CREATE FUNCTION [dbo].[fuConvertToUnsign1] ( @strInput NVARCHAR(4000) ) 
RETURNS NVARCHAR(4000) 
AS 
BEGIN 
	IF @strInput IS NULL RETURN @strInput 
	IF @strInput = '' RETURN @strInput 
	DECLARE @RT NVARCHAR(4000) 
	DECLARE @SIGN_CHARS NCHAR(136) 
	DECLARE @UNSIGN_CHARS NCHAR (136) 
	SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) 
	SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' 
	DECLARE @COUNTER int 
	DECLARE @COUNTER1 int 
	SET @COUNTER = 1 
	WHILE (@COUNTER <=LEN(@strInput)) 
		BEGIN 
			SET @COUNTER1 = 1 
			WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) 
				BEGIN 
					IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) 
						BEGIN 
							IF @COUNTER=1 
								SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) 
							ELSE 
								SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) 
								BREAK 
						END 
					SET @COUNTER1 = @COUNTER1 +1 
				END 
			SET @COUNTER = @COUNTER +1 
		END 
	SET @strInput = replace(@strInput,' ','-') 
	RETURN @strInput 
END
GO
