using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication6.Models
{
    public class Product
    {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public decimal Price { get; set; }
    }

    public class Parameter
    {
        public int Id { get; set; }
        public int TId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }
    }
}