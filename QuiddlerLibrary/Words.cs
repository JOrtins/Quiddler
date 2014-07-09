/**	
* @file			Words.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-23
* @version		1.0
* @revisions	
* @desc			A portable word dictionary validation system that enables .txt and .accdb file parsing and local caching on initialization
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuiddlerLibrary
{
    public enum WordSource
    {
        TextFile = 1,
        Database = 2,
    }

    public interface IWords
    {
        bool validate(string str);
    }

    public class Words : IWords
    {
        // Member variables and accessor methods
        private static string _connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=";
        private DataSet _dataSet = null;
        private List<string> _words = null;
        private WordSource _sourceType;
        private string _fileName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="file"></param>
        public Words(WordSource source, string file)
        {
            try
            {
                // Initialize members
                _sourceType = source;
                _fileName = file;
                _words = new List<string>();

                // Use specified retrieval method
                if (_sourceType == WordSource.TextFile)
                {
                    getWordsFromFile(_fileName);
                }
                else if (_sourceType == WordSource.Database)
                {
                    getWordsFromDb(_fileName);
                }
            }
            catch (FileLoadException ex)
            {
                throw ex;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;                
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* ======================================================= */
        /* =============== INTERFACEABLE METHODS ================= */
        /* ======================================================= */

        /// <summary>
        /// Validates a word against the localized dictionary
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool validate(string str)
        {
            try
            {
                return _words.Contains(str);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* ======================================================= */
        /* ================== RETRIEVAL METHODS ================== */
        /* ======================================================= */
 
        /// <summary>
        /// Retrieves words from a database dictionary and stores them locally
        /// </summary>
        /// <param name="dbName"></param>
        private void getWordsFromDb(string dbName)
        {
            try
            {
                // Create connection object
                OleDbConnection conn = new OleDbConnection(_connectionString + dbName);

                // Create data adapters
                OleDbDataAdapter adapter = new OleDbDataAdapter();

                // Pass SQL logic to the new adapter
                adapter.SelectCommand = new OleDbCommand("SELECT * FROM Words;", conn);

                // Initialize dataset
                _dataSet = new DataSet();

                // Open database connection
                conn.Open();

                // Fill the dataset
                adapter.Fill(_dataSet, "Words");

                // Close database connection
                conn.Close();

                // Clear previous words
                _words.Clear();

                // Populate words from dataset
                foreach (DataRow row in _dataSet.Tables["Words"].Rows)
                {
                    _words.Add(row.Field<string>("word"));
                }
            }
            catch (FileLoadException ex)
            {
                throw ex;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
            catch (OleDbException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves words from a textfile dictionary and stores them locally
        /// </summary>
        /// <param name="fileName"></param>
        private void getWordsFromFile(string fileName)
        {
            try
            {
                // Clear previous words
                _words.Clear();

                StreamReader reader;

                // Open the stream
                using (reader = new StreamReader(File.OpenRead(@fileName)))
                {
                    // Until end of file
                    while (!reader.EndOfStream)
                    {
                        // Add the new word
                        _words.Add(reader.ReadLine());
                    }
                }
                // Close the stream
                reader.Close();
            }
            catch (FileLoadException ex)
            {
                throw ex;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
            catch (OleDbException ex)
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
