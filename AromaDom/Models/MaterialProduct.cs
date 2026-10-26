using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace AromaDom.Models
{
    public class MaterialProduct
    {
        private static readonly CultureInfo RuCulture = new CultureInfo("ru-RU");

        public int ProductId { get; set; }
        public string Article { get; set; }
        public string ProductName { get; set; }
        public decimal QuantityPerUnit { get; set; }
        public string UnitSymbol { get; set; }

        public string ArticleText
        {
            get { return "Артикул: " + Article; }
        }

        public string QuantityText
        {
            get
            {
                return "Необходимое количество: " + QuantityPerUnit.ToString("0.#####", RuCulture) + " " + UnitSymbol;
            }
        }
    }
}
