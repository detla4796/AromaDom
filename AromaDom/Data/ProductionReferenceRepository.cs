using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AromaDom.Data
{
    public class ProductionReferenceRepository : IProductionReference
    {
        public double? GetProductTypeCoefficient(int productTypeId)
        {
            return GetNumber("select TypeCoefficient from ProductTypes where ProductTypeId = @id", productTypeId);
        }

        public double? GetMaterialTypeLossPercent(int materialTypeId)
        {
            return GetNumber("select LossPercent from MaterialTypes where MaterialTypeId = @id", materialTypeId);
        }

        private double? GetNumber(string sql, int id)
        {
            using (var connection = new SqlConnection(MaterialRepository.ConnectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return null;
                }

                return Convert.ToDouble(result);
            }
        }
    }
}
