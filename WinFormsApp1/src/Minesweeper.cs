using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Minesweeper : Form
    {
        private int GridSize;
        private int BombCount;
        private const int CellSize = 30;
        private const int GridTopOffset = 80;

        private byte[,] Positions = null!;
        private Button[,] ButtonList = null!;
        private readonly Random rand = new Random();

        private ComboBox difficultySelector = null!;
        private Button restartBtn = null!;
        private Panel gridPanel = null!;

        private bool firstClick = true;

        public Minesweeper()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            SetupUI();
            InitializeGame();
        }

        private void SetupUI()
        {
            difficultySelector = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Left = 10,
                Top = 10,
                Width = 120
            };
            difficultySelector.Items.AddRange(new string[] { "Easy", "Medium", "Hard" });
            difficultySelector.SelectedIndex = 0;
            difficultySelector.SelectedIndexChanged += (s, e) => InitializeGame();
            this.Controls.Add(difficultySelector);

            restartBtn = new Button
            {
                Text = "Restart",
                Width = 100,
                Height = 30,
                Left = 150,
                Top = 10
            };
            restartBtn.Click += (s, e) => InitializeGame();
            this.Controls.Add(restartBtn);

            gridPanel = new Panel
            {
                Left = 0,
                Top = GridTopOffset,
                AutoScroll = true
            };
            this.Controls.Add(gridPanel);
        }

        private void InitializeGame()
        {
            gridPanel.Controls.Clear();
            firstClick = true;

            switch (difficultySelector.SelectedItem?.ToString())
            {
                case "Easy":
                    GridSize = 8;
                    BombCount = 10;
                    break;
                case "Medium":
                    GridSize = 15;
                    BombCount = 30;
                    break;
                case "Hard":
                    GridSize = 20;
                    BombCount = 60;
                    break;
                default:
                    GridSize = 10;
                    BombCount = 15;
                    break;
            }

            Positions = new byte[GridSize, GridSize];
            ButtonList = new Button[GridSize, GridSize];

            int totalWidth = GridSize * CellSize + 16;
            int totalHeight = GridSize * CellSize + GridTopOffset + 40;
            this.ClientSize = new Size(totalWidth, totalHeight);
            gridPanel.Size = new Size(GridSize * CellSize, GridSize * CellSize);

            GenerateButtons();
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
                        Top = y * CellSize,
                        Tag = new Point(x, y)
                    };
                    btn.MouseUp += Btn_MouseUp;
                    gridPanel.Controls.Add(btn);
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
                    if (btn.Text == "⚑") btn.Text = "";
                    else if (btn.Enabled) btn.Text = "⚑";
                }
                else if (e.Button == MouseButtons.Left)
                {
                    if (btn.Text == "⚑") return;

                    // 🟢 first click safety
                    if (firstClick)
                    {
                        firstClick = false;
                        GenerateBombsSafe(x, y);
                        GeneratePositionValues();
                    }

                    if (Positions[x, y] == 9)
                    {
                        btn.Text = "💣";
                        MessageBox.Show("💥 Game Over!");
                        RevealAllBombs();
                    }
                    else
                    {
                        RevealCell(x, y);
                        CheckWinCondition();
                    }
                }
            }
        }

        private void GenerateBombsSafe(int safeX, int safeY)
        {
            int bombsPlaced = 0;

            bool IsSafeZone(int x, int y)
            {
                // include clicked cell + surrounding 8 neighbors
                return Math.Abs(x - safeX) <= 1 && Math.Abs(y - safeY) <= 1;
            }

            while (bombsPlaced < BombCount)
            {
                int x = rand.Next(GridSize);
                int y = rand.Next(GridSize);

                if (Positions[x, y] != 9 && !IsSafeZone(x, y))
                {
                    Positions[x, y] = 9;
                    bombsPlaced++;
                }
            }
        }

        private void GeneratePositionValues()
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

        private void RevealCell(int x, int y)
        {
            Button btn = ButtonList[x, y];
            if (!btn.Enabled || btn.Text == "⚑") return;

            btn.Enabled = false;
            btn.Text = Positions[x, y] == 0 ? "" : Positions[x, y].ToString();

            if (Positions[x, y] == 0)
                RevealAdjacentZeros(x, y);
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
                        RevealCell(newX, newY);
                }
            }
        }

        private void RevealAllBombs()
        {
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    if (Positions[x, y] == 9)
                        ButtonList[x, y].Text = "💣";

                    ButtonList[x, y].Enabled = false;
                }
            }
        }

        private void CheckWinCondition()
        {
            int revealed = 0;
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    if (!ButtonList[x, y].Enabled && Positions[x, y] != 9)
                        revealed++;
                }
            }

            if (revealed == GridSize * GridSize - BombCount)
            {
                MessageBox.Show("🎉 You Win!");
                RevealAllBombs();
            }
        }
    }
}
