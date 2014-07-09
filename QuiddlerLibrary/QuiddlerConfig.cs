/**	
* @file			QuiddlerConfig.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-27
* @version		1.0
* @revisions	
* @desc			A class structure for loading external game configurations 
 *              from an XML document to be used within the game service
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace QuiddlerLibrary
{
    public interface IQuiddlerConfig
    {
        int UserCount { get; }
        int RoundCount { get; }
        int GameLimit { get; }
        int TurnLimit { get; }
    }

    public class QuiddlerConfig : IQuiddlerConfig
    {
        // Member variables and accessor methods
        private int _numOfUsers = 0;
        private int _numOfRounds = 0;
        private int _gameLimit = 0;
        private int _turnLimit = 0;

        public int UserCount { 
            get { return _numOfUsers; } 
            private set { _numOfUsers = value; } 
        }

        public int RoundCount
        {
            get { return _numOfRounds; }
            private set { _numOfRounds = value; }
        }

        public int GameLimit
        {
            get { return _gameLimit; }
            private set { _gameLimit = value; }
        }

        public int TurnLimit
        {
            get { return _turnLimit; }
            private set { _turnLimit = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName"></param>
        public QuiddlerConfig(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    // Initialize the document
                    XmlDocument _doc = new XmlDocument();

                    // Create an XmlReader
                    XmlReader reader = new XmlTextReader(fileName);

                    // Load the document
                    _doc.Load(reader);

                    //Close the reader
                    reader.Close();

                    // Retrieve the node list
                    XmlNodeList nodes = _doc.GetElementsByTagName("quiddler_config");

                    // Each element in the node list
                    foreach (XmlElement elem in nodes)
                    {
                        _numOfUsers = Convert.ToInt32(elem.GetElementsByTagName("user_count")[0].InnerXml);
                        _numOfRounds = Convert.ToInt32(elem.GetElementsByTagName("round_count")[0].InnerXml);
                        _gameLimit = Convert.ToInt32(elem.GetElementsByTagName("game_limit")[0].InnerXml);
                        _turnLimit = Convert.ToInt32(elem.GetElementsByTagName("turn_limit")[0].InnerXml);
                    }
                }
                else
                {
                    throw new FileNotFoundException("Configuration File Not Found");
                }
            }
            catch (InvalidCastException ex)
            {
                throw ex;    
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
