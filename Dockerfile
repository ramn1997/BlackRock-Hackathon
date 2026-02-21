# docker build -t blk-hacking-ind-ram-n .
# Selection Criteria: Using Alpine Linux for a minimal footprint and reduced attack surface.
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy csproj and restore
COPY ["RetirementSystem.API/RetirementSystem.API.csproj", "RetirementSystem.API/"]
RUN dotnet restore "RetirementSystem.API/RetirementSystem.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/RetirementSystem.API"
RUN dotnet build "RetirementSystem.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RetirementSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the application to run on port 5477
ENV ASPNETCORE_URLS=http://+:5477
EXPOSE 5477

ENTRYPOINT ["dotnet", "RetirementSystem.API.dll"]
