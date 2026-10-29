using System.Diagnostics;

namespace Arqui1.Hub.Services;

public sealed record LabJob(string Name, int DurationMs);

public sealed class JobProgress(LabJob job)
{
    public LabJob Job { get; } = job;
    public double StartedAtMs { get; set; } = -1;
    public double FinishedAtMs { get; set; } = -1;
    public bool Started => StartedAtMs >= 0;
    public bool Done => FinishedAtMs >= 0;
}

/// <summary>
/// Compara ejecutar trabajos de E/S uno tras otro contra lanzarlos todos y esperar con Task.WhenAll.
/// Quien llama crea la lista de <see cref="JobProgress"/> para poder dibujar el avance mientras corre.
/// </summary>
public static class ConcurrencyLab
{
    public static IReadOnlyList<LabJob> DefaultJobs { get; } =
    [
        new("Consultar inventario", 900),
        new("Leer precios del proveedor", 700),
        new("Calcular impuestos", 500),
        new("Enviar correo de confirmación", 800),
        new("Registrar en AppLogs.txt", 400),
    ];

    public static List<JobProgress> CreateProgress(IEnumerable<LabJob> jobs) =>
        jobs.Select(j => new JobProgress(j)).ToList();

    /// <returns>Milisegundos que tardó todo el lote.</returns>
    public static async Task<double> RunSequentialAsync(IReadOnlyList<JobProgress> jobs, CancellationToken cancellationToken = default)
    {
        var clock = Stopwatch.StartNew();
        foreach (var job in jobs)
            await RunJobAsync(job, clock, cancellationToken);
        return clock.Elapsed.TotalMilliseconds;
    }

    /// <returns>Milisegundos que tardó todo el lote.</returns>
    public static async Task<double> RunConcurrentAsync(IReadOnlyList<JobProgress> jobs, CancellationToken cancellationToken = default)
    {
        var clock = Stopwatch.StartNew();
        // Se inician todas las tareas y luego se espera a que terminen todas.
        await Task.WhenAll(jobs.Select(job => RunJobAsync(job, clock, cancellationToken)));
        return clock.Elapsed.TotalMilliseconds;
    }

    private static async Task RunJobAsync(JobProgress job, Stopwatch clock, CancellationToken cancellationToken)
    {
        job.StartedAtMs = clock.Elapsed.TotalMilliseconds;
        // Task.Delay simula una operación de E/S: no ocupa ningún hilo mientras espera.
        await Task.Delay(job.Job.DurationMs, cancellationToken);
        job.FinishedAtMs = clock.Elapsed.TotalMilliseconds;
    }
}
