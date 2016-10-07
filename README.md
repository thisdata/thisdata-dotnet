ThisData.NET [![Build Status](https://travis-ci.org/thisdata/thisdata-dotnet.png?branch=master)](https://travis-ci.org/thisdata/thisdata-dotnet)
=============

ThisData.NET is a .NET / C# client for the ThisData Login Intelligence API (https://thisdata.com).

## Setup
Install the [latest ThisData.NET package](https://www.nuget.org/packages/ThisData.NET) from Nuget
```
Install-Package ThisData.Net
```

Create a ThisData client
```csharp
var thisdata = new ThisData.Client("YOUR API KEY FROM THISDATA");
```

## Track Events
Find the point in your code just after a login success, failure or password reset .
We recommend you use the `TrackAsync` method to send data to the ThisData API 

```csharp
public void Track(string verb, string userId = "", string name = "", string email = "", 
	string mobile = "", string source = "", string logoUrl = "", string sessionId = "", 
	bool cookieExpected = false, string deviceId = "")
```

To track a successful log-in
```csharp
thisdata.TrackAsync("log-in", userId: "john12345", name: "John Titor", email: "john+titor@thisdata.com");
```

### Optional params
* `userId` - string - A unique identifier for the user
* `name` - string The full name of the user
* `email` - string - An email address for sending unusual activity alerts to
* `mobile` - E.164 format - A mobile number for sending unusual activity SMS alerts to. e.g. +15555555555
* `source` - Used to indicate the source of the event and override company or app name in audit log and notifications
* `logoUrl` - Used to override logo used in email notifications
* `sessionId` - If you use a database to track sessions, you can send us the session ID
* `cookieExpected` - Send true when using our optional Javascript tracking library, and we'll know to expect a cookie
* `deviceId` - A unique device identifier. Typically used for tracking mobile devices

### Event Types
Event types are called `Verbs` and are available as constants e.g. `AuditMessageVerbs.LOG_IN`

For a full list of supported verbs see http://help.thisdata.com/v1.0/docs/verbs


## Verify Identity
Use the `Verify` method to enable contextual authentication in your app. It accepts the same parameters as the `Track` event with the exception of the event type/verb.

```js
public VerifyResult Verify(string userId = "", string name = "", string email = "", string mobile = "", 
	string source = "", string sessionId = "", bool cookieExpected = false)
```

Verify will return a risk score between 0->1 which indicates our level confidence that the user is who they say they are.


0.0 - low risk/high confidence it's the real user

1.0 - high risk/low confidence it's the real user


```csharp
VerifyResult res = thisdata.Verify(userId: "john12345", deviceId: "xxx-xxx-xxx");

if(res.Score > 0.9){
	// Step authentication, prompt for password or 2FA code
}
```

## Get a list of Events
You can get a list of events enriched with their risk score and location data for use in custom audit logs. 
See the [docs for possible query filters and paging params](http://help.thisdata.com/docs/v1getevents).

```csharp
public EventsResult GetEvents(string userId = "", string[] verbs = null, string source = "", 
	int limit = 50, int offset = 0, DateTime? after = null, DateTime? before = null);
```

Get last successful log-in time and location for a user.

```
EventsResult = thisdata.GetEvents(userId: "john12345", limit:1, verbs: new string[]{"log-in"});

string lastLoginCountry = EventsResult.Results[0].Location.Address.CountryName;
```


## Webhooks
To extract the webhook body use the `GetWebhookPayload` method which will return a `ThisData.Models.WebhookPayload` object.

```csharp
public WebhookPayload GetWebhookPayload(string secret = "") 
```

Validates the webhook signature using a shared secret. If no secret is provided it defaults to your API Key
```csharp
WebhookPayload payload = client.GetWebhookPayload("your-secret");

if (payload.WasUser.HasValue) // Its a user responding to a was this you notification
{
    if (!payload.WasUser.Value)
    {
        // The user confirmed it was not them
        ResetUserSession(payload.User.Id);
    }
}
```

For more information about types of webhooks you can receive see http://help.thisdata.com/docs/webhooks


## API Documentation
API Documentation is available at [http://help.thisdata.com/docs/apiv1events](http://help.thisdata.com/docs/apiv1events).

## Contributing
Bug reports and pull requests are welcome on GitHub at [https://github.com/thisdata/thisdata-dotnet](https://github.com/thisdata/thisdata-dotnet)