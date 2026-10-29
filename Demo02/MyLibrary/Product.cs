namespace MyLibrary;

// Constructor primario: los parámetros quedan disponibles en toda la clase.
public class Product(int id, string name, decimal unitPrice, int unitsInStock)
{
    //propiedades
    public int Id => id;
    public string Name => name;
    public decimal UnitPrice => unitPrice;
    public int UnitsInStock => unitsInStock;

    // Solo la propia clase puede cambiar el estado.
    public bool Discontinued { get; private set; }

    public void Discontinue() => Discontinued = true;

    public bool GetStatus() => Discontinued;

    public decimal GetInventoryValue() => UnitPrice * UnitsInStock;

    public override string ToString() =>
        $"[{Id}] {Name} - {UnitPrice:C} x {UnitsInStock} uds.{(Discontinued ? " (descontinuado)" : "")}";
}
