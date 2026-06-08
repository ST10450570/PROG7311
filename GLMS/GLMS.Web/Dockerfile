# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy nuget config first to avoid Windows fallback path issues
COPY nuget.config .

# Copy project file and restore
COPY GLMS.Web.csproj .
RUN dotnet restore "GLMS.Web.csproj"

# Copy all source files
COPY . .

# Publish
RUN dotnet publish "GLMS.Web.csproj" -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GLMS.Web.dll"]