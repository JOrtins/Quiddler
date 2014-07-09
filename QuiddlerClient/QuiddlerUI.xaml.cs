/**	
* @file			QuiddlerUI.xaml.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-23
* @version		1.0
* @revisions	
* @desc			A client implementation for the Quiddler game service, communicates with the QuiddlerService through the WPF based QuiddlerServer
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using System.Windows.Controls.Primitives;
using QuiddlerLibrary;

namespace QuiddlerClient
{
    /// <summary>
    /// Interaction logic for QuiddlerUI.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class QuiddlerUI : Window, ICallback
    {
        // Member variable and accessor methods
        UserDC objUser = null;
        List<ToggleButton> buttons = null;
        private int clientCallbackId = 0;
        // Config the Endpoint details
        private IQuiddlerService remoteProxy = null;
        private IAudioDeck sounds = null;
        private int userCount = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="callbackID"></param>
        public QuiddlerUI(IQuiddlerService proxy, int callbackID)
        {
            InitializeComponent();
            try
            {
                // Initialize members
                buttons = new List<ToggleButton>();

                // Load the proxy and callbackID from the login window
                remoteProxy = proxy;
                clientCallbackId = callbackID;

                // Group buttons into an aggregrate object
                groupButtons();

                // Load sounds
                sounds = new AudioDeck();
                sounds.Add("user_joined", "{path_here}");
                sounds.Add("user_left", "{path_here}");
                sounds.Add("user_scored", "{path_here}");
                sounds.Add("user_quit", "{path_here}");
                sounds.Add("game_over", "{path_here}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Begin service negotations
        /// </summary>
        /// <param name="username"></param>
        public void join(string username)
        {
            try
            {
                // Create new user object
                objUser = new UserDC(username);
                objUser.ID = clientCallbackId;

                // Join the server
                remoteProxy.Join(objUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Retrieves a collection of cards from the service and populates all cards previously used
        /// </summary>
        /// <param name="initialize"></param>
        private void getCards(bool initialize)
        {
            try
            {
                // Get cards needed
                string[] cards;

                // If starting new game
                if (initialize)
                {
                    cards = remoteProxy.getCards(buttons.Count);
                }
                // Replace used cards
                else
                {
                    int cardCount = 0;

                    // Each quiddler card button
                    foreach (ToggleButton button in buttons)
                    {
                        // The card was used (or being initialized)
                        if ((button.IsChecked ?? true) || initialize)
                        {
                            cardCount++;
                        }
                    }
                    // Retrieve cards from the service
                    cards = remoteProxy.getCards(cardCount);
                }

                // Load the new cards
                int index = 0;
                foreach (ToggleButton button in buttons)
                {
                    // The card was used (or being initialized)
                    if ((button.IsChecked ?? true) || initialize)
                    {
                        button.Content = cards[index];
                        index++;
                    }
                    // Uncheck the button
                    button.IsChecked = false;
                }
            }
            catch (Exception)
            {
                // Do nothing, handled by the service's forceEndGame method
            }
        }

        /* ======================================================= */
        /* =================== HELPER METHODS ==================== */
        /* ======================================================= */

        /// <summary>
        /// Enables/Disables core game components based on the specified state
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="readyState"></param>
        public void GamePlayable(bool gameState, bool readyState)
        {
            try
            {
                btnReady.IsEnabled = readyState;
                btnOne.IsEnabled = gameState;
                btnTwo.IsEnabled = gameState;
                btnThree.IsEnabled = gameState;
                btnFour.IsEnabled = gameState;
                btnFive.IsEnabled = gameState;
                btnSix.IsEnabled = gameState;
                btnSeven.IsEnabled = gameState;
                btnEight.IsEnabled = gameState;
                btnSubmit.IsEnabled = gameState;
                btnClear.IsEnabled = gameState;

                // Hide waiting condition
                if (gameState)
                {
                    
                    lblWaiting.Visibility = Visibility.Hidden;
                }
                // Indicate waiting condition
                else
                {
                    lblWaiting.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Groups all card buttons into a collection for simplified conditional checking 
        /// </summary>
        private void groupButtons()
        {
            try
            {
                // Clear the list
                buttons.Clear();

                // Add the buttons
                buttons.Add(btnOne);
                buttons.Add(btnTwo);
                buttons.Add(btnThree);
                buttons.Add(btnFour);
                buttons.Add(btnFive);
                buttons.Add(btnSix);
                buttons.Add(btnSeven);
                buttons.Add(btnEight);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Unchecks all buttons and clears the word string
        /// </summary>
        private void reset()
        {
            try
            {
                foreach (ToggleButton button in buttons)
                {
                    if (button.IsChecked ?? true)
                    {
                        button.IsChecked = false;
                    }
                }
                txtBoxWord.Text = "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Scales a label relative to its content length
        /// </summary>
        /// <param name="label"></param>
        private void scaleText(Label label)
        {
            try
            {
                string text = label.Content.ToString();
                if (text.Length < 15)
                {
                    label.FontSize = 48.0;
                }
                else if (text.Length > 15 && text.Length < 25)
                {
                    label.FontSize = 38.0;
                }
                else if (text.Length > 25 && text.Length < 35)
                {
                    label.FontSize = 28.0;
                }
                else
                {
                    label.FontSize = 22.0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* ======================================================= */
        /* ==================== WORD METHODS ===================== */
        /* ======================================================= */

        /// <summary>
        /// Returns the string result of a sub string post concatenation
        /// </summary>
        /// <param name="full"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public string append(string full, string sub)
        {
            try
            {
                string f = full.ToLower();
                string s = sub.ToLower();
                return f + s;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Returns a list of all cards currently in use by the client
        /// </summary>
        /// <returns></returns>
        private List<string> getCardsUsed()
        {
            try
            {
                List<string> cardsUsed = new List<string>();
                // Each card button
                foreach (ToggleButton button in buttons)
                {
                    // If the button was checked
                    if (button.IsChecked ?? true)
                    {
                        cardsUsed.Add(button.Content.ToString());
                    }
                }
                return cardsUsed;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Returns the string result of a sub string removal
        /// </summary>
        /// <param name="full"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public string remove(string full, string sub)
        {
            try
            {
                string f = full.ToLower();
                string s = sub.ToLower();
                return (f.IndexOf(s) < 0) ? f : f.Remove(f.IndexOf(s), s.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// A helper method to encapsulate the append/remove string 
        /// control structure used by the card button event handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="button"></param>
        /// <param name="output"></param>
        private void updateWord(object sender, ToggleButton button, TextBox output)
        {
            try
            {
                // Add new value to word
                if ((sender as ToggleButton).IsChecked ?? true)
                {
                    output.Text = append(output.Text.ToString(), button.Content.ToString());
                }
                // Remove existing value from word
                else
                {
                    output.Text = remove(output.Text.ToString(), button.Content.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* ======================================================= */
        /* ==================== EVENT HANDLERS =================== */
        /* ======================================================= */

        /// <summary>
        /// Window Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Play until completed
                sounds.PlaySync("user_quit");

                // Unregister the user from the service
                if (clientCallbackId != 0 && remoteProxy != null)
                {
                    remoteProxy.Unregister_client(clientCallbackId);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "You're no longer connected to the server, click OK to close.",
                    "Connection Lost",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// User is ready to play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Indicate waiting condition
                lblWaiting.Visibility = Visibility.Visible;
                // Prepare user for gameplay
                remoteProxy.userReady(clientCallbackId);
                btnReady.IsEnabled = false;
                txtBoxScore.Text = "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// User submits a word for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate word submitted
                if (remoteProxy.validate(txtBoxWord.Text))
                {
                    // Update score
                    txtBoxScore.Text = Convert.ToString(
                        remoteProxy.updateScore(
                            clientCallbackId,
                            getCardsUsed())
                        );

                    // Replace used cards with new ones
                    getCards(false);

                    // Clear the word box
                    txtBoxWord.Text = "";
                    txtBoxStatus.Text = "Correct";

                    // Set game state
                    GamePlayable(false, false);

                    // Play sound
                    sounds.Play("user_scored");
                }
                // Invalid word
                else
                {
                    // Update score (ending turn only)
                    txtBoxScore.Text = Convert.ToString(
                        remoteProxy.updateScore(
                            clientCallbackId,
                            new List<string>())
                        );

                    // Reset the client
                    reset();
                    txtBoxStatus.Text = "Incorrect";
                    reset();

                    // Set game state
                    GamePlayable(false, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Resets game interface state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOne_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                updateWord(sender, btnOne, txtBoxWord);
                txtBoxStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnTwo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                updateWord(sender, btnTwo, txtBoxWord);
                txtBoxStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnThree_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                updateWord(sender, btnThree, txtBoxWord);
                txtBoxStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                updateWord(sender, btnFour, txtBoxWord);
                txtBoxStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                updateWord(sender, btnFive, txtBoxWord);
                txtBoxStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSix_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                updateWord(sender, btnSix, txtBoxWord);
                txtBoxStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSeven_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                updateWord(sender, btnSeven, txtBoxWord);
                txtBoxStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                updateWord(sender, btnEight, txtBoxWord);
                txtBoxStatus.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /* ======================================================= */
        /* =================== CALLBACK METHODS ================== */
        /* ======================================================= */

        /// <summary>
        /// Implementation of the ICallback contract
        /// </summary>
        /// <param name="info"></param>
        private delegate void ClientUpdateDelegate(CallbackInfoDC info);
        public void UpdateGui(CallbackInfoDC info)
        {
            if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                try
                {
                    // Update user count
                    txtNumUsers.Text = info.usersLst.Length.ToString();

                    // Update round
                    lblRound.Content = "Round: " + info.roundStatus;

                    // Update user list (names & scores)
                    lstPlayers.Items.Clear();
                    foreach (string user in info.usersLst)
                    {
                        lstPlayers.Items.Add(user);
                    }

                    // User joined the game
                    if (userCount < info.usersLst.Length)
                    {
                        // Update count
                        userCount = info.usersLst.Length;

                        // Play sound
                        sounds.Play("user_joined");
                    }
                    // User left the game
                    else if (userCount > info.usersLst.Length)
                    {
                        // Update count
                        userCount = info.usersLst.Length;

                        //Play sound
                        sounds.Play("user_left");
                    }

                    // Starting a new game
                    if (info.startGame)
                    {
                        lblEndGameMsg.Visibility = Visibility.Hidden;
                        txtBoxStatus.Text = "";
                        getCards(true);
                    }
                    // Ending the current game
                    else if (info.endGame)
                    {
                        lblEndGameMsg.Content = info.endGameMsg;
                        scaleText(lblEndGameMsg);
                        lblEndGameMsg.Visibility = Visibility.Visible;

                        // Play sound
                        sounds.Stop("user_scored");
                        sounds.PlaySync("game_over");
                    }

                    // Enable/Disable game controls
                    GamePlayable(info.gameState, info.readyState);

                    // Hide waiting condition (Endgame Override)
                    if (info.endGame)
                    {
                        lblWaiting.Visibility = Visibility.Hidden;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Only the main dispatcher thread can change the GUI
                this.Dispatcher.BeginInvoke(new ClientUpdateDelegate(UpdateGui), info);
            }
        }

    }
}
