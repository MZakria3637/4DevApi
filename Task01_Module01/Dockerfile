#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Task01_Module01/Task01_Module01.csproj", "Task01_Module01/"]
RUN dotnet restore "Task01_Module01/Task01_Module01.csproj"
COPY . .
WORKDIR "/src/Task01_Module01"
RUN dotnet build "Task01_Module01.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Task01_Module01.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Task01_Module01.dll"]