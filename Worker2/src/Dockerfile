FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

COPY ["./Worker1/Worker1.csproj", "Worker1/"]
COPY ["./DAM2.Shared/DAM2.Shared.csproj", "DAM2.Shared/"]
COPY ["./Proto.Actor/Proto.Actor.csproj", "Proto.Actor/"]
COPY ["./Proto.Cluster/Proto.Cluster.csproj", "Proto.Cluster/"]
COPY ["./Proto.Cluster.Identity.MongoDb/Proto.Cluster.Identity.MongoDb.csproj", "Proto.Cluster.Identity.MongoDb/"]
COPY ["./Proto.Cluster.Kubernetes/Proto.Cluster.Kubernetes.csproj", "Proto.Cluster.Kubernetes/"]
COPY ["./Proto.Remote/Proto.Remote.csproj", "Proto.Remote/"]
COPY ["./Proto.Remote.GrpcCore/Proto.Remote.GrpcCore.csproj", "Proto.Remote.GrpcCore/"]
COPY ["./Proto.Cluster.Consul/Proto.Cluster.Consul.csproj", "Proto.Cluster.Consul/"]

RUN dotnet restore "./Worker1/Worker1.csproj"

COPY . .
WORKDIR "/src/Worker1"
RUN dotnet build "Worker1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Worker1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Worker1.dll"]
