using System;
using System.Linq;
using MinesweeperAPI.Models;

namespace MinesweeperAPI.Services
{
    public class GameService : IGameService
    {
        private static readonly Random _random = new Random();
        private static readonly Dictionary<Guid, GameState> _games = new Dictionary<Guid, GameState>();

        public GameInfoResponse StartNewGame(NewGameRequest request)
        {
            if (request.Width > 30 || request.Height > 30 || request.Mines_count >= request.Width * request.Height)
            {
                throw new ArgumentException("Invalid game parameters.");
            }

            var gameId = Guid.NewGuid();
            var field = GenerateField(request.Width, request.Height, request.Mines_count);

            _games[gameId] = new GameState
            {
                Width = request.Width,
                Height = request.Height,
                Mines_count = request.Mines_count,
                Field = field,
                Completed = false
            };

            return new GameInfoResponse
            {
                Game_id = gameId,
                Width = request.Width,
                Height = request.Height,
                Mines_count = request.Mines_count,
                Completed = false,
                Field = GetHiddenField(field)
            };
        }

        public GameInfoResponse MakeTurn(GameTurnRequest request)
        {
            if (!_games.TryGetValue(request.Game_id, out var gameState))
            {
                throw new ArgumentException("Game not found.");
            }

            if (gameState.Completed)
            {
                throw new InvalidOperationException("Game is already completed.");
            }

            if (request.Row < 0 || request.Row >= gameState.Height || request.Col < 0 || request.Col >= gameState.Width)
            {
                throw new ArgumentException("Invalid cell coordinates.");
            }

            if (gameState.Field[request.Row][request.Col] == 'X')
            {
                gameState.Completed = true;
                return new GameInfoResponse
                {
                    Game_id = request.Game_id,
                    Width = gameState.Width,
                    Height = gameState.Height,
                    Mines_count = gameState.Mines_count,
                    Completed = true,
                    Field = gameState.Field
                };
            }

            if (gameState.Field[request.Row][request.Col] == '0')
            {
                OpenAdjacentCells(gameState, request.Row, request.Col);
            }

            var hiddenField = GetHiddenField(gameState.Field);

            if (CheckWinCondition(hiddenField, gameState.Mines_count))
            {
                gameState.Completed = true;
                return new GameInfoResponse
                {
                    Game_id = request.Game_id,
                    Width = gameState.Width,
                    Height = gameState.Height,
                    Mines_count = gameState.Mines_count,
                    Completed = true,
                    Field = gameState.Field
                };
            }

            return new GameInfoResponse
            {
                Game_id = request.Game_id,
                Width = gameState.Width,
                Height = gameState.Height,
                Mines_count = gameState.Mines_count,
                Completed = false,
                Field = hiddenField
            };
        }

        private char[][] GenerateField(int width, int height, int minesCount)
        {
            var field = new char[height][];
            for (var i = 0; i < height; i++)
            {
                field[i] = new char[width];
                for (var j = 0; j < width; j++)
                {
                    field[i][j] = ' ';
                }
            }

            var minesPlaced = 0;
            while (minesPlaced < minesCount)
            {
                var row = _random.Next(height);
                var col = _random.Next(width);

                if (field[row][col] != 'X')
                {
                    field[row][col] = 'X';
                    minesPlaced++;
                }
            }

            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    if (field[i][j] != 'X')
                    {
                        field[i][j] = CountAdjacentMines(field, i, j).ToString()[0];
                    }
                }
            }

            return field;
        }

        private int CountAdjacentMines(char[][] field, int row, int col)
        {
            var count = 0;
            for (var i = row - 1; i <= row + 1; i++)
            {
                for (var j = col - 1; j <= col + 1; j++)
                {
                    if (i >= 0 && i < field.Length && j >= 0 && j < field[0].Length && field[i][j] == 'X')
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void OpenAdjacentCells(GameState gameState, int row, int col)
        {
            var queue = new Queue<(int, int)>();
            queue.Enqueue((row, col));

            while (queue.Count > 0)
            {
                var (r, c) = queue.Dequeue();
                if (gameState.Field[r][c] == ' ')
                {
                    gameState.Field[r][c] = '0';
                    for (var i = r - 1; i <= r + 1; i++)
                    {
                        for (var j = c - 1; j <= c + 1; j++)
                        {
                            if (i >= 0 && i < gameState.Height && j >= 0 && j < gameState.Width && gameState.Field[i][j] == ' ')
                            {
                                queue.Enqueue((i, j));
                            }
                        }
                    }
                }
            }
        }

        private char[][] GetHiddenField(char[][] field)
        {
            return field.Select(row => row.Select(cell => cell == 'X' || cell == 'M' ? ' ' : cell).ToArray()).ToArray();
        }

        private bool CheckWinCondition(char[][] hiddenField, int minesCount)
        {
            var openedCells = hiddenField.Sum(row => row.Count(cell => cell != ' '));
            var totalCells = hiddenField.Length * hiddenField[0].Length;
            return openedCells == totalCells - minesCount;
        }

        private class GameState
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public int Mines_count { get; set; }
            public char[][] Field { get; set; }
            public bool Completed { get; set; }
        }
    }
}