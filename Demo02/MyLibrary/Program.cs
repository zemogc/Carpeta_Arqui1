using MyLibrary;


Employee Maria = new Secretary();
Maria.Name = "Maria";

Employee Pedro = new Manager();
Pedro.Name = "Pedro";

Developer Miguel = new Developer();


Console.WriteLine($"Días: {Miguel.GetPaymentDays()}");

WriteEmployeeInfo(Maria);
WriteEmployeeInfo(Pedro);
WriteEmployeeInfo(Miguel);

void WriteEmployeeInfo(Employee employee)
{
    Console.WriteLine($"Nombre: {employee.Name}, Pago Mensual: {employee.CalculateMonthlyPayment()}, Días de Pago: {employee.GetPaymentDays()}");
}