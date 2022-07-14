FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Go.Equinox.Api.Service/Go.Equinox.Api.Service.csproj", "Go.Equinox.Api.Service/"]
RUN dotnet restore "Go.Equinox.Api.Service/Go.Equinox.Api.Service.csproj"
COPY . .
WORKDIR "/src/Go.Equinox.Api.Service"
RUN dotnet build "Go.Equinox.Api.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Go.Equinox.Api.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Go.Equinox.Api.Service.dll"]
