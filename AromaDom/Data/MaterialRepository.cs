using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using AromaDom.Models;

namespace AromaDom.Data
{
    public class MaterialRepository
    {
        public const string ConnectionString =
            "Server=DESKTOP-456A60L\\MSSQLSERVER01;Database=AromaDom;Trusted_Connection=True;TrustServerCertificate=True;";

        public List<Material> GetMaterials()
        {
            var materials = new List<Material>();
            const string sql =
                "select MaterialId, MaterialType, MaterialName, UnitSymbol, UnitPrice, " +
                "StockQuantity, MinimumQuantity, PackageQuantity, TotalRecipeQuantity " +
                "from vMaterialCatalog order by MaterialName";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        materials.Add(new Material
                        {
                            MaterialId = reader.GetInt32(0),
                            MaterialType = reader.GetString(1),
                            MaterialName = reader.GetString(2),
                            UnitSymbol = reader.GetString(3),
                            UnitPrice = reader.GetDecimal(4),
                            StockQuantity = reader.GetDecimal(5),
                            MinimumQuantity = reader.GetDecimal(6),
                            PackageQuantity = reader.GetDecimal(7),
                            RequiredQuantity = Math.Round(reader.GetDecimal(8), 2, MidpointRounding.AwayFromZero)
                        });
                    }
                }
            }

            return materials;
        }

        public List<MaterialProduct> GetProductsByMaterial(int materialId)
        {
            var products = new List<MaterialProduct>();
            const string sql =
                "select ProductId, Article, ProductName, QuantityPerUnit, UnitSymbol " +
                "from vMaterialProducts where MaterialId = @id order by ProductName";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", materialId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new MaterialProduct
                        {
                            ProductId = reader.GetInt32(0),
                            Article = reader.GetString(1),
                            ProductName = reader.GetString(2),
                            QuantityPerUnit = reader.GetDecimal(3),
                            UnitSymbol = reader.GetString(4)
                        });
                    }
                }
            }

            return products;
        }

        public List<LookupItem> GetMaterialTypes()
        {
            return GetLookupItems("select MaterialTypeId, TypeName from MaterialTypes order by TypeName");
        }

        public List<LookupItem> GetUnits()
        {
            return GetLookupItems("select UnitId, Symbol from Units order by Symbol");
        }

        private List<LookupItem> GetLookupItems(string sql)
        {
            var items = new List<LookupItem>();

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new LookupItem
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            return items;
        }

        public Material GetMaterialById(int materialId)
        {
            const string sql =
                "select MaterialId, MaterialTypeId, MaterialName, UnitId, UnitPrice, " +
                "StockQuantity, MinimumQuantity, PackageQuantity " +
                "from Materials where MaterialId = @id";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", materialId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new Material
                    {
                        MaterialId = reader.GetInt32(0),
                        MaterialTypeId = reader.GetInt32(1),
                        MaterialName = reader.GetString(2),
                        UnitId = reader.GetInt32(3),
                        UnitPrice = reader.GetDecimal(4),
                        StockQuantity = reader.GetDecimal(5),
                        MinimumQuantity = reader.GetDecimal(6),
                        PackageQuantity = reader.GetDecimal(7)
                    };
                }
            }
        }

        public bool IsNameTaken(string materialName, int excludeMaterialId)
        {
            const string sql =
                "select count(*) from Materials " +
                "where MaterialName = @name and MaterialId <> @id";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@name", materialName);
                command.Parameters.AddWithValue("@id", excludeMaterialId);
                connection.Open();
                return (int)command.ExecuteScalar() > 0;
            }
        }

        public void AddMaterial(Material material)
        {
            const string sql =
                "insert into Materials " +
                "(MaterialName, MaterialTypeId, UnitId, UnitPrice, StockQuantity, MinimumQuantity, PackageQuantity) " +
                "values (@name, @typeId, @unitId, @price, @stock, @minimum, @package)";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                AddMaterialParameters(command, material);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateMaterial(Material material)
        {
            const string sql =
                "update Materials set MaterialName = @name, MaterialTypeId = @typeId, UnitId = @unitId, " +
                "UnitPrice = @price, StockQuantity = @stock, MinimumQuantity = @minimum, " +
                "PackageQuantity = @package where MaterialId = @id";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                AddMaterialParameters(command, material);
                command.Parameters.AddWithValue("@id", material.MaterialId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void AddMaterialParameters(SqlCommand command, Material material)
        {
            command.Parameters.AddWithValue("@name", material.MaterialName);
            command.Parameters.AddWithValue("@typeId", material.MaterialTypeId);
            command.Parameters.AddWithValue("@unitId", material.UnitId);
            command.Parameters.AddWithValue("@price", material.UnitPrice);
            command.Parameters.AddWithValue("@stock", material.StockQuantity);
            command.Parameters.AddWithValue("@minimum", material.MinimumQuantity);
            command.Parameters.AddWithValue("@package", material.PackageQuantity);
        }
    }
}
