# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy everything, including project files, to the container
COPY . .

# Restore dependencies for the main project and referenced projects
RUN dotnet restore "./dotnet-woodys-wild-guess/dotnet.woodyswildguess.csproj"

# Build and publish the application
RUN dotnet publish "./dotnet-woodys-wild-guess/dotnet.woodyswildguess.csproj" -c Release -o /app/publish

# Use the official ASP.NET Core runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory for the runtime
WORKDIR /app

# Copy the published output from the build stage to the runtime stage
COPY --from=build /app/publish .

# Expose port 80 for the web application
EXPOSE 80

# Set the entry point for the container
ENTRYPOINT ["dotnet", "./dotnet-woodys-wild-guess/dotnet.woodyswildguess.dll"]
