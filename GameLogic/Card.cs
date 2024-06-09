using System;

namespace GameLogic
{
    public class Card
    {

        private int m_Row;
        private int m_Column;
        private char m_Value;
        private bool m_IsRevealed;
        
        

        public char Value { get => m_Value; set => m_Value =value; }
        public bool IsRevealed { get => m_IsRevealed; set => m_IsRevealed=value; }
        public int Row { get => m_Row; set => m_Row = value; }
        public int Column { get => m_Column; set => m_Column = value; }

        public Card(int I_Row, int i_Col,char i_Value)
        {
            Row = I_Row;
            Column = i_Col;
            Value = i_Value;
            IsRevealed = false;
        }
    }
}
