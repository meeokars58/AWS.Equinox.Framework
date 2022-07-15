FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AWS.Equinox.Services.Configuration.ApiClient/AWS.Equinox.Services.Configuration.ApiClient.csproj", "AWS.Equinox.Services.Configuration.ApiClient/"]
RUN dotnet restore "AWS.Equinox.Services.Configuration.ApiClient/AWS.Equinox.Services.Configuration.ApiClient.csproj"
COPY . .
WORKDIR "/src/AWS.Equinox.Services.Configuration.ApiClient"
RUN dotnet build "AWS.Equinox.Services.Configuration.ApiClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AWS.Equinox.Services.Configuration.ApiClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AWS.Equinox.Services.Configuration.ApiClient.dll"]
