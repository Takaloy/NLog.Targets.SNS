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
        string GetTopicArnFor(string topicName);
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
            var response = await _client.PublishAsync(request);
            return response;
        }

        public string GetTopicArnFor(string topicName)
        {
            return _snsTopics.GetOrAdd(topicName, GetTopicArn);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private string GetTopicArn(string topicName)
        {
            var topic = _client.FindTopic(topicName);

            if (topic != null)
                return topic.TopicArn;

            var response = _client.CreateTopic(topicName);
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