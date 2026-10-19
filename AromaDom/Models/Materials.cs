using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AromaDom.Models
{
    public class Material
    {
        private static readonly CultureInfo RuCulture = new CultureInfo("ru-RU");

        public int MaterialId { get; set; }
        public int MaterialTypeId { get; set; }
        public int UnitId { get; set; }
        public string MaterialType { get; set; }
        public string MaterialName { get; set; }
        public string UnitSymbol { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal MinimumQuantity { get; set; }
        public decimal PackageQuantity { get; set; }
        public decimal RequiredQuantity { get; set; }

        public string MinimumText
        {
            get { return "Минимальное количество: " + Format(MinimumQuantity) + " " + UnitSymbol; }
        }

        public string StockText
        {
            get { return "Количество на складе: " + Format(StockQuantity) + " " + UnitSymbol; }
        }

        public string PriceText
        {
            get { return "Цена: " + UnitPrice.ToString("N2", RuCulture) + " р"; }
        }

        public string RequiredText
        {
            get { return "Требуемое количество: " + Format(RequiredQuantity) + " " + UnitSymbol; }
        }

        private static string Format(decimal value)
        {
            return value.ToString("N2", RuCulture);
        }
    }
}
