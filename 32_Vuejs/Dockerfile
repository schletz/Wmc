# Build container. Load full SDK as base image.
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# TODO: Adapt the directories!
COPY Spengernews.Application Spengernews.Application
COPY Spengernews.Webapi      Spengernews.Webapi

# Compile the app
RUN dotnet restore "Spengernews.Webapi"
RUN dotnet build   "Spengernews.Webapi" -c Release -o /app/build
RUN dotnet publish "Spengernews.Webapi" -c Release -o /app/publish /p:UseAppHost=false

# App container. Only needs runtime (smaller image)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
EXPOSE 80
EXPOSE 443
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Spengernews.Webapi.dll"]
