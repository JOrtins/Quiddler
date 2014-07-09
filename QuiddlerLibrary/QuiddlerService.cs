/**	
* @file			QuiddlerService.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-23
* @version		1.0
* @revisions	
* @desc			An implementation of the Quiddler game service business logic class
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Diagnostics;
using System.Data.OleDb;
using System.IO;

namespace QuiddlerLibrary
{
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IQuiddlerService
    {
        [OperationContract(IsOneWay = true)]
        void Join(UserDC objUser);
        [OperationContract]
        Tuple<int, string> Register_client();
        [OperationContract(IsOneWay = true)]
        void Unregister_client(int id);
        [OperationContract]
        string[] getCards(int cardCount);
        [OperationContract]
        int updateScore(int userId, List<string> cardsUsed);
        [OperationContract]
        bool validate(string word);
        [OperationContract]
        void userReady(int userId);
        [OperationContract]
        bool usernameExists(string name);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class QuiddlerService : IQuiddlerService, IDisposable
    {
        // Member variables and accessor methods
        private Dictionary<int, UserDC> users = null;
        private Dictionary<int, ICallback> clients = null;
        private IWords words = null;
        private IDeck deck = null;
        private ILogger logger = null;
        private IQuiddlerConfig settings = null;
        private int clientId = 1;
        private int roundCount = 0;
        private bool gameInProgress = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public QuiddlerService()
        {
            try
            {
                // Create a StreamWriter object for logging events
                logger = new Logger("Quiddler_Service.log", LoggingMode.Debug);

                // Load configuration settings
                settings = new QuiddlerConfig("Resources/gameConfig.xml");

                // Initialize members
                users = new Dictionary<int, UserDC>();
                clients = new Dictionary<int, ICallback>();
                deck = new Deck();
                words = new Words(WordSource.TextFile, "Resources/Data/Words.txt");

                // Log success
                logger.logInfo("Service created");
            }
            catch (FileLoadException ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
            catch (FileNotFoundException ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
            catch (OleDbException ex)
            {
                logger.logError(ex.Message);
                throw ex;

            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Destructor (Finalize) method
        /// </summary>
        ~QuiddlerService()
        {
            Dispose(false);
        }

        /* ======================================================= */
        /* =================== FACTORY METHODS =================== */
        /* ======================================================= */
  
        /// <summary>
        /// Returns a Quiddler card from the top of the deck as a string of letter
        /// </summary>
        /// <returns></returns>
        private string getCard()
        {
            try
            {
                return deck.getCard();
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Generates a list of player names and scores
        /// </summary>
        /// <returns></returns>
        private string[] getPlayerList()
        {
            try
            {
                List<string> players = new List<string>();
                foreach (UserDC user in users.Values)
                {
                    players.Add(user.Name.ToString() + " : " + user.Score.ToString());
                }
                logger.logInfo("Currently " + players.Count + " players are in the game");
                return players.ToArray<string>();
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Generates a round status string for the client callback
        /// </summary>
        /// <param name="endGame"></param>
        /// <returns></returns>
        private string getRoundStatus(bool endGame)
        {
            try
            {
                string status = "";
                if (endGame)
                {
                    status = settings.RoundCount + "/" + settings.RoundCount;
                }
                else
                {
                    status = roundCount + "/" + settings.RoundCount;
                }
                logger.logInfo("Round: " + status);
                return status;
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }
    
        /// <summary>
        /// Returns a string of the winner's name and score for the current game
        /// </summary>
        /// <param name="endGame"></param>
        /// <param name="noMoreCards"></param>
        /// <returns></returns>
        private string getWinner(bool endGame, bool noMoreCards)
        {
            try
            {
                // Game has ended
                if (endGame || noMoreCards)
                {
                    UserDC highestUser = new UserDC("highestUser");

                    // Each user in the game
                    foreach (var user in users.Values)
                    {
                        // New highest score
                        if (highestUser.Score < user.Score)
                        {
                            highestUser = user;
                        }
                    }
                    string result = "";
                    // No score attained
                    if (highestUser.Score == 0)
                    {
                        result = "Nobody wins, please play again";
                    }
                    else if (users.Count > 1)
                    {
                        // Ran out of cards
                        if (noMoreCards)
                        {
                            result = "End of deck, " + highestUser.Name + " wins with a score of " + highestUser.Score + "!";
                        }
                        // Normal condition
                        else
                        {
                            result = "The winner is " + highestUser.Name + " with a score of " + highestUser.Score + "!";
                        }
                    }
                    else
                    {
                        // Ran out of cards
                        if (noMoreCards)
                        {
                            result = "End of deck, you scored " + highestUser.Score;
                        }
                        // Normal condition
                        else
                        {
                            result = "You scored " + highestUser.Score;
                        }
                        // User got a good score
                        if (highestUser.Score > 40)
                        {
                            result += ", Good Job!";
                        }
                    }
                    logger.logInfo("Game Over: \"" + result + "\"");
                    return result;
                }
                // Game continues
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /* ======================================================= */
        /* ================== CALLBACK METHODS =================== */
        /* ======================================================= */

        /// <summary>
        /// Forces a gameover state when the deck runs out of cards
        /// </summary>
        private void forceEndGame()
        {
            try
            {
                logger.logInfo("An end game condition has been forced");

                // Create the callback object
                CallbackInfoDC info = new CallbackInfoDC(getPlayerList(), false, true, false, true, getWinner(false, true), getRoundStatus(true));

                // Update all clients
                foreach (var callback in clients.Values)
                {
                    callback.UpdateGui(info);
                }
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }
    
        /// <summary>
        /// Update all connected clients with the current game's state
        /// </summary>
        /// <param name="startGame"></param>
        /// <param name="endGame"></param>
        private void updateAllClients(bool startGame, bool endGame)
        {
            try
            {
                // Dynamically changing state variables
                bool gameState = false;
                bool readyState = false;
                bool waiting = false;

                // Starting a new game
                if (startGame)
                {
                    logger.logInfo("Starting the game");

                    // Indicate game is running
                    gameInProgress = true;

                    // Clear/Increment round count
                    roundCount = 1;

                    // Reset user values
                    foreach (UserDC user in users.Values)
                    {
                        user.Score = 0;
                        user.isReady = false;
                        user.turnEnded = false;
                    }

                    // Enable all users to play
                    gameState = true;
                    readyState = false;
                }

                // Starting a new round
                else if (readyForNewRound())
                {
                    logger.logInfo("Beginning a new round");

                    // Reset user's turn
                    foreach (UserDC user in users.Values)
                    {
                        user.turnEnded = false;
                    }

                    // Enable all users to play
                    gameState = true;
                    readyState = false;

                    // Increment the rount count
                    roundCount++;

                    // Re-evaluate end game status
                    endGame = !(roundCount <= settings.RoundCount);
                }
                // Waiting to start a new game
                else if (!gameInProgress)
                {
                    readyState = true;
                }
                // Ending the game
                if (endGame)
                {
                    logger.logInfo("Ending the game");

                    // Indicate game has ended
                    gameInProgress = false;

                    // Disable all users from playing
                    gameState = false;
                    readyState = true;
                }
                // Waiting for other players
                else
                {
                    waiting = true;
                }

                // Process statistics
                string[] players = getPlayerList();
                string winner = getWinner(endGame, false);

                // Waiting for other users
                if (waiting)
                {
                    // Update all clients (user specific callback)
                    foreach (var cb in clients)
                    {
                        cb.Value.UpdateGui(new CallbackInfoDC(players, startGame, endGame, !(users[cb.Key].turnEnded), readyState, winner, getRoundStatus(endGame)));
                    }
                }
                // All users ready
                else
                {
                    // Create the callback object
                    CallbackInfoDC info = new CallbackInfoDC(players, startGame, endGame, gameState, readyState, winner, getRoundStatus(endGame));

                    // Update all clients
                    foreach (var callback in clients.Values)
                    {
                        callback.UpdateGui(info);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }
 

        /* ======================================================= */
        /* ================= PREDICATE FUNCTIONS ================= */
        /* ======================================================= */

        /// <summary>
        /// Determines if all users have finished their turns
        /// </summary>
        /// <returns></returns>
        private bool readyForNewRound()
        {
            try
            {
                // Each user in the current game
                foreach (UserDC user in users.Values)
                {
                    // Turn has not ended yet
                    if (!user.turnEnded)
                    {
                        logger.logDebug("Still waiting on " + user.Name + " to finish their turn");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Determines whether all users are ready to play
        /// </summary>
        /// <returns></returns>
        private bool readyToPlay()
        {
            try
            {
                // Each user in the game
                foreach (var user in users.Values)
                {
                    // User is not ready
                    if (!user.isReady)
                    {
                        logger.logDebug("Still waiting for " + user.Name + " to be ready");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /* ======================================================= */
        /* ==================== PROXY METHODS ==================== */
        /* ======================================================= */

        /// <summary>
        /// Returns a collection of cards requested by the user
        /// </summary>
        /// <param name="cardCount"></param>
        /// <returns></returns>
        public string[] getCards(int cardCount)
        {
            try
            {
                logger.logInfo("Client request for " + cardCount + " cards");

                // Create an array of size card count
                string[] cards = new string[cardCount];

                // Draw a new card until the count specified
                for (int i = 0; i < cardCount; i++)
                {
                    cards[i] = deck.getCard();
                    logger.logDebug("Drew an '" + cards[i] + "' card from the deck");
                }

                // Return cards to the user
                return cards;
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                forceEndGame();
                return new string[0];
            }
        }

        /// <summary>
        /// Adds a new user data contract to the user list and updates all active clients
        /// </summary>
        /// <param name="objUser"></param>
        public void Join(UserDC objUser)
        {
            try
            {
                // Add the new user object
                users.Add(objUser.ID, objUser);

                logger.logInfo(objUser.Name + " joined the game");

                // Update all existing clients
                updateAllClients(false, false);
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Register a new client to the game
        /// </summary>
        /// <returns></returns>
        public Tuple<int, string> Register_client()
        {
            try
            {
                // Game remains joinable
                if (clients.Count < settings.UserCount && !gameInProgress)
                {
                    // Store a callback for the current calling client
                    ICallback callback = OperationContext.Current.GetCallbackChannel<ICallback>();
                    clients.Add(clientId, callback);
                    logger.logInfo("New client with ID#" + clientId + " was added to the game");
                    return new Tuple<int, string>(clientId++, "Success");
                }
                // Game is in progress
                else if (gameInProgress)
                {
                    logger.logInfo("Login attempt made while the game was in progress");
                    return new Tuple<int, string>(-1, "The game is currently in progress");
                }
                // Game at max player limit
                else
                {
                    logger.logInfo("Login attempt made while the game was full");
                    return new Tuple<int, string>(-1, "The game is already full");
                }
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Unregister a client from the game and update all remaining clients
        /// </summary>
        /// <param name="id"></param>
        public void Unregister_client(int id)
        {
            try
            {
                logger.logInfo(users[id].Name + " is leaving the game");

                clients.Remove(id);
                users.Remove(id);

                // All user's left the game
                if (users.Count == 0)
                {
                    // Reset game state
                    gameInProgress = false;
                    roundCount = 0;
                }

                updateAllClients(false, false);
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }
   
        /// <summary>
        /// Updates the current user's game score and turn status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cardsUsed"></param>
        /// <returns></returns>
        public int updateScore(int userId, List<string> cardsUsed)
        {
            try
            {
                // Cards were used (validation check)
                if (cardsUsed.Count > 0)
                {
                    logger.logInfo(users[userId].Name + " completed a word");

                    // Update user's score
                    users[userId].Score = users[userId].Score + deck.getScore(cardsUsed);

                    logger.logInfo(users[userId].Name + "'s score is now " + users[userId].Score);
                }

                // End user's turn
                users[userId].turnEnded = true;

                // Update all clients
                updateAllClients(false, false);

                return users[userId].Score;
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Update specific user whether he is ready or not and
        /// then loop over the list of users to see if all of them are ready
        /// </summary>
        public void userReady(int userId)
        {
            try
            {
                // Update user's ready status
                users[userId].isReady = true;

                if (readyToPlay())
                {
                    // Prepare a new deck
                    logger.logInfo("Generating a new Quiddler deck");
                    deck.populate();
                    deck.shuffle();

                    // Update all users
                    updateAllClients(true, false);
                }
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Verifies whether a specific username already exists within the game
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool usernameExists(string name)
        {
            try
            {
                // Each user in the game
                foreach (UserDC user in users.Values)
                {
                    // A user has the specified name
                    if (user.Name.Equals(name))
                    {
                        logger.logInfo("Login attempt made with duplicate username");
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Validates the user's word against the Words class' internal dictionary
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool validate(string word)
        {
            try
            {
                return words.validate(word);
            }
            catch (Exception ex)
            {
                logger.logError(ex.Message);
                return false;
            }
        }

        /* ======================================================= */
        /* ==================== FINALIZATION ===================== */
        /* ======================================================= */
   
        /// <summary>
        /// Implementation of the IDisposable interface
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
 
        /// <summary>
        /// Calls the logger's dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (logger != null)
                {
                    logger.Dispose();
                }
            }
        }

    }
}
