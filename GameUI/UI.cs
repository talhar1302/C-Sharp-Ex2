using GameLogic;
using Ex02.ConsoleUtils;
using System;
using System.Collections.Generic;

namespace GameUI
{
    public class UI
    {
        private const int k_MinBoardSize = 4;
        private const int k_MaxBoardSize = 6;
        private const string k_QuitChar = "Q";
        private const char k_FirstColumn = 'A';
        // Constant dictionary for card values - The key is the logic representaion, the value is the UI representaion
        private readonly Dictionary<char, char> r_CardValues = new Dictionary<char, char>();
        private void InitializeCardsValues(List<char> i_CardValues)
        {
            foreach (char cardValue in i_CardValues)
            {
                r_CardValues[cardValue] = cardValue;
            }
        }
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
                    Console.WriteLine("Player name cannot be empty. Please enter a valid name.\n");
                }
            } while (string.IsNullOrWhiteSpace(playerName));

            return playerName;
        }

        public bool GetYesNoInput(string i_Prompt)
        {
            string input;
            bool result = false;

            do
            {
                Console.Write(i_Prompt);
                input = Console.ReadLine().ToLower();
                if (input == "yes")
                {
                    result = true;
                }
                else if (input == "no")
                {
                    result = false;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'yes' or 'no'.\n");
                }
            } while (input != "yes" && input != "no");

            return result;
        }

        public (int, int) GetBoardSize()
        {
            int rows = 0, columns = 0;
            bool validInput = false;
         
            while (!validInput) // Validate rows
            {
                Console.Write("Enter the number of rows (4,5,6): ");
                if (int.TryParse(Console.ReadLine(), out rows) && rows >= k_MinBoardSize && rows <= k_MaxBoardSize)
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("Invalid row size. Please try again.\n");
                }
            }

            validInput = false; // Reset for column validation
            while (!validInput)
            {
                bool evenBoardSize = true;
                Console.Write("Enter the number of columns (4,5,6): ");
                if (int.TryParse(Console.ReadLine(), out columns) && columns >= k_MinBoardSize && columns <= k_MaxBoardSize)
                {
                    if (!Board.IsValidBoard(rows, columns))
                    {
                        evenBoardSize = false;
                    }
                    else
                    {
                        validInput = true;
                    }
                }

                if (!evenBoardSize)
                {
                    Console.WriteLine("The board size has to be an even number. Please try again.\n");
                    continue;
                }

                if (!validInput)
                {
                    Console.WriteLine("Invalid column size. Please try again.\n");
                    continue;
                }             
            }

            return (rows, columns);
        }


        public List<Player> GetPlayers()
        {
            List<Player> players = new List<Player> { new Player(GetPlayerName("Enter player 1 name: "), ePlayerType.Human) };
            bool isAgainstComputer = GetYesNoInput("Do you want to play against the computer? (yes/no): ");
            bool isComputerHard = false;

            if (isAgainstComputer)
            {
                isComputerHard = GetYesNoInput("Do you want the computer to play on hard difficulty? (yes/no): ");
                ePlayerType playerType = isComputerHard ? ePlayerType.AI : ePlayerType.ComputerRandom;
                players.Add(new Player("Computer", playerType));
            }
            else
            {
                players.Add(new Player(GetPlayerName("Enter player 2 name: "), ePlayerType.Human));
            }

            return players;
        }

        public void StartGame(Game i_Game)
        {
            InitializeCardsValues(i_Game.GetBoard().CardValues);
            ClearScreen();
            PrintBoard(i_Game.GetBoard(), i_RevealAll: false);
        }

        public void DisplayTurn(Player i_Player)
        {
            Console.WriteLine($"{i_Player.Name}'s turn. Score: {i_Player.Score}");
        }

        public bool MakeMove(Game i_Game, Player i_CurrentPlayer, out (int row, int col) io_Move)
        {
            io_Move = GetMove(i_Game, i_CurrentPlayer);
            bool isValidMove = io_Move.row != -1;

            return isValidMove;
        }

        public void DisplayBoardAndCard(Game i_Game, Player i_Player, int i_Row, int i_Col, string i_CardOrder)
        {
            ClearScreen();
            PrintBoard(i_Game.GetBoard(), i_RevealAll: false);
            DisplayTurn(i_Player);
            DisplayCard(r_CardValues[i_Game.GetBoard().GetCards()[i_Row, i_Col].Value], i_CardOrder);
            if (i_Player.PlayerType != ePlayerType.Human)
            {
                System.Threading.Thread.Sleep(2000); // Wait for 2 seconds
            }
        }

        public void DisplayCard(char i_Card, string i_CardOrder)
        {
            Console.WriteLine($"{i_CardOrder} card: {i_Card}");
        }

        public void DisplayMatchResult(Game i_Game, bool i_IsMatch, Player i_Player)
        {
            if (i_IsMatch)
            {
                Console.WriteLine($"It's a match! {i_Player.Name} gets another turn.");
            }
            else
            {
                Console.WriteLine("Not a match.");
            }

            System.Threading.Thread.Sleep(2000); // Wait for 2 seconds
            ClearScreen();
            PrintBoard(i_Game.GetBoard(), i_RevealAll: false);
        }

        public void EndGame(Game i_Game, List<Player> i_Players)
        {
            ClearScreen();
            PrintBoard(i_Game.GetBoard(), i_RevealAll: true);
            DisplayGameOver();
            DisplayFinalScores(i_Players);
            DisplayWinnerOrTie(i_Game);
        }

        public void PrintBoard(Board i_Board, bool i_RevealAll = false)
        {
            Card[,] cards = i_Board.GetCards();
            int rows = cards.GetLength(0);
            int columns = cards.GetLength(1);

            Console.Write("  ");
            for (int c = 0; c < columns; c++)
            {
                Console.Write((char)(k_FirstColumn + c) + " ");
            }

            Console.WriteLine();
            Console.Write(" ");
            for (int c = 0; c < columns; c++)
            {
                Console.Write("==");
            }

            Console.Write("=");
            Console.WriteLine();
            for (int i = 0; i < rows; i++)
            {
                Console.Write((i + 1).ToString() + "|");
                for (int j = 0; j < columns; j++)
                {
                    if (cards[i, j].IsRevealed || i_RevealAll)
                    {
                        Console.Write(r_CardValues[cards[i, j].Value]);
                    }
                    else
                    {
                        Console.Write(" ");
                    }

                    Console.Write("|");
                }

                Console.WriteLine();
                Console.Write(" ");
                for (int c = 0; c < columns; c++)
                {
                    Console.Write("==");
                }

                Console.Write("=");
                Console.WriteLine();
            }
        }

        public void DisplayGameOver()
        {
            Console.WriteLine("Game over. Thank you for playing!");
        }

        public void DisplayFinalScores(List<Player> i_Players)
        {
            Console.WriteLine("\nFinal Scores:");
            foreach (Player player in i_Players)
            {
                Console.WriteLine($"{player.Name}: {player.Score}");
            }
        }

        public void DisplayWinnerOrTie(Game i_Game)
        {
            Player winner = i_Game.DetermineWinner();

            if (i_Game.GetGameState() == eGameState.Win)
            {
                Console.WriteLine($"The winner is {winner.Name} with {winner.Score} points!");
            }
            else if (i_Game.GetGameState() == eGameState.Draw)
            {
                Console.WriteLine("The game is a draw!");
            }
        }

        public bool PromptForNewGame()
        {
            return GetYesNoInput("Would you like to start a new game? (yes/no to quit): ");
        }

        public (int row, int col) GetMove(Game i_Game, Player i_CurrentPlayer)
        {
            (int row, int col) move;

            switch (i_CurrentPlayer.PlayerType)
            {
                case ePlayerType.ComputerRandom:
                    move = i_Game.GetComputerRandomMove();
                    break;
                case ePlayerType.AI:
                    move = i_Game.GetComputerAIMove();
                    break;
                default:
                    move = GetMoveFromUser(i_Game);
                    break;
            }

            return move;
        }

        public (int, int) GetMoveFromUser(Game i_Game)
        {
            (int row, int col) returnedMove = (-1, -1);
            (int row, int col)? move = null;
            bool validInput = false;
            string errorType = "";

            while (validInput == false)
            {
                Console.Write("Enter a card to reveal (e.g., A1 or Q to quit): ");
                string input = Console.ReadLine();
                if (input.ToUpper() == k_QuitChar)
                {
                    break;
                }

                move = ParseInput(input, out errorType);
                if (move == null)
                {
                    Console.WriteLine(errorType);
                    continue;
                }
                else
                {
                    eInputError error = CheckMoveValidation(i_Game, move.Value);
                    switch (error)
                    {
                        case eInputError.NoError:
                            validInput = true;
                            returnedMove = move.Value;
                            break;
                        case eInputError.OutOfBounds:
                            Console.WriteLine("Position out of bounds. Try again.\n");
                            break;
                        case eInputError.CardAlreadyRevealed:
                            Console.WriteLine("Card has already been revealed. Try again.\n");
                            break;
                    }
                }
            }

            return returnedMove;
        }

        private eInputError CheckMoveValidation(Game i_Game, (int row, int col) i_Move)
        {
            return i_Game.CheckMoveValidation(i_Move.row, i_Move.col);
        }

        private (int, int)? ParseInput(string i_Input, out string io_ErrorType)
        {
            (int row, int col)? parseInput = null;

            if (i_Input.Length != 2)
            {
                io_ErrorType = "Invalid input length(has to be 2). Please try again.\n";
            }
            else if (!char.IsLetter(i_Input[0]))
            {
                io_ErrorType = "Invalid column number. Please try again.\n";
            }
            else if (!char.IsDigit(i_Input[1]))
            {
                io_ErrorType = "Invalid row number. Please try again.\n";
            }
            else
            {
                int col = char.ToUpper(i_Input[0]) - Board.FirstCardValue;
                int row = int.Parse(i_Input[1].ToString()) - 1;
                parseInput = (row, col);
                io_ErrorType = "";
            }

            return parseInput;
        }

        public void DisplayExitGameMessage()
        {
            Console.WriteLine("\nExiting game. Thank you for playing!");
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}