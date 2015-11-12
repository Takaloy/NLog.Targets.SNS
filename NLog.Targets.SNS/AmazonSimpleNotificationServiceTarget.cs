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

        [RequiredParameter]
        public string TopicArn { get; set; }

        [RequiredParameter]
        public string AmazonCredentialType { get; set; }

        [RequiredParameter]
        public string RegionEndPoint { get; set; }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            var credential = _awsCredentialResolver.ResolveFor(AmazonCredentialType);
            _messageDespatcher = new MessageDespatcher(TopicArn, credential, RegionEndPoint);
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