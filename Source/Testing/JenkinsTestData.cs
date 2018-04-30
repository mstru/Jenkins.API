using System.IO;
using System.Reflection;

namespace Jenkins.API.Testing
{
    public class JenkinsTestData
    {
        public static string SpecificBuildXml;
        public const string JenkinsUrl = "";
        public const string ProjectName = "";

        /// <summary>
        /// Inicializácia <see cref="JenkinsTestData"/> triedy.
        /// </summary>
        static JenkinsTestData()
        {
            string currentExecutionFolder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
            string filePath = Path.Combine(currentExecutionFolder, "Testing", "XmlTestFiles", "SpecificBuildXml.xml");
            SpecificBuildXml = File.ReadAllText(filePath);
            filePath = Path.Combine(currentExecutionFolder, "Testing", "XmlTestFiles", "BuildStatusSingleBuildNodeXml.xml");
            BuildStatusSingleBuildNodeXml = File.ReadAllText(filePath);
            filePath = Path.Combine(currentExecutionFolder, "Testing", "XmlTestFiles", "BuildStatusXml.xml");
            BuildStatusXml = File.ReadAllText(filePath);
        }

        /// <summary>
        /// Získanie alebo nastavenie XML zostavy.
        /// </summary>
        /// <value>
        /// Stav zostavy.
        /// </value>
        public static string BuildStatusSingleBuildNodeXml { get; set; }

        /// <summary>
        /// Získanie alebo nastavenie XML zostavy.
        /// </summary>
        /// <value>
        /// Stav zostavy.
        /// </value>
        public static string BuildStatusXml { get; set; }
    }
}
