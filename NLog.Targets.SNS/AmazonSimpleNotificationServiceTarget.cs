using System;
using System.Threading;
using System.Threading.Tasks;
using NLog.Config;

namespace NLog.Targets.SNS
{
    [Target("SNS")]
    public class AmazonSimpleNotificationServiceTarget : AsyncTaskTarget
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

        private async Task<string> GetTopicArn()
        {
            if (!string.IsNullOrEmpty(TopicArn))
                return TopicArn;

            if (string.IsNullOrEmpty(Topic))
                return null;

            if (string.IsNullOrEmpty(AccountNumber))
                return await _messageDespatcher.GetTopicArnFor(Topic).ConfigureAwait(false);

            return $"arn:aws:sns:{RegionEndPoint}:{AccountNumber}:{Topic}";
        }

        protected override async Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            if (_messageDespatcher == null)
                throw new ArgumentNullException(nameof(_messageDespatcher));

            if (logEvent?.LoggerName?.Equals("Amazon", StringComparison.InvariantCultureIgnoreCase) ?? false)   //prevent an infinite loop
                return;

            var message = Layout.Render(logEvent);
            var arn = await GetTopicArn().ConfigureAwait(false);
            _messageDespatcher.DespatchAsync(arn, message).ConfigureAwait(false).GetAwaiter().GetResult();
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