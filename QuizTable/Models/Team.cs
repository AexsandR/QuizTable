using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuizTable.Models
{
    public class Team: INotifyPropertyChanged
    {

        string[] _points;
        int[] pointsInt;
        string _name = "";
        int addPoints = 0;
        double pointTour = 0;
        double speedAddPoint;
        double speedMove = 0;
        Brush _color = Brushes.Transparent;
        Thickness _pos = new Thickness(10,0,10,0);
        int _indexTabel = 0;
        Brush color1 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86CAFA"));
        Brush color2 = Brushes.Transparent;
        int _tour = 0;
        int _nextPosIndex;
        public Team(int countTour)
        {
            _nextPosIndex = _indexTabel;
            _points = new string[countTour];
            _points[0] = "0";
            pointsInt = new int[countTour];
        }
        public Brush Color 
        {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged("Color");
            }
        }
        public Brush ColorFont { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1088F1"));
        public Brush Color1 { get; set; }
        public Brush Color2 { get; set; }

        public bool Swap { get; set; }
        public int NextPos 
        {
            get;
            set;
        }
        public int IndexTabel
        {
            get => _indexTabel; set { _indexTabel = value; OnPropertyChanged(nameof(IndexTabel)); }
        }
        public int Height { get; set; } 
        public int Tour
        {
            get { return _tour; }
            set
            {
                if(value < Points.Length && value >= 0)
                {
                    _tour = value;
                    if (Points[value] is null)
                        Points[value] = "0";
                    SetPoints();
                    SetPos();
                    OnPropertyChanged(nameof(Points));
                    OnPropertyChanged(nameof(SumPoints));
                }
            }
        }
        public string Name { get { return _name; } set { _name = value; } }
        public string[] Points
        {
            get { return _points; }
            set { _points = value; OnPropertyChanged(nameof(Points)); }
        }
        public int SumPoints {
            get
            {
                int sum = 0;
                for (int i = 0; i < pointsInt.Length; i++)
                {
                    sum += pointsInt[i];
                }
                return sum;
            }
        }
        public Thickness Pos 
        { 
            get=>_pos; 
            set
            {
                _pos = value;
                OnPropertyChanged("Pos");
            }
        } 

        public void AddPoint(int point)
        {
            if (Tour < 0 || Tour >= _points.Length)
                return;
            addPoints += point;
            pointsInt[Tour] += point;
            if (addPoints == 0)
                Swap = false;
            else
                Swap = true;

        }

        private void SetPoints()
        {
            for(int i = 0; i < pointsInt.Length; i++)
            {
                if (Points[i] is null)
                    break;
                Points[i] = pointsInt[i].ToString();
            }
        }
        public void StartAnimationPointsAdd(int tick)
        {
            pointTour = Convert.ToInt32(Points[Tour]);
            speedAddPoint = (double)addPoints / tick;
        }
        public void AnimationPointsAdd()
        {
            OnPropertyChanged(nameof(Points));
            pointTour += speedAddPoint;

            if (pointsInt[Tour] <= (int)Math.Round((decimal)pointTour))
            {
                OnPropertyChanged(nameof(SumPoints));
                addPoints = 0;
                speedAddPoint = 0;
                pointTour = pointsInt[Tour];

            }
            Points[Tour] = ((int)Math.Round((decimal)pointTour)).ToString();
        }
        public void StartAnimationMove(double speed)
        {
            speedMove = speed;

            if(IndexTabel > NextPos)
            {
                speedMove *= -1;
            }
        }
        public void AnimationMove()
        {
            Pos = new Thickness(Pos.Left, Pos.Top + speedMove, Pos.Right, Pos.Bottom);
            if (NextPos * Height >= Math.Round(Pos.Top) && speedMove < 0 || NextPos * Height <= Math.Round(Pos.Top) && speedMove > 0) {
                SetPos();
                UpdateColor();
            }
        }
        public void SetPos()
        {
            Pos = new Thickness(Pos.Left, NextPos * Height, Pos.Right, Pos.Bottom);
            IndexTabel = NextPos;
            UpdateColor();
        }

        public void UpdateColor()
        {
            if (IndexTabel % 2 == 1)
            {
                Color1 = color1;
                Color2 = color2;
            }
            else
            {
                Color2 = color1;
                Color1 = color2;
            }
            OnPropertyChanged("Color1");
            OnPropertyChanged("Color2");

        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
