FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["gateway.csproj", "./"]
RUN dotnet restore "gateway.csproj"

COPY . .
RUN dotnet build "gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5110
ENV ASPNETCORE_URLS=http://+:5110

ENTRYPOINT ["dotnet", "gateway.dll"]