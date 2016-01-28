ThisData.NET
=============

ThisData.NET is a .NET / C# client for the ThisData Login Intelligence API (https://thisdata.com).

## Usage
Install the [latest ThisData.NET package](https://www.nuget.org/packages/ThisData.NET) from Nuget

Add the reference to the Nuget package
```
using ThisData;
```

Find the point in your code just after a login is authenticated 
and add the following async post to the ThisData API
```
var client = new ThisData.Client("YOUR API KEY FROM THISDATA");
client.Login("user123", "123.123.123.123", "Rich Chetwynd", "rich123@thisdata.com", Request.UserAgent);
```

You can fire and forget and leave it there of if you 
want to track the response for debugging etc then do this
```
var result = await client.Login("user123", "123.123.123.123", "Rich Chetwynd", "rich123@thisdata.com", Request.UserAgent);
```

It returns an HttpResponseMessage so you can access StatusCode etc
```
var statusCode = result.StatusCode
var responseBody = await result.Content.ReadAsStringAsync()
```


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
