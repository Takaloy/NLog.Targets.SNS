using System;
using Amazon.Runtime;

namespace NLog.Targets.SNS
{
    internal interface IAwsCredentialResolver
    {
        AWSCredentials ResolveFor(string credentialTypeString, string accessKey, string secretKey);
    }

    internal class AwsCredentialResolver: IAwsCredentialResolver
    {
        public AWSCredentials ResolveFor(string credentialTypeString, string accessKey, string secretKey)
        {
            Type credentialType = null;

            if (!string.IsNullOrWhiteSpace(credentialTypeString))
                credentialType = Type.GetType(credentialTypeString);

            if (credentialType == null)
                credentialType = typeof (BasicAWSCredentials);

            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey) && credentialType == typeof(BasicAWSCredentials))
                return new BasicAWSCredentials(accessKey,secretKey);

            return (AWSCredentials) Activator.CreateInstance(credentialType);
        }
    }
}