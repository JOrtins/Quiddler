/**	
* @file			Connection.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-26
* @version		1.0
* @revisions	
* @desc			A class structure for obtaining and managing the calling machine's Internet Protocol addresses
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QuiddlerLibrary
{
    public interface IConnection
    {
        string LocalIP { get; }
        string getAddress(AddressFamily family);
    }

    public class Connection : IConnection
    {
        // Member variables and accessor methods
        private IPHostEntry _host;
        private string _localIP = "?";
        private List<string> _addresses;


        public string LocalIP
        {
            get { return _localIP; }
            private set { _localIP = value; }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public Connection()
        {
            try
            {
                // Retrieve host information
                _host = Dns.GetHostEntry(Dns.GetHostName());

                // Initialize the address collection
                _addresses = new List<String>();

                // Get the machine's local host address
                _localIP = getAddress(AddressFamily.InterNetwork);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
  
        /// <summary>
        /// Returns the address of the host based on its AddressFamily
        /// </summary>
        /// <param name="family"></param>
        /// <returns>string</returns>
        public string getAddress(AddressFamily family)
        {
            try
            {
                string address = "?";
                // Each address in the collection
                foreach (IPAddress ip in _host.AddressList)
                {
                    // Address Family matches
                    if (ip.AddressFamily == family)
                    {
                        // Get Host Ethernet Adapter address only
                        address = ip.ToString();
                        break;
                    }
                }
                return address;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
