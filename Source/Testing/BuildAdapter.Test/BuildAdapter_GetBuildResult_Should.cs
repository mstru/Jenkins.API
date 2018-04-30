using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jenkins.API.Infrastructure;

namespace Jenkins.API.Testing.BuildAdapter.Test
{
    [TestClass]
    public class BuildAdapter_GetBuildResult_Should
    {
        [TestMethod]
        public void GetBuildResult()
        {
            var buildAdapter = new Jenkins.API.Builders.Services.BuildAdapter(new HttpAdapter(), JenkinsTestData.JenkinsUrl, JenkinsTestData.ProjectName);
            string buildResult = buildAdapter.GetBuildResult(JenkinsTestData.SpecificBuildXml);

            Assert.AreEqual<string>(
                "SUCCESS",
                buildResult,
                "The returned build result was not correct.");
        }
    }
}
