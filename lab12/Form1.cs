using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab12
{
    public partial class Form1 : Form
    {
        private class TeamInfo
        {
            public TeamInfo(String name, double lambda)
            {
                this.name = name;
                this.lambda = lambda;
            }
            public String name;
            public int victories;
            public int draws;
            public int losses;
            public int scored;
            public int missed;
            public int points;
            public double lambda;
        }
        private TeamInfo[] teams = new TeamInfo[8];
        private TeamInfo[] stat = new TeamInfo[8];

        // Отображает кто с кем играл за все время
        private bool[,] plays = new bool[8, 8];
        private int tourNumber = 0;
        public Form1()
        {
            InitializeComponent();            
            SetDefaultState();          
        }    
        
        public void SetDefaultState()
        {
            cleanButton.Enabled = false;
            nextTourButton.Enabled = true;
            teams[0] = new TeamInfo("ЗЕНИТ", 2);
            teams[1] = new TeamInfo("КРАСНОДАР", 2);
            teams[2] = new TeamInfo("ПФК ЦСКА", 1.5);
            teams[3] = new TeamInfo("ДИНАМО-МОСКВА", 1.7);
            teams[4] = new TeamInfo("РУБИН", 1.8);
            teams[5] = new TeamInfo("СПАРТАК-МОСКВА", 1.9);
            teams[6] = new TeamInfo("ЛОКОМОТИВ", 1.6);
            teams[7] = new TeamInfo("ТЕРЕК", 1.7);            
            tourNumber = 0;
            tourLabel.Text = tourNumber.ToString() + " / 7";
            FillDataGridView(teams);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i <= j)
                    {
                        plays[i, j] = true;
                    }
                    else
                    {
                        plays[i, j] = false;
                    }
                }
            }
        }

        private void FillDataGridView(TeamInfo[] mas)
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < 8; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1[0, i].Value = mas[i].name;
                dataGridView1[1, i].Value = tourNumber;                
                dataGridView1[2, i].Value = mas[i].victories;
                dataGridView1[3, i].Value = mas[i].draws;
                dataGridView1[4, i].Value = mas[i].losses;
                dataGridView1[5, i].Value = mas[i].scored;
                dataGridView1[6, i].Value = mas[i].missed;
                dataGridView1[7, i].Value = mas[i].points;
            }
        }

        private void nextTourButton_Click(object sender, EventArgs e)
        {
            tourNumber++;
            PlayTour();
            teams.CopyTo(stat, 0);
            QuickSortingTeams(0, stat.Length - 1);
            FillDataGridView(stat);
            tourLabel.Text = tourNumber.ToString() + " / 7";
            if (tourNumber == 7)
            {
                cleanButton.Enabled = true;
                nextTourButton.Enabled = false;
            }
        }

        private void PlayTour()
        {
            int playsNumber = 0;
            // Отображает какая команда уже играла в текущем туре
            bool[] currentPlays = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!plays[i, j] && !currentPlays[i] && !currentPlays[j])
                    {
                        plays[i, j] = true;
                        currentPlays[i] = true;
                        currentPlays[j] = true;
                        // Забитые i-ой командой мячи 
                        int firstScored = PoissonDistribution(teams[i]);
                        teams[i].scored += firstScored;
                        teams[j].missed += firstScored;
                        // Забитые j-ой командой мячи
                        int secondScored = PoissonDistribution(teams[j]);
                        teams[j].scored += secondScored;
                        teams[i].missed += secondScored;

                        if (firstScored > secondScored)
                        {
                            teams[i].victories++;
                            teams[j].losses++;
                        }
                        else if (firstScored < secondScored)
                        {
                            teams[j].victories++;
                            teams[i].losses++;
                        }
                        else
                        {
                            teams[j].draws++;
                            teams[i].draws++;
                        }
                        // Подсчет итоговых очков
                        teams[i].points = teams[i].victories * 3 + teams[i].draws * 1;
                        teams[j].points = teams[j].victories * 3 + teams[j].draws * 1;
                        playsNumber++;
                        if (playsNumber == 4)
                        {
                            return;
                        }
                    }
                }
            }
        }

        private int PoissonDistribution(TeamInfo firstTeam)
        {
            int m = 0;
            double S = 0;
            
            while (S >= (-firstTeam.lambda))
            {
                m++;
                Generator();
                double alpha = R;
                S += Math.Log(alpha);
            }

            return m;
        }

        // Быстрая сортировка по итоговым очкам
        private void QuickSortingTeams(int first, int last)
        {
            TeamInfo pivot = stat[(last - first) / 2 + first];
            TeamInfo temp;
            int i = first, j = last;
            while (i <= j)
            {
                while (stat[i].points > pivot.points && i <= last)
                {
                    i++;
                }
                while (stat[j].points < pivot.points && j >= first)
                {
                    j--;
                }
                if (i <= j)
                {
                    temp = stat[i];
                    stat[i] = stat[j];
                    stat[j] = temp;
                    i++;
                    j--;
                }
            }
            if (j > first)
            {
                QuickSortingTeams(first, j);
            }
            if (i < last)
            {
                QuickSortingTeams(i, last);
            }
        }

        double U = 1;
        const int p = 5087; // большое простое число
        int M = 2900;       // M = p - 3^n. Берем n = 7, потому что 3^8 > p
        double R;
        private void Generator() // метод вычетов. Модификация Коробова
        {
            R = U / p;
            U = (U * M) % p;
        }

        private void cleanButton_Click(object sender, EventArgs e)
        {
            SetDefaultState();
        }
    }
}
