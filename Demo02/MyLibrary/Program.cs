using System.Globalization;
using MyLibrary;

CultureInfo.CurrentCulture = new CultureInfo("es-CO");

// Polimorfismo: variables del tipo base que apuntan a distintos tipos derivados.
Employee Maria = new Secretary();
Maria.Name = "Maria";

Employee Pedro = new Manager();
Pedro.Name = "Pedro";

Developer Miguel = new Developer();
Miguel.Name = "Miguel";

Console.WriteLine($"Días: {Miguel.GetPaymentDays()}");

WriteEmployeeInfo(Maria);
WriteEmployeeInfo(Pedro);
WriteEmployeeInfo(Miguel);

Console.WriteLine();

Product Sugar = new Product(1, "Azúcar refinada", 4_500m, 120);
Book CleanCode = new Book(2, "Clean Code", 180_000m, 8, "Robert C. Martin");
CleanCode.Discontinue();

Console.WriteLine(Sugar);
Console.WriteLine(CleanCode);
Console.WriteLine($"Valor del inventario: {Sugar.GetInventoryValue() + CleanCode.GetInventoryValue():C}");

void WriteEmployeeInfo(Employee employee)
{
    Console.WriteLine($"Nombre: {employee.Name}, Pago Mensual: {employee.CalculateMonthlyPayment():C}, Días de Pago: {employee.GetPaymentDays()}");
}
