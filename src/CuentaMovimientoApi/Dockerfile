FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082
ENV ASPNETCORE_URLS=http://+:8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/CuentaMovimientoApi/CuentaMovimientoApi.csproj", "src/CuentaMovimientoApi/"]
RUN dotnet restore "src/CuentaMovimientoApi/CuentaMovimientoApi.csproj"
COPY . .
WORKDIR "/src/src/CuentaMovimientoApi"
RUN dotnet build "CuentaMovimientoApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CuentaMovimientoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CuentaMovimientoApi.dll"]
