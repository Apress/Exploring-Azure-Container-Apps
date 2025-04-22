FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

USER app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["BenefitsManager.Frontend.WebPortal.Ui/BenefitsManager.Frontend.WebPortal.Ui.csproj", "BenefitsManager.Frontend.WebPortal.Ui/"]
RUN dotnet restore "BenefitsManager.Frontend.WebPortal.Ui/BenefitsManager.Frontend.WebPortal.Ui.csproj"
COPY . .
WORKDIR "/src/BenefitsManager.Frontend.WebPortal.Ui"
RUN dotnet build "BenefitsManager.Frontend.WebPortal.Ui.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "BenefitsManager.Frontend.WebPortal.Ui.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BenefitsManager.Frontend.WebPortal.Ui.dll"]
