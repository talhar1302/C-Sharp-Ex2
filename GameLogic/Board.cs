using System;
using System.Collections.Generic;

namespace GameLogic
{
    public class Board
    {
        private Card[,] cards;
        private List<char> m_CardValues;
        private int rows;
        private int columns;

        public int Rows { get => rows; set => rows = value; }
        public int Columns { get => columns; set => columns = value; }

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
            for (int i = 0; i<(rows* columns) / 2; i++)
            {
                m_CardValues.Add(value);
                m_CardValues.Add(value);
                value++;
            }
        }
        private void InitializeBoard()
        {
            cards = new Card[rows, columns];
            Random random = new Random();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    int index = random.Next(m_CardValues.Count);
                    cards[row, col] = new Card(row, col, m_CardValues[index]);
                    m_CardValues.RemoveAt(index);
                }
            }
        }

        public bool IsRevealed(int row, int col)
        {
            return cards[row, col].IsRevealed;
        }

        public char RevealCard(int row, int col)
        {
            cards[row, col].IsRevealed = true;
            return cards[row, col].Value;
        }

        public void HideCard(int row, int col)
        {
            cards[row, col].IsRevealed = false;
        }

        public bool AllCardsRevealed()
        {
            foreach (var card in cards)
            {
                if (!card.IsRevealed) return false;
            }
            return true;
        }

        public Card[,] GetCards()
        {
            return cards;
        }
    }
}
