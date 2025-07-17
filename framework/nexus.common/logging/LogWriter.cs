
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace nexus.common.helper
{
    /// <summary>
    /// Implementation of a file-based log writer.
    /// </summary>
    public class LogWriter : IDisposable
    {
        public const string defaultPath = @"c:\Nexus\Logging";
        public const string defaultName = @"Default";
        public const string defaultExt  = @"Log";

        #region Public Enumerations
        public enum LogTypes
        {
            Continuous,	// Entries are appended to a single log file upto size maxSize     <filename>.xxx.log
            Daily,      // Entries are appended to the respective <day-of-week> log file   <filename>.Day.Log
            Weekly,     // Entries are appended to the respective <week-of-year> log file  <filename>.Wxx.Log
            Monthly,    // Entries are appended to the respective <month-of-year> log file <filename>.Mth.Log
        }
        #endregion
		
        #region Private Data
        private bool        enabled     = false;                // Enables or disables logging
        private bool        logError    = false;                // Flag to indicate a logging error has occurred
        private LogTypes    logType     = LogTypes.Continuous;  // Logging method to be used when the Write method is called
        private string      filePath;                           // Location and Base name of the log file(s).
        private string      fileName;                           // Base name of the log file(s)
        private string      fileExt     = defaultExt;           // Extension of the log file(s)
        private string      logFile;                            // Actual fully qualified file reference

        private bool        timeStamp   = false;                // Enables prefixing message with a timestamp
        private DateTime    currentDT   = DateTime.Now;         // Caches the date and time to prevent problems around date/time rollover
        private bool        threadId    = false;                // Enables prefixing message with a thread identifier
        private int         maxSize     = 1024;                 // Max log file size (in K) for continuous logging
        private Object      threadSync  = new Object();         // Object used for thread locking
        #endregion
	
        #region Constructors
        public LogWriter() : this ( defaultPath, defaultName, LogTypes.Continuous )
        {
            this.enabled  = false;
        }

        public LogWriter( string strLogPath = defaultPath, string strLogName = defaultName, LogTypes format = LogTypes.Continuous )
        {
            this.FilePath = strLogPath;
            this.FileName = strLogName;
            this.logType  = format;
            this.enabled  = false;
            ChkSetup( true );
        }
        #endregion

        #region Public Properties
        /// <summary>Controls all logging activity</summary>
        public  bool        IsEnabled       { get { return ( enabled );     } set { ChkSetup( value ); } }

        /// <summary>Indicates if a logging error has occurred</summary>
        public  bool        IsError         { get { return ( logError );    } }

        /// <summary>Controls whether a timestamp is prefixed to all messages</summary>
        public  bool        IsTimeStamped   { get { return ( timeStamp );   } set { timeStamp = value; } }
        /// <summary>Controls whether a thread ID is prefixed to all messages</summary>
        public  bool        IsThreadId      { get { return ( threadId  );   } set { threadId  = value; } }
		
        public  bool        Exists          { get { return ( ChkExists() ); } }
        public  String      LogFile         { get { return ( logFile );     } }

        // Changes of these properties only take effect on an "enable" or on day rollover
        public  LogTypes    LogType         { get { return ( logType );     } set { logType   = value; } }
        public  String      FilePath        { get { return ( filePath    ); } set { filePath  = value; } }
        public  String      FileName        { get { return ( fileName    ); } set { fileName  = value; } }
        #endregion

        #region Public Methods
        public void Write( string strLogMessage )
        {
            DateTime now;
            DateTime fileCreationDate;

	        // Only log if the logger is enabled
	        if ( this.enabled )
            {
                // Do this in a thread safe manner
                lock ( this.threadSync )
	            {
                    // Cache the current date/time
                    now = DateTime.Now;
                    if ( currentDT.Date != now.Date )
                    {
                        // Check for a file name change due to date rollover
                        currentDT = now;
                        ChkFileName();
                    }
                    currentDT = now;

	                // Does the current log file exist. When Exists is called the cached date time will be refreshed
	                if ( Exists )
	                {
	                    // Append the message to the current log, if it was an old log, it has been deleted based on the logtype
	                    this.WriteAppend(strLogMessage);
	                }
                }
	        }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Appends a message to the current log file
        /// </summary>
        /// <param name="txt">The message text to be appended</param>
        private void    WriteAppend( string msg )
        {
            string hdr    = "";
            int    count  = 0;
            bool   logged = false;

            // Write the log entry
            while ( !logged )
            {
                try
                {
                    // Always append the message to the current log, if the log doesn't exist, it will be created
                    using (StreamWriter logStream = new StreamWriter( this.logFile, true, Encoding.ASCII) )
                    {
                        if ( logError ) 
                        {
                            logStream.WriteLine( "*** One or more log messages have failed to be logged" );
                            logError = false;
                        }

                        if ( this.timeStamp )
                        {
                            // Timestamping is enabled, reformat with the timestamp added to the log entry
                            hdr = "[" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + "]";
                        }

                        if ( this.threadId )
                        {
                            // ThreadId is enabled; prefix with thread identity
                            hdr += "[" + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString("D4") + "]";
                        }

                        logStream.WriteLine( hdr + " - " + msg );
                        logged = true;
                    }
                }
                catch ( IOException )
                {
                    // 
                    if ( count++ > 10 )
                    {
                        logError = true;
                        count    = 0;
                        break;
                    }
                    
                    // Try again after a small delay
                    System.Threading.Thread.Sleep(25);
                }
                catch 
                {
                    // Nothing we can do - let system continue but without logging
                    enabled  = false;
                    logError = true;
                    break;
                }
            }
        }

        private void    ChkSetup( bool enable )
        {
            if ( enable && !enabled )
            {
                // About to enable - build and validate the logFile name
                currentDT = DateTime.Now;
                ChkFileName();
            }

            this.enabled = enable;
        }

        private void    ChkFileName()
        {
            string fn = "";

            //
            // Rebuild the name in case of changes ------------------------------------------------
            //

            try
            {
                if ( String.IsNullOrEmpty(filePath) ) filePath = defaultPath;
                if ( String.IsNullOrEmpty(fileName) ) fileName = defaultName;

                fn = Path.Combine(filePath, fileName);

                switch ( this.logType )
                {
	                    case LogTypes.Continuous  : fn = Path.ChangeExtension(fn, this.fileExt);
                                                    break;

	                    case LogTypes.Daily       : fn = Path.ChangeExtension(fn, this.currentDT.ToString("ddd") + "." + this.fileExt);
                                                    break;
//??Temporary - Need WeekOfYear in DateTime
	                    case LogTypes.Weekly      : fn = Path.ChangeExtension(fn, "W" + (this.currentDT.DayOfYear/7).ToString("d2") + "." + this.fileExt);
                                                    break;

	                    case LogTypes.Monthly     : fn = Path.ChangeExtension(fn, this.currentDT.ToString("MMM") + "." + this.fileExt);
                                                    break;
					
	                    default                   : break;
		                                            //throw new SwitchCaseException(this.logTypes.ToString(), typeof(LogTypes), this.logTypes); 
                }
            }
            catch
            {
                fn = Path.Combine(defaultPath, defaultName);
                fn = Path.ChangeExtension(fn, defaultExt);
            }

            //
            // Make sure it exists and is accessible ----------------------------------------------
            //

            try
            {
                this.logFile = Path.GetFullPath( fn );

                // Get the directory from the full path.
                string path = Path.GetDirectoryName(this.logFile);

                // If the directory does not exist, create it
                if ( !Directory.Exists( path ) )
                {
                    Directory.CreateDirectory( path );
                }
                // If the file does not exist, create it
                if ( !File.Exists( logFile ) )
                {
                    File.CreateText( logFile ).Close();
                }

                // Fix file creation date of a recycled current log file
		        DateTime fileCreationDate = File.GetCreationTime(this.logFile);
		        switch ( this.LogType )
		        {
			        case LogTypes.Daily       : // Are we looking at an old log file
                                                if (fileCreationDate.Date != this.currentDT.Date)
                                                {
                                                    RecycleFile();
                                                }
                                                break;

			        case LogTypes.Weekly      : // Are we looking at an old log file
                                                if (fileCreationDate.Year != this.currentDT.Year)
                                                {
                                                    RecycleFile();
                                                }
                                                break;

			        case LogTypes.Monthly     : // Are we looking at an old log file
                                                if (fileCreationDate.Year != this.currentDT.Year)
                                                {
                                                    RecycleFile();
                                                }
                                                break;
		        }
            }
            catch
            {
                enabled  = false;
                logError = true;
            }
        }

        private bool    ChkExists  ()
        {
            // If the file does not exist, create it as may have been deleted
            if ( !File.Exists( logFile ) )
            {
                File.CreateText( logFile ).Close();
            }
	        return ( File.Exists(this.logFile) ); 
        }

        private void    RecycleFile()
        {
	         // We need to set the creation time to now as the OS reuses the filenames original creation time
	         // even if you delete it.
            File.SetCreationTime(this.logFile, this.currentDT);
            File.Delete(this.logFile);
        }
        #endregion

        #region IDisposable Implementation
        // To detect redundant calls
        private bool disposedValue = false;
        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                }

            }
            this.disposedValue = true;
        }
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
