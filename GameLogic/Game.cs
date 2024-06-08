using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class Game
    {
        private Random random = new Random();
        private Board m_board;
        private bool m_ExistAIPlayer = false;
        private List<Player> m_players;
        private Dictionary<char, List<Card>> rememberedCards;
        private Card[] m_SavedCardsForMatch;
        private int m_currentPlayerIndex;
        private (int row, int col) m_SavedFirstMoveLocation;
        private (int row, int col) m_SavedSecondMoveLocation;
        private GameState gameState;

        public Game(int rows, int columns, List<Player> playerNames)
        {
            m_board = new Board(rows, columns);
            m_players = new List<Player>();

            foreach (var player in playerNames)
            {
                if (player.PlayerType == ePlayerType.AI)
                    m_ExistAIPlayer = true;
                m_players.Add(player);
            }
            rememberedCards = new Dictionary<char, List<Card>>();
            m_SavedCardsForMatch = new Card[2];
            m_currentPlayerIndex = 0;
            gameState = GameState.Playing;
        }

        public Board GetBoard()
        {
            return m_board;
        }

        public Player GetCurrentPlayer()
        {
            return m_players[m_currentPlayerIndex];
        }

        public GameState GetGameState()
        {
            return gameState;
        }

        public void MakeMove(int i_row, int i_col, int i_NumOfCalling)
        {
            RevealCard(i_row, i_col);
            if (i_NumOfCalling == 1)
            {
                m_SavedFirstMoveLocation = (i_row, i_col);
            }
            if (i_NumOfCalling == 2)
            {
                m_SavedSecondMoveLocation = (i_row, i_col);
            }
        }


        public void CheckMove(out bool o_IsMatch)
        {
            Player currentPlayer = GetCurrentPlayer();
            if (CheckMatch(m_SavedFirstMoveLocation.row, m_SavedFirstMoveLocation.col, m_SavedSecondMoveLocation.row, m_SavedSecondMoveLocation.col))
            {
                o_IsMatch = true;
                currentPlayer.IncreaseScore();
                ClearMatchedCards(GetBoard().GetCards()[m_SavedFirstMoveLocation.row, m_SavedFirstMoveLocation.col].Value);
            }
            else
            {
                o_IsMatch = false;
                HideCards(m_SavedFirstMoveLocation.row, m_SavedFirstMoveLocation.col, m_SavedSecondMoveLocation.row, m_SavedSecondMoveLocation.col);
                SwitchPlayer();
            }

            if (AllCardsRevealed())
            {
                UpdateGameState();
            }
        }
        public void RevealCard(int i_row, int i_col)
        {
            char value = m_board.RevealCard(i_row, i_col);
            if (m_ExistAIPlayer)
            {
                RememberCard(i_row, i_col, value);
            }
        }

        public void HideCards(int i_row1, int i_col1, int i_row2, int i_col2)
        {
            m_board.HideCard(i_row1, i_col1);
            m_board.HideCard(i_row2, i_col2);
        }

        public bool AllCardsRevealed()
        {
            return m_board.AllCardsRevealed();
        }

        public eInputError CheckMoveValidation(int i_row, int i_col)
        {
            if (!(i_row >= 0 && i_col >= 0 && i_row < m_board.Rows && i_col < m_board.Columns))
            {
                return eInputError.OutOfBounds;
            }
            if (m_board.IsRevealed(i_row, i_col))
            {
                return eInputError.CardAlreadyRevealed;
            }
            return eInputError.NoError;
        }

        public bool CheckMatch(int i_row1, int i_col1, int i_row2, int i_col2)
        {
            return m_board.GetCards()[i_row1, i_col1].Value.Equals(m_board.GetCards()[i_row2, i_col2].Value);
        }

        public void SwitchPlayer()
        {
            m_currentPlayerIndex = (m_currentPlayerIndex + 1) % m_players.Count;
        }

        public Player DetermineWinner()
        {
            return m_players.OrderByDescending(p => p.Score).FirstOrDefault();
        }

        public (int, int) GetComputerRandomMove()
        {
            int row, col;
            do
            {
                row = random.Next(m_board.Rows);
                col = random.Next(m_board.Columns);
            } while (m_board.IsRevealed(row, col));

            return (row, col);
        }

        public (int, int) GetComputerAIMove()
        {
            if (m_SavedCardsForMatch[0] != null)
            {
                Card firstCard = m_SavedCardsForMatch[0];
                m_SavedCardsForMatch[0] = null;
                return (firstCard.Row, firstCard.Column);
            }

            if (m_SavedCardsForMatch[1] != null)
            {
                Card secondCard = m_SavedCardsForMatch[1];
                m_SavedCardsForMatch[1] = null;
                return (secondCard.Row, secondCard.Column);
            }

            (int row, int col) firstMove = GetComputerRandomMove();
            char firstValue = m_board.GetCards()[firstMove.row, firstMove.col].Value;

            RememberCard(firstMove.row, firstMove.col, firstValue);

            return firstMove;
        }

        public void RememberCard(int i_row, int i_col, char i_value)
        {
            Card card = new Card(i_row, i_col, i_value);
            if (!rememberedCards.ContainsKey(i_value))
            {
                rememberedCards[i_value] = new List<Card>();
            }
            else
            {
                rememberedCards[i_value].RemoveAll(c => c.Row == i_row && c.Column == i_col);
            }
            rememberedCards[i_value].Add(card);

            if (rememberedCards[i_value].Count == 2)
            {
                m_SavedCardsForMatch[0] = rememberedCards[i_value][0];
                m_SavedCardsForMatch[1] = rememberedCards[i_value][1];
            }
        }

        public void ClearMatchedCards(char i_value)
        {          
            if (rememberedCards.ContainsKey(i_value))
            {
                rememberedCards[i_value].Clear();
                rememberedCards.Remove(i_value);
                m_SavedCardsForMatch[0] = null;
                m_SavedCardsForMatch[1] = null;
            }
        }

        private void UpdateGameState()
        {
            Player winner = DetermineWinner();
            if (winner != null && m_players.Count(p => p.Score == winner.Score) > 1)
            {
                gameState = GameState.Draw;
            }
            else if (winner != null)
            {
                gameState = GameState.Win;
            }
            else
            {
                gameState = GameState.Draw;
            }
        }

        public void QuitGame()
        {
            gameState = GameState.Quit;
        }
    }
}
