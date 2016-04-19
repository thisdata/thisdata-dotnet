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
* `mobile` - E.164 format - An mobile number for sending unusual activity SMS alerts to. e.g. +15555555555
* `source` - Used to indicate the source of the event and override company or app name in audit log and notifications
* `logoUrl` - Used to override logo used in email notifications

### Event Types
Event types are called `Verbs` and are available as constants e.g. `AuditMessageVerbs.LOG_IN`

For a full list of supported verbs see http://help.thisdata.com/v1.0/docs/verbs

### Webhooks
For convenience you can validate webhooks sent by ThisData using the `ValidateWebhook` method. It will return a boolean indicating if the signature included in the webhook header matches with the signed webhook body using your secret key. 
```
client.ValidateWebhook("your-secret")
```
For more information about types of webhooks you can recieve see http://help.thisdata.com/docs/webhooks

## API Documentation

API Documentation is available at [http://help.thisdata.com/docs/apiv1events](http://help.thisdata.com/docs/apiv1events).

