/**	
* @file			Login.xaml.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-23
* @version		1.0
* @revisions	
* @desc			A client implementation for the Quiddler login interface, communicates with the QuiddlerService through the WPF based QuiddlerServer
 *              Routes callback requests to the QuiddlerClient window
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

namespace QuiddlerClient
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class Login : Window, ICallback
    {
        QuiddlerUI widQuiddler = null;
        private static string ipCheck = "^([0-9]{1,3})\\.([0-9]{1,3})\\.([0-9]{1,3})\\.([0-9]{1,3})$";
        private DuplexChannelFactory<IQuiddlerService> remoteFactory;
        private IQuiddlerService remoteProxy = null;
        private delegate void UpdateProgBarDelegate(DependencyProperty dp, Object value);

        /// <summary>
        /// Animates the login window's progress bar
        /// </summary>
        /// <param name="theProgBarDelegate"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="val"></param>
        private void AnimateProgressBar(UpdateProgBarDelegate theProgBarDelegate, int max, int min, int val)
        {
            try
            {
                // setup progress bar starting properties
                progressBar1.Maximum = max;
                progressBar1.Minimum = min;
                progressBar1.Value = val;

                // update the progress bar
                double counter = 0;
                for (int i = 0; i <= 2000; ++i)
                {
                    // synchronous delegate
                    // using the windows object dispatcher property
                    this.Dispatcher.Invoke(theProgBarDelegate,
                        System.Windows.Threading.DispatcherPriority.Background,
                        new object[] { ProgressBar.ValueProperty, counter++ });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Re-configures an existing endpoint definition
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="port"></param>
        private void ConfigureEndpoint(string addr, string port)
        {
            try
            {
                // Configure the Endpoint details
                remoteFactory = new DuplexChannelFactory<IQuiddlerService>(this, "QuiddlerConfig");
                remoteFactory.Endpoint.Address = new EndpointAddress(
                    "net.tcp://"
                    + (addr.Equals("") ? "localhost" : addr)
                    + ":"
                    + (port.Equals("") ? "8007" : port)
                    + "/QuiddlerLibrary/QuiddlerService"
                );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Callback implementation to invoke the game window
        /// </summary>
        /// <param name="CBinfo"></param>
        public void UpdateGui(CallbackInfoDC CBinfo)
        {
            try {
                widQuiddler.UpdateGui(CBinfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Update's the login status message
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

        private void btnJoinLocal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtUserName.Text != "")
                {
                    // Clear previous status
                    UpdateStatus("");

                    // Configure the Endpoint details
                    remoteFactory = new DuplexChannelFactory<IQuiddlerService>(this, "QuiddlerConfig");

                    // Indicate waiting condition
                    UpdateStatus("Connecting to " + remoteFactory.Endpoint.Address.ToString() + "...");

                    // Create an instance of my UpdateProgressarDelegate
                    UpdateProgBarDelegate theProgBarDelegate = new UpdateProgBarDelegate(progressBar1.SetValue);

                    // Activate a remote QuiddlerService object
                    remoteProxy = remoteFactory.CreateChannel();

                    // Animate progress bar
                    AnimateProgressBar(theProgBarDelegate, 2000, 0, 0);

                    // Username already taken
                    if (remoteProxy.usernameExists(txtUserName.Text))
                    {
                        UpdateStatus("That username is already taken");
                    }
                    else
                    {
                        // Create a new client instance
                        Tuple<int, string> result = remoteProxy.Register_client();
                        if (result.Item1 != -1)
                        {
                            widQuiddler = new QuiddlerUI(remoteProxy, result.Item1);
                            widQuiddler.join(txtUserName.Text);
                            widQuiddler.lblWelcome2.Content = txtUserName.Text;
                            widQuiddler.Show();
                            this.Close();
                        }
                        else
                        {
                            UpdateStatus(result.Item2);
                        }
                    }

                    // Reset progress bar
                    progressBar1.Value = 0;
                }
                else
                {
                    UpdateStatus("Please enter a valid username");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus(ex.Message);
            }
        }

        private void btnJoinNetworked_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Clear previous status
                UpdateStatus("");

                if (txtServAddr.Text != "")
                {
                    // Valid IP Address
                    if (Regex.IsMatch(txtServAddr.Text, ipCheck))
                    {
                        if (txtUserName.Text != "")
                        {
                            // Configure server endpoint
                            ConfigureEndpoint(txtServAddr.Text, txtServPort.Text);

                            // Indicate waiting condition
                            UpdateStatus("Connecting to " + remoteFactory.Endpoint.Address.ToString() + "...");

                            // Create an instance of my UpdateProgressarDelegate
                            UpdateProgBarDelegate theProgBarDelegate = new UpdateProgBarDelegate(progressBar1.SetValue);

                            // Activate a remote QuiddlerService object
                            remoteProxy = remoteFactory.CreateChannel();

                            // Animate progress bar
                            AnimateProgressBar(theProgBarDelegate, 2000, 0, 0);

                            // Username already taken
                            if (remoteProxy.usernameExists(txtUserName.Text))
                            {
                                UpdateStatus("That username is already taken");
                            }
                            else
                            {
                                // Create a new client instance
                                Tuple<int, string> result = remoteProxy.Register_client();
                                if (result.Item1 != -1)
                                {
                                    widQuiddler = new QuiddlerUI(remoteProxy, result.Item1);
                                    widQuiddler.join(txtUserName.Text);
                                    widQuiddler.lblWelcome2.Content = txtUserName.Text;
                                    widQuiddler.Show();
                                    this.Close();
                                }
                                else
                                {
                                    UpdateStatus(result.Item2);
                                }
                            }

                            // Reset progress bar
                            progressBar1.Value = 0;
                        }
                        else
                        {
                            UpdateStatus("Please enter a valid username");
                        }
                    }
                    else
                    {
                        UpdateStatus("Invalid address, please try again");
                    }
                }
                else
                {
                    UpdateStatus("Please enter a valid IP address");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus(ex.Message);
            }
        }

    }
}
