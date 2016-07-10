using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Arrow.Text;
using Arrow.Execution;

namespace Arrow.Logging.Log4Net
{
	/// <summary>
	/// A version of the standard RollingFileAppender that allows filename to contain Arrow settings in the filename
	/// NOTE: $( and ) are used to enclose settings, not ${ and }. This is because log4net uses these markers itself
	/// and will try to expand them
	/// </summary>
	public class RollingFileAppender : log4net.Appender.RollingFileAppender
	{
		/// <summary>
		/// Opens the file
		/// </summary>
		/// <param name="fileName">The name of the log file</param>
		/// <param name="append">Whether to append or not</param>
		protected override void OpenFile(string fileName, bool append)
		{
			MethodCall.AllowFail(()=>
			{
				// Make sure the directory exists
				FileInfo fileInfo=new FileInfo(fileName);
				string directory=fileInfo.DirectoryName;
				if(!Directory.Exists(directory)) Directory.CreateDirectory(directory);
			});
			
			PurgeOldLogs();
			
			base.OpenFile(fileName,append);
		}
		
		private string ExpandFilename(string filename)
		{
			return TokenExpander.ExpandText(filename,"$(",")");
		}

		/// <summary>
		/// The name of the file. When set VariableExpansion will be applied to the value passed in
		/// </summary>
		public override string File
		{
			get{return base.File;}
			set{base.File=ExpandFilename(value);}
		}
		
		/// <summary>
		/// Identifies which files should be considered for purging
		/// </summary>
		public string PurgeMask{get;set;}
		
		/// <summary>
		/// How old the log file must be before it is purged.
		/// This is the number of days since the file was last written
		/// </summary>
		public int PurgeDays{get;set;}
		
		private void PurgeOldLogs()
		{
			string purgeMask=(this.PurgeMask==null ? null : ExpandFilename(this.PurgeMask));
			LogPurger.Purge(purgeMask,this.PurgeDays);
		}
	}
}
