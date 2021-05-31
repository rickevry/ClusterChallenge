FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

COPY ["Worker1.csproj", "."]
RUN dotnet restore "./Worker1.csproj"

COPY . .
WORKDIR "/src"
RUN dotnet build "Worker1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Worker1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker1.dll"]
