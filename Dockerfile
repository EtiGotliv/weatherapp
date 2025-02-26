
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY WeatherApp.csproj ./
RUN dotnet restore "WeatherApp.csproj"
COPY . .
RUN dotnet publish "WeatherApp.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "WeatherApp.dll"]
