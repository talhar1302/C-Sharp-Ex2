using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic
{
    public class Game
    {
        private Random random = new Random();
        private Board board;
        private bool m_ExistAIPlayer = false;
        private List<Player> players;
        private Dictionary<char, List<Card>> rememberedCards;
        private Card[] m_SavedCardsForMatch;
        private int currentPlayerIndex;

        public Game(int rows, int columns, List<Player> playerNames)
        {
            board = new Board(rows, columns);
            players = new List<Player>();

            foreach (var player in playerNames)
            {
                if (player.PlayerType == ePlayerType.AI)
                    m_ExistAIPlayer = true;
                players.Add(player);
            }
            rememberedCards = new Dictionary<char, List<Card>>();
            m_SavedCardsForMatch = new Card[2];
            currentPlayerIndex = 0;
        }

        public Board GetBoard()
        {
            return board;
        }

        public Player GetCurrentPlayer()
        {
            return players[currentPlayerIndex];
        }

        public void RevealCard(int row, int col)
        {
            char value = board.RevealCard(row, col);
            if(m_ExistAIPlayer == true) 
            {
            RememberCard(row, col, value);
            }
        }

        public void HideCards(int row1, int col1, int row2, int col2)
        {
            board.HideCard(row1, col1);
            board.HideCard(row2, col2);
        }

        public bool AllCardsRevealed()
        {
            return board.AllCardsRevealed();
        }

        public eInputError CheckMoveValidation(int row, int col)
        {
            eInputError error = eInputError.NoError;
            if (!(row >= 0 && col >= 0 && row <= board.Rows - 1 && col <= board.Columns - 1))
            {
                error = eInputError.OutOfBounds;
            }
            if (board.IsRevealed(row, col))
            {
                error = eInputError.CardAlreadyRevealed;
            }
            return error;
        }

        public bool CheckMatch(int row1, int col1, int row2, int col2)
        {
            return board.GetCards()[row1, col1].Value.Equals(board.GetCards()[row2, col2].Value);
        }

        public void SwitchPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        }

        public Player DetermineWinner()
        {
            return players.OrderByDescending(p => p.Score).FirstOrDefault();
        }

        public (int, int) GetComputerRandomMove()
        {
            int row, col;
            do
            {
                row = random.Next(board.Rows);
                col = random.Next(board.Columns);
            } while (board.IsRevealed(row, col));

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
            char firstValue = board.GetCards()[firstMove.row, firstMove.col].Value;

            RememberCard(firstMove.row, firstMove.col, firstValue);

            return firstMove;
        }

        public void RememberCard(int row, int col, char value)
        {
            Card card = new Card(row, col, value);
            if (!rememberedCards.ContainsKey(value))
            {
                rememberedCards[value] = new List<Card>();
            }
            else
            {
                rememberedCards[value].RemoveAll(c => c.Row == row && c.Column == col);
            }
            rememberedCards[value].Add(card);

            if (rememberedCards[value].Count == 2)
            {
                m_SavedCardsForMatch[0] = rememberedCards[value][0];
                m_SavedCardsForMatch[1] = rememberedCards[value][1];
            }
        }

        public void ClearMatchedCards(char value)
        {
            if(m_ExistAIPlayer == false)
            {
                return;
            }
            if (rememberedCards.ContainsKey(value))
            {
                rememberedCards[value].Clear();
                rememberedCards.Remove(value);
                m_SavedCardsForMatch[0] = null;
                m_SavedCardsForMatch[1] = null;
            }
        }
    }
}
