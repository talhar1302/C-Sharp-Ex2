using System;
using System.Collections.Generic;

namespace GameLogic
{
    public class Board
    {
        private const char k_FirstCardValue = 'A';
        private readonly Card[,] r_Cards;
        private readonly List<char> r_CardValues;
        private List<char> m_TempCardValuesForInitializeBoard;
        private readonly int r_Rows;
        private readonly int r_Columns;

        public static char FirstCardValue { get => k_FirstCardValue; }
        public int Rows { get => r_Rows; }
        public int Columns { get => r_Columns; }
        public List<char> CardValues { get => r_CardValues; }
        public Board(int i_Rows, int i_Columns)
        {
            r_Rows = i_Rows;
            r_Columns = i_Columns;
            r_Cards = new Card[r_Rows, r_Columns];
            r_CardValues = new List<char>();
            InitializeCards();
            InitializeBoard();
        }

        public static bool IsValidBoard(int i_Rows, int i_Columns)
        {
            return (i_Rows * i_Columns) % 2 == 0;
        }

        private void InitializeCards()
        {
            m_TempCardValuesForInitializeBoard = new List<char>();
            char value = k_FirstCardValue;

            for (int i = 0; i < (r_Rows * r_Columns) / 2; i++)
            {
                r_CardValues.Add(value);
                m_TempCardValuesForInitializeBoard.Add(value);
                m_TempCardValuesForInitializeBoard.Add(value);
                value++;
            }
        }
        private void InitializeBoard()
        {
            Random random = new Random();
            for (int row = 0; row < r_Rows; row++)
            {
                for (int col = 0; col < r_Columns; col++)
                {
                    int index = random.Next(m_TempCardValuesForInitializeBoard.Count);
                    r_Cards[row, col] = new Card(row, col, m_TempCardValuesForInitializeBoard[index]);
                    m_TempCardValuesForInitializeBoard.RemoveAt(index);
                }
            }
        }

        public bool IsRevealed(int i_Row, int i_Col)
        {
            return r_Cards[i_Row, i_Col].IsRevealed;
        }

        public char RevealCard(int i_Row, int i_Col)
        {
            r_Cards[i_Row, i_Col].IsRevealed = true;
            return r_Cards[i_Row, i_Col].Value;
        }

        public void HideCard(int i_Row, int i_Col)
        {
            r_Cards[i_Row, i_Col].IsRevealed = false;
        }

        public bool AllCardsRevealed()
        {
            foreach (Card card in r_Cards)
            {
                if (!card.IsRevealed) return false;
            }
            return true;
        }

        public Card[,] GetCards()
        {
            return r_Cards;
        }
    }
}
