FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY Bka.TVMazeSraper.Api/Bka.TVMazeSraper.Api.csproj Bka.TVMazeSraper.Api/
RUN dotnet restore Bka.TVMazeSraper.Api/Bka.TVMazeSraper.Api.csproj
COPY . .
WORKDIR /src/Bka.TVMazeSraper.Api
RUN dotnet build Bka.TVMazeSraper.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Bka.TVMazeSraper.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Bka.TVMazeSraper.Api.dll"]
