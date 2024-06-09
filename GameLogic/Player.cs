using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class Player
    {
        private string m_Name;
        private int m_Score;
        private ePlayerType m_PlayerType;
        public string Name { get => m_Name; set => m_Name = value; }
        public int Score { get=> m_Score; set => m_Score=value; }
        public ePlayerType PlayerType { get => m_PlayerType; set => m_PlayerType = value; }

        public Player(string i_Name, ePlayerType i_PlayerType)
        {
            Name = i_Name;
            Score = 0;
            PlayerType = i_PlayerType;      
        }

        public void IncreaseScore()
        {
            Score++;
        }

    }

}
