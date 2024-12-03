using System;
using System.Collections.Concurrent;

namespace fourWins
{
    public class Solver
    {
        private static ConcurrentDictionary<int, int> hashMap = new ConcurrentDictionary<int, int>();

        private static int z = 0;

        public static (int, int) Solve(int[,] board, int depth)
        {
            z = 0;
            int bestEval = int.MinValue;
            int bestMove = -1;
            int[] columnOrder = { 3, 2, 4, 1, 5, 0, 6 };

            foreach (int col in columnOrder)
            {
                int pos = FindPosition(board, col);
                if (pos == -1) continue;

                board[pos, col] = 2;
                int eval = MinMax(board, false, depth - 1, int.MinValue, int.MaxValue);
                board[pos, col] = 0;

                if (eval > bestEval)
                {
                    bestEval = eval;
                    bestMove = col;
                }
            }

            return (bestMove, z);
        }

        public static int MinMax(int[,] board, bool myTurn, int depth, int alpha, int beta)
        {
            if (depth == 0 || CheckForWinner(board)){
                z++;
                return -CalculateBoardScore(board);
            }

            int boardHash = HashBoard(board);
            if (hashMap.TryGetValue(boardHash, out int cachedValue))
                return cachedValue;

            int eval = myTurn ? int.MinValue : int.MaxValue;
            int[] columnOrder = { 3, 2, 4, 1, 5, 0, 6 };

            foreach (int col in columnOrder)
            {
                int pos = FindPosition(board, col);
                if (pos == -1) continue;

                board[pos, col] = myTurn ? 2 : 1;
                int childEval = MinMax(board, !myTurn, depth - 1, alpha, beta);
                board[pos, col] = 0;

                eval = myTurn ? Math.Max(eval, childEval) : Math.Min(eval, childEval);
                if (myTurn) alpha = Math.Max(alpha, eval);
                else beta = Math.Min(beta, eval);

                if (beta <= alpha) break;
            }

            hashMap[boardHash] = eval;
            return eval;
        }

        public static int HashBoard(int[,] board)
        {
            int hash = 17;
            foreach (int value in board)
                hash = hash * 31 + value.GetHashCode();
            return hash;
        }

        public static int FindPosition(int[,] board, int col)
        {
            for (int row = 5; row >= 0; row--)
                if (board[row, col] == 0)
                    return row;
            return -1;
        }

        public static bool CheckForWinner(int[,] board)
        {
            int[,] dirs = { { 1, 0 }, { 0, 1 }, { 1, 1 }, { 1, -1 } };
            for (int row = 0; row < 6; row++)
                for (int col = 0; col < 7; col++)
                    if (board[row, col] != 0)
                        for (int i = 0; i < dirs.GetLength(0); i++)
                            if (CheckDirection(board, row, col, dirs[i, 0], dirs[i, 1], board[row, col]))
                                return true;
            return false;
        }

        private static bool CheckDirection(int[,] board, int row, int col, int rowDir, int colDir, int player)
        {
            int count = 1;
            for (int i = 1; i < 4; i++)
            {
                int r = row + i * rowDir, c = col + i * colDir;
                if (!IsWithinBounds(r, c) || board[r, c] != player) break;
                count++;
            }
            for (int i = 1; i < 4; i++)
            {
                int r = row - i * rowDir, c = col - i * colDir;
                if (!IsWithinBounds(r, c) || board[r, c] != player) break;
                count++;
            }
            return count >= 4;
        }

        public static bool IsWithinBounds(int row, int col)
        {
            return row >= 0 && row < 6 && col >= 0 && col < 7;
        }

        public static int CalculateBoardScore(int[,] board)
        {
            int score = 0;
            for (int row = 0; row < 6; row++)
                for (int col = 0; col < 7; col++)
                    if (board[row, col] != 0)
                    {
                        score += EvaluateDirection(board, row, col, 1, 0);
                        score += EvaluateDirection(board, row, col, 0, 1);
                        score += EvaluateDirection(board, row, col, 1, 1);
                        score += EvaluateDirection(board, row, col, 1, -1);
                    }
            return score;
        }

        public static int EvaluateDirection(int[,] board, int row, int col, int rowDir, int colDir)
        {
            int player = board[row, col];
            int score = 0, countPlayer = 0, countOpponent = 0;

            for (int i = 0; i < 4; i++)
            {
                int newRow = row + i * rowDir;
                int newCol = col + i * colDir;

                if (IsWithinBounds(newRow, newCol))
                {
                    if (board[newRow, newCol] == player)
                        countPlayer++;
                    else if (board[newRow, newCol] != 0)
                        countOpponent++;
                }
            }

            if (countPlayer == 4) score += 100000;
            else if (countPlayer == 3 && countOpponent == 0) score += 100;
            else if (countPlayer == 2 && countOpponent == 0) score += 10;

            if (countOpponent == 4) score -= 100000;
            else if (countOpponent == 3 && countPlayer == 0) score -= 100;
            else if (countOpponent == 2 && countPlayer == 0) score -= 10;

            return score;
        }
    }
}
