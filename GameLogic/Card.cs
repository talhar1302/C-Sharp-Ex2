namespace GameLogic
{
    public class Card
    {

        int m_row;
        int m_column;

        public char Value { get; }
        public bool IsRevealed { get; set; }
        public int Row { get => m_row; set => m_row = value; }
        public int Column { get => m_column; set => m_column = value; }

        public Card(int row, int col,char value)
        {
            Row = row;
            Column = col;
            Value = value;
            IsRevealed = false;
        }
    }
}
