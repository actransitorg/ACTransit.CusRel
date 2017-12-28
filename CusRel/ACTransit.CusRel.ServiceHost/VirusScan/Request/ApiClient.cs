using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ACTransit.CusRel.ServiceHost.VirusScan.Request
{
    public class ApiClient
    {
        public class Request
        {
            public string Url;
            public string Method;
            public NameValueCollection Headers;
            public string Body;
            public Stream PayloadStream;
            public bool ResponseAsString = true;
        }

        public class File
        {
            public string Name { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public string Base64Data { get; set; }
        }

        public Response ClientRequest(Request request)
        {
            Response result;
            if (request.ResponseAsString)
                result = new StringResponse();
            else
                result = new StreamResponse();
            ServicePointManager.ServerCertificateValidationCallback = (MyCertValidationCb);

            var req = (HttpWebRequest)WebRequest.Create(request.Url);
            req.Proxy = null;
            req.Method = request.Method.ToUpper();
            req = AddHeaders(req, request.Headers);
            if (req.Method == "POST")
            {
                if (req.ContentType == null)
                    req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = request.Body?.Length ?? request.PayloadStream.Length;
                var output = new StreamWriter(req.GetRequestStream());
                output.Write((object)request.Body ?? request.PayloadStream);
                output.Close();
            }

            var res = req.GetResponse() as HttpWebResponse;
            if (res != null)
            {
                result.StatusCode = res.StatusCode;
                result.StatusDescription = res.StatusDescription;
                if (res.SupportsHeaders)
                    foreach (var name in res.Headers.AllKeys)
                    {
                        var value = res.Headers[name].Trim();
                        if (name == "Set-Cookie")
                        {
                            if (value.StartsWith("__RequestVerificationToken"))
                                result.RequestVerificationToken = value.Substring(0, value.IndexOf(";"));
                            else if (value.StartsWith("FedAuth"))
                                result.FedAuth.Add(value.Substring(0, value.IndexOf("=")), value.Substring(value.IndexOf("=") + 1));
                        }
                    }

                byte[] buffer = null;
                if (res.ContentLength < 1)
                {
                    var encoding = Encoding.ASCII;
                    using (var reader = new StreamReader(res.GetResponseStream(), encoding))
                    {
                        if (result is StreamResponse)
                        {
                            var stream = new MemoryStream();
                            reader.BaseStream.CopyTo(stream);
                            ((StreamResponse) result).Body = stream;
                        }
                        else
                            ((StringResponse) result).Body = reader.ReadToEnd();
                    }
                } else
                {
                    buffer = new byte[res.ContentLength];
                    var stream = res.GetResponseStream();
                    if (stream != null)
                        using (var reader = new BinaryReader(stream))
                            buffer = reader.ReadBytes(buffer.Length);
                }
                res.Close();
                if (buffer != null)
                {
                    if (result is StreamResponse)
                        ((StreamResponse)result).Body = new MemoryStream(buffer);
                    else
                        ((StringResponse)result).Body = StreamToString(new MemoryStream(buffer));
                }
            }
            return result;
        }

        public string StreamToString(MemoryStream stream)
        {
            if (stream == null) return null;
            var sr = new StreamReader(stream, Encoding.UTF8);
            return sr.ReadToEnd();
        }

        private const string CRLF = "\r\n";
        public static string BuildMultipartForm(string boundary, List<KeyValuePair<string, string>> fields, List<File> files)
        {
            var l = new StringBuilder();
            if (fields != null)
                foreach (var field in fields)
                {
                    l.Append("--").Append(boundary).Append(CRLF);
                    l.Append($"Content-Disposition: form-data; name=\"{field.Key}\"").Append(CRLF);
                    l.Append(CRLF);
                    l.Append(field.Value).Append(CRLF);
                }
            if (files != null)
                foreach (var file in files)
                {
                    l.Append("--").Append(boundary).Append(CRLF);
                    l.Append($"Content-Disposition: form-data; name=\"{file.Name}\"; filename=\"{file.FileName}\"").Append(CRLF);
                    l.Append($"Content-Type: {file.ContentType}").Append(CRLF);
                    l.Append($"Content-Encoding: base64").Append(CRLF);                
                    l.Append(CRLF);
                    l.Append(file.Base64Data).Append(CRLF);
                }
            l.Append("--").Append(boundary).Append("--").Append(CRLF);
            return l.ToString();
        }

        public static bool MyCertValidationCb(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
                return false;
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                var z = Zone.CreateFromUrl(((HttpWebRequest)sender).RequestUri.ToString());
                return z.SecurityZone == System.Security.SecurityZone.Intranet || z.SecurityZone == System.Security.SecurityZone.MyComputer;
            }
            return true;
        }

        public HttpWebRequest AddHeaders(HttpWebRequest request, NameValueCollection Headers)
        {
            if (Headers == null) return request;
            foreach (var name in Headers.AllKeys)
            {
                var value = Headers[name];
                switch (name.ToLower())
                {
                    case "accept":
                        request.Accept = value;
                        break;
                    case "connection":
                        if (value.ToLower() == "keep-alive")
                            request.KeepAlive = true;
                        else
                            request.Connection = value;
                        break;
                    case "content-length":
                        request.ContentLength = Convert.ToInt32(value);
                        break;
                    case "content-type":
                        request.ContentType = value;
                        break;
                    case "date":
                        request.Date = Convert.ToDateTime(value);
                        break;
                    case "expect":
                        request.Expect = value;
                        break;
                    case "host":
                        request.Host = value;
                        break;
                    case "if-modified-since":
                        request.IfModifiedSince = Convert.ToDateTime(value);
                        break;
                    case "protocol-version":
                        request.ProtocolVersion = new Version(value);
                        break;
                    case "referer":
                        request.Referer = value;
                        break;
                    case "transfer-encoding":
                        request.TransferEncoding = value;
                        break;
                    case "user-agent":
                        request.UserAgent = value;
                        break;

                    default:
                        request.Headers.Add(name, value);
                        break;
                }
            }
            return request;
        }

        public Guid Authenticate(string Url, string Username, string Password)
        {
            var result = new Guid();
            try
            {
                var postBody = $"Username={Username}&Password={Password}";
                var postPage = ClientRequest(new Request
                {
                    Url = Url,
                    Body = postBody,
                    Method = "POST",
                    Headers = new NameValueCollection { }
                });
                result = GetUserID(postPage.ToString());

            }
            catch (Exception e)
            {

            }
            return result;
        }

        public Guid GetUserID(string Body)
        {
            var re = new Regex(@"{""UserID"":""(.*?)""}");
            return new Guid(re.IsMatch(Body) ? re.Match(Body).Groups[1].Value : null);
        }
    }

    public class CookieWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; private set; }

        public CookieWebClient()
        {
            CookieContainer = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = CookieContainer;
            }
            return request;
        }
    }

    public class Response
    {
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string RequestVerificationToken { get; set; }
        public NameValueCollection FedAuth { get; set; }

        protected object body { get; set; }

        public Response()
        {
            FedAuth = new NameValueCollection();
        }

        public override string ToString()
        {
            return (string) body;
        }
    }


    public class StreamResponse : Response
    {
        public MemoryStream Body
        {
            get { return body as MemoryStream; }
            set { body = value; }
        }

        public StreamResponse()
        {
            FedAuth = new NameValueCollection();
        }
    }

    public class StringResponse : Response
    {
        public string Body
        {
            get { return body as string; }
            set { body = value; }
        }

        public StringResponse()
        {
            FedAuth = new NameValueCollection();
        }
    }



}
