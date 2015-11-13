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
            _messageDespatcher = new MessageDespatcher(GetTopicArn(), credential, RegionEndPoint);
        }

        private string GetTopicArn()
        {
            if (!string.IsNullOrEmpty(TopicArn))
                return TopicArn;

            return $"arn:aws:sns:{RegionEndPoint}:{AccountNumber}:{Topic}";
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (_messageDespatcher == null)
                throw new ArgumentNullException(nameof(_messageDespatcher));

            var message = Layout.Render(logEvent);
            _messageDespatcher.DespatchAsync(message).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}