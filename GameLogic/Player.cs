using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic
{
    public class Player
    {
        public string Name { get; }
        public int Score { get; set; }
        public ePlayerType PlayerType { get; set; }

        public Player(string name, ePlayerType playerType)
        {
            Name = name;
            Score = 0;
            PlayerType = playerType;      
        }

        public void IncreaseScore()
        {
            Score++;
        }

    }

}
