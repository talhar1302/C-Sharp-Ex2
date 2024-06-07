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
            { 'A', 'A' }, { 'B', 'B' }, { 'C', 'C' }, { 'D', 'D' }, { 'E', 'E' }, { 'F', 'F' },
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
            string playerName;
            do
            {
                Console.Write(prompt);
                playerName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    Console.WriteLine("Player name cannot be empty. Please enter a valid name.");
                }
            } while (string.IsNullOrWhiteSpace(playerName));
            return playerName;
        }

        public bool GetYesNoInput(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine().ToLower();
                if (input != "yes" && input != "no")
                {
                    Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
                }
            } while (input != "yes" && input != "no");

            return input == "yes";
        }

        public (int, int) GetBoardSize()
        {
            int rows = 0, columns = 0;
            bool validInput = false;
            while (!validInput)
            {
                Console.Write("Enter the number of rows (4,5,6): ");
                if (int.TryParse(Console.ReadLine(), out rows) && rows >= k_MinBoardSize && rows <= k_MaxBoardSize)
                {
                    Console.Write("Enter the number of columns (4,5,6): ");
                    if (int.TryParse(Console.ReadLine(), out columns) && columns >= k_MinBoardSize && columns <= k_MaxBoardSize && Board.IsValidBoard(rows, columns))
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid column size. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid row size. Please try again.");
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

        public void DisplayBoardAndCard(Game game, Player currentPlayer, int row, int col, string cardOrder)
        {
            ClearScreen();
            PrintBoard(game.GetBoard(), revealAll: false);
            DisplayTurn(currentPlayer);
            DisplayCard(CardValues[game.GetBoard().GetCards()[row, col].Value], cardOrder);        
        }

        public void DisplayWinnerOrTie(Game game)
        {
            Player winner = game.DetermineWinner();
            if (game.GetGameState() == GameState.Win)
            {
                Console.WriteLine($"The winner is {winner.Name} with {winner.Score} points!");
            }
            else if (game.GetGameState() == GameState.Draw)
            {
                Console.WriteLine("The game is a draw!");
            }
        }

        public bool PromptForNewGame()
        {
            string input;
            do
            {
                Console.WriteLine("Would you like to start a new game? (yes/Q to quit): ");
                input = Console.ReadLine().ToLower();
                if (input != "yes" && input != "q")
                {
                    Console.WriteLine("Invalid input. Please enter 'yes' to play again or 'Q' to quit.");
                }
            } while (input != "yes" && input != "q");

            return input == "yes";
        }
    }
}
