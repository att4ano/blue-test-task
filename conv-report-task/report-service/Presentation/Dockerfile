FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5068
EXPOSE 5068

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["report-service/Presentation/Presentation.csproj", "report-service/Presentation/"]
COPY ["report-service/Infrastructure.Kafka/Infrastructure.Kafka.csproj", "report-service/Infrastructure.Kafka/"]
COPY ["report-service/Application/Application.csproj", "report-service/Application/"]
COPY ["report-service/Contracts/Contracts.csproj", "report-service/Contracts/"]
COPY ["report-service/Infrastructure.Data/Infrastructure.Data.csproj", "report-service/Infrastructure.Data/"]
COPY ["report-service/Presentation.Kafka/Presentation.Kafka.csproj", "report-service/Presentation.Kafka/"]
RUN dotnet restore "report-service/Presentation/Presentation.csproj"
COPY . .
WORKDIR "/src/report-service/Presentation"
RUN dotnet build "Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presentation.dll"]
