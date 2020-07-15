# Auther

Auther is a simple sample ASP.NET Core WebAPI with only an External Authentication through Slack's OAuth2.

## Where should I look?

All of the Authentication is configured in `Startup.ConfigureServices` in the `WebApi` project.

Trying to access `{server}/Protected/MyInfo` will force you to authenticate through Slack if you haven't already.

## How do I try this out?

1. Setup a Slack App

You can create a new Slack app [here](https://api.slack.com/apps).

Copy your App's `Client ID` and `Client Secret`.

In the **OAuth & Permissions** section add a **Redirect URL** of `https://localhost:5001/auth/oauthCallback`.

> Keep in mind that if you change the setting in `Startup.cs` the URL may be different.

Under **User Token Scopes** add `identity.avatar` and `identity.basic` so that our app may access this information through Slack.

> Avatar currently isn't implemented.

2. Setup `appsettings.Development.json`

Copy `appsettings.json` from the WebApi project and paste it as `appsettings.Development.json` in the same directory.

Then edit the file and paste your `Client ID` and `Client Secret` in the appropriate fields.

Also make sure the `Jwt` options are correct, mainly add a secure `EncryptionKey` and double check the `Issuer` (Your server URL) and `Audience` (Front-end URL).

> `Audience` is not being checked, this can be easily turned on in `Startup.cs`

3. Run the project

At this point you should trigger the OAuth2 flow by accessing `https://localhost:5001/Users/LoginWithSlack?redirectUrl=https://spelos.net` through your browser.

The `redirectUrl=` parameter should be the same front-end that invoked this login redirect. If your application only has one front-end, you may configure the redirect automatically on the server-side. Alternativally, you may come up with a URL white-list system.

4. Copy the JWT token

The token will be in the URL after you go through the OAuth2 flow. Copy it and try to access `https://localhost:5001/Protected/MyInfo` through something like [postman](https://www.postman.com/). Don't forget to set the token. In Postman, you can set it under the `Authorization` tab. Select type: `Bearer Token` and paste it into the `Token` field.

## Is this still being improved?

Yes, while this works for basic usage, WebAPIs should rarely redirect to the OAuth2 flow on their own.

Ideally, you'd get a generic "404 Unauthorized" response and you have to visit the initial OAuth2 page from your front-end application.

The use of JwtBarer token would also be more appropriate rather than Secure Cookies.
