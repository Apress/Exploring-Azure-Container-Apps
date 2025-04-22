FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

USER app
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
ARG configuration=Release
WORKDIR /src
COPY ["BenefitsManager.Backend.Bff.Api/BenefitsManager.Backend.Bff.Api.csproj", "BenefitsManager.Backend.Bff.Api/"]
RUN dotnet restore "BenefitsManager.Backend.Bff.Api/BenefitsManager.Backend.Bff.Api.csproj"
COPY . .
WORKDIR "/src/BenefitsManager.Backend.Bff.Api"
RUN dotnet build "BenefitsManager.Backend.Bff.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "BenefitsManager.Backend.Bff.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BenefitsManager.Backend.Bff.Api.dll"]
