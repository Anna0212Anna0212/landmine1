using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace landmine1
{
    public partial class Form1 : Form
    {
        Button[,] btn = new Button[10, 10];

        public Form1()
        {
            InitializeComponent();

            CreateMine();        // 產生地雷
            CalculateNumbers(); // 計算周圍數字
            CreateMap();         // 建立按鈕
        }

        private void CalculateNumbers()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    if (mine[row, col])
                    {
                        number[row, col] = -1;
                        continue;
                    }

                    int count = 0;

                    for (int dr = -1; dr <= 1; dr++)
                    {
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            int nr = row + dr;
                            int nc = col + dc;

                            if (nr >= 0 && nr < 10 &&
                                nc >= 0 && nc < 10)
                            {
                                if (mine[nr, nc])
                                    count++;
                            }
                        }
                    }

                    number[row, col] = count;
                }
            }
        }

        // 是否有地雷
        bool[,] mine = new bool[10, 10];

        // 周圍地雷數
        int[,] number = new int[10, 10];

        // 是否已翻開
        bool[,] opened = new bool[10, 10];

        private void CreateMine()
        {
            Random rnd = new Random();

            int count = 0;

            while (count < 15)
            {
                int r = rnd.Next(10);
                int c = rnd.Next(10);

                if (!mine[r, c])
                {
                    mine[r, c] = true;
                    count++;
                }
            }
        }

        private void OpenCell(int row, int col)
        {
            if (row < 0 || row >= 10 ||
                col < 0 || col >= 10)
                return;

            if (opened[row, col])
                return;

            if (mine[row, col])
                return;

            opened[row, col] = true;

            Button b = btn[row, col];

            b.Enabled = false;

            if (number[row, col] > 0)
            {
                b.Text = number[row, col].ToString();
                return;
            }

            b.Text = "";

            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    OpenCell(row + dr, col + dc);
                }
            }
        }

        private void ShowAllMines()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    if (mine[row, col])
                    {
                        btn[row, col].Text = "💣";
                        btn[row, col].BackColor = Color.Red;
                    }
                }
            }
        }

        private void CreateMap()
        {
            int size = 50;
            int startX = 0;
            int startY = 80;

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    btn[row, col] = new Button();

                    btn[row, col].Width = size;
                    btn[row, col].Height = size;

                    btn[row, col].Location =
                        new Point(startX + col * size,
                                  startY + row * size);

                    btn[row, col].Tag = $"{row},{col}";

                    btn[row, col].Click += button1_Click;

                    this.Controls.Add(btn[row, col]);
                }
            }
        }

        private bool CheckWin()
        {
            int count = 0;

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    if (opened[row, col])
                        count++;
                }
            }

            return count == 85; //100-15顆地雷
        }

        bool gameOver = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (gameOver)
                return;

            Button b = (Button)sender;

            string[] pos = b.Tag.ToString().Split(',');

            int row = int.Parse(pos[0]);
            int col = int.Parse(pos[1]);

            if (opened[row, col])
                return;

            if (mine[row, col])
            {
                b.Text = "💣";
                b.BackColor = Color.Red;

                ShowAllMines();

                MessageBox.Show("遊戲結束");
                gameOver = true;

                return;
            }

            OpenCell(row, col);

            if (CheckWin())
            {
                MessageBox.Show("恭喜過關！");
            }
        }
    }
}
