namespace MyLibrary;

// Clase abstracta: define el contrato y deja que cada tipo de empleado decida cómo se le paga.
internal abstract class Employee
{
    public string Name { get; set; } = string.Empty;

    public abstract decimal CalculateMonthlyPayment();

    // Método virtual: por defecto se paga una vez al mes.
    public virtual int GetPaymentDays() => 30;
}

internal class Secretary : Employee
{
    public override decimal CalculateMonthlyPayment() => 2_500_000m;
}

internal class Manager : Employee
{
    public decimal Bonus { get; set; } = 1_000_000m;

    public override decimal CalculateMonthlyPayment() => 6_000_000m + Bonus;
}

internal class Developer : Employee
{
    public override decimal CalculateMonthlyPayment() => 5_000_000m;

    // Los desarrolladores cobran quincenal.
    public override int GetPaymentDays() => 15;
}
