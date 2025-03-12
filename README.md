# Project Management System

This is a simple .NET-based API project for managing projects and their associated tasks. It includes support for Microsoft SQL Server and a Docker setup for easy deployment.

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/)
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (for local development only, not needed for running with Docker)

## Getting Started

1. **Clone the Repository**  
   Clone this repository to your local machine:
   ```bash
   git clone https://github.com/orenkats/ProjectManagementSystem.git
   ```

2. **Set Up the Docker Environment**  
   The project includes a `Dockerfile` and a `compose.yaml` file. These allow you to run both the SQL Server and the API in separate containers.

3. **Start the Application**  
   To build and run the project using Docker Compose, execute the following command from the project’s root directory:
   ```bash
   docker-compose up --build -d
   ```
   This command:
    - Builds the API from the Dockerfile
    - Starts the SQL Server container
    - Starts the API container, which connects to the SQL Server

4. **Verify the Application**  
   Once the containers are up and running:
    - API should be accessible at [http://localhost:5000](http://localhost:5000) (default port).
    - The SQL Server is exposed on port 1433, so you can connect to it using tools like `sqlcmd` or Azure Data Studio:
      ```
      Server: localhost,1433
      User: sa
      Password: YourStrongPassword!
      ```

5. **Stop the Application**  
   When you’re done, you can stop and remove the containers with:
   ```bash
   docker-compose down
   ```

## Implementation Details

- **Authorization Mechanism**
    - The project uses **JWT-based authorization** integrated with Amazon Cognito.
    - Authentication is handled by the `AuthController` via the Cognito user pool.
    - Role-based authorization is configured using `cognito:groups` claims to allow for multiple user roles (e.g., Admin, Manager, User).
    - JWT tokens are validated on every request to ensure secure and authenticated access.

- **Database Initialization and Retry Policy**
    - The project depends on a **SQL Server container**, which may not be ready immediately when the API starts.
    - A **retry policy** is implemented during startup to repeatedly attempt database connectivity until the SQL Server instance is ready. This ensures that migrations are applied as soon as the database becomes accessible.

- **Global Error Handling**
    - All unhandled exceptions are caught by a **global error handler middleware**.
    - This middleware:
        - Logs detailed error information for troubleshooting.
        - Returns a structured error response to clients.
        - Prevents unhandled exceptions from exposing sensitive server details.

- **Migrations and Database Setup**
    - Migrations are applied automatically on startup.
    - This ensures that the database schema is always up to date without requiring manual intervention.

- **User Management**
    - Amazon Cognito is configured to handle user authentication.
    - Users are added to the Cognito user pool manually through the AWS Management Console.
    - After adding a user, roles are assigned by placing them in the appropriate groups (e.g., Admin, Manager, User).
    - These groups determine the user’s access levels within the application and are used in role-based authorization checks.

## Troubleshooting

- If you encounter any issues, check the Docker logs for each service:
  ```bash
  docker-compose logs api
  docker-compose logs sqlserver
  ```
- Make sure that no other services are using ports `5000` or `1433` on your local machine.
- If the database container isn’t starting properly, verify that Docker is running and that you have sufficient memory available.

## For a project expecting 10,000 users daily and a client-side application, consider the following:

- **Use a Scalable Hosting Service:**  
  Deploy the backend API on a cloud platform like AWS, Azure, or Google Cloud. These platforms can easily scale the infrastructure as the number of users grows.

- **Implement Load Balancing:**  
  Add a load balancer in front of multiple API instances. This will distribute incoming traffic evenly, reducing the chances of downtime or slow response times.

- **Use a CDN for the Client-Side:**  
  Serve static files (like HTML, CSS, and JavaScript) through a Content Delivery Network (CDN). This ensures faster load times for users regardless of their location.

- **Optimize the Database:**  
  Ensure the database can handle high traffic by using a managed, scalable SQL database or a cloud-based database solution. Configure caching and indexes to speed up data retrieval.

- **Monitor and Adjust:**  
  Set up monitoring tools to keep an eye on performance, errors, and user patterns. As traffic increases, make adjustments—like adding more server instances or optimizing code—to maintain a smooth user experience.

- **Database Strategy:**
  
  In addition to scaling the application layer, implement a database strategy to handle the large volume of read and write requests. Use a primary database instance for handling all write operations to maintain data 
  integrity. Introduce read replicas to offload SELECT queries, enabling the system to process more concurrent read requests without affecting write performance. This read/write separation not only improves response 
  times but also ensures that the database remains responsive as user demand grows.