FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

COPY ["Worker2.csproj", "."]
RUN dotnet restore "./Worker2.csproj"

COPY . .
WORKDIR "/src"
RUN dotnet build "Worker2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Worker2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker2.dll"]
