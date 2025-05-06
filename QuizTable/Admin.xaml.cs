using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuizTable.Models;
using QuizTable.ViewModels;
using QuizTable.Views;

namespace QuizTable
{

    public partial class Admin : Window
    {
        private Window _table = new MainWindow();
        private MainWindowViewModel _viewModel;
        private const string SaveFileName = "quiz_table.csv";

        public Admin()
        {
            InitializeComponent();
            _table = new MainWindow();
            _viewModel = (MainWindowViewModel)_table.DataContext;
            _table.Show();

            InitializeRounds();
            InitializeTeams();
        }

        private void InitializeRounds()
        {
            UpdateRoundComboBox();
        }

        private void InitializeTeams()
        {
            TeamComboBox.ItemsSource = _viewModel.Teams;
        }

        private void UpdateRoundComboBox()
        {
            RoundComboBox.Items.Clear();
            for (int i = 1; i <= MainWindowViewModel.COUNT_TOUR; i++)
            {
                RoundComboBox.Items.Add(i.ToString());
            }
            if (RoundComboBox.Items.Count > 0)
            {
                RoundComboBox.SelectedIndex = 0;
            }
        }

        private void UpdateTable()
        {
            TeamRoundsDataGrid.Items.Clear();
            foreach (var team in _viewModel.Teams)
            {
                TeamRoundsDataGrid.Items.Add(team);
            }
        }
        #region Управление командами
        private void AddTeam_Click(object sender, RoutedEventArgs e)
        {
            string teamName = Microsoft.VisualBasic.Interaction.InputBox("Введите название команды:", "Добавление команды");
            if (!string.IsNullOrWhiteSpace(teamName))
            {
                _viewModel.AddTeam(teamName);
                var obj = _viewModel.Teams[^1];
                TeamRoundsDataGrid.Items.Add(obj);
                UpdateTable();
                TeamComboBox.Items.Refresh();
            }
        }

        private void RemoveTeam_Click(object sender, RoutedEventArgs e)
        {
            if (TeamComboBox.SelectedItem is Team selectedTeam)
            {
                _viewModel.RemoveTeam(selectedTeam.Name);
                TeamComboBox.Items.Refresh();
                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите команду для удаления");
            }
        }
        #endregion

        #region Управление баллами
        private void AddPoints_Click(object sender, RoutedEventArgs e)
        {

            if (TeamComboBox.SelectedItem is Team selectedTeam &&
                int.TryParse(PointsTextBox.Text, out int points))
            {
                _viewModel.AddPointTeam(selectedTeam.Name, points);
                UpdateTable();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно");
            }
        }

        private void GenerationPoints_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GenerationPoints();
            _viewModel.Update();
        }
        #endregion

        #region Дополнительно
        private void RefreshTable_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Update();
        }

        private void SaveTable_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Save(SaveFileName);
            MessageBox.Show("Таблица сохранена в файл " + SaveFileName);
        }


        #endregion
        private void RoundComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SetTour(int.Parse(RoundComboBox.SelectedItem.ToString()));
            UpdateTable();
        }

        private void LoadTable_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Load(SaveFileName);
            if (_viewModel.Teams.Count > 0)
                RoundComboBox.SelectedIndex = _viewModel.Teams[0].Tour;
            UpdateTable();
        }
    }
}
