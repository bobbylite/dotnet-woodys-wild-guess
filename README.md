# Woody's Wild Guess :construction_worker_man:

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![API](https://img.shields.io/badge/API-3C873A?style=for-the-badge&logo=api&logoColor=white)

Woody's Wild Guess is a flexible construction estimation tool built with .NET Blazor. It's designed with cutting edge WASM (Web Assembly) technology that makes for an extremely responsive user experience. This provides a seamless user experience that leverages the power of OpenAI's latest Large Language Models combined with Twitter/X's Application Developer platform to deliver the world's first construction estimation tooling in your pocket. 

## Identity Driven Estimations :construction_worker_man:
The driving force behind all the built in cutting edge tech is your identity.  This allows organizations to trust that any given identity has the right tools for the job through Identity and Access Management controls combined with a RBAC architecure (Role Based Access Control). This means that only certain roles can access certain AI agents, administration panels, twitter controls etc.. 

## Availability & Scalability :rocket:
Leveraging the power of Azure App Services, we can ensure the highest levels of availability, scalability and efficiency which guarantees you can always rely on Woody's Wild Guess to get the job done on time and under budget.

## :file_folder: Projects

The solution contains two projects:

1. **Blazor Server**: This project handles the server-side rendering of the Blazor application.
2. **Blazor Client**: This project is responsible for the client-side logic and UI of the Blazor application.

## :rocket: Getting Started

To get started with the Woody's Wild Guess application, follow these steps:

1. Clone the repository: `git clone https://github.com/bobbylite/dotnet-woodys-wild-guess.git`
2. Navigate to the project directory: `cd dotnet-woodys-wild-guess`
3. Restore the packages: `dotnet restore`
4. Create an Auth0 Idp tenant (or whatever Idp of your choosing)
5. Register a Web App within the Auth0 tenant or equivalent authentication process. 
5. Create a Twitter Application within the Twitter Developer portal
6. Register the Twitter Application to allow posts via OpenId Connect Authorization Code w/ PKCE grant type
7. Add ```local.woodyswildguess.com``` to your hosts file for local development

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
```console
dotnet run --project woodyswildguess --launch-profile https
```

### :rocket: Building and Running Your Blazor App with Docker
#### Step 1: :hammer_and_wrench: Build the Docker Image
Easily package your Blazor app into a container image using this command:

```console
sudo dotnet publish --os linux --arch x64 -t:PublishContainer
```
This command will:

- Target a Linux OS (--os linux) and 64-bit architecture (--arch x64).
- Use the PublishContainer target to create a container image directly from your .csproj configuration.

#### Step 2: :running_man: Run the Container
Run your containerized app and map it to your local port with:

```console
docker run -p 8080:8080 dotnet-woodys-wild-world-image
```
This command will:

- Spin up a new Docker container from your image named dotnet-woodys-wild-world-image.
- Map port 8080 inside the container to port 8080 on your host machine, making your Blazor app available at http://localhost:8080.


### :star: Why Containerize?
Containerizing your Blazor app ensures a consistent and isolated environment for deployment, making it easier to run your app anywhere with Docker, from your local machine to the cloud. Plus, it's perfect for ensuring a smooth, production-ready experience! 🐳✨

This version highlights the process in a more engaging way, includes emojis for a modern touch, and provides a brief explanation of the commands and benefits of containerization.

## :gear: Built With

- [.NET 8](https://dotnet.microsoft.com/en-us/)
- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- [.NET Core Web API](https://dotnet.microsoft.com/en-us/apps/aspnet/apis)
- [Auth0](https://auth0.com/)

## :handshake: Contributing

Contributions, issues, and feature requests are welcome! Feel free to check [issues page](https://github.com/bobbylite/dotnet-woodys-wild-guess/issues).

## :memo: License

This project is [MIT](https://choosealicense.com/licenses/mit/) licensed.