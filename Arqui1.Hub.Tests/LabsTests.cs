using System.Reflection;
using Arqui1.Hub.Data;
using Arqui1.Hub.Services;
using Microsoft.AspNetCore.Components;
using MyLibrary;

namespace Arqui1.Hub.Tests;

public class LabsTests
{
    private static readonly LabJob[] Jobs = [new("A", 300), new("B", 200), new("C", 250)];

    [Fact]
    public async Task Secuencial_tarda_la_suma_de_las_duraciones()
    {
        var progress = ConcurrencyLab.CreateProgress(Jobs);

        var elapsed = await ConcurrencyLab.RunSequentialAsync(progress);

        Assert.True(elapsed >= 750, $"Tardó {elapsed} ms");
        Assert.All(progress, p => Assert.True(p.Done));
        // Cada tarea empieza cuando terminó la anterior.
        Assert.True(progress[1].StartedAtMs >= progress[0].FinishedAtMs);
    }

    [Fact]
    public async Task Concurrente_tarda_lo_de_la_tarea_mas_lenta()
    {
        var progress = ConcurrencyLab.CreateProgress(Jobs);

        var elapsed = await ConcurrencyLab.RunConcurrentAsync(progress);

        Assert.InRange(elapsed, 300, 700);
        // Todas arrancan antes de que termine la más corta.
        Assert.All(progress, p => Assert.True(p.StartedAtMs < progress.Min(x => x.FinishedAtMs)));
    }

    [Fact]
    public void Cada_laboratorio_de_una_leccion_es_una_pagina_que_existe()
    {
        var routes = typeof(Program).Assembly.GetTypes()
            .SelectMany(t => t.GetCustomAttributes<RouteAttribute>())
            .Select(r => r.Template.TrimStart('/'))
            .ToHashSet();

        var labs = LessonCatalog.All.Where(l => l.LabUrl is not null).ToList();

        Assert.NotEmpty(labs);
        Assert.All(labs, l => Assert.Contains(l.LabUrl!, routes));
    }

    [Fact]
    public void La_nomina_de_Demo02_usa_el_override_de_cada_tipo()
    {
        List<Employee> payroll =
        [
            new Secretary { Name = "Maria" },
            new Manager { Name = "Pedro", Bonus = 500_000m },
            new Developer { Name = "Miguel" },
        ];

        Assert.Equal(2_500_000m + 6_500_000m + 5_000_000m, payroll.Sum(e => e.CalculateMonthlyPayment()));
        Assert.Equal([30, 30, 15], payroll.Select(e => e.GetPaymentDays()));
    }
}
