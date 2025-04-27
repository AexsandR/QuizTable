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
using System.Windows.Shapes;
using QuizTable.Views;
using QuizTable.ViewModels;

namespace QuizTable
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        Window table = new MainWindow();
        MainWindowViewModel viewModel;
        public Admin()
        {
            InitializeComponent();
            table.Show();
            viewModel = (MainWindowViewModel)table.DataContext;
            
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            viewModel.Update();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.NextTour();
        }
        private void generationPoints(object sender, RoutedEventArgs e)
        {
            viewModel.GenerationPoints();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            viewModel.PrevTour();
        }
    }
}
