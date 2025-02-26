# Fase 1: React builden met Vite
FROM node:18 AS build-react

WORKDIR /app/client

# Kopieer package.json en package-lock.json en installeer dependencies
COPY client/package.json client/package-lock.json ./ 
RUN npm install

# Kopieer de rest van de frontend-code en bouw de app met Vite
COPY client/ ./ 
RUN npm run build

# Debug: Controleer waar de bestanden staan
RUN ls -la /app/client && ls -la /app/client/dist
RUN ls -la /app/API/wwwroot || echo "wwwroot bestaat nog niet"

# Fase 2: .NET 9 backend builden
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY API/API.csproj API/
RUN dotnet restore "API/API.csproj"

COPY . . 
WORKDIR "/src/API" 
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/API"
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# Fase 3: Combineren en klaar maken voor productie
FROM base AS final

WORKDIR /app

# Kopieer de backend inclusief wwwroot naar de container
COPY --from=publish /app/publish . 

# Kopieer het certificaat voor HTTPS
COPY certificates/restore.pfx /app/restore.pfx

# Kestrel configureren met HTTPS-certificaat
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/restore.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="Pa$$w0rd"

# Stel de ASPNETCORE_URLS in om enkel HTTPS te gebruiken
ENV ASPNETCORE_URLS="https://+:443"

# Exposeer alleen poort 443 (HTTPS)
EXPOSE 443

# Omgevingsvariabele voor de database verbindingstring
# Gebruik 'host.docker.internal' om te verbinden met je lokale SQL Server
ENV ConnectionStrings__DefaultConnection="Server=host.docker.internal,1433;Database=shop;User Id=sa;Password=Password@1;TrustServerCertificate=True"

# Start de .NET backend
ENTRYPOINT ["dotnet", "API.dll"]
