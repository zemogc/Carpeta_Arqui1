namespace MyLibrary;

public class Product(int id, string name, decimal UnitPrice, int UnitsInStock)
{
    //propiedades
    public int Id => id;
    public string Name => name;
    public decimal UnitPrice => UnitPrice;
    public int UnitsInStock => UnitsInStock;

    public bool Discontinued {get; private set; }
        Discontinued = true;

    public bool GetStatus() => Discontinued;


}


