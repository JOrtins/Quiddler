/**	
* @file			QuiddlerSvr.xaml.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-23
* @version		1.0
* @revisions	
* @desc			An implementation of the Quiddler game service host as a WPF application
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ServiceModel;
using QuiddlerLibrary;
using System.Text.RegularExpressions;

namespace QuiddlerServer
{
    /// <summary>
    /// Interaction logic for QuiddlerSvr.xaml
    /// </summary>
    public partial class QuiddlerSvr : Window
    {
        // Member variables        
        private ILogger logger = null;
        private Connection conn = null;
        ServiceHost servHost = null;
        private static string ipCheck = "^([0-9]{1,3})\\.([0-9]{1,3})\\.([0-9]{1,3})\\.([0-9]{1,3})$";
        IAudioDeck sounds = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public QuiddlerSvr()
        {
            InitializeComponent();
            try
            {
                // Create a logger object
                logger = new Logger("Quiddler_Server.log", LoggingMode.Debug);

                // Get public server machine address
                conn = new Connection();

                // Load server sounds
                sounds = new AudioDeck();
                sounds.Add("serverStart", "{path_here}");
                sounds.Add("serverStop", "{path_here}");
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                UpdateStatus(ex.Message);
            }
        }

        /// <summary>
        /// Re-configures an existing endpoint definition
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="port"></param>
        private void ConfigureAddress(string addr, string port)
        {
            try
            {
                foreach (var ep in servHost.Description.Endpoints)
                {
                    ep.Address = new EndpointAddress(
                        "net.tcp://"
                        + (addr.Equals("") ? conn.LocalIP : addr)
                        + ":"
                        + (port.Equals("") ? "8007" : port)
                        + "/QuiddlerLibrary/QuiddlerService"
                    );
                }
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                UpdateStatus(ex.Message);
            }
        }

        /// <summary>
        /// Post the last address in the host description
        /// </summary>
        private void publishAddress()
        {
            try
            {
                foreach (var ep in servHost.Description.Endpoints)
                {
                    UpdateAddress(ep.Address.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                UpdateStatus(ex.Message);
            }
        }

        /// <summary>
        /// Update the server's address display
        /// </summary>
        /// <param name="addr"></param>
        private void UpdateAddress(string addr)
        {
            try
            {
                lblAddress.Content = "Address: " + addr;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Update the server's status message
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateStatus(string msg)
        {
            try
            {
                lblStatus.Content = "Status: " + msg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /* ======================================================= */
        /* ==================== EVENT HANDLERS =================== */
        /* ======================================================= */

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Close the connection
                if (servHost != null && servHost.State == CommunicationState.Opened)
                {
                    // Shut down the service
                    logger.logInfo("Terminating Quiddler service...");
                    UpdateStatus("Closing the Quiddler service...");
                    servHost.Close();
                    // Termination success
                    logger.logInfo("Quiddler service terminated.");
                    UpdateStatus("Quiddler service terminated.");
                }

                // Close the logger
                if (logger != null)
                {
                    logger.Dispose();
                }
            }
            catch (Exception ex)
            {
                UpdateStatus(ex.Message);
            }
        }

        private void btnStartLocal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Endpoint Address
                servHost = new ServiceHost(typeof(QuiddlerService));

                // Clear previous status
                UpdateStatus("");

                // Start the service
                servHost.Open();
                logger.logInfo("Quiddler local service activated.");
                UpdateStatus("Quiddler local service activated.");

                // Toggle buttons
                btnStartLocal.IsEnabled = false;
                btnStartNetworked.IsEnabled = false;
                btnStop.IsEnabled = true;

                // Post address to the interface
                publishAddress();

                // Play start sound
                sounds.Play("serverStart");
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                UpdateStatus(ex.Message);
            }
        }

        private void btnStartNetworked_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Endpoint Address
                servHost = new ServiceHost(typeof(QuiddlerService));

                // Clear previous status
                UpdateStatus("");

                // User specified an address
                if (!txtAddress.Text.Equals(""))
                {

                    // Valid IP Address
                    if (Regex.IsMatch(txtAddress.Text, ipCheck))
                    {
                        // Configure using address & port
                        ConfigureAddress(txtAddress.Text, txtPortNum.Text);
                    }
                    else
                    {
                        logger.logInfo("Invalid connection attempt made using: " + txtAddress.Text);
                        UpdateStatus("Invalid address, please try again");
                    }
                }
                else
                {
                    // Configure using port only
                    ConfigureAddress("", txtPortNum.Text);
                }

                // Start the service
                servHost.Open();
                logger.logInfo("Quiddler network service activated.");
                UpdateStatus("Quiddler network service activated.");

                // Toggle buttons
                btnStartLocal.IsEnabled = false;
                btnStartNetworked.IsEnabled = false;
                btnStop.IsEnabled = true;

                // Post address to the interface
                publishAddress();

                // Play start sound
                sounds.Play("serverStart");
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                UpdateStatus(ex.Message);
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Clear previous status
                UpdateStatus("");

                // Clear current address
                UpdateAddress("");

                // Shut down the service
                logger.logInfo("Terminating Quiddler service...");
                UpdateStatus("Closing the Quiddler service...");

                servHost.Close();

                // Termination success
                logger.logInfo("Quiddler service terminated.");
                UpdateStatus("Quiddler service terminated.");

                // Toggle buttons
                btnStartLocal.IsEnabled = true;
                btnStartNetworked.IsEnabled = true;
                btnStop.IsEnabled = false;

                // Play stop sound
                sounds.Play("serverStop");
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                UpdateStatus(ex.Message);
            }
        }

    }
}
