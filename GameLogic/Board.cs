using System;
using System.Collections.Generic;

namespace GameLogic
{
    public class Board
    {
        private Card[,] m_cards;
        private List<char> m_CardValues;
        private int m_rows;
        private int m_columns;

        public int Rows { get => m_rows; set => m_rows = value; }
        public int Columns { get => m_columns; set => m_columns = value; }

        public Board(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            InitializeCards();
            InitializeBoard();
        }

        public static bool IsValidBoard(int rows, int columns)
        {
            return (rows * columns) % 2 == 0;
        }

        private void InitializeCards()
        {
            m_CardValues = new List<char>();
            char value = 'A';
            for (int i = 0; i<(m_rows* m_columns) / 2; i++)
            {
                m_CardValues.Add(value);
                m_CardValues.Add(value);
                value++;
            }
        }
        private void InitializeBoard()
        {
            m_cards = new Card[m_rows, m_columns];
            Random random = new Random();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    int index = random.Next(m_CardValues.Count);
                    m_cards[row, col] = new Card(row, col, m_CardValues[index]);
                    m_CardValues.RemoveAt(index);
                }
            }
        }

        public bool IsRevealed(int row, int col)
        {
            return m_cards[row, col].IsRevealed;
        }

        public char RevealCard(int row, int col)
        {
            m_cards[row, col].IsRevealed = true;
            return m_cards[row, col].Value;
        }

        public void HideCard(int row, int col)
        {
            m_cards[row, col].IsRevealed = false;
        }

        public bool AllCardsRevealed()
        {
            foreach (var card in m_cards)
            {
                if (!card.IsRevealed) return false;
            }
            return true;
        }

        public Card[,] GetCards()
        {
            return m_cards;
        }
    }
}
