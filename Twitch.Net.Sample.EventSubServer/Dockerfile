FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Twitch.Net.Sample.EventSubServer/Twitch.Net.Sample.EventSubServer.csproj", "Twitch.Net.Sample.EventSubServer/"]
RUN dotnet restore "Twitch.Net.Sample.EventSubServer/Twitch.Net.Sample.EventSubServer.csproj"
COPY . .
WORKDIR "/src/Twitch.Net.Sample.EventSubServer"
RUN dotnet build "Twitch.Net.Sample.EventSubServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Twitch.Net.Sample.EventSubServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Twitch.Net.Sample.EventSubServer.dll"]
