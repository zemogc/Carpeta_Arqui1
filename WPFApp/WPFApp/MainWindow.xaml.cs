using System.Windows;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messagers.Content = $"Listo. Hilo de UI: #{Environment.CurrentManagedThreadId}";
        }

        // 1. Se crea un delegado Action que apunta a un método con nombre.
        private void ActionDelegate_Click(object sender, RoutedEventArgs e)
        {
            Action Code = new Action(ShowMessage);
            Task T = new Task(Code);
            T.Start();
        }

        // 2. Método anónimo: el código va en línea con la palabra clave delegate.
        private void AnonymousMethod_Click(object sender, RoutedEventArgs e)
        {
            Task T2 = new Task(delegate
            {
                WriteLog("Ejecutando una tarea en un método anónimo.");
            });
            T2.Start();
        }

        // 3. Expresión lambda: la forma más corta de escribir el mismo delegado.
        private void Lambda_Click(object sender, RoutedEventArgs e)
        {
            Task T3 = new Task(() => WriteLog("Ejecutando una tarea con una expresión lambda."));
            T3.Start();
        }

        // 4. Task.Run + await: el trabajo pesado sale del hilo de UI y la ventana sigue respondiendo.
        private async void AsyncAwait_Click(object sender, RoutedEventArgs e)
        {
            AsyncButton.IsEnabled = false;
            WriteLog("Calculando números primos hasta 2.000.000...");

            int Primes = await Task.Run(() => CountPrimes(2_000_000));

            WriteLog($"Se encontraron {Primes:N0} primos. La UI nunca se congeló.");
            AsyncButton.IsEnabled = true;
        }

        private void Clear_Click(object sender, RoutedEventArgs e) => Log.Items.Clear();

        void ShowMessage()
        {
            WriteLog("Ejecutando el método ShowMessage");
        }

        // Las tareas corren en el ThreadPool: para tocar la UI hay que volver al hilo del Dispatcher.
        void WriteLog(string message)
        {
            int ThreadId = Environment.CurrentManagedThreadId;
            Dispatcher.Invoke(() =>
            {
                Log.Items.Add($"{DateTime.Now:HH:mm:ss}  [hilo #{ThreadId,2}]  {message}");
                Log.ScrollIntoView(Log.Items[^1]);
                Messagers.Content = $"Última tarea en el hilo #{ThreadId}";
            });
        }

        static int CountPrimes(int limit)
        {
            int Count = 0;
            for (int n = 2; n <= limit; n++)
            {
                bool IsPrime = true;
                for (int d = 2; d * d <= n; d++)
                {
                    if (n % d == 0) { IsPrime = false; break; }
                }
                if (IsPrime) Count++;
            }
            return Count;
        }
    }
}
