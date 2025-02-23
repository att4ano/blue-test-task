FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["client-service/Presentation/Presentation.csproj", "client-service/Presentation/"]
COPY ["client-service/Contracts/Contracts.csproj", "client-service/Contracts/"]
COPY ["client-service/Application/Application.csproj", "client-service/Application/"]
COPY ["client-service/Infrastructure.Kafka/Infrastructure.Kafka.csproj", "client-service/Infrastructure.Kafka/"]
RUN dotnet restore "client-service/Presentation/Presentation.csproj"
COPY . .
WORKDIR "/src/client-service/Presentation"
RUN dotnet build "Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presentation.dll"]
