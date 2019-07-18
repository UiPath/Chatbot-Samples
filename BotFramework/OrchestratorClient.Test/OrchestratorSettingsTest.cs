using System;
using Xunit;

namespace UiPath.ChatbotSamples.BotFramework.OrchestratorClient.Test
{
    public class OrchestratorSettingsTest
    {
        [Fact]
        public void OnlyTwoAuthModeAllowed()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "another mode",
                TenancyName = "Default",
                UsernameOrEmailAddress = "Test",
                Password = "Test",
                BaseUrl = "Http://localhost",
                Strategy = "JobsCount",
                JobsCount = 1,
                ProcessKeys = new ProcessKey[] { new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "test" } },
            };

            Assert.False(setting.Validate());
        }

        [Fact]
        public void UerNamePasswordRequiredForBasicAuth()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "Basic",
                BaseUrl = "Http://localhost",
                Strategy = "JobsCount",
                JobsCount = 1,
                ProcessKeys = new ProcessKey[] { new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "test" } },
            };

            Assert.False(setting.Validate());
        }

        [Fact]
        public void UerTokenRequiredForCloudAuth()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "Cloud",
                BaseUrl = "Http://localhost",
                Strategy = "JobsCount",
                JobsCount = 1,
                ProcessKeys = new ProcessKey[] { new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "test" } },
            };

            Assert.False(setting.Validate());
        }

        [Fact]
        public void UrlShouldBeValid()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "Basic",
                TenancyName = "Default",
                UsernameOrEmailAddress = "Test",
                Password = "Test",
                BaseUrl = "invalid",
                Strategy = "JobsCount",
                JobsCount = 1,
                ProcessKeys = new ProcessKey[] { new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "test" } },
            };

            Assert.False(setting.Validate());
        }

        [Fact]
        public void TwoStrategyAllowed()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "Basic",
                TenancyName = "Default",
                UsernameOrEmailAddress = "Test",
                Password = "Test",
                BaseUrl = "Http://localhost",
                Strategy = "invalid",
                ProcessKeys = new ProcessKey[] { new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "test" } },
            };

            Assert.False(setting.Validate());
        }

        [Fact]
        public void SpecificStrategyShouldHaveAtLeastOneRobot()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "Basic",
                TenancyName = "Default",
                UsernameOrEmailAddress = "Test",
                Password = "Test",
                BaseUrl = "Http://localhost",
                Strategy = "Specific",
                RobotIds = null,
            };

            Assert.False(setting.Validate());
        }

        [Fact]
        public void JobsCountStrategyShouldHavePositiveCount()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "Basic",
                TenancyName = "Default",
                UsernameOrEmailAddress = "Test",
                Password = "Test",
                BaseUrl = "Http://localhost",
                Strategy = "JobsCount",
                JobsCount = -1,
            };

            Assert.False(setting.Validate());
        }

        [Fact]
        public void ShouldHaveAtLeastOneProcess()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "Basic",
                TenancyName = "Default",
                UsernameOrEmailAddress = "Test",
                Password = "Test",
                BaseUrl = "Http://localhost",
                Strategy = "JobsCount",
                JobsCount = 1,
                ProcessKeys = null,
            };

            Assert.False(setting.Validate());
        }

        [Fact]
        public void ProcessKeyShouldBeGuid()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "Basic",
                TenancyName = "Default",
                UsernameOrEmailAddress = "Test",
                Password = "Test",
                BaseUrl = "Http://localhost",
                Strategy = "JobsCount",
                JobsCount = 1,
                ProcessKeys = new ProcessKey[] { new ProcessKey() { Key = "1234", Process = "test" } },
            };

            Assert.False(setting.Validate());
        }

        [Fact]
        public void ValidBasicAuth()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "Basic",
                TenancyName = "Default",
                UsernameOrEmailAddress = "Test",
                Password = "Test",
                BaseUrl = "Http://localhost",
                Strategy = "JobsCount",
                JobsCount = 1,
                ProcessKeys = new ProcessKey[] { new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "test" } },
            };

            Assert.True(setting.Validate());
        }

        [Fact]
        public void ValidCloudAuth()
        {
            var setting = new OrchestratorSettings()
            {
                AuthMode = "Cloud",
                RefreshToken = "randometoken",
                ServiceInstanceLogicalName = "test",
                AccountLogicalName = "test",
                BaseUrl = "Https://platform.uipath.com",
                Strategy = "Specific",
                RobotIds = new long[] { 1, 2, 3},
                ProcessKeys = new ProcessKey[] { new ProcessKey() { Key = Guid.NewGuid().ToString(), Process = "test" } },
            };

            Assert.True(setting.Validate());
        }
    }
}
