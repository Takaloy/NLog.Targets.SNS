using Xunit;

namespace NLog.Targets.SNS.Tests
{
    public class AmazonSimpleNotificationServiceTargetTests
    {
        [Fact]
        public void Test()
        {
            var logger = LogManager.GetCurrentClassLogger();
             logger.Info("hello world");
        }
    }
}