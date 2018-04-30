namespace Jenkins.API.Interfaces
{
    /// <summary>
    /// Rozhranie obsahuje metódy, ktoré vytvárajú Jenkins build a čakajú na jeho vykonanie
    /// </summary>
    public interface IBuildAdapter
    {
        /// <summary>
        /// Získanie čísla zostavania v poradí.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <param name="queuedBuildName">Názov zostavy v poradí.</param>
        /// <returns>Následujúce čislo zostavy.</returns>
        /// <exception cref="System.Exception">Ak zostava so zadaným názvom nie je vo fronte.</exception>
        int GetQueueBuildNumber(string xmlContent, string queuedBuildName);

        /// <summary>
        /// Získanie čísla TFS zostavania.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <returns>TFS build číslo.</returns>
        /// <exception cref="System.ArgumentException">TfsBuildNumber nebol nastavený.</exception>
        string GetBuildTfsBuildNum(string xmlContent);

        /// <summary>
        /// Určuje či je [is project building] [the specified XML content].
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <returns>Je projekt stále v stave zostavenia.</returns>
        bool IsProjectBuilding(string xmlContent);

        /// <summary>
        /// Získanie build zostavania.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <returns>Výsledok zostavenia.</returns>
        string GetBuildResult(string xmlContent);


        /// <summary>
        /// Získanie ďalšieho čísla zostavania.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <returns>Číslo ďalšieho zostavenia.</returns>
        string GetNextBuildNum(string xmlContent);

        /// <summary>
        /// Získanie mena.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <returns>Meno.</returns>
        string GetUserName(string xmlContent);

        /// <summary>
        /// Zísanie build status XML.
        /// </summary>
        /// <returns>Build status XML.</returns>
        string GetBuildStatusXml();

        /// <summary>
        /// Získamie build status XML.
        /// </summary>
        /// <param name="nextBuildNumber">Následujúce build číslo.</param>
        /// <returns>Build status XML.</returns>
        string GetSpecificBuildStatusXml(string nextBuildNumber);

        /// <summary>
        /// Trigger build
        /// </summary>
        /// <param name="tfsBuildNumber">TFS build číslo</param>
        /// <returns>Odpoveď</returns>
        string TriggerBuild(string tfsBuildNumber);

        /// <summary>
        /// Inicializácia build URL.
        /// </summary>
        /// <param name="nextBuildNumber">Následujúce build číslo.</param>
        void InitializeSpecificBuildUrl(string nextBuildNumber);
    }
}
