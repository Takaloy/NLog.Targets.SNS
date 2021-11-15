using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace NLog.Targets.SNS
{
    internal interface IMessageDespatcher : IDisposable
    {
        Task<PublishResponse> DespatchAsync(string topicArn, string message);
        Task<string> GetTopicArnFor(string topicName);
    }

    internal class MessageDespatcher : IMessageDespatcher
    {

        private readonly ConcurrentDictionary<string, string> _snsTopics =
            new ConcurrentDictionary<string, string>();

        private AmazonSimpleNotificationServiceClient _client;

        public MessageDespatcher(AWSCredentials credentials, string regionEndpointString)
        {
            _client = new AmazonSimpleNotificationServiceClient(credentials, RegionEndpoint.GetBySystemName(regionEndpointString));
        }

        public async Task<PublishResponse> DespatchAsync(string topicArn, string message)
        {
            if (string.IsNullOrEmpty(topicArn) || string.IsNullOrEmpty(message))
            {
                return new PublishResponse(); //do not proceed
            }

            var request = new PublishRequest(topicArn, message);
            var response = await _client.PublishAsync(request).ConfigureAwait(false);
            return response;
        }

        public async Task<string> GetTopicArnFor(string topicName)
        {
            var arn = await GetTopicArn(topicName).ConfigureAwait(false);
            return _snsTopics.GetOrAdd(topicName, arn);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private async Task<string> GetTopicArn(string topicName)
        {
            var topic = await _client.FindTopicAsync(topicName).ConfigureAwait(false);

            if (topic != null)
                return topic.TopicArn;

            var response = await _client.CreateTopicAsync(topicName).ConfigureAwait(false);
            return response?.TopicArn;
        }

        ~MessageDespatcher()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (_client == null) return;
            _client.Dispose();
            _client = null;
        }
    }
}