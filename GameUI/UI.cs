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
        private readonly Dictionary<char, char> r_CardValues = new Dictionary<char, char>
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

        public string GetPlayerName(string i_Prompt)
        {
            string playerName;
            do
            {
                Console.Write(i_Prompt);
                playerName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    Console.WriteLine("Player name cannot be empty. Please enter a valid name.");
                }
            } while (string.IsNullOrWhiteSpace(playerName));
            return playerName;
        }

        public bool GetYesNoInput()
        {
            string input;
            do
            {
                Console.Write("Do you want to play against the computer? (yes/no): ");
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

        public void PrintBoard(Board i_Board, bool revealAll = false)
        {
            Card[,] cards = i_Board.GetCards();
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
                        Console.Write(r_CardValues[cards[i, j].Value]); // Printing the predefined adaptive char
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

        public void DisplayTurn(Player i_Player)
        {
            Console.WriteLine($"{i_Player.Name}'s turn. Score: {i_Player.Score}");
        }

        public void DisplayCard(char i_Card, string i_CardOrder)
        {
            Console.WriteLine($"{i_CardOrder} card: {i_Card}");
        }

        public void DisplayMatch(string i_Name)
        {
            Console.WriteLine($"It's a match! {i_Name} gets another turn.");
        }

        public void DisplayNoMatch()
        {
            Console.WriteLine("Not a match.");
        }

        public void DisplayWinner(Player i_Player)
        {
            Console.WriteLine($"{i_Player.Name} wins with {i_Player.Score} points!");
        }

        public void DisplayTie()
        {
            Console.WriteLine("It's a tie!");
        }

        public void DisplayGameOver()
        {
            Console.WriteLine("Game over. Thank you for playing!");
        }

        public void DisplayFinalScores(List<Player> i_Players)
        {
            Console.WriteLine("\nFinal Scores:");
            foreach (var player in i_Players)
            {
                Console.WriteLine($"{player.Name}: {player.Score}");
            }
        }

        public (int, int) GetUserMove(Game i_Game)
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
                    eInputError error = CheckMoveValidation(i_Game, move.Value);
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

        private eInputError CheckMoveValidation(Game i_Game, (int row, int col) i_Move)
        {
            return i_Game.CheckMoveValidation(i_Move.row, i_Move.col);
        }

        private (int, int)? ParseInput(string i_Input)
        {
            (int row, int col)? parseInput = null;
            if (i_Input.Length == 2 && char.IsLetter(i_Input[0]) && char.IsDigit(i_Input[1]))
            {
                int col = char.ToUpper(i_Input[0]) - 'A';
                int row = int.Parse(i_Input[1].ToString()) - 1;
                parseInput = (row, col);
            }
            return parseInput;
        }

        public (int row, int col) GetMove(Game i_Game, Player i_CurrentPlayer)
        {
            (int row, int col) move;
            switch (i_CurrentPlayer.PlayerType)
            {
                case ePlayerType.Random:
                    move = i_Game.GetComputerRandomMove();
                    break;

                case ePlayerType.AI:
                    move = i_Game.GetComputerAIMove();
                    break;
                case ePlayerType.AI_WEAK:
                    move = i_Game.GetComputerRandomMove();
                    break;
                default:
                    move = GetUserMove(i_Game);
                    break;
            }
            return move;
        }

        public void DisplayBoardAndCard(Game i_Game, Player i_CurrentPlayer, int i_row, int i_col, string i_CardOrder)
        {
            ClearScreen();
            PrintBoard(i_Game.GetBoard(), revealAll: false);
            DisplayTurn(i_CurrentPlayer);
            DisplayCard(r_CardValues[i_Game.GetBoard().GetCards()[i_row, i_col].Value], i_CardOrder);        
        }

        public void DisplayWinnerOrTie(Game i_Game)
        {
            Player winner = i_Game.DetermineWinner();
            if (i_Game.GetGameState() == GameState.Win)
            {
                Console.WriteLine($"The winner is {winner.Name} with {winner.Score} points!");
            }
            else if (i_Game.GetGameState() == GameState.Draw)
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

        public bool GetComputerLevel()
        {
            string input;
            do
            {
                Console.WriteLine("Do you want the computer to play on hard difficulty (yes/no):");
                input = Console.ReadLine().ToLower();
                if (input != "yes" && input != "no")
                {
                    Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.");
                }
            } while (input != "yes" && input != "no");

            return input == "yes";

        }

        public void DisplayExitGameMessage()
        {
            Console.WriteLine("Thank you for playing!");
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
