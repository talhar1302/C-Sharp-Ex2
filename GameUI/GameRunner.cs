using GameLogic;
using System;
using System.Collections.Generic;

namespace GameUI
{
    public class GameRunner
    {
        private readonly UI r_UI;
        public GameRunner(UI i_UI)
        {
            r_UI = i_UI;
        }
        public void Run()
        {
            bool playAgain = true;
            do
            {
                r_UI.ClearScreen();
                var (rows, columns) = r_UI.GetBoardSize();
                List<Player> players = r_UI.GetPlayers();
                Game game = new Game(rows, columns, players);
                r_UI.StartGame(game);
                while (game.GetGameState() == eGameState.Playing)
                {
                    Player currentPlayer = game.GetCurrentPlayer();
                    r_UI.DisplayTurn(currentPlayer);
                    if (ProcessMove(game, currentPlayer, out playAgain) == false)
                    {
                        goto Exit;
                    }
                }

                r_UI.EndGame(game, players);
                playAgain = r_UI.PromptForNewGame();
            } while (playAgain == true);

        Exit:
            r_UI.DisplayExitGameMessage();
            return;

        }
        private bool ProcessMove(Game i_Game, Player i_CurrentPlayer, out bool io_PlayAgain)
        {
            bool isSuccess = true;
            io_PlayAgain = true;
            (int row, int col) move1;
            (int row, int col) move2;

            if (!r_UI.MakeMove(i_Game, i_CurrentPlayer, out move1))
            {
                io_PlayAgain = false;
                isSuccess = false;
            }
            else
            {
                i_Game.MakeMove(move1.row, move1.col, 1);
                r_UI.DisplayBoardAndCard(i_Game, i_CurrentPlayer, move1.row, move1.col, "First");
                if (!r_UI.MakeMove(i_Game, i_CurrentPlayer, out move2))
                {
                    io_PlayAgain = false;
                    isSuccess = false;
                }
                else
                {
                    i_Game.MakeMove(move2.row, move2.col, 2);
                    r_UI.DisplayBoardAndCard(i_Game, i_CurrentPlayer, move2.row, move2.col, "Second");
                    i_Game.CheckMove(out bool isMatch);
                    r_UI.DisplayMatchResult(i_Game, isMatch, i_CurrentPlayer);
                    if (i_Game.GetGameState() != eGameState.Playing)
                    {
                        isSuccess = false;
                    }
                }
            }

            return isSuccess;
        }
    }
}
