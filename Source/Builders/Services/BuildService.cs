using Jenkins.API.Interfaces;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Jenkins.API.Builders.Services
{
    /// <summary>
    /// Trieda obsahuje metódy, ktoré vytvárajú Jenkins build a čakajú na jeho vykonanie.
    /// </summary>
    public class BuildService
    {
        private readonly IBuildAdapter buildAdapter;

        /// <summary>
        /// Inicializácia novej inštancie <see cref="BuildService"/> triedy.
        /// </summary>
        /// <param name="buildAdapter">Build služba.</param>
        /// <exception cref="System.ArgumentNullException">Adresa Jenkins url nemože byť prázdna.</exception>
        /// alebo
        /// Názov projektu nemôže byť null.
        /// alebo
        /// Názov zostavy nemôže byť null.
        public BuildService(IBuildAdapter buildAdapter)
        {
            if (buildAdapter == null)
            {
                throw new ArgumentNullException("The ArgumentNullException was not throwed in case of BuildService is equal to null.");
            }
            this.buildAdapter = buildAdapter;
        }

        /// <summary>
        /// Spustí zostavu Jenkins a čaká na jeho vykonanie.
        /// </summary>
        /// <param name="tfsBuildNum">TFS build.</param>
        /// <returns>Výsledok zostavenia.</returns>
        public string Run(string tfsBuildNum)
        {
            //Ak nie je zadané číslo tfs Build, ktoré prešlo WF aktivitou, priradíme Guid, aby bol názov jedinečný.
            if (string.IsNullOrEmpty(tfsBuildNum))
            {
                tfsBuildNum = Guid.NewGuid().ToString();
            }

            string nextBuildNum = this.GetNextBuildNum();

            this.TriggerBuild(tfsBuildNum, nextBuildNum);
            this.WaitUntilBuildStarts(nextBuildNum);

            string realBuildNumber = this.GetRealBuildNumber(tfsBuildNum);

            this.buildAdapter.InitializeSpecificBuildUrl(realBuildNumber);
            this.WaitUntilBuildFinish(realBuildNumber);

            string buildResult = this.GetBuildStatus(realBuildNumber);

            return buildResult;
        }

        /// <summary>
        /// Získate stav stavania z adresy URL. Uzol XML.
        /// </summary>
        /// <param name="realBuildNumber">Skutočné číslo zostavenia.</param>
        /// <returns>Build status.</returns>
        internal string GetBuildStatus(string realBuildNumber)
        {
            string buildStatus = this.buildAdapter.GetSpecificBuildStatusXml(realBuildNumber);
            string buildResult = this.buildAdapter.GetBuildResult(buildStatus);
            Debug.WriteLine("Result from the build: {0}", buildResult);

            return buildResult;
        }

        internal void WaitUntilBuildFinish(string realBuildNumber)
        {
            bool shouldContinue = false;
            string buildStatus = string.Empty;
            do
            {
                buildStatus = this.buildAdapter.GetSpecificBuildStatusXml(realBuildNumber);
                bool isProjectBuilding = this.buildAdapter.IsProjectBuilding(buildStatus);
                if (!isProjectBuilding)
                {
                    shouldContinue = true;
                }
                Debug.WriteLine("Waits 5 seconds before the new check if the build is completed...");
                Thread.Sleep(5000);
            }
            while (!shouldContinue);
        }

        internal string GetRealBuildNumber(string tfsBuildNumber)
        {
            string buildStatus = this.buildAdapter.GetBuildStatusXml();
            string nextBuildNumber = this.buildAdapter.GetQueueBuildNumber(buildStatus, tfsBuildNumber).ToString();

            return nextBuildNumber;
        }

        /// <summary>
        /// Čaká, kým sa začne stavať -> Not throwing webexception 404 Not Found. Nie je nájdené pre špecifickú URL zostavenia Jenkins.
        /// </summary>
        /// <param name="nextBuildNumber">Ďalšie build num.</param>
        internal void WaitUntilBuildStarts(string nextBuildNumber)
        {
            int retryCount = 30;
            bool isBuildtriggered = false;
            string buildStatus = string.Empty;
            do
            {
                if (!isBuildtriggered && retryCount == 0)
                {
                    throw new Exception("The build didn't start in 30 seconds.");
                }
                try
                {
                    buildStatus = this.buildAdapter.GetSpecificBuildStatusXml(nextBuildNumber);
                    Debug.WriteLine(buildStatus);
                    isBuildtriggered = true;
                }
                catch (WebException ex)
                {
                    if (ex.Message.Equals("The remote server returned an error: (404) Not Found."))
                    {
                        retryCount--;
                        Thread.Sleep(1000);
                        Debug.WriteLine("wait 1 second until the build is triggered...");
                    }
                }
            }
            while (!isBuildtriggered || retryCount == 0);
        }

        /// <summary>
        /// Triggers the build.
        /// </summary>
        /// <param name="tfsBuildNumber">TFS build.</param>
        /// <param name="nextBuildNumber">Ďalšie build num.</param>
        /// <returns>Odpoveď.</returns>
        /// <exception cref="System.Exception">Ďalšia zostava s rovnakým číslom zostavy je už spustená.</exception>
        internal string TriggerBuild(string tfsBuildNumber, string nextBuildNumber)
        {
            string buildStatus = string.Empty;
            bool isAlreadyBuildTriggered = false;
            try
            {
                buildStatus = this.buildAdapter.GetSpecificBuildStatusXml(nextBuildNumber);
                Debug.WriteLine(buildStatus);
            }
            catch (WebException ex)
            {
                if (!ex.Message.Equals("The remote server returned an error: (404) Not Found."))
                {
                    isAlreadyBuildTriggered = true;
                }
            }
            if (isAlreadyBuildTriggered)
            {
                throw new Exception("Another build with the same build number is already triggered.");
            }
            string response = this.buildAdapter.TriggerBuild(tfsBuildNumber);

            return response;
        }

        /// <summary>
        ///  Dostávame ďalšie BuildNumber z výsledku Jenkins Build Status XML.
        /// </summary>
        /// <returns>Ďalšie build číslo.</returns>
        internal string GetNextBuildNum()
        {
            string buildStatus = this.buildAdapter.GetBuildStatusXml();
            string nextBuildNumber = this.buildAdapter.GetNextBuildNum(buildStatus);

            return nextBuildNumber;
        }
    }
}
