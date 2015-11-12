using System;
using Amazon.Runtime;

namespace NLog.Targets.SNS
{
    public interface IAwsCredentialResolver
    {
        AWSCredentials ResolveFor(string credentialTypeString);
    }

    public class AwsCredentialResolver: IAwsCredentialResolver
    {
        public AWSCredentials ResolveFor(string credentialTypeString)
        {
            Type credentialType = null;

            if (!string.IsNullOrWhiteSpace(credentialTypeString))
                credentialType = Type.GetType(credentialTypeString);

            if (credentialType == null)
                credentialType = typeof (BasicAWSCredentials);

            return (AWSCredentials) Activator.CreateInstance(credentialType);
        }
    }
}