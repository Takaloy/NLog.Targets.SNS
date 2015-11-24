# NLog.Targets.SNS

NLog target for Amazon SNS. This target will publish to specified Amazon SNS. For more information about Targets, read [here](https://github.com/NLog/NLog/wiki/Targets "NLog Targets").

License: [MIT](https://raw.githubusercontent.com/Takaloy/NLog.Targets.SNS/master/LICENSE).

[![Build status](https://ci.appveyor.com/api/projects/status/v96402hp71igqyrq/branch/master?svg=true)](https://ci.appveyor.com/project/Takaloy/nlog-targets-sns/branch/master)
[![Version](https://img.shields.io/nuget/v/NLog.Targets.SNS.svg)](https://www.nuget.org/packages/NLog.Targets.SNS)


### Example Config

specify topic arn explicitly

```xml
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

```xml
<target xsi:type="SNS"
            name="s"
            RegionEndpoint ="eu-west-1"
            AccountNumber="{your-account-number}"
            Topic="{your-topic}"
            AmazonCredentialType="Amazon.Runtime.StoredProfileAWSCredentials, AWSSDK"
            layout="${message}"/>
```

posting from and to the same aws, specifying account number is optional. target will try and discover it on your behalf.

```xml
<target xsi:type="SNS"
            name="s"
            RegionEndpoint ="eu-west-1"
            AccountNumber="{your-account-number}"
            Topic="{your-topic}"
            AmazonCredentialType="Amazon.Runtime.StoredProfileAWSCredentials, AWSSDK"
            layout="${message}"/>
```

basic aws credentials with accesskey and secretkey

```xml
<target xsi:type="SNS"
            name="s"
            RegionEndpoint ="eu-west-1"
            AccountNumber="{your-account-number}"
            Topic="{your-topic}"
			AccessKey="{your-access-key}"
			SecretKey="{your-secret-key}"
            layout="${message}"/>
```


