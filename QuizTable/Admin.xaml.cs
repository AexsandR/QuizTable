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
            _viewModel = new MainWindowViewModel();
            _table = new MainWindow { DataContext = _viewModel };
            _table.Show();

            InitializeRounds();
            InitializeTeams();
            SaveToCsv();
        }

        private void InitializeRounds()
        {
            UpdateRoundComboBox();
        }

        private void InitializeTeams()
        {
            TeamComboBox.ItemsSource = _viewModel.Teams;
            TeamComboBox.DisplayMemberPath = "Name";
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


        #region Управление командами
        private void AddTeam_Click(object sender, RoutedEventArgs e)
        {
            string teamName = Microsoft.VisualBasic.Interaction.InputBox("Введите название команды:", "Добавление команды");
            if (!string.IsNullOrWhiteSpace(teamName))
            {
                _viewModel.AddTeam(teamName);
                TeamComboBox.Items.Refresh();
                SaveToCsv();
            }
        }

        private void RemoveTeam_Click(object sender, RoutedEventArgs e)
        {
            if (TeamComboBox.SelectedItem is Team selectedTeam)
            {
                _viewModel.RemoveTeam(selectedTeam.Name);
                TeamComboBox.Items.Refresh();
                SaveToCsv();
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
                _viewModel.Update();
                SaveToCsv();
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
            SaveToCsv();
        }
        #endregion

        #region Дополнительно
        private void RefreshTable_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Update();
        }

        private void SaveTable_Click(object sender, RoutedEventArgs e)
        {
            SaveToCsv();
            MessageBox.Show("Таблица сохранена в файл " + SaveFileName);
        }

        private void LoadTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists(SaveFileName))
                {
                    MessageBox.Show("Файл сохранения не найден");
                    return;
                }

                var lines = File.ReadAllLines(SaveFileName);
                if (lines.Length == 0) return;

                var header = lines[0].Split(',');
                UpdateRoundComboBox();

                _viewModel.Teams.Clear();

                for (int i = 1; i < lines.Length; i++)
                {
                    var parts = lines[i].Split(',');
                    if (parts.Length < 1) continue;

                    string teamName = parts[0];
                    _viewModel.AddTeam(teamName);

                    var team = _viewModel.Teams.FirstOrDefault(t => t.Name == teamName);
                    if (team != null)
                    {
                        for (int j = 1; j < parts.Length && j <= team.Points.Length; j++)
                        {
                            if (int.TryParse(parts[j], out int p))
                                team.Points[j - 1] = p.ToString();
                        }
                    }
                }

                TeamComboBox.Items.Refresh();
                _viewModel.Update();
                MessageBox.Show("Таблица успешно загружена из файла " + SaveFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}");
            }
        }

        private void SaveToCsv()
        {
            try
            {
                using (var writer = new StreamWriter(SaveFileName))
                {
                    writer.Write("Команда");
                    for (int i = 1; i <= MainWindowViewModel.COUNT_TOUR; i++)
                        writer.Write($",Тур {i}");
                    writer.WriteLine();

                    foreach (var team in _viewModel.Teams)
                    {
                        writer.Write(team.Name);
                        for (int i = 0; i < MainWindowViewModel.COUNT_TOUR; i++)
                            writer.Write($",{team.Points[i]}");
                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
            }
        }
        #endregion
    }
}
