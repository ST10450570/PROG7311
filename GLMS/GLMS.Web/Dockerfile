# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY GLMS.Web.csproj .
RUN dotnet restore "GLMS.Web.csproj"

# Copy remaining source
COPY . .

# Publish the application
RUN dotnet publish "GLMS.Web.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "GLMS.Web.dll"]