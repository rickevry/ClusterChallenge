FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /src

COPY ["Client1/Client1.csproj", "Client1/"]
RUN dotnet restore "Client1/Client1.csproj"

COPY . .
WORKDIR "/src/Client1"
RUN dotnet build "Client1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Client1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Client1.dll"]
