# CartEase API Project
CartEase is a RESTful API designed to manage shopping cart items for an online store. It allows users to perform CRUD operations on their shopping cart items, attach images to items, and securely authenticate using Google OAuth 2.0.

This README provides steps on how to run the project using Docker and Docker Compose.

## Prerequisites
 * Ensure you have Docker and Docker Compose installed on your machine.

## Project Structure

YourProjectRoot/  
│  
├── docker-compose.yml  
│  
├── YourSolution.sln  
│  
├── src/  
│   ├── CartEase.Api/  
│   │   ├── CartEase.Api.csproj  
│   │   ├── Dockerfile  
│   │   └── ...  
│   │
│   ├── CartEase.Application/  
│   │   └── CartEase.Application.csproj  
│   │
│   ├── CartEase.Core/  
│   │   └── CartEase.Core.csproj  
│   │  
│   ├── CartEase.Infrastructure/  
│   │   └── CartEase.Infrastructure.csproj  
│   │  
│   └── CartEase.Models/  
│       └── CartEase.Models.csproj  
│
└── README.md

## Steps to Run the Project Using Docker
1. ### Navigate to Project Root:
   Open a terminal or command prompt and navigate to the root directory of the project, where the docker-compose.yml file is located.
   ```
   cd /path/to/ProjectRoot
   ```
2. ### Environment Variables and Secrets
   Before running the project, ensure you've set the required environment variables in the docker-compose.yml file: Replace YourStrongPassword with your actual SQL Server SA password.
Replace YourGithubClientID and YourGithubClientSecret with your GitHub OAuth App's client ID and secret.
🔒 Security Note: For production deployments, consider using more secure methods like Docker secrets or external environment variable files to store these sensitive details.


3. ### Build and Start Containers:
   Run the following command to build the Docker images (if not built) and start the containers:
```bash
docker-compose up -d
```
   This will launch the API, SQL Server, and Seq services. The -d flag runs the containers in detached mode, meaning they'll run in the background.

4. ### Access the Application:
   With the services running, you can access:
    * The API at http://localhost:8000
    * Seq dashboard at http://localhost:5341

5. ### Stop Containers:
   When you're done, you can stop the containers using:
```bash
docker-compose down
```
This command will stop and remove all the containers defined in the docker-compose.yml file.

## Conclusion
You've successfully set up and run the CartEase API project using Docker! If you encounter any issues, refer to Docker and Docker Compose's official documentation or reach out for support.