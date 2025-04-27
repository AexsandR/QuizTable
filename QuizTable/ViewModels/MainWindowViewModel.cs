using System.Collections.ObjectModel;
using System.Windows;
using QuizTable.Models;
using System.ComponentModel;
using System.Windows.Media;
namespace QuizTable.ViewModels
{
    public class MainWindowViewModel: BaseViewModel
    {
        int _height = 51;
        const int HEEGHT_STEP = 51;
        ObservableCollection<Team> _teams = new ObservableCollection<Team>();
        int ticksAddPoint = 10;
        int tickMoveLabelTeam = 60;
        const int COUNT_TOUR = 7;
        BackgroundWorker worker = new();

        public Brush ColorFont{ get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#146FE8"));
        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Team> Teams
        {
            get => _teams;
            set
            {
                _teams = value;
                OnPropertyChanged();
            }
        }
        public MainWindowViewModel() {
            worker.WorkerReportsProgress = true;
            worker.DoWork += Animation;
            worker.ProgressChanged += UpdateLabel;
        }
        #region удаление/добавленеи команд
        /// <summary>
        /// Добавляет новую команду
        /// </summary>
        /// <param name="teamName">Название команды</param>
        /// <returns></returns>
        public void AddTeam(string teamName)
        {
            var team = new Team(COUNT_TOUR);
            team.Name = teamName;
            team.Pos = new Thickness(0, Height, 0, 0);
            team.Height = HEEGHT_STEP;
            team.IndexTabel = Teams.Count + 1;
            Teams.Add(team);
            Height = (_teams.Count + 1) * HEEGHT_STEP;
            SortTeams();
        }
        /// <summary>
        /// Удаляет команду
        /// </summary>
        /// <param name="teamName">название команды</param>
        /// <returns></returns>
        public bool RemoveTeam(string teamName)
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                if (Teams[i].Name == teamName)
                {
                    Teams.Remove(Teams[i]);
                    Height -= HEEGHT_STEP;
                    UpdatePosRemove(i);
                    SortTeams();
                    return true;
                }
            }
            return false;
        }

        private void UpdatePosRemove(int startIndex)
        {
            for (int i = startIndex; i < Teams.Count; i++) 
            {
                Teams[i].Pos = new Thickness(10, Teams[i].Pos.Top - HEEGHT_STEP, 10, 0);
            }
        }
        #endregion
        #region переключение туров
        /// <summary>
        /// Переключает на следующий тур
        /// </summary>
        /// <returns>
        /// true - переключение выполнено успешно
        /// false - переключение выполнено неуспешно
        /// </returns>
        public bool NextTour()
        {
            SortingParticipants();
            if (Teams[0].Tour == Teams[0].Points.Length)
                return false;
            foreach (var team in Teams)
            {
                team.Tour++;
                OnPropertyChanged("Teams");
            }
            return true;
        }
        /// <summary>
        /// Переключает на предыдущий тур
        /// </summary>
        /// <returns>
        /// true - переключение выполнено успешно
        /// false - переключение выполнено неуспешно
        /// </returns>
        public bool PrevTour()
        {
            if (Teams[0].Tour == 0)
                return false;
            foreach (var team in Teams)
            {
                team.Tour--;
                OnPropertyChanged("Teams");
            }
            return true;
        }
        #endregion
        #region добавление/снятие очков
        /// <summary>
        /// Снимает очки у команды.
        /// Для данного метода нужно запустить анимацию для того чтобы очки добавились команде или переключить тур
        /// </summary>
        /// <param name="teamName">название команды</param>
        /// <param name="points">кол-во очков, которое вы хотите снять</param>
        public void RemovePointTeam(string teamName, int points)
        {
            AddPointTeam(teamName, -points);
        }

        /// <summary>
        /// добовляет очки у команды
        /// Для данного метода нужно запустить анимацию для того чтобы очки добавились команде или переключить тур
        /// </summary>
        /// <param name="teamName">название команды</param>
        /// <param name="points">кол-во очков, которое вы хотите снять</param>
        public void AddPointTeam(string teamName, int points)
        {
            foreach (var team in Teams)
            {
                if(team.Name == teamName)
                {
                    team.AddPoint(points);
                    return;
                }
            }
        }
        /// <summary>
        /// тестовый метод выдает очки каждой команде. Работает такжк как и AddPointTeam
        /// </summary>
        public void GenerationPoints()
        {
            var rdn = new Random();
            foreach (var team in Teams)
            {
                AddPointTeam(team.Name, rdn.Next(0, 99));
            }
        }
        #endregion
        #region сортировки команд
        private void SortTeams()
        {
            SortingParticipants();
            foreach (var team in Teams)
            {
                team.SetPos();
            }
        }
        private void SortingParticipants()
        {
            var newListTeam = Teams.OrderByDescending(team => team.SumPoints).ThenBy(team => team.Name).ToList();
            int lastIndex = Teams.Count - 1;
            foreach (var team in newListTeam)
            {
                team.NextPos = newListTeam.FindIndex(team1 => team1.Name == team.Name) + 1;
                if (team.Swap)
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Teams.Move(team.IndexTabel - 1, lastIndex--);
                    });
            }
        }
        #endregion
        #region запуск анимаций
        /// <summary>
        /// Обновление экрана
        /// </summary>
        public void Update()
        {
            if (worker.IsBusy)
                return;
            worker.RunWorkerAsync();

        }
        private void StartAnimationPointsAdd()
        {
            foreach (var team in Teams)
            {
                team.StartAnimationPointsAdd(ticksAddPoint);
            }
        }
        private void StartAnimationMove()
        {
            var maxS = Teams.Max(team => Math.Abs(team.NextPos - team.IndexTabel));
            foreach (var team in Teams)
            {
                team.StartAnimationMove((double)(maxS * HEEGHT_STEP) / tickMoveLabelTeam);
            }
        }
        #endregion
        #region анимации
        private void Animation(object? sender, DoWorkEventArgs e)
        {
            StartAnimationPointsAdd();
            var worker = sender as BackgroundWorker;
            int tick = 0;
            while (tick < ticksAddPoint)
            {
                Thread.Sleep(50);
                foreach (var team in Teams)
                    team.AnimationPointsAdd();
                worker.ReportProgress(0, null);
                tick++;
            }
            SortingParticipants();
            tick = 0;
            StartAnimationMove();
            while (tick < tickMoveLabelTeam)
            {
                foreach (var team in Teams)
                    team.AnimationMove();
                Thread.Sleep(16);
                worker.ReportProgress(0, null);
                tick++;
            }

        }
        private void UpdateLabel(object? sender, ProgressChangedEventArgs e)
        {
            OnPropertyChanged("Teams");

        }
        #endregion
    }

}