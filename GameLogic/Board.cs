using System;
using System.Collections.Generic;

namespace GameLogic
{
    public class Board
    {
        private const char r_FirstCardValue = 'A';
        private Card[,] m_Cards;
        private List<char> m_CardValues;
        private int m_Rows;
        private int m_Columns;

        public int Rows { get => m_Rows; set => m_Rows = value; }
        public int Columns { get => m_Columns; set => m_Columns = value; }

        public static char FirstCardValue { get => r_FirstCardValue; }

        public Board(int i_Rows, int i_Columns)
        {
            Rows = i_Rows;
            Columns = i_Columns;
            InitializeCards();
            InitializeBoard();
        }

        public static bool IsValidBoard(int i_Rows, int i_Columns)
        {
            return (i_Rows * i_Columns) % 2 == 0;
        }

        private void InitializeCards()
        {
            m_CardValues = new List<char>();
            char value = R_FirstCardValue;
            for (int i = 0; i<(m_Rows* m_Columns) / 2; i++)
            {
                m_CardValues.Add(value);
                m_CardValues.Add(value);
                value++;
            }
        }
        private void InitializeBoard()
        {
            m_Cards = new Card[m_Rows, m_Columns];
            Random random = new Random();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    int index = random.Next(m_CardValues.Count);
                    m_Cards[row, col] = new Card(row, col, m_CardValues[index]);
                    m_CardValues.RemoveAt(index);
                }
            }
        }

        public bool IsRevealed(int i_Row, int i_Col)
        {
            return m_Cards[i_Row, i_Col].IsRevealed;
        }

        public char RevealCard(int i_Row, int i_Col)
        {
            m_Cards[i_Row, i_Col].IsRevealed = true;
            return m_Cards[i_Row, i_Col].Value;
        }

        public void HideCard(int i_Row, int i_Col)
        {
            m_Cards[i_Row, i_Col].IsRevealed = false;
        }

        public bool AllCardsRevealed()
        {
            foreach (var card in m_Cards)
            {
                if (!card.IsRevealed) return false;
            }
            return true;
        }

        public Card[,] GetCards()
        {
            return m_Cards;
        }
    }
}
