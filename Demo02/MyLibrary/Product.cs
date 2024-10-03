using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class Product(int id, string name, decimal UnitPrice, int UnitsInStock)
    {
    {
        //Field, Campo, variable

        //propiedades
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }

        //Constructor 
        public Product(int id, string name, decimal UnitPrice, int UnitsInStock) 
        {
            Id = id;
            Name = name;
            UnitPrice = UnitPrice;
            UnitsInStock = UnitsInStock;

        }
            
    }
}
