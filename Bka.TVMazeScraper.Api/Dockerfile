FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY Bka.TVMazeScraper.Api/Bka.TVMazeScraper.Api.csproj Bka.TVMazeScraper.Api/
RUN dotnet restore Bka.TVMazeScraper.Api/Bka.TVMazeScraper.Api.csproj
COPY . .
WORKDIR /src/Bka.TVMazeScraper.Api
RUN dotnet build Bka.TVMazeScraper.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Bka.TVMazeScraper.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Bka.TVMazeScraper.Api.dll"]
