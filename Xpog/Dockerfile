#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Xpog/Xpog.csproj", "Xpog/"]
RUN dotnet restore "Xpog/Xpog.csproj"
COPY . .
COPY ["Xpog/appsettings.docker.json", "Xpog/appsettings.json"]
WORKDIR "/src/Xpog"
RUN dotnet build "Xpog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Xpog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Xpog.dll"]
