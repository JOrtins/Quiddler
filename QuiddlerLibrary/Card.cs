/**	
* @file			Card.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-23
* @version		1.0
* @revisions	
* @desc			A class structure for a Quiddler card, used for data storage and deck generation
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuiddlerLibrary
{
    public class Card
    {
        // Member Variables
        private string _letter;
        private int _deckCount;
        private int _value;


        // Accessor Methods
        public string Letter { get { return _letter; } set { _letter = value; } }
        public int DeckCount { get { return _deckCount; } set { _deckCount = value; } }
        public int Value { get { return _value; } set { _value = value; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="l"></param>
        /// <param name="c"></param>
        /// <param name="v"></param>
        public Card(string l, int c, int v)
        {
            this._letter = l;
            this._deckCount = c;
            this._value = v;
        }
    }
}
