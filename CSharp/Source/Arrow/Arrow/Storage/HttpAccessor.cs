using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

#pragma warning disable SYSLIB0014

namespace Arrow.Storage
{
	class HttpAccessor : Accessor
	{
		public HttpAccessor(Uri uri) : base(uri)
		{
			ValidateScheme(uri,Uri.UriSchemeHttp);
		}

		public override Stream OpenRead()
		{
			// Avoid keeping the connection open by copying the results into a memory stream and returning it to the user
			WebRequest request=WebRequest.Create(this.Uri);
			request.UseDefaultCredentials=true;
			using(WebResponse response=request.GetResponse())
			using(Stream responseStream=response.GetResponseStream())
			{
				byte[] buffer=new byte[128];
				MemoryStream stream=new MemoryStream();
				
				int bytesRead=0;
				while((bytesRead=responseStream.Read(buffer,0,buffer.Length))>0)
				{
					stream.Write(buffer,0,bytesRead);
				}
				
				// We need to move back to the beginning to make it look like a fresh stream
				stream.Position=0;
				
				return stream;
			}
		}
	}
}
