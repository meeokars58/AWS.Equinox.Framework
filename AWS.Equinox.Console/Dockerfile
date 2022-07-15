FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AWS.Equinox.Console/AWS.Equinox.Console.csproj", "AWS.Equinox.Console/"]
RUN dotnet restore "AWS.Equinox.Console/AWS.Equinox.Console.csproj"
COPY . .
WORKDIR "/src/AWS.Equinox.Console"
RUN dotnet build "AWS.Equinox.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AWS.Equinox.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AWS.Equinox.Console.dll"]
