using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json.Linq;
using System;

namespace Raven.Database.Data
{
	/// <summary>
	/// Extensions for handling metadata
	/// </summary>
    public static class MetadataExtensions
    {
    	private static readonly HashSet<string> HeadersToIgnoreServerDocument =
    		new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    		{
				"Non-Authoritive-Information",
				"Content-Type"
    		};

        private static readonly HashSet<string> HeadersToIgnoreClient = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			// Entity headers - those are NOT ignored
			/*
            "Allow",
            "Content-Disposition",
            "Content-Encoding",
            "Content-Language",
            "Content-Location",
            "Content-MD5",
            "Content-Range",
            "Content-Type",
            "Expires",
            
             */
			// ignoring this header, we handle this internally
			"Last-Modified",
			// Ignoring this header, since it may
			// very well change due to things like encoding,
			// adding metadata, etc
			"Content-Length",
			// Special things to ignore
			"Keep-Alive",
			"X-Requested-With",
			// Request headers
			"Accept-Charset",
			"Accept-Encoding",
			"Accept",
			"Accept-Language",
			"Authorization",
			"Cookie",
			"Expect",
			"From",
			"Host",
			"If-Match",
			"If-Modified-Since",
			"If-None-Match",
			"If-Range",
			"If-Unmodified-Since",
			"Max-Forwards",
			"Referer",
			"TE",
			"User-Agent",
			//Response headers
			"Accept-Ranges",
			"Age",
			"Allow",
			"ETag",
			"Location",
			"Retry-After",
			"Server",
			"Set-Cookie2",
			"Set-Cookie",
			"Vary",
			"Www-Authenticate",
			// General
			"Cache-Control",
			"Connection",
			"Date",
			"Pragma",
			"Trailer",
			"Transfer-Encoding",
			"Upgrade",
			"Via",
			"Warning",
		};

		/// <summary>
		/// Filters the headers from unwanted headers
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="isServerDocument">if set to <c>true</c> [is server document].</param>
		/// <returns></returns>
        public static JObject FilterHeaders(this NameValueCollection self, bool isServerDocument)
        {
            var metadata = new JObject();
            foreach (var header in self.AllKeys)
            {
                if (HeadersToIgnoreClient.Contains(header))
                    continue;
				if(isServerDocument && HeadersToIgnoreServerDocument.Contains(header))
					continue;
                var values = self.GetValues(header);
                if (values.Length == 1)
                    metadata.Add(header, GetValue(values[0]));
                else
                    metadata.Add(header, new JArray(values.Select(GetValue)));
            }
            return metadata;
        }

	    private static JToken GetValue(string val)
	    {
            if (val.StartsWith("{"))
                return JObject.Parse(val);
            if (val.StartsWith("["))
                return JArray.Parse(val);
	        return new JValue(val);
	    }
    }
}
