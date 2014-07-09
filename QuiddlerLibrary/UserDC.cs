/**	
* @file			UserDC.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-23
* @version		1.0
* @revisions	
* @desc			A user data contract wrapper class containing specific player state variables
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
    public class UserDC
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool isReady { get; set; }

        [DataMember]
        public bool turnEnded { get; set; }

        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_name"></param>
        public UserDC(string _name)
        {
            Name = _name;
            isReady = false;
            turnEnded = false;
            Score = 0;
        }
    }
}
