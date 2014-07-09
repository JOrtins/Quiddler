/**	
* @file			CallbackInfoDC.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-23
* @version		1.0
* @revisions	
* @desc			A wrapper for data members passed through the client callback process
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace QuiddlerLibrary
{
    [DataContract]
    public class CallbackInfoDC
    {
        [DataMember]
        public string[] usersLst { get; private set; }

        [DataMember]
        public bool startGame { get; private set; }

        [DataMember]
        public bool endGame { get; private set; }

        [DataMember]
        public bool gameState { get; private set; }

        [DataMember]
        public bool readyState { get; private set; }

        [DataMember] 
        public string endGameMsg { get; private set; }

        [DataMember]
        public string roundStatus { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_userLst"></param>
        /// <param name="_startGame"></param>
        /// <param name="_endGame"></param>
        /// <param name="_gameState"></param>
        /// <param name="_readyState"></param>
        /// <param name="_endGameMsg"></param>
        /// <param name="_roundStatus"></param>
        public CallbackInfoDC(string[] _userLst,bool _startGame,bool _endGame, bool _gameState, bool _readyState, string _endGameMsg, string _roundStatus)
        {
            this.usersLst = _userLst;
            this.startGame = _startGame;
            this.endGame = _endGame;
            this.gameState = _gameState;
            this.readyState = _readyState;
            this.endGameMsg = _endGameMsg;
            this.roundStatus = _roundStatus;
        }
    }
}
