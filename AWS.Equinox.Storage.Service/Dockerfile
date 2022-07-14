FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AWS.Equinox.Storage.Service/AWS.Equinox.Storage.Service.csproj", "AWS.Equinox.Storage.Service/"]
RUN dotnet restore "AWS.Equinox.Storage.Service/AWS.Equinox.Storage.Service.csproj"
COPY . .
WORKDIR "/src/AWS.Equinox.Storage.Service"
RUN dotnet build "AWS.Equinox.Storage.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AWS.Equinox.Storage.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AWS.Equinox.Storage.Service.dll"]
