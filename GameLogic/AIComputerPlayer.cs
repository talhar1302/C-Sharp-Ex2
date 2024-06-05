﻿using System.Collections.Generic;

namespace GameLogic
{
    public class AIComputerPlayer<T> : ComputerPlayer<T>
    {
        private List<Card<T>> exposedCards;
        private Card<T> m_savedCardForMatch;
        private bool m_isCardSaved = false;
        public AIComputerPlayer(string name) : base(name)
        {
            exposedCards = new List<Card<T>>();
        }

        public override (int, int) GetMove(Board<T> board)
        {
            int row, column;
            int returnedRow, returnedColumn;
            // Check for a known pair
            for (int i = 0; i < exposedCards.Count; i++)
            {
                for (int j = i + 1; j < exposedCards.Count; j++)
                {
                    row = exposedCards[i].Row;
                    column = exposedCards[i].Column;
                    if (exposedCards[i].Value.Equals(exposedCards[j].Value))
                    {
                        if (!m_isCardSaved && !board.IsRevealed(row, column))
                        {
                            m_savedCardForMatch = exposedCards[i];
                            returnedRow = m_savedCardForMatch.Row;
                            returnedColumn = m_savedCardForMatch.Column;
                            m_isCardSaved = true;
                            return (returnedRow, returnedColumn);
                        }
                        else
                        {
                            row = exposedCards[j].Row;
                            column = exposedCards[j].Column;
                            if (!board.IsRevealed(row, column))
                            {
                                m_savedCardForMatch = exposedCards[j];
                                returnedRow = m_savedCardForMatch.Row;
                                returnedColumn = m_savedCardForMatch.Column;
                                m_isCardSaved = false;
                                exposedCards.Remove(exposedCards[j]);
                                exposedCards.Remove(exposedCards[i]);
                                return (returnedRow, returnedColumn);
                            }
                        }
                    }
                }
            }

            // No known pairs, choose a random move  
            do
            {
                row = random.Next(board.Rows);
                column = random.Next(board.Columns);
            } while (board.IsRevealed(row, column));
            RememberCard(row, column, board.GetCards()[row, column].Value);
            return (row, column);
        }

        public void RememberCard(int row, int col, T value)
        {
            exposedCards.Add(new Card<T>(row, col, value));
        }
    }
}
