--create table ImportMaterialType
--(
--TypeName nvarchar(150),
--LossPercentText nvarchar(50)
--)

--create table ImportMaterialProducts
--(
--MaterialName nvarchar(250),
--ProductName nvarchar(250),
--QuantityPerUnitText nvarchar(100)
--)

--create table ImportMaterials
--(
--MaterialName nvarchar(250),
--MaterialTypeName nvarchar(150),
--UnitPriceText nvarchar(100),
--StockQuantityText nvarchar(100),
--MinimumQuantityText nvarchar(100),
--PackageQuantityText nvarchar(100),
--UnitSymbol nvarchar(20)
--)

--create table ImportProductType 
--(
--TypeName nvarchar(150),
--TypeCoefficient nvarchar(50)
--)

--create table ImportProduct
--(
--Article nvarchar(40),
--ProductTypeName nvarchar(150),
--ProductName nvarchar(250),
--MinimumPartnerPriceText nvarchar(50)
--)

--use AromaDom
--go
--insert into MaterialTypes (TypeName, LossPercent)
--select TypeName,
--TRY_CONVERT(decimal(7,4), replace(replace(LossPercentText, '%', ''), ',', '.'))
--from ImportMaterialType

--insert into ProductTypes (TypeName, TypeCoefficient)
--select ltrim(rtrim(TypeName)),
--try_convert(decimal(12,6), try_convert(float, replace(TypeCoefficient, ',', '.')))
--from ImportProductType
--go

--insert into Units (Symbol, UnitName)
--select distinct ltrim(rtrim(UnitSymbol)),
--case ltrim(rtrim(UnitSymbol))
--when N'кг' then N'килограмм'
--when N'л' then N'литр'
--when N'м' then N'метр'
--when N'шт' then N'штука'
--else ltrim(rtrim(UnitSymbol))
--end
--from ImportMaterials
--where ltrim(rtrim(UnitSymbol)) not in (select Symbol from Units)
--go

--insert into Materials (MaterialName, MaterialTypeId, UnitId, UnitPrice, StockQuantity, MinimumQuantity, PackageQuantity)
--select ltrim(rtrim(im.MaterialName)),
--mt.MaterialTypeId,
--u.UnitId,
--try_convert(decimal(18,2), try_convert(float, replace(im.UnitPriceText, ',', '.'))),
--try_convert(decimal(18,3), try_convert(float, replace(im.StockQuantityText, ',', '.'))),
--try_convert(decimal(18,3), try_convert(float, replace(im.MinimumQuantityText, ',', '.'))),
--try_convert(decimal(18,3), try_convert(float, replace(im.PackageQuantityText, ',', '.')))
--from ImportMaterials im
--join MaterialTypes mt on mt.TypeName = ltrim(rtrim(im.MaterialTypeName))
--join Units u on u.Symbol = ltrim(rtrim(im.UnitSymbol))
--go

--insert into Products (Article, ProductName, ProductTypeId, MinimumPartnerPrice)
--select ltrim(rtrim(ip.Article)),
--ltrim(rtrim(ip.ProductName)),
--pt.ProductTypeId,
--try_convert(decimal(18,2), try_convert(float, replace(ip.MinimumPartnerPriceText, ',', '.')))
--from ImportProduct ip
--join ProductTypes pt on pt.TypeName = ltrim(rtrim(ip.ProductTypeName))
--go

--insert into ProductMaterials (ProductId, MaterialId, QuantityPerUnit)
--select p.ProductId,
--m.MaterialId,
--try_convert(decimal(18,5), try_convert(float, replace(imp.QuantityPerUnitText, ',', '.')))
--from ImportMaterialProducts imp
--join Materials m on m.MaterialName = ltrim(rtrim(imp.MaterialName))
--join Products p on p.ProductName = ltrim(rtrim(imp.ProductName))
--go

drop table ImportMaterialType
drop table ImportProductType
drop table ImportMaterials
drop table ImportProduct
drop table ImportMaterialProducts
go