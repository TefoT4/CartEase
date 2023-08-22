# CartEase Web API Project

This repository contains a sample implementation of a web API using .NET Core 7, Entity Framework Core, and GitHub authentication.

## Getting Started

Follow these steps to set up the project on your local machine.

### Prerequisites

- [.NET Core 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- Docker (Optional, but helpful if you want to containerize the application)

### Clone the Repository

```bash
git clone https://github.com/your-username/CartEase.git
cd CartEase
```

### Setup Configuration
Navigate to the API project folder.
```bash
cd src/CartEase.Api
```

Update GitHub OAuth settings. Open the appsettings.json file or your environment-specific configurations and set 
your ClientId and ClientSecret:
```bash
{
  "AuthSettings": {
    "ClientId": "YOUR_GITHUB_CLIENT_ID",
    "ClientSecret": "YOUR_GITHUB_CLIENT_SECRET"
  }
}
```

### Note on Authentication:
This project uses GitHub OAuth2 for authentication. Ensure you've registered an OAuth App on GitHub and obtained your ClientId and ClientSecret. The callback URL in your GitHub OAuth App configuration should be set to https://localhost:7029/github-login.

### Authentication
Swagger UI, by default, makes AJAX calls to your endpoints. When you're triggering an OAuth 
flow (which expects redirects and interactions with external websites), AJAX isn't the right approach.

Instead of using Swagger UI to initiate the OAuth flow, try accessing the Github Login endpoint directly from your browser. 
```bash
Navigate to:
https://localhost:7029/Authentication/github-login
```
This should trigger the OAuth flow, redirecting you to GitHub for authentication.

I have intentionally excluded the 'LoginWithGitHub' with github endpoint to prevent confusion.
```bash
[ApiExplorerSettings(IgnoreApi = true)]
```

### Database Setup
Navigate to the Infrastructure project folder.
```bash
cd ../CartEase.Infrastructure
```

Run Entity Framework Core migrations to create the database.
```bash
dotnet ef database update --startup-project ../CartEase.Api
```

Add a side note. Migrations are added by running the following command:
```bash
 dotnet ef migrations add <MigrationNameGoesHere> --startup-project ../CartEase.Api
```

### Run the Application
```bash
dotnet run
```

### API Documentation
You can access the API documentation using Swagger UI. Open your browser and navigate to:
```bash
http://localhost:5000/swagger
```