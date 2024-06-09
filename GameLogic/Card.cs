using System;

namespace GameLogic
{
    public class Card
    {
        private readonly int r_Row;
        private readonly int r_Column;
        private readonly char r_Value;
        private bool m_IsRevealed;   
        public int Row { get => r_Row; }
        public int Column { get => r_Column; }
        public char Value { get => r_Value; }
        public bool IsRevealed { get => m_IsRevealed; set => m_IsRevealed = value; }

        public Card(int i_Row, int i_Col, char i_Value)
        {
            r_Row = i_Row;
            r_Column = i_Col;
            r_Value = i_Value;
            IsRevealed = false;
        }
    }
}
