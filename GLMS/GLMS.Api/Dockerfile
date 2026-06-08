# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (layer caching)
COPY GLMS.API.csproj .
RUN dotnet restore "GLMS.API.csproj"

# Copy the rest of the source
COPY . .

# Publish the application
RUN dotnet publish "GLMS.API.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create uploads directory (used by the application)
RUN mkdir -p /app/wwwroot/uploads

# Copy published output from build stage
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "GLMS.API.dll"]