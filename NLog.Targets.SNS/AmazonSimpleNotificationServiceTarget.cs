using System;
using NLog.Config;

namespace NLog.Targets.SNS
{
    [Target("SNS")]
    public class AmazonSimpleNotificationServiceTarget : TargetWithLayout
    {
        private readonly IAwsCredentialResolver _awsCredentialResolver;
        private IMessageDespatcher _messageDespatcher;

        public AmazonSimpleNotificationServiceTarget()
        {
            _awsCredentialResolver = new AwsCredentialResolver();
        }
        
        public string TopicArn { get; set; }
        
        public string AmazonCredentialType { get; set; }

        public string AccountNumber { get; set; }

        public string Topic { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        [RequiredParameter]
        public string RegionEndPoint { get; set; }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            var credential = _awsCredentialResolver.ResolveFor(AmazonCredentialType, AccessKey, SecretKey);
            _messageDespatcher = new MessageDespatcher(credential, RegionEndPoint);
        }

        private string GetTopicArn()
        {
            if (!string.IsNullOrEmpty(TopicArn))
                return TopicArn;

            if (string.IsNullOrEmpty(Topic))
                return null;

            if (string.IsNullOrEmpty(AccountNumber))
                return _messageDespatcher.GetTopicArnFor(Topic);

            return $"arn:aws:sns:{RegionEndPoint}:{AccountNumber}:{Topic}";
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (_messageDespatcher == null)
                throw new ArgumentNullException(nameof(_messageDespatcher));

            if (logEvent?.LoggerName?.Equals("Amazon", StringComparison.InvariantCultureIgnoreCase) ?? false)   //prevent an infinite loop
                return;

            var message = Layout.Render(logEvent);
            _messageDespatcher.DespatchAsync(GetTopicArn(), message).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_messageDespatcher != null)
                {
                    _messageDespatcher.Dispose();
                    _messageDespatcher = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}