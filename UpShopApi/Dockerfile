# -----------------------------
# Stage 1: Build
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["UpShopApi/UpShopApi.csproj", "UpShopApi/"]
RUN dotnet restore "UpShopApi/UpShopApi.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/UpShopApi"
RUN dotnet publish "UpShopApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------
# Stage 2: Runtime
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

# Expose HTTP and HTTPS ports
EXPOSE 8080
EXPOSE 8081

# Set environment variable for dev/prod
ENV DOTNET_ENVIRONMENT=Development

# Entry point
ENTRYPOINT ["dotnet", "UpShopApi.dll"]
