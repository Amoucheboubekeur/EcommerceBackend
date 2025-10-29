# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier tous les fichiers de solution
COPY . .

# Restaurer toutes les dépendances
RUN dotnet restore ./EcommerceApi/EcommerceApi.csproj

# Publier le projet API (Release)
RUN dotnet publish ./EcommerceApi/EcommerceApi.csproj -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copier les fichiers publiés
COPY --from=build /app/out .

# Exposer le port (Render lira la variable PORT)
EXPOSE 7146

# Définir le point d'entrée
ENTRYPOINT ["dotnet", "EcommerceApi.dll"]
