use QuanLyQuanCafe
go

delete BillInfo;
delete Bill;
update TableFood set status = N'Trống'

update Bill set idTable = 8 where id = 20
update Bill set idTable = 7 where id = 17
select idTable from BillInfo as BI, Bill as B where BI.idBill = B.id and B.status = 0
select * from Bill;
select * from BillInfo;
select * from TableFood;

go

select * from Bill where idTable = 11 and status = 0
select * from Bill where idTable = 12 and status = 0 

exec USP_SwitchTables @id1 = 2 , @id2 = 3

select GETDATE();

alter table Bill add 
totalPrice float default 0,
paid float default 0
go

select * from TableFood;

select usname, dplname, type from Account;