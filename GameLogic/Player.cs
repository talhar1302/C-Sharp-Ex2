using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class Player
    {
        private readonly string r_Name;
        private int m_Score;
        private readonly ePlayerType r_PlayerType;
        public string Name { get => r_Name; }
        public int Score { get=> m_Score; set => m_Score=value; }
        public ePlayerType PlayerType { get => r_PlayerType; }
        public Player(string i_Name, ePlayerType i_PlayerType)
        {
            r_Name = i_Name;
            Score = 0;
            r_PlayerType = i_PlayerType;      
        }
        public void IncreaseScore()
        {
            Score++;
        }
    }
}
