FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AWS.Equinox.ApiClient/AWS.Equinox.ApiClient.csproj", "AWS.Equinox.ApiClient/"]
RUN dotnet restore "AWS.Equinox.ApiClient/AWS.Equinox.ApiClient.csproj"
COPY . .
WORKDIR "/src/AWS.Equinox.ApiClient"
RUN dotnet build "AWS.Equinox.ApiClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AWS.Equinox.ApiClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AWS.Equinox.ApiClient.dll"]
