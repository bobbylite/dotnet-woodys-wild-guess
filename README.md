# Woody's Wild Guess :construction_worker_man:

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![API](https://img.shields.io/badge/API-3C873A?style=for-the-badge&logo=api&logoColor=white)

Woody's Wild Estimations is a Blazor application . It's designed to provide a seamless user experience for our bald community.

## :file_folder: Projects

The solution contains three projects:

1. **Blazor Server**: This project handles the server-side rendering of the Blazor application.
2. **Blazor Client**: This project is responsible for the client-side logic and UI of the Blazor application.

## :rocket: Getting Started

To get started with the Woody's Wild Guess application, follow these steps:

1. Clone the repository: `git clone https://github.com/bobbylite/dotnet-woodys-wild-guess.git`
2. Navigate to the project directory: `cd dotnet-woodys-wild-guess`
3. Restore the packages: `dotnet restore`
4. Run the application: `dotnet run`

## :computer: Running the Project Locally

To run the Woody's Wild Guess application locally, follow these steps:

### Configure Woody's Wild Guess Web Application

First, you need to configure the application settings via the ```appsettings.json``` file.


#### Step 1
Configure your Identity Provider under the ```Authentication``` section of the application's appsettings.json configuration file. 

1. Open ```appsettings.json``` or ```appsettings.Development.json```
2. Navigate to ```Authentication``` section of the file. 
3. Add in your Identity Provider details.

Example: 
```json
  "Authentication": {
    "DefaultScheme": "OpenIdConnect",
    "Schemes": {
      "OpenIdConnect": {
        "SignInScheme": "Cookies",
        "Authority": "https://woodys-wild-guess.us.auth0.com",
        "ClientId": "{your-client-id}",
        "ClientSecret": "{your-client-secret}",
        "ResponseType": "code",
        "CallbackPath": "/signin-oidc",
        "SaveTokens": true,
        "Scope": [
          "openid",
          "profile"
        ]
      }
    }
  }
```

#### Step 2
Configure your Twitter Application under the ```TwitterOptions``` section of the application's appsettings.json configuration file. 

1. Open ```appsettings.json``` or ```appsettings.Development.json```
2. Navigate to ```TwitterOptions``` section of the file. 
3. Add in the details for your registered Twitter Application.  you can find these details on the Twitter Developer page where your Twitter Application was registered and created.
4. Make sure that whatever redirect URI is added here also matches the one in your Twitter Application. This is needed for the OpenId Connect Authroziation Code grant type.

example: 
```json
  "TwitterOptions": {
    "BaseUrl": "https://twitter.com/i/oauth2/authorize",
    "ResponseType": "code",
    "ClientId": "{your-client-id}",
    "ClientSecret": "{your-client-secret}",
    "RedirectUri": "https://local.woodyswildguess.com:7243/callback",
    "Scope": [
      "offline.access",
      "users.read",
      "tweet.read",
      "tweet.write"
    ],
    "State": "state",
    "CodeChallenge": "challenge",
    "CodeChallengeMethod": "plain"
  }
```

### Running Woody's Wild Guess Web Application
After configurations are completed, you can run the web application locally.
Run the project with the following command:
```sh
dotnet run --project woodyswildguess --launch-profile https
```

## :gear: Built With

- [.NET 8](https://dotnet.microsoft.com/en-us/)
- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- [.NET Core Web API](https://dotnet.microsoft.com/en-us/apps/aspnet/apis)
- [Auth0](https://auth0.com/)

## :handshake: Contributing

Contributions, issues, and feature requests are welcome! Feel free to check [issues page](https://github.com/bobbylite/dotnet-woodys-wild-guess/issues).

## :memo: License

This project is [MIT](https://choosealicense.com/licenses/mit/) licensed.