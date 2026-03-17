SELECT TOP (1000) [ProductID]
      ,[Name]
      ,[ProductNumber]
      ,[Color]
      ,[StandardCost]
      ,[ListPrice]
      ,[Size]
      ,[Weight]
      ,[ProductCategoryID]
      ,[ProductModelID]
      ,[SellStartDate]
      ,[SellEndDate]
      ,[DiscontinuedDate]
      ,[ThumbNailPhoto]
      ,[ThumbnailPhotoFileName]
      ,[rowguid]
      ,[ModifiedDate]
  FROM [AdventureWorksLT2022].[SalesLT].[Product]


select distinct Color as product_color from SalesLT.Product
where Color is not null;

select * from SalesLT.ProductCategory

DECLARE @PAGE_NUMBER INT = 3;
DECLARE @ROWS_PER_PAGE INT = 10;

select * from SalesLT.ProductCategory
order by Name DESC
offset (@PAGE_NUMBER - 1) * @ROWS_PER_PAGE rows
fetch next @ROWS_PER_PAGE rows only;

select * from SalesLT.ProductCategory
order by Name DESC

-- like
SELECT * FROM SalesLT.Customer
WHERE FirstName LIKE '%R%';

SELECT * FROM SalesLT.Customer
WHERE FirstName = 'Robert';

-- between
select FirstName, LastName, ModifiedDate from SalesLT.Customer
where ModifiedDate between '2006-01-01' and '2007-01-01'

-- count
select count(*) as total_customers from SalesLT.Customer

select * from SalesLT.SalesOrderHeader
select count(*) from SalesLT.SalesOrderHeader
select distinct count(*) from SalesLT.SalesOrderHeader


select *  from SalesLT.SalesOrderHeader
order by AccountNumber

select SUM(TotalDue) as total_ventas from SalesLT.SalesOrderHeader


select COUNT(*) FROM SalesLT.Product

-- group
select ProductCategoryID as category_id, COUNT(ProductCategoryID) as total FROM SalesLT.Product
GROUP BY ProductCategoryID
ORDER BY total DESC

select * from SalesLT.Product p
inner join SalesLT.ProductCategory pc
	on p.ProductCategoryID =pc.ProductCategoryID


--select 
--	soh.SalesOrderID as sales_order_id,
--	c.FirstName as customer_first_name
--from SalesLT.SalesOrderHeader soh
--INNER JOIN SalesLT.Customer c 
--	on c.CustomerID = soh.CustomerID
--INNER JOIN

DECLARE @Fecha DATETIME2 = CONVERT(DATETIME2, '2025-04-02', 23)


Spotiy
Youtube
Steam
Sololearn
Linkedin
Whatsapp
X
Udemy
Instagram