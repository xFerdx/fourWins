using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace fourWins
{
    public partial class Form1 : Form
    {
        private Button[,] buttons = new Button[6, 7];
        private bool isPlayerOneTurn = true;
        private int[,] board = new int[6, 7];

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        public Form1()
        {
            InitializeComponent();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            int buttonWidth = 80;
            int buttonHeight = 80;

            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    buttons[row, col] = new Button
                    {
                        Width = buttonWidth,
                        Height = buttonHeight,
                        Left = col * buttonWidth,
                        Top = row * buttonHeight,
                        BackColor = Color.LightGray,
                        Tag = new Point(row, col),
                        Font = new Font("Arial", 24, FontStyle.Bold)
                    };
                    buttons[row, col].Click += new EventHandler(Button_Click);
                    this.Controls.Add(buttons[row, col]);
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            var position = (Point)button.Tag;
            int row = position.X;
            int col = position.Y;

            bool fullCol = true;
            for (int i = 5; i >= 0; i--)
            {
                if (board[i, col] == 0)
                {
                    board[i, col] = 1;
                    buttons[i, col].BackColor = Color.Red;
                    if (CheckForWinner(i, col))
                        return;
                    fullCol = false;
                    break;
                }
            }

            if(fullCol)
                return;

            var result = Solver.Solve(this.board, 10);
            int col2 = result.Item1;
            int z = result.Item2;

            for (int i = 5; i >= 0; i--)
            {
                if (board[i, col2] == 0)
                {
                    board[i, col2] = 2;
                    buttons[i, col2].BackColor = Color.Yellow;
                    CheckForWinner(i, col2);
                    break;
                }
            }
            
            AllocConsole();
            Console.WriteLine(Solver.CalculateBoardScore(board));
            Console.WriteLine(z);
        }

        private bool CheckForWinner(int row, int col)
        {
            int[,] dirs = new int[,] { { 1, 0 }, { 0, 1 }, { 1, 1 }, { 1, -1 } };
            int player = board[row, col];
            for (int i = 0; i < dirs.GetLength(0); i++)
            {
                if (CheckDirection(row, col, dirs[i, 0], dirs[i, 1], player))
                {
                    MessageBox.Show($"Spieler {(player == 1 ? "1 (Rot)" : "2 (Gelb)")} hat gewonnen!");
                    ResetBoard();
                    return true;
                }
            }
            return false;
        }

        private bool CheckDirection(int row, int col, int rowDir, int colDir, int player)
        {
            int count = 1;
            for (int i = 1; i < 4; i++)
            {
                int r = row + i * rowDir;
                int c = col + i * colDir;
                if (r < 0 || r >= 6 || c < 0 || c >= 7 || board[r, c] != player) break;
                count++;
            }
            for (int i = 1; i < 4; i++)
            {
                int r = row - i * rowDir;
                int c = col - i * colDir;
                if (r < 0 || r >= 6 || c < 0 || c >= 7 || board[r, c] != player) break;
                count++;
            }
            return count >= 4;
        }

        private void ResetBoard()
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    buttons[row, col].BackColor = Color.LightGray;
                    board[row, col] = 0;
                }
            }
        }
    }
}
