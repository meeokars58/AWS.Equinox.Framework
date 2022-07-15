FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AWS.Equinox.BackgroundServer/AWS.Equinox.BackgroundServer.csproj", "AWS.Equinox.BackgroundServer/"]
RUN dotnet restore "AWS.Equinox.BackgroundServer/AWS.Equinox.BackgroundServer.csproj"
COPY . .
WORKDIR "/src/AWS.Equinox.BackgroundServer"
RUN dotnet build "AWS.Equinox.BackgroundServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AWS.Equinox.BackgroundServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AWS.Equinox.BackgroundServer.dll"]
