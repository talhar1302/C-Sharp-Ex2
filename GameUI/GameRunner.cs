using GameLogic;
using System;
using System.Collections.Generic;

namespace GameUI
{
    public class GameRunner
    {
        private UI ui;

        public GameRunner(UI ui)
        {
            this.ui = ui;
        }

        public void Run()
        {
            bool playAgain, IsMatch;
            do
            {
                ui.ClearScreen();
                int rows, columns;
                (rows, columns) = ui.GetBoardSize();
                List<Player> players = new List<Player> { new Player(ui.GetPlayerName("Enter player 1 name: "), ePlayerType.Human) };

                if (ui.GetYesNoInput("Do you want to play against the computer? (yes/no): "))
                {
                    players.Add(new Player("Computer", ePlayerType.AI));
                }
                else
                {
                    players.Add(new Player(ui.GetPlayerName("Enter player 2 name: "), ePlayerType.Human));
                }

                Game game = new Game(rows, columns, players);
                ui.ClearScreen();
                ui.PrintBoard(game.GetBoard(), revealAll: false);

                while (game.GetGameState() == GameState.Playing)
                {
                    Player currentPlayer = game.GetCurrentPlayer();
                    ui.DisplayTurn(currentPlayer);

                    (int row1, int col1) = ui.GetMove(game, currentPlayer);
                    game.MakeMove(row1, col1, 1);
                    ui.DisplayBoardAndCard(game, currentPlayer, row1, col1, "First");
                    if (currentPlayer.PlayerType != ePlayerType.Human)
                    {
                        System.Threading.Thread.Sleep(2000); // Wait for 2 seconds
                    }

                    (int row2, int col2) = ui.GetMove(game, currentPlayer);
                    game.MakeMove(row2, col2, 2);
                    ui.DisplayBoardAndCard(game, currentPlayer, row2, col2, "Second");

                    game.CheckMove(out IsMatch);
                    if (IsMatch)
                    {
                        ui.DisplayMatch(currentPlayer.Name);
                    }
                    else
                    {
                        ui.DisplayNoMatch();
                    }
                    System.Threading.Thread.Sleep(2000); // Wait for 2 seconds
                    ui.ClearScreen();
                    ui.PrintBoard(game.GetBoard(), revealAll: false);

                    if (game.GetGameState() != GameState.Playing)
                    {
                        break;
                    }
                }

                ui.ClearScreen();
                ui.PrintBoard(game.GetBoard(), revealAll: true);
                ui.DisplayGameOver();
                ui.DisplayFinalScores(players);
                ui.DisplayWinnerOrTie(game);

                playAgain = ui.PromptForNewGame();

                if (!playAgain)
                {
                    Console.WriteLine("Thank you for playing!");
                    Console.WriteLine("Press enter to exit");
                    Console.ReadLine();
                }

            } while (playAgain);
        }
    }
}
