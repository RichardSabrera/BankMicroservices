FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ClientePersonaApi/ClientePersonaApi.csproj", "src/ClientePersonaApi/"]
RUN dotnet restore "src/ClientePersonaApi/ClientePersonaApi.csproj"
COPY . .
WORKDIR "/src/src/ClientePersonaApi"
RUN dotnet build "ClientePersonaApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClientePersonaApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClientePersonaApi.dll"]
