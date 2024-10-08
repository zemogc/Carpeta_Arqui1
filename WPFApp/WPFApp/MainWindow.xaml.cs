using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        void CreateTask()
        {
            Task T;
            Action Code = new Action(ShowMesage);
            T = new Task(Code);
            Task T2 = new Task(Code);
            {
                MessageBox.Show("Ejecutando una tarea en un método anónimo.");
            }
        });

        private void ShowMesage()
        {
            throw new NotImplementedException();
        }

        Task T3 = new Task(
            delegate
            {
                ShowMesage();
            });

        }
        void ShowMessage()
        {
            MessageBox.Show("Ejecutando el método ShowMessage");
        }
    }
}

//min37:40
