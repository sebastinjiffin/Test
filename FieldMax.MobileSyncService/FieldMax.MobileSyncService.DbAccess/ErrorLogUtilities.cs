// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorLogUtilities.cs" company="Experion">
//   FieldMax
// </copyright>
// <summary>
//   Summary description for ErrorLogUtilities
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FieldMax.MobileSyncService.DbAccess
{
    using System;
    using System.Configuration;
    using LogWriter;

    //using Logger;

    /// <summary>
    /// Summary description for ErrorLogUtilities
    /// </summary>
    public class ErrorLogUtilities
    {
        /// <summary>
        /// The is logging enabled.
        /// </summary>
        private static bool? isloggingEnabled;

        /// <summary>
        /// The synchronize lock
        /// </summary>
        private static readonly object SyncLock = new object();

        /// <summary>
        /// The method name.
        /// </summary>
        private string methodName;

        /// <summary>
        /// The is property
        /// </summary>
        private bool isproperty;

        /// <summary>
        /// The current logging time.
        /// </summary>
        DateTime currentLoggingTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogUtilities"/> class.
        /// </summary>
        public ErrorLogUtilities()
        {
            this.InitilizeLogStatus();
        }

        /// <summary>
        /// The initilize log status.
        /// </summary>
        private void InitilizeLogStatus()
        {
            try
            {
                if (isloggingEnabled == null)
                {
                    lock (SyncLock)
                    {
                        isloggingEnabled = ConfigurationManager.AppSettings.Get("EnableLogging").Equals("true");
                    }
                }
            }
            catch (Exception)
            {
                lock (SyncLock)
                {
                    isloggingEnabled = false;
                }
            }
        }

        /// <summary>
        /// The set log file path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string SetLogFilePath()
        {
            try
            {
                string logFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Log\ErrorLog.txt";
                int indxDot = logFilePath.LastIndexOf(@".");
                string date = string.Format(
                    "{0}-{1}-{2}",
                    DateTime.Now.Day,
                    DateTime.Now.Month.ToString(),
                    DateTime.Now.Year.ToString());
                return logFilePath = logFilePath.Insert(indxDot, date);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        /// <summary>
        /// The set log file path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string SetLogFileNewPath()
        {
            try
            {
                string logFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Log\DetailedLog.txt";
                int indxDot = logFilePath.LastIndexOf(@".");
                string date = string.Format(
                    "{0}-{1}-{2}",
                    DateTime.Now.Day,
                    DateTime.Now.Month.ToString(),
                    DateTime.Now.Year.ToString());
                return logFilePath = logFilePath.Insert(indxDot, date);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return null;
            }
        }

        /// <summary>
        /// The log message.
        /// </summary>
        /// <param name="messageSource">
        /// The message source.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        public void LogMessage(string messageSource, string message, string level)
        {
            if (isloggingEnabled == false)
            {
                return;
            }

            string[] msg = messageSource.Split(':');

            //TimeSpan timeDifference;
            //timeDifference = DateTime.Now.Subtract(this.currentLoggingTime);
            //if (timeDifference.Days > 0)
            //{
            if (msg.Length >= 4)
            {
                ErrorLog.LogFilePath = SetLogFileNewPath();
            }
            else
            {
                ErrorLog.LogFilePath = SetLogFilePath();
            }
            this.currentLoggingTime = DateTime.Now;
            //}
            string logLevel = "L3";
            ErrorLog.Level = logLevel;
            ErrorLog.ErrorRoutine(false, messageSource + ":- " + message, level);
        }
    }
}
