# NLog.Targets.SNS
NLog target for Amazon SNS 

### Example Config
```json
<target xsi:type="SNS" 
            name="s" 
            RegionEndpoint ="eu-west-1"
            TopicArn ="arn:aws:sns:eu-west-1:{your-account-number}:{your-topic}"
            AmazonCredentialType="Amazon.Runtime.StoredProfileAWSCredentials, AWSSDK"
            layout="${message}"/>
```
