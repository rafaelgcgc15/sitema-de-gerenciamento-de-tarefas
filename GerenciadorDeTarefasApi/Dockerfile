FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GerenciadorDeTarefasApi.csproj", "."]
RUN dotnet restore "./GerenciadorDeTarefasApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "GerenciadorDeTarefasApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GerenciadorDeTarefasApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GerenciadorDeTarefasApi.dll"]