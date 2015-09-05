using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.PowerShell.Tools
{
	/// <summary>
	/// Reads binary data from a file and outputs it as a BinaryRow object
	/// 
	/// To get a traditional hex dump style output do:
	///   Read-Binary -Filename file.dat | Write-Host
	/// </summary>
	[Cmdlet("Read","Binary")]
	[OutputType(typeof(BinaryRow))]
	public class ReadBinaryCommand : PSCmdlet
	{
		public ReadBinaryCommand()
		{
			this.BlockSize=16;
		}

		[Parameter
		(
			Mandatory=true,
			HelpMessage="The file whose binary data should be read",
			ValueFromPipeline=true,
			ValueFromPipelineByPropertyName=true
		)]
		[ValidateNotNullOrWhitespace]
		public string Filename{get;set;}

		[Parameter
		(
			HelpMessage="The file whose binary data should be written"
		)]
		public int BlockSize{get;set;}

		protected override void ProcessRecord()		
		{
			if(this.BlockSize<1) throw new ArgumentException("BlockSize must be at least 1");

			using(var stream=OpenFile())
			{
				long address=0;

				byte[] buffer=new byte[this.BlockSize];

				int bytesRead=0;
				while((bytesRead=stream.Read(buffer,0,buffer.Length))!=0)
				{
					byte[] data=new byte[bytesRead];
					Array.Copy(buffer,data,bytesRead);

					var row=new BinaryRow(address,data,buffer.Length);					
					WriteObject(row);

					address+=bytesRead;
				}
			}
		}

		private Stream OpenFile()
		{
			var filename=this.GetUnresolvedProviderPathFromPSPath(this.Filename);
			return File.OpenRead(filename);
		}
	}
}
