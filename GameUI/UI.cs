using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    using GameLogic;
    using System;
    using Ex02.ConsoleUtils;

    public class UI
    {
        private const int k_MinBoardSize = 4;
        private const int k_MaxBoardSize = 6;
        public const string k_QuitChar = "Q";
        // Constant dictionary for card values
        private readonly Dictionary<char, char> CardValues = new Dictionary<char, char>
        {
            { 'A', '#' }, { 'B', 'B' }, { 'C', 'C' }, { 'D', 'D' }, { 'E', 'E' }, { 'F', 'F' },
            { 'G', 'G' }, { 'H', 'H' }, { 'I', 'I' }, { 'J', 'J' }, { 'K', 'K' }, { 'L', 'L' },
            { 'M', 'M' }, { 'N', 'N' }, { 'O', 'O' }, { 'P', 'P' }, { 'Q', 'Q' }, { 'R', 'R' },
            { 'S', 'S' }, { 'T', 'T' }, { 'U', 'U' }, { 'V', 'V' }, { 'W', 'W' }, { 'X', 'X' },
            { 'Y', 'Y' }, { 'Z', 'Z' }
        };

        public void ClearScreen()
        {
            Screen.Clear();
        }

        public string GetPlayerName(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        public bool GetYesNoInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().ToLower() == "yes";
        }

        public (int, int) GetBoardSize()
        {
            int rows, columns;
            while (true)
            {
                Console.Write("Enter the number of rows (4,5,6): ");
                rows = int.Parse(Console.ReadLine());
                Console.Write("Enter the number of columns (4,5,6): ");
                columns = int.Parse(Console.ReadLine());

                if ((rows >= k_MinBoardSize && rows <= k_MaxBoardSize) && (columns >= k_MinBoardSize && columns <= k_MaxBoardSize) && Board.IsValidBoard(rows, columns))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid board size. Please try again.");
                }
            }
            return (rows, columns);
        }

        public void PrintBoard(Board board, bool revealAll = false)
        {
            Card[,] cards = board.GetCards();
            int rows = cards.GetLength(0);
            int columns = cards.GetLength(1);

            // Print the column headers
            Console.Write("  ");
            for (int c = 0; c < columns; c++)
            {
                Console.Write((char)('A' + c) + " ");
            }
            Console.WriteLine();

            // Print the top border
            Console.Write(" ");
            for (int c = 0; c < columns; c++)
            {
                Console.Write("==");
            }
            Console.Write("=");
            Console.WriteLine();

            // Print the board rows
            for (int i = 0; i < rows; i++)
            {
                Console.Write((i + 1).ToString() + "|");
                for (int j = 0; j < columns; j++)
                {
                    if (cards[i, j].IsRevealed || revealAll)
                    {
                        Console.Write(CardValues[cards[i, j].Value]); // Printing the predefined adaptive char
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                    Console.Write("|");
                }
                Console.WriteLine();
                // Print the top border
                Console.Write(" ");
                for (int c = 0; c < columns; c++)
                {
                    Console.Write("==");
                }
                Console.Write("=");
                Console.WriteLine();
            }
        }

        public void DisplayTurn(Player player)
        {
            Console.WriteLine($"{player.Name}'s turn. Score: {player.Score}");
        }

        public void DisplayCard(char card, string cardOrder)
        {
            Console.WriteLine($"{cardOrder} card: {card}");
        }

        public void DisplayMatch(string i_Name)
        {
            Console.WriteLine($"It's a match! {i_Name} gets another turn.");
        }

        public void DisplayNoMatch()
        {
            Console.WriteLine("Not a match.");
        }

        public void DisplayWinner(Player player)
        {
            Console.WriteLine($"{player.Name} wins with {player.Score} points!");
        }

        public void DisplayTie()
        {
            Console.WriteLine("It's a tie!");
        }

        public void DisplayGameOver()
        {
            Console.WriteLine("Game over. Thank you for playing!");
        }

        public void DisplayFinalScores(List<Player> players)
        {
            Console.WriteLine("\nFinal Scores:");
            foreach (var player in players)
            {
                Console.WriteLine($"{player.Name}: {player.Score}");
            }
        }

        public (int, int) GetUserMove(Game game)
        {
            while (true)
            {
                Console.Write("Enter a card to reveal (e.g., A1 or Q to quit): ");
                string input = Console.ReadLine();
                if (input.ToUpper() == k_QuitChar)
                {
                    Environment.Exit(0);
                }

                (int row, int col)? move = ParseInput(input);
                if (move == null)
                {
                    Console.WriteLine("Invalid input. Try again.\n");
                }
                else
                {
                    eInputError error = CheckMoveValidation(game, move.Value);
                    switch (error)
                    {
                        case eInputError.NoError:
                            return move.Value;
                        case eInputError.OutOfBounds:
                            Console.WriteLine("Position out of bounds. Try again.\n");
                            break;
                        case eInputError.CardAlreadyRevealed:
                            Console.WriteLine("Card has already been revealed. Try again.\n");
                            break;
                    }
                }
            }
        }

        private eInputError CheckMoveValidation(Game game, (int row, int col) move)
        {
            return game.CheckMoveValidation(move.row, move.col);
        }

        private (int, int)? ParseInput(string input)
        {
            (int row, int col)? parseInput = null;
            if (input.Length == 2 && char.IsLetter(input[0]) && char.IsDigit(input[1]))
            {
                int col = char.ToUpper(input[0]) - 'A';
                int row = int.Parse(input[1].ToString()) - 1;
                parseInput = (row, col);
            }
            return parseInput;
        }

        public (int row, int col) GetMove(Game game, Player currentPlayer)
        {
            (int row, int col) move;
            switch (currentPlayer.PlayerType)
            {
                case ePlayerType.Random:
                    move = game.GetComputerRandomMove();
                    break;

                case ePlayerType.AI:
                    move = game.GetComputerAIMove();
                    break;
                default:
                    move = GetUserMove(game);
                    break;
            }
            return move;
        }

        public void DisplayBoardAndCard(Game game, int row, int col, string cardOrder)
        {
            ClearScreen();
            PrintBoard(game.GetBoard(), revealAll: false);
            Player currentPlayer = game.GetCurrentPlayer();
            DisplayTurn(currentPlayer);
            DisplayCard(CardValues[game.GetBoard().GetCards()[row, col].Value], cardOrder);
            if (game.GetCurrentPlayer().PlayerType != ePlayerType.Human)
            {
                System.Threading.Thread.Sleep(2000); // Wait for 2 seconds
            }
        }

        public void DisplayWinnerOrTie(Game game)
        {
            Player winner = game.DetermineWinner();
            if (winner != null)
            {
                DisplayWinner(winner);
            }
            else
            {
                DisplayTie();
            }
        }

        public bool PromptForNewGame()
        {
            Console.WriteLine("Would you like to start a new game? (yes/Q to quit): ");
            string input = Console.ReadLine().ToLower();
            return input == "yes";
        }
    }
}
