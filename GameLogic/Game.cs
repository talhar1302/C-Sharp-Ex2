using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class Game
    {
        private Random random = new Random();
        private Board m_Board;
        private bool m_ExistAIPlayer = false;
        private List<Player> m_Players;
        private Dictionary<char, List<Card>> m_RememberedCards;
        private Card[] m_SavedCardsForMatch;
        private int m_CurrentPlayerIndex;
        private (int row, int col) m_SavedFirstMoveLocation;
        private (int row, int col) m_SavedSecondMoveLocation;
        private GameState gameState;

        public Game(int i_Rows, int i_Columns, List<Player> i_PlayerNames)
        {
            m_Board = new Board(i_Rows, i_Columns);
            m_Players = new List<Player>();

            foreach (var player in i_PlayerNames)
            {
                if (player.PlayerType == ePlayerType.AI)
                    m_ExistAIPlayer = true;
                m_Players.Add(player);
            }
            m_RememberedCards = new Dictionary<char, List<Card>>();
            m_SavedCardsForMatch = new Card[2];
            m_CurrentPlayerIndex = 0;
            gameState = GameState.Playing;
        }

        public Board GetBoard()
        {
            return m_Board;
        }

        public Player GetCurrentPlayer()
        {
            return m_Players[m_CurrentPlayerIndex];
        }

        public eGameState GetGameState()
        {
            return gameState;
        }

        public void MakeMove(int i_Row, int i_Col, int i_NumOfCalling)
        {
            RevealCard(i_Row, i_Col);
            if (i_NumOfCalling == 1)
            {
                m_SavedFirstMoveLocation = (i_Row, i_Col);
            }
            if (i_NumOfCalling == 2)
            {
                m_SavedSecondMoveLocation = (i_Row, i_Col);
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
        public void RevealCard(int i_Row, int i_Col)
        {
            char value = m_Board.RevealCard(i_Row, i_Col);
            if (m_ExistAIPlayer)
            {
                RememberCard(i_Row, i_Col, value);
            }
        }

        public void HideCards(int i_Row1, int i_Col1, int i_Row2, int i_Col2)
        {
            m_Board.HideCard(i_Row1, i_Col1);
            m_Board.HideCard(i_Row2, i_Col2);
        }

        public bool AllCardsRevealed()
        {
            return m_Board.AllCardsRevealed();
        }

        public eInputError CheckMoveValidation(int i_Row, int i_Col)
        {
            if (!(i_Row >= 0 && i_Col >= 0 && i_Row < m_Board.Rows && i_Col < m_Board.Columns))
            {
                return eInputError.OutOfBounds;
            }
            if (m_Board.IsRevealed(i_Row, i_Col))
            {
                return eInputError.CardAlreadyRevealed;
            }
            return eInputError.NoError;
        }

        public bool CheckMatch(int i_Row1, int i_Col1, int i_Row2, int i_Col2)
        {
            return m_Board.GetCards()[i_Row1, i_Col1].Value.Equals(m_Board.GetCards()[i_Row2, i_Col2].Value);
        }

        public void SwitchPlayer()
        {
            m_CurrentPlayerIndex = (m_CurrentPlayerIndex + 1) % m_Players.Count;
        }

        public Player DetermineWinner()
        {
            return m_Players.OrderByDescending(p => p.Score).FirstOrDefault();
        }

        public (int, int) GetComputerRandomMove()
        {
            int row, col;
            do
            {
                row = random.Next(m_Board.Rows);
                col = random.Next(m_Board.Columns);
            } while (m_Board.IsRevealed(row, col));

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
            char firstValue = m_Board.GetCards()[firstMove.row, firstMove.col].Value;

            RememberCard(firstMove.row, firstMove.col, firstValue);

            return firstMove;
        }

        public void RememberCard(int i_Row, int i_Col, char i_Value)
        {
            Card card = new Card(i_Row, i_Col, i_Value);
            if (!m_RememberedCards.ContainsKey(i_Value))
            {
                m_RememberedCards[i_Value] = new List<Card>();
            }
            else
            {
                m_RememberedCards[i_Value].RemoveAll(c => c.Row == i_Row && c.Column == i_Col);
            }
            m_RememberedCards[i_Value].Add(card);

            if (m_RememberedCards[i_Value].Count == 2)
            {
                m_SavedCardsForMatch[0] = m_RememberedCards[i_Value][0];
                m_SavedCardsForMatch[1] = m_RememberedCards[i_Value][1];
            }
        }

        public void ClearMatchedCards(char i_Value)
        {          
            if (m_RememberedCards.ContainsKey(i_Value))
            {
                m_RememberedCards[i_Value].Clear();
                m_RememberedCards.Remove(i_Value);
                m_SavedCardsForMatch[0] = null;
                m_SavedCardsForMatch[1] = null;
            }
        }

        private void UpdateGameState()
        {
            Player winner = DetermineWinner();
            if (winner != null && m_Players.Count(p => p.Score == winner.Score) > 1)
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
