namespace Jenkins.API.Interfaces
{
    /// <summary>
    /// Vytvára a vykonáva webové requesty.
    /// </summary>
    public interface IHttpAdapter
    {
        /// <summary>
        /// Metóda Get na získanie.
        /// </summary>
        /// <param name="url">Adresa</param>
        /// <returns>Odpoveď zo servera</returns>
        string HttpGet(string url);


        /// <summary>
        /// Metoda Post na aktualizáciu dát.
        /// </summary>
        /// <param name="url">Adresa</param>
        /// <param name="postData">Post data</param>
        /// <returns>Odpoveď zo servera</returns>
        string HttpPost(string url, string postData);
    }
}
