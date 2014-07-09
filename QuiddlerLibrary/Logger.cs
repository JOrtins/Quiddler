/**	
* @file			Logger.cs
* @author		Khalid Andari & Jeremy Ortins
* @date			2013-03-23
* @version		1.0
* @revisions	
* @desc			A portable stream writer wrapper that incorporates various line formatting styles, timestamping and levels of logging for flexibility.
*               Includes the implementation of the IDisposable interface
 *              "Inspired by the Apache log4j logging library for Java" - Jeremy Ortins
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuiddlerLibrary
{
    public enum LoggingMode
    {
        Debug = 4,
        Info = 3,
        Warning = 2,
        Error = 1,
    }

    public interface ILogger
    {
        void logDebug(string message);
        void logInfo(string message);
        void logWarning(string message);
        void logError(string message);
        void setMode(LoggingMode mode);
        void Dispose();
    }

    public class Logger : ILogger, IDisposable
    {
        // Member variables and accessor methods
        private StreamWriter _logger = null;
        private LoggingMode _loggingMode;
        private string _logFile;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="mode"></param>
        public Logger(string fileName, LoggingMode mode)
        {
            try
            {
                this._logFile = fileName;
                this._loggingMode = mode;
                this._logger = new StreamWriter(_logFile);
            }
            catch (FileLoadException ex)
            {
                throw ex;
            }
            catch (FileNotFoundException ex)
            {
                throw ex;   
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Destructor (Finalize) method
        /// </summary>
        ~Logger()
        {
            Dispose(false);
        }

        /// <summary>
        /// Changes the level of messages logged out to file
        /// </summary>
        /// <param name="mode"></param>
        public void setMode(LoggingMode mode)
        {
            try
            {
                this._loggingMode = mode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* ======================================================= */
        /* =================== STREAM METHODS ==================== */
        /* ======================================================= */

        /// <summary>
        /// Writes a debug line to the output stream file
        /// </summary>
        /// <param name="message"></param>
        public void logDebug(string message)
        {
            try
            {
                // Debug, Info, Warnings & Errors
                if (this._loggingMode == LoggingMode.Debug)
                {
                    this._logger.WriteLine(DateTime.Now + " -DEBUG- " + message);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Writes an error line to the output stream file
        /// </summary>
        /// <param name="message"></param>
        public void logError(string message)
        {
            try
            {
                // Errors only
                if (this._loggingMode >= LoggingMode.Error)
                {
                    this._logger.WriteLine(DateTime.Now + " -ERROR- " + message);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Writes an info line to the output stream file
        /// </summary>
        /// <param name="message"></param>
        public void logInfo(string message)
        {
            try
            {
                // Info, Warnings & Errors
                if (this._loggingMode >= LoggingMode.Info)
                {
                    this._logger.WriteLine(DateTime.Now + " -INFO-  " + message);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Writes a warning line to the output stream file
        /// </summary>
        /// <param name="message"></param>
        public void logWarning(string message)
        {
            try
            {
                // Warnings & Errors
                if (this._loggingMode >= LoggingMode.Warning)
                {
                    this._logger.WriteLine(DateTime.Now + " -WARN-  " + message);
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
            catch (Exception ex)
            {
                throw ex;
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
        /// Calls the StreamWriter's dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._logger != null)
                {
                    this.logInfo("Closing the logger.");
                    this._logger.Dispose();
                }
            }
        }

    }
}
