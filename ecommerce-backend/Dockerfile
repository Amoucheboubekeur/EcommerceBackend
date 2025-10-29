# Étape 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copier le fichier solution et restaurer les dépendances
COPY *.sln .
COPY EcommerceApi/*.csproj ./EcommerceApi/
RUN dotnet restore

# Copier tout le code et publier
COPY . .
WORKDIR /app/EcommerceApi
RUN dotnet publish -c Release -o out

# Étape 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/EcommerceApi/out .

# Exposer le port (à adapter si nécessaire)
EXPOSE 7146

# Lancer l'application
ENTRYPOINT ["dotnet", "EcommerceApi.dll"]
