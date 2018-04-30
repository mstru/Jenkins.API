using System.IO;
using System.Net;
using System.Text;

namespace Jenkins.API.Infrastructure
{
    /// <summary>
    /// Vytvára a vykonáva webové requesty.
    /// </summary>
    public class HttpAdapter : Jenkins.API.Interfaces.IHttpAdapter
    {
        /// <summary>
        /// Metóda Get na získanie dát.
        /// </summary>
        /// <param name="url">Adresa</param>
        /// <returns>Odpoveď zo servera.</returns>
        public string HttpGet(string url)
        {
            string resp = string.Empty;
            var request = (HttpWebRequest)WebRequest.Create(url);
            var httpResponse = (HttpWebResponse)request.GetResponse();

            Stream respStream = httpResponse.GetResponseStream();
            var reader = new StreamReader(respStream);
            resp = reader.ReadToEnd();
            respStream.Close();
            reader.Close();

            return resp;
        }

        /// <summary>
        /// Metoda Post na aktualizáciu dát.
        /// </summary>
        /// <param name="url">Adresa</param>
        /// <param name="postData">Post data</param>
        /// <returns>Odpoveď zo servera.</returns>
        public string HttpPost(string url, string postData)
        {
            WebRequest req = WebRequest.Create(url);
            req.Method = "POST";
            req.Proxy = null;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = byteArray.Length;
            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = req.GetResponse();
            dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }
    }
}
