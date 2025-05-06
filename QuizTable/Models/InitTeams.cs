using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizTable.Models
{
    public class InitTeams
    {
        public static List<Team> LoadTeams(string path)
        {
            List<Team> teams = new();
            if (!File.Exists(path))
                return teams;

            
            Team team;
            string[] data;
            string[] textFromFile = File.ReadAllLines(path);
            int countTour = textFromFile[0].Split("\t").Length - 2;

            for (int i = 1; i < textFromFile.Length; i++)
            {
                data = textFromFile[i].Split("\t");
                team = new(countTour);
                team.Name = data[0];
                for (int j = 1; j < data.Length - 1; j++)
                {
                    if (data[j] == "")
                        continue;
                    else if (j != 1)
                        team.Tour++;
                    team.AddPoint(int.Parse(data[j]));
                }
                teams.Add(team);
            }
            
            return teams;
        }
        public static bool Save(string path, List<Team> teams)
        {
            if (teams.Count == 0)
                return false;
            int countTour = teams[0].Points.Length;
            string data = CreateHeader(countTour) + CreateDataForFile(teams);
            try
            {
                File.WriteAllText(path, data, Encoding.Unicode);
                return true;
            }
            catch
            {
                return false;
            }


        }
        private static string CreateHeader(int countTour)
        {
            string header = "Команада\t";
            for (int i = 0; i < countTour; i++)
                header += (i + 1).ToString() + "\t";
            header += "Итог\n";
            return header;
        }
        private static string CreateDataForFile(List<Team> teams)
        {
            string res = "";
            foreach(var team in teams)
            {
                res += team.Name + "\t";
                foreach (var point in team.Points)
                    res += point + "\t";
                res += team.SumPoints.ToString()+ "\n";
            }
            return res;
        } 
    }
}
