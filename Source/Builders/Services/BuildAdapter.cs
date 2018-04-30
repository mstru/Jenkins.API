using Jenkins.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace Jenkins.API.Builders.Services
{
    /// <summary>
    /// Trieda obsahuje metódy, ktoré vytvárajú Jenkins build úlohu a čakajú na jeho vykonanie.
    /// </summary>
    public class BuildAdapter : IBuildAdapter
    {
        /// <summary>
        /// Vytvára a vykonáva webové requesty.
        /// </summary>
        public readonly IHttpAdapter httpAdapter;

        /// <summary>
        /// Parametrizovaná zostava url.
        /// </summary>
        private readonly string paramQueueBuilderUrl;

        /// <summary>
        /// Build status url.
        /// </summary>
        private readonly string buildStatusUrl;

        /// <summary>
        /// Jenkins adresa servera.
        /// </summary>
        private readonly string jenkinsUrl;

        /// <summary>
        /// Názov projektu.
        /// </summary>
        private readonly string projectName;

        /// <summary>
        /// Build status.
        /// </summary>
        private string specBuildNumStatusUrl;

        /// <summary>
        /// Inicializácia novej inštancie <see cref="BuildAdapter"/> triedy.
        /// </summary>
        /// <param name="httpAdaper">Http pomocník.</param>
        /// <param name="jenkinsUrl">Jenkins URL.</param>
        /// <param name="projectName">Názov projektu.</param>
        public BuildAdapter(IHttpAdapter httpAdaper, string jenkinsUrl, string projectName) : this(httpAdaper)
        {
            if (string.IsNullOrEmpty(jenkinsUrl))
            {
                throw new ArgumentNullException("The ArgumentNullException was not throwed in case of empty Jenkins Service URL.");
            }
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("The ArgumentNullException was not throwed in case of empty project name.");
            }
            this.jenkinsUrl = jenkinsUrl;
            this.projectName = projectName;
            this.buildStatusUrl = this.GenerateBuildStarusUrl(jenkinsUrl, projectName);
            this.paramQueueBuilderUrl = this.GenerateParamQueueBuidlUrl(jenkinsUrl, projectName);
        }

        /// <summary>
        /// Inicializácia novej inštancie <see cref="BuildAdapter"/> triedy
        /// </summary>
        /// <param name="httpAdapter">Http adaptér</param>
        /// <exception cref="ArgumentNullException">Hodený ArgumentNullException v prípade, že httpAdapter sa rovná null.</exception>
        public BuildAdapter(IHttpAdapter httpAdapter)
        {
            if (httpAdapter == null)
            {
                throw new ArgumentNullException("The ArgumentNullException was not throwed in case of httpAdapter is equal to null.");
            }
            this.httpAdapter = httpAdapter;
        }

        /// <summary>
        /// Získanie build status XML.
        /// </summary>
        /// <returns>Build status XML.</returns>
        public string GetBuildStatusXml()
        {
            string buildStatus = this.httpAdapter.HttpGet(this.buildStatusUrl);

            return buildStatus;
        }

        /// <summary>
        /// Trigger build.
        /// </summary>
        /// <param name="tfsBuildNumber">TFS build číslo</param>
        /// <returns>Odpoveď</returns>
        public string TriggerBuild(string tfsBuildNumber)
        {
            string response = this.httpAdapter.HttpPost(this.paramQueueBuilderUrl, string.Concat("TfsBuildNumber=", tfsBuildNumber));

            return response;
        }

        /// <summary>
        /// Získamie build status XML.
        /// </summary>
        /// <param name="nextBuildNumber">Následujúce build číslo.</param>
        /// <returns>Build status XML.</returns>
        public string GetSpecificBuildStatusXml(string nextBuildNumber)
        {
            this.InitializeSpecificBuildUrl(nextBuildNumber);
            string buildStatus = this.httpAdapter.HttpGet(this.specBuildNumStatusUrl);

            return buildStatus;
        }

        /// <summary>
        /// Inicializácia build URL.
        /// </summary>
        /// <param name="nextBuildNumber">Následujúce build číslo.</param>
        public void InitializeSpecificBuildUrl(string nextBuildNumber)
        {
            this.specBuildNumStatusUrl = this.GenerateSpecificBuildNumStatusUrl(nextBuildNumber, this.jenkinsUrl, this.projectName);
        }

        /// <summary>
        /// Generovanie build status URL.
        /// </summary>
        /// <param name="jenkinsUrl">Jenkins url</param>
        /// <param name="projectName">Názov projektu</param>
        /// <returns>Vygenerovaná webová adresa URL.</returns>
        /// <exception cref="ArgumentNullException">Webová adresa nebola vygenerovaná správne.</exception>
        internal string GenerateBuildStarusUrl(string jenkinsUrl, string projectName)
        {
            string url = string.Empty;
            Uri result = default(Uri);
            if (Uri.TryCreate(string.Concat(jenkinsUrl, "/job/", projectName, "/api/xml"), UriKind.Absolute, out result))
            {
                url = result.AbsoluteUri;
            }
            else
            {
                throw new ArgumentException("The Build status Url was not created correctly.");
            }
            return url;
        }

        /// <summary>
        /// Generovanie build URL parametrizovanej zostavy
        /// </summary>
        /// <param name="jenkinsUrl">Jenkins server url</param>
        /// <param name="projectName">Názov projektu</param>
        /// <returns>Vygenerovaná webová adresa url.</returns>
        /// <exception cref="ArgumentNullException">Parametrizovaná webová adresa nebola vygenerovaná správne.</exception>
        internal string GenerateParamQueueBuidlUrl(string jenkinsUrl, string projectName)
        {
            string url = string.Empty;
            Uri result = default(Uri);

            if (Uri.TryCreate(string.Concat(jenkinsUrl, "/job", projectName, "buildWithParameters"), UriKind.Absolute, out result))
            {
                url = result.AbsoluteUri;
            }
            else
            {
                throw new ArgumentException("The Parameterized Queue Build Url was not created correctly.");
            }
            return url;
        }

        /// <summary>
        /// Generovanie build status URL.
        /// </summary>
        /// <param name="buildNumber">Build číslo.</param>
        /// <param name="jenkinsUrl">Jenkins server URL.</param>
        /// <param name="projectName">Názov projektu.</param>
        /// <returns>Vygenerovaná webová adresa url.</returns>
        /// <exception cref="System.ArgumentException">Špeciálna webová adresa nebola vygenerovaná správne.</exception>
        internal string GenerateSpecificBuildNumStatusUrl(string buildNumber, string jenkinsUrl, string projectName)
        {
            string generatedUrl = string.Empty;
            Uri result = default(Uri);
            if (Uri.TryCreate(string.Concat(jenkinsUrl, "/job/", projectName, "/", buildNumber, "/api/xml"), UriKind.Absolute, out result))
            {
                generatedUrl = result.AbsoluteUri;
            }
            else
            {
                throw new ArgumentException("The Specific Build Number Url was not created correctly.");
            }

            return generatedUrl;
        }

        /// <summary>
        /// Získanie čísla zostavania v poradí.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <param name="queuedBuildName">Názov zostavy v poradí.</param>
        /// <returns>Následujúce čislo zostavy.</returns>
        /// <exception cref="System.Exception">Ak zostava so zadaným názvom nie je vo fronte.</exception>
        public int GetQueueBuildNumber(string xmlContent, string queuedBuildName)
        {
            IEnumerable<XElement> buildElements = this.GetAllElementsWithNodeName(xmlContent, "build");
            string nextBuildNumString = string.Empty;
            int nextBuildNum = -1;

            foreach (XElement currentElement in buildElements)
            {
                nextBuildNumString = currentElement.Element("number").Value;
                string currentBuildSpecifyUrl = this.GenerateSpecificBuildNumStatusUrl(nextBuildNumString, this.jenkinsUrl, this.projectName);
                string newBuildStatus = this.httpAdapter.HttpGet(currentBuildSpecifyUrl);
                string currentBuildName = this.GetBuildTfsBuildNum(newBuildStatus);
                if (queuedBuildName.Equals(currentBuildName))
                {
                    nextBuildNum = int.Parse(nextBuildNumString);
                    Debug.WriteLine("The real build number is {0}", nextBuildNum);
                    break;
                }
            }
            if (nextBuildNum == -1)
            {
                throw new Exception(string.Format("Build with name {0} was not find in the queued builds.", queuedBuildName));
            }
            return nextBuildNum;
        }


        /// <summary>
        /// Získanie čísla TFS zostavania.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <returns>TFS build číslo.</returns>
        /// <exception cref="System.ArgumentException">TfsBuildNumber nebol nastavený.</exception>
        public string GetBuildTfsBuildNum(string xmlContent)
        {
            IEnumerable<XElement> foundElements = from el in this.GetAllElementsWithNodeName(xmlContent, "parameter").Elements()
                                                  where el.Value == "TfsBuildNumber"
                                                  select el;

            if (foundElements.Count() == 0)
            {
                throw new ArgumentException("The TfsBuildNumber was not set!");
            }
            string tfsBuildNumber = foundElements.First().NodesAfterSelf().OfType<XElement>().First().Value;

            return tfsBuildNumber;
        }

        /// <summary>
        /// Určuje či je [is project building] [the specified XML content].
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <returns>Je projekt stále v stave zostavenia.</returns>
        public bool IsProjectBuilding(string xmlContent)
        {
            bool isBuilding = false;
            string isBuildingStr = this.GetXmlNodeValue(xmlContent, "building");
            isBuilding = bool.Parse(isBuildingStr);

            return isBuilding;
        }

        /// <summary>
        /// Získanie build zostavania.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <returns>Výsledok zostavenia.</returns>
        public string GetBuildResult(string xmlContent)
        {
            string buildResult = this.GetXmlNodeValue(xmlContent, "result");
            return buildResult;
        }

        /// <summary>
        /// Získanie ďalšieho čísla zostavania.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <returns>Číslo ďalšieho zostavenia.</returns>
        public string GetNextBuildNum(string xmlContent)
        {
            string nextBuildNumber = this.GetXmlNodeValue(xmlContent, "nextBuildNumber");
            return nextBuildNumber;
        }

        /// <summary>
        /// Získanie mena.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <returns>Meno.</returns>
        public string GetUserName(string xmlContent)
        {
            string userName = this.GetXmlNodeValue(xmlContent, "userName");
            return userName;
        }

        /// <summary>
        /// Získanie xml uzla.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <param name="xmlNodeName">Názov XML uzla.</param>
        /// <returns>Hodnota uzla.</returns>
        /// <exception cref="System.Exception">Vyvolá výnimku ak nie sú nájdené žiadne prvky pre zadaný uzol.</exception>
        internal string GetXmlNodeValue(string xmlContent, string xmlNodeName)
        {
            IEnumerable<XElement> foundElemenets = this.GetAllElementsWithNodeName(xmlContent, xmlNodeName);

            if (foundElemenets.Count() == 0)
            {
                throw new Exception(string.Format("No elements were found for node {0}", xmlNodeName));
            }
            string elementValue = foundElemenets.First().Value;

            return elementValue;
        }

        /// <summary>
        /// Získanie všetkých prvkov.
        /// </summary>
        /// <param name="xmlContent">Obsah XML.</param>
        /// <param name="xmlNodeName">Názov XML elementu.</param>
        /// <returns>Nájdené elementy.</returns>
        internal IEnumerable<XElement> GetAllElementsWithNodeName(string xmlContent, string xmlNodeName)
        {
            XDocument document = XDocument.Parse(xmlContent);
            XElement root = document.Root;
            IEnumerable<XElement> foundElemenets = from element in root.Descendants(xmlNodeName)
                                                   select element;

            return foundElemenets;
        }
    }
}
