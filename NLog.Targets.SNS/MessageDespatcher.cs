using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace NLog.Targets.SNS
{
    public interface IMessageDespatcher
    {
        Task<PublishResponse> DespatchAsync(string message);
    }

    public class MessageDespatcher : IMessageDespatcher
    {
        private readonly string _topicArn;
        private readonly AWSCredentials _credentials;
        private readonly string _regionEndpointString;

        public MessageDespatcher(string topicArn, AWSCredentials credentials, string regionEndpointString)
        {
            _topicArn = topicArn;
            _credentials = credentials;
            _regionEndpointString = regionEndpointString;
        }

        public async Task<PublishResponse> DespatchAsync(string message)
        {
            var regionEndpoint = RegionEndpoint.GetBySystemName(_regionEndpointString);
            using (var client = new AmazonSimpleNotificationServiceClient(_credentials, regionEndpoint))
            {
                var request = new PublishRequest(_topicArn, message);
                var response = await client.PublishAsync(request);
                return response;
            }
        }
    }
}