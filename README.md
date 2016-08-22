ThisData.NET [![Build Status](https://travis-ci.org/thisdata/thisdata-dotnet.png?branch=master)](https://travis-ci.org/thisdata/thisdata-dotnet)
=============

ThisData.NET is a .NET / C# client for the ThisData Login Intelligence API (https://thisdata.com).

## Setup
Install the [latest ThisData.NET package](https://www.nuget.org/packages/ThisData.NET) from Nuget
```
Install-Package ThisData.Net
```

Create a ThisData client
```
var client = new ThisData.Client("YOUR API KEY FROM THISDATA");
```

## Track Events
Find the point in your code just after a login success, failure or password reset .
We recommend you use the `TrackAsync` method to send data to the ThisData API 
```
client.TrackAsync("log-in", userId: "john123455", name: "John Titor", email: "john+titor@thisdata.com");
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

### Event Types
Event types are called `Verbs` and are available as constants e.g. `AuditMessageVerbs.LOG_IN`

For a full list of supported verbs see http://help.thisdata.com/v1.0/docs/verbs

### Webhooks
To extract the webhook body use the `GetWebhookPayload` method which will return a `ThisData.Models.WebhookPayload` object.

```
client.GetWebhookPayload("your-secret"); // Validates the webhook signature using a shared secret
```

## Verify 
Use the `Verify` method to enable contextual authentication in your app. It accepts the same parameters as the `Track` event with the exception of the event type/verb. 

Verify will return a risk score between 0->1 which indicates our level confidence that the user is who they say they are. 

0.0 - low risk/high confidence
1.0 - high risk/low confidence 


For more information about types of webhooks you can receive see http://help.thisdata.com/docs/webhooks

## API Documentation

API Documentation is available at [http://help.thisdata.com/docs/apiv1events](http://help.thisdata.com/docs/apiv1events).

