# Auther

Auther is a simple sample ASP.NET Core WebAPI with only an External Authentication through Slack's OAuth2.

## Where should I look?

All of the Authentication is configured in `Startup.ConfigureServices` in the `WebApi` project.

Trying to access `{server}/Protected/MyInfo` will force you to authenticate through Slack if you haven't already.

## How do I try this out?

1. Setup a Slack App

You can create a new Slack app [here](https://api.slack.com/apps).

Copy your App's `Client ID` and `Client Secret`.

In the **OAuth & Permissions** section add a **Redirect URL** of `https://localhost:5001/authorization-code/callback`.

> Keep in mind that if you change the setting in `Startup.cs` the URL may be different.

Under **User Token Scopes** add `identity.avatar` and `identity.basic` so that our app may access this information through Slack.

> Avatar currently isn't implemented.

2. Setup `appsettings.Development.json`

Copy `appsettings.json` from the WebApi project and paste it as `appsettings.Development.json` in the same directory.

Then edit the file and paste your `Client ID` and `Client Secret` in the appropriate fields.

3. Run the project

At this point you should trigger the OAuth2 flow by accessing `https://localhost:5001/Protected/MyInfo` through your browser.

## Is this still being improved?

Yes, while this works for basic usage, WebAPIs should rarely redirect to the OAuth2 flow on their own.

Ideally, you'd get a generic "404 Unauthorized" response and you have to visit the initial OAuth2 page from your front-end application.

The use of JwtBarer token would also be more appropriate rather than Secure Cookies.
