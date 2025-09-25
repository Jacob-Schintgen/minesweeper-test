using System;
using System.Drawing;
using System.Windows.Forms;

// NOTE : This is a simple implementation of Minesweeper in C# using Windows Forms.

// TODO : Add features like timer, score tracking, difficulty levels, and better graphics.
// TODO : Look into using a more efficient data structure for the grid and bomb placements, and avoid bombs on first click.

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private const int GridSize = 15;
        private const int BombCount = 20;
        private const int CellSize = 30;
        private const int GridTopOffset = 50;

        private readonly byte[,] Positions = new byte[GridSize, GridSize];
        private readonly Button[,] ButtonList = new Button[GridSize, GridSize];
        private readonly Random rand = new Random();

        public Form1()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ClientSize = new Size(GridSize * CellSize, GridSize * CellSize + GridTopOffset);

            this.Load += Form1_Load;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            AddRestartButton();
            GenerateBombs();
            GeneratePositionValue();
            GenerateButtons();
        }

        private void AddRestartButton()
        {
            Button restartBtn = new Button
            {
                Text = "Restart",
                Width = 100,
                Height = 30,
                Left = (GridSize * CellSize - 100) / 2,
                Top = 10
            };
            restartBtn.Click += (s, e) => Application.Restart();
            this.Controls.Add(restartBtn);
        }

        private void GenerateBombs()
        {
            int bombsPlaced = 0;
            while (bombsPlaced < BombCount)
            {
                int x = rand.Next(GridSize);
                int y = rand.Next(GridSize);
                if (Positions[x, y] != 9)
                {
                    Positions[x, y] = 9;
                    bombsPlaced++;
                }
            }
        }

        private void GeneratePositionValue()
        {
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    if (Positions[x, y] == 9) continue;

                    int bombCount = 0;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            int newX = x + dx, newY = y + dy;
                            if (newX >= 0 && newX < GridSize && newY >= 0 && newY < GridSize)
                            {
                                if (Positions[newX, newY] == 9)
                                    bombCount++;
                            }
                        }
                    }
                    Positions[x, y] = (byte)bombCount;
                }
            }
        }

        private void GenerateButtons()
        {
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    Button btn = new Button
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Left = x * CellSize,
                        Top = y * CellSize + GridTopOffset,
                        Tag = new Point(x, y)
                    };
                    btn.MouseUp += Btn_MouseUp;
                    this.Controls.Add(btn);
                    ButtonList[x, y] = btn;
                }
            }
        }

        private void Btn_MouseUp(object? sender, MouseEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Point pos)
            {
                int x = pos.X, y = pos.Y;

                if (e.Button == MouseButtons.Right)
                {
                    // Toggle flag
                    if (btn.Text == "⚑")
                    {
                        btn.Text = "";
                    }
                    else if (btn.Enabled)
                    {
                        btn.Text = "⚑";
                    }
                }
                else if (e.Button == MouseButtons.Left)
                {
                    if (btn.Text == "⚑") return; // Don't reveal flagged cells

                    if (Positions[x, y] == 9)
                    {
                        btn.Text = "B";
                        MessageBox.Show("Just one more try!");
                        RevealAllBombs();
                    }
                    else
                    {
                        RevealCell(x, y);
                    }
                }
            }
        }

        private void RevealCell(int x, int y)
        {
            Button btn = ButtonList[x, y];
            if (!btn.Enabled || btn.Text == "⚑") return;

            btn.Enabled = false;
            btn.Text = Positions[x, y] == 0 ? "" : Positions[x, y].ToString();

            if (Positions[x, y] == 0)
            {
                RevealAdjacentZeros(x, y);
            }
        }

        private void RevealAllBombs()
        {
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    if (Positions[x, y] == 9)
                        ButtonList[x, y].Text = "B";

                    ButtonList[x, y].Enabled = false;
                }
            }
        }

        private void RevealAdjacentZeros(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int newX = x + dx, newY = y + dy;

                    if (newX >= 0 && newX < GridSize && newY >= 0 && newY < GridSize)
                    {
                        RevealCell(newX, newY);
                    }
                }
            }
        }
    }
}
