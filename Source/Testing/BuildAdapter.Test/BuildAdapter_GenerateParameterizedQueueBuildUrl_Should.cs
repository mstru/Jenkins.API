using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jenkins.API.Infrastructure;
using System;

namespace Jenkins.API.Testing.BuildAdapter.Test
{
    public class BuildAdapter_GenerateParameterizedQueueBuildUrl_Should
    {
        [TestMethod]
        public void GenerateParameterizedQueueBuildUrl()
        {
            var buildAdapter = new Jenkins.API.Builders.Services.BuildAdapter(new HttpAdapter(), JenkinsTestData.JenkinsUrl, JenkinsTestData.ProjectName);
            string resutedUrl = buildAdapter.GenerateParamQueueBuidlUrl(JenkinsTestData.JenkinsUrl, JenkinsTestData.ProjectName);

            Assert.AreEqual<string>(
                @"...job...",
                resutedUrl,
                "The generated Parameterized Queue Build Url was not correct.");
        }

        [TestMethod]
        [ExpectedException(exceptionType: typeof(ArgumentException), noExceptionMessage: "The Argument exception was not throwed in case of incorrect ParameterizedQueueBuildUrl")]
        public void ThrowArgumentException_WhenIncorrectUrlIsBuilt()
        {
            string jenkunsServerUrl = " &^s";
            string projectName = "#";
            var buildAdapter = new Jenkins.API.Builders.Services.BuildAdapter(new HttpAdapter());

            buildAdapter.GenerateParamQueueBuidlUrl(jenkunsServerUrl, projectName);
        }
    }
}
