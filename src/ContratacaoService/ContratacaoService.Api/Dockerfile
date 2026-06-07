FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY INDT.Seguros.slnx ./
COPY src/ src/
RUN dotnet restore src/ContratacaoService/ContratacaoService.Api/ContratacaoService.Api.csproj
RUN dotnet publish src/ContratacaoService/ContratacaoService.Api/ContratacaoService.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ContratacaoService.Api.dll"]
