/**	
* @file			Deck.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-23
* @version		1.0
* @revisions	
* @desc			A class structure for the Quiddler deck, used for generating, populating and dealing cards. Includes an internal score counting method.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuiddlerLibrary
{
    public interface IDeck
    {
        string getCard();
        int getScore(List<string> cardsUsed);
        void populate();
        void shuffle();
    }

    public class Deck : IDeck
    {
        // Member variables and accessor methods
        private List<Card> _deckTemplate = null;
        private List<string> _cards = null;
        private int _cardIdx = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public Deck()
        {
            // Initialize members
            _deckTemplate = new List<Card>();
            _cards = new List<string>();

            // Load the template
            loadTemplate();

            // Populate the deck
            populate();

            // Shuffle the deck
            shuffle();
        }

        /// <summary>
        /// Returns a Quiddler card from the top of the deck as a string of letter
        /// </summary>
        /// <returns></returns>
        public string getCard()
        {
            try
            {
                // Cards remaining in the deck
                if (_cardIdx < _cards.Count - 1)
                {
                    string card = _cards[_cardIdx];
                    _cardIdx++;
                    return card;
                }
                else
                {
                    throw new IndexOutOfRangeException("No more _cards in the deck");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }   

        /// <summary>
        /// Calculates the user's score based on a list of card letters used
        /// </summary>
        /// <param name="cardsUsed"></param>
        /// <returns></returns>
        public int getScore(List<string> cardsUsed)
        {
            int score = 0;
            // Each card used
            foreach (string sCard in cardsUsed)
            {
                // Each card type
                foreach (Card cCard in _deckTemplate)
                {
                    // Card matches current template
                    if (sCard.Equals(cCard.Letter))
                    {
                        // Increment score
                        score += cCard.Value;
                        break;
                    }
                }
            }
            return score;
        }

        /// <summary>
        /// Loads the deckTemplate used in generating a Quiddler game deck
        /// </summary>
        private void loadTemplate()
        {
            try
            {
                // Clear the template
                _deckTemplate.Clear();

                // Populate the template
                _deckTemplate.Add(new Card("A", 10, 2));
                _deckTemplate.Add(new Card("B", 2, 8));
                _deckTemplate.Add(new Card("C", 2, 8));
                _deckTemplate.Add(new Card("D", 4, 5));
                _deckTemplate.Add(new Card("E", 12, 2));
                _deckTemplate.Add(new Card("F", 2, 6));
                _deckTemplate.Add(new Card("G", 4, 6));
                _deckTemplate.Add(new Card("H", 2, 7));
                _deckTemplate.Add(new Card("I", 8, 2));
                _deckTemplate.Add(new Card("J", 2, 13));
                _deckTemplate.Add(new Card("K", 2, 8));
                _deckTemplate.Add(new Card("L", 4, 3));
                _deckTemplate.Add(new Card("M", 2, 5));
                _deckTemplate.Add(new Card("N", 6, 5));
                _deckTemplate.Add(new Card("O", 8, 2));
                _deckTemplate.Add(new Card("P", 2, 6));
                _deckTemplate.Add(new Card("Q", 2, 15));
                _deckTemplate.Add(new Card("R", 6, 5));
                _deckTemplate.Add(new Card("S", 4, 3));
                _deckTemplate.Add(new Card("T", 6, 3));
                _deckTemplate.Add(new Card("U", 6, 4));
                _deckTemplate.Add(new Card("V", 2, 11));
                _deckTemplate.Add(new Card("W", 2, 10));
                _deckTemplate.Add(new Card("X", 2, 12));
                _deckTemplate.Add(new Card("Y", 4, 4));
                _deckTemplate.Add(new Card("Z", 2, 14));
                _deckTemplate.Add(new Card("CL", 2, 10));
                _deckTemplate.Add(new Card("ER", 2, 7));
                _deckTemplate.Add(new Card("IN", 2, 7));
                _deckTemplate.Add(new Card("QU", 2, 9));
                _deckTemplate.Add(new Card("TH", 2, 9));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Populates the Quiddler deck of cards using the deckTemplate
        /// </summary>
        public void populate()
        {
            try
            {
                // Remove all existing cards
                _cards.Clear();

                // Reset the card index
                _cardIdx = 0;

                // Each card type in the template
                foreach (Card card in _deckTemplate)
                {
                    // Number of cards available
                    for (int i = 0; i < card.DeckCount; i++)
                    {
                        _cards.Add(card.Letter);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Shuffles the Quiddler deck of cards using random number generation
        /// </summary>
        public void shuffle()
        {
            try
            {
                Random rand = new Random();
                string temp;

                // Each card in the deck
                for (int i = 0; i < _cards.Count; i++)
                {
                    // Choose a random index
                    int randIdx = rand.Next(_cards.Count);

                    if (randIdx != i)
                    {
                        // Swap cards
                        temp = _cards[i];
                        _cards[i] = _cards[randIdx];
                        _cards[randIdx] = temp;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
