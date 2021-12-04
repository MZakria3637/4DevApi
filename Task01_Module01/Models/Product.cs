using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task01_Module01.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public int ProductSize { get; set; }

        public string ProductName { get; set; }
        public int ProductCategoryId { get; set; }
        public string PhotoFileName { get; set; }
    }
}
