using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jenkins.API.Infrastructure;
using System;

namespace Jenkins.API.Testing.BuildAdapter.Test
{
    [TestClass]
    public class BuildAdapter_GenerateBuildStatusUrl_Should
    {
        [TestMethod]
        public void GenerateBuildStatusUrl()
        {
            var buildAdapter = new Jenkins.API.Builders.Services.BuildAdapter(new HttpAdapter(), JenkinsTestData.JenkinsUrl, JenkinsTestData.ProjectName);
            string resultUrl = buildAdapter.GenerateBuildStarusUrl(JenkinsTestData.JenkinsUrl, JenkinsTestData.ProjectName);

            Assert.AreEqual<string>(
                @"...api xml...",
                resultUrl,
                "The Build status Url was not created correctly.");
        }

        [TestMethod]
        [ExpectedException(exceptionType: typeof(ArgumentException), noExceptionMessage: "The Argument exception was not throwed in case of incorrect Build status Url")]
        public void ThrowArgumentException_WhenIncorrectUrlIsBuilt()
        {
            string jenkunsServerUrl = " &^s";
            string projectName = "#";
            var buildAdapter = new Jenkins.API.Builders.Services.BuildAdapter(new HttpAdapter());

            buildAdapter.GenerateBuildStarusUrl(jenkunsServerUrl, projectName);
        }
    }
}
