# NLog.Targets.SNS
NLog target for Amazon SNS 

### Example Config

specify topic arn explicitly


```json
<target xsi:type="SNS" 
            name="s" 
            RegionEndpoint ="eu-west-1"
            TopicArn ="arn:aws:sns:eu-west-1:{your-account-number}:{your-topic}"
            AmazonCredentialType="Amazon.Runtime.StoredProfileAWSCredentials, AWSSDK"
            layout="${message}"/>
```

work out topic by convention. 
```
$"arn:aws:sns:{RegionEndPoint}:{AccountNumber}:{Topic}"
```


```json
<target xsi:type="SNS"
            name="s"
            RegionEndpoint ="eu-west-1"
            AccountNumber="{your-account-number}"
            Topic="{your-topic}"
            AmazonCredentialType="Amazon.Runtime.StoredProfileAWSCredentials, AWSSDK"
            layout="${message}"/>
```