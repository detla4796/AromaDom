using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AromaDom.Data
{
    public interface IProductionReference
    {
        double? GetProductTypeCoefficient(int productTypeId);
        double? GetMaterialTypeLossPercent(int materialTypeId);
    }

    public class ProductionCalculator
    {
        private const int InvalidResult = -1;
        private readonly IProductionReference reference;

        public ProductionCalculator(IProductionReference reference)
        {
            this.reference = reference;
        }

        public int CalculateProductCount(int productTypeId, int materialTypeId, int rawMaterialQuantity, double firstParameter, double secondParameter)
        {
            if (!AreArgumentsValid(productTypeId, materialTypeId, rawMaterialQuantity, firstParameter, secondParameter))
            {
                return InvalidResult;
            }

            try
            {
                double? coefficient = reference.GetProductTypeCoefficient(productTypeId);
                double? lossPercent = reference.GetMaterialTypeLossPercent(materialTypeId);

                if (!coefficient.HasValue || !lossPercent.HasValue)
                {
                    return InvalidResult;
                }

                if (coefficient.Value <= 0 || lossPercent.Value < 0 || lossPercent.Value >= 100)
                {
                    return InvalidResult;
                }

                decimal materialPerProduct = (decimal)firstParameter * (decimal)secondParameter * (decimal)coefficient.Value;

                if (materialPerProduct <= 0)
                {
                    return InvalidResult;
                }

                decimal usableMaterial = rawMaterialQuantity * (1m - (decimal)lossPercent.Value / 100m);
                decimal productCount = Math.Floor(usableMaterial / materialPerProduct);

                if (productCount > int.MaxValue)
                {
                    return InvalidResult;
                }

                return (int)productCount;
            }
            catch (Exception)
            {
                return InvalidResult;
            }
        }

        private static bool AreArgumentsValid(int productTypeId, int materialTypeId, int rawMaterialQuantity, double firstParameter, double secondParameter)
        {
            if (productTypeId <= 0 || materialTypeId <= 0 || rawMaterialQuantity < 0)
            {
                return false;
            }

            if (double.IsNaN(firstParameter) || double.IsInfinity(firstParameter) || firstParameter <= 0)
            {
                return false;
            }

            if (double.IsNaN(secondParameter) || double.IsInfinity(secondParameter) || secondParameter <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
