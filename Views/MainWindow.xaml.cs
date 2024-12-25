using System.Windows;
using MatrixMath.ViewModels;

namespace MatrixMath.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Устанавливаем DataContext = MainViewModel, 
            // в котором лежат MatrixA, MatrixB, InverseA, InverseB и т.д.

            DataContext = new MainViewModel();
        }
    }
}
