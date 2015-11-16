using System.Collections.Concurrent;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace NLog.Targets.SNS
{
    internal interface IMessageDespatcher
    {
        Task<PublishResponse> DespatchAsync(string topicArn, string message);
        string GetTopicArnFor(string topicName);
    }

    internal class MessageDespatcher : IMessageDespatcher
    {
        private readonly ConcurrentDictionary<string, string> _snsTopics =
            new ConcurrentDictionary<string, string>();

        private readonly AWSCredentials _credentials;
        private readonly RegionEndpoint _regionEndpoint;

        public MessageDespatcher(AWSCredentials credentials, string regionEndpointString)
        {
            _credentials = credentials;
            _regionEndpoint = RegionEndpoint.GetBySystemName(regionEndpointString);
        }

        public async Task<PublishResponse> DespatchAsync(string topicArn, string message)
        {
            if (string.IsNullOrEmpty(topicArn) || string.IsNullOrEmpty(message))
            {
                return new PublishResponse(); //do not proceed
            }

            using (var client = new AmazonSimpleNotificationServiceClient(_credentials, _regionEndpoint))
            {
                var request = new PublishRequest(topicArn, message);
                var response = await client.PublishAsync(request);
                return response;
            }
        }

        public string GetTopicArnFor(string topicName)
        {
            return _snsTopics.GetOrAdd(topicName, GetTopicArn);
        }

        private string GetTopicArn(string topicName)
        {
            using (var client = new AmazonSimpleNotificationServiceClient(_credentials, _regionEndpoint))
            {
                var topic = client.FindTopic(topicName);

                if (topic != null)
                    return topic.TopicArn;

                var response = client.CreateTopic(topicName);
                return response?.TopicArn;
            }
        }
    }
}