FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TaskHive.WebApi/TaskHive.WebApi.csproj", "TaskHive.WebApi/"]
COPY ["TaskHive.Application/TaskHive.Application.csproj", "TaskHive.Application/"]
COPY ["TaskHive.Core/TaskHive.Core.csproj", "TaskHive.Core/"]
COPY ["TaskHive.Infrastructure/TaskHive.Infrastructure.csproj", "TaskHive.Infrastructure/"]
RUN dotnet restore "TaskHive.WebApi/TaskHive.WebApi.csproj"
COPY . .
WORKDIR "/src/TaskHive.WebApi"
RUN dotnet build "TaskHive.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskHive.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskHive.WebApi.dll"]