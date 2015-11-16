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

posting from and to the same aws, specifying account number is optional. target will try and discover it on your behalf.
```json
<target xsi:type="SNS"
            name="s"
            RegionEndpoint ="eu-west-1"
            AccountNumber="{your-account-number}"
            Topic="{your-topic}"
            AmazonCredentialType="Amazon.Runtime.StoredProfileAWSCredentials, AWSSDK"
            layout="${message}"/>
```


basic aws credentials with accesskey and secretkey

```json
<target xsi:type="SNS"
            name="s"
            RegionEndpoint ="eu-west-1"
            AccountNumber="{your-account-number}"
            Topic="{your-topic}"
			AccessKey="{your-access-key}"
			SecretKey="{your-secret-key}"
            layout="${message}"/>
```