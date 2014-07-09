/**	
* @file			AudioDeck.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-28
* @version		1.0
* @revisions	
* @desc			A wrapper for a managed collection of playable sound files
 *              Interal method calls are wrapped to ensure only loaded sounds are called
 *              Allows implementing class to run successfully without sound
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace QuiddlerLibrary
{
    /// <summary>
    /// Private child class
    /// </summary>
    class Audio
    {
        // Member variables and accessor methods
        private bool _loaded;
        private SoundPlayer _sound;

        public bool Loaded
        {
            get { return _loaded; }
            set { _loaded = value; }
        }

        public SoundPlayer Sound
        {
            get { return _sound; }
            set { _sound = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Audio()
        {
            this._loaded = false;
            this._sound = new SoundPlayer();
        }
    }

    public interface IAudioDeck
    {
        void Add(string key, string fileName);
        void Loop(string key);
        void Play(string key);
        void PlaySync(string key);
        void Remove(string key);
        void Stop(string key);
    }

    public class AudioDeck : IAudioDeck
    {
        // Member variables and accessor methods
        Dictionary<string, Audio> sounds = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public AudioDeck()
        {
            try
            {
                sounds = new Dictionary<string, Audio>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Adds a sound to the collection with the key specified
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fileName"></param>
        public void Add(string key, string fileName)
        {
            try
            {
                // Create dictionary entry
                sounds.Add(key, new Audio());

                // Load sound file
                sounds[key].Sound = Load(fileName);

                // Indicate load success
                sounds[key].Loaded = true;
            }
            catch (Exception)
            {
                try
                {
                    // Indicate file error
                    sounds[key].Loaded = false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Returns a loaded SoundPlayer for the file specified
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private SoundPlayer Load(string fileName)
        {
            try
            {
                // Initialize the player
                SoundPlayer sound = new SoundPlayer();

                // Designate the file path
                sound.SoundLocation = fileName;

                // Load the file
                sound.Load();

                // Return the new sound
                return sound;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Loops a sound from the collection with the key specified
        /// </summary>
        /// <param name="key"></param>
        public void Loop(string key)
        {
            try
            {
                // Sound exists
                if (sounds.ContainsKey(key))
                {
                    // File loaded
                    if (sounds[key].Loaded)
                    {
                        sounds[key].Sound.PlayLooping();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Plays a sound from the collection with the key specified
        /// </summary>
        /// <param name="key"></param>
        public void Play(string key)
        {
            try
            {
                // Sound exists
                if (sounds.ContainsKey(key))
                {
                    // File loaded
                    if (sounds[key].Loaded)
                    {
                        sounds[key].Sound.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Plays a sound until completion from the collection with the key specified
        /// </summary>
        /// <param name="key"></param>
        public void PlaySync(string key)
        {
            try
            {
                // Sound exists
                if (sounds.ContainsKey(key))
                {
                    // File loaded
                    if (sounds[key].Loaded)
                    {
                        sounds[key].Sound.PlaySync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Removes a sound from the collection with the key specified
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            try
            {
                sounds.Remove(key);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Stops a sound from the collection with the key specified
        /// </summary>
        /// <param name="key"></param>
        public void Stop(string key)
        {
            try
            {
                // Sound exists
                if (sounds.ContainsKey(key))
                {
                    // File loaded
                    if (sounds[key].Loaded)
                    {
                        sounds[key].Sound.Stop();
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
