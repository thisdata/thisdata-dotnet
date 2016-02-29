ThisData.NET
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
* userId - string - A unique identifier for the user
* name - string The full name of the user
* email - string - An email address for sending unusual activity alerts to
* mobile - E.164 format - An mobile number for sending unusual activity SMS alerts to. e.g. +15555555555

### Event Types
* log-in - A successful login
* log-in-denied - A failed login
* password-reset-attempt - Requested to change password
* password-reset-success - Successfully changed password

## API Documentation

API Documentation is available at [http://help.thisdata.com/docs/apiv1events](http://help.thisdata.com/docs/apiv1events).

## License

```
            .-::-.                      
        `-:++++++++:-`                  
    .-/+++/-`:-`-/+++/-.               
  .:+++/:.    :-    .:++++:.            
.+++/-`       :-       `-/+++`          
/++.        `-//.`        .++:          
/++.     `-/++++++/-`     .++:          
/++.     /++++++++++:     .++:          
/++.     /++  MIT ++/     .++:          
/++.     /++++++++++:     .++:          
/++.     `-/++++++/-`     .++:          
:++.        `.//.`        .++:          
`+++/-`       :-       `-/+++`          
  .:++++:.    :-    .:++++:.            
    .-/+++/-`:-`-/+++/-.               
        `-:++++++++:-`                  
            .-::-.                      
                                    
```
