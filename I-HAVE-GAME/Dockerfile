# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ["I-HAVE-GAME.csproj", "./"]
RUN dotnet restore "I-HAVE-GAME.csproj"

# copy everything else and publish
COPY . ./
RUN dotnet publish "I-HAVE-GAME.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Allow Render (or other platforms) to set PORT env var; default to 8080
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "I-HAVE-GAME.dll"]
