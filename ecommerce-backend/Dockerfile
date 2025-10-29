# Étape 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier le fichier .sln et les projets
COPY *.sln ./
COPY EcommerceApi/*.csproj EcommerceApi/

# Restaurer les dépendances
RUN dotnet restore

# Copier le reste du code
COPY . .

# Compiler et publier
WORKDIR /src/EcommerceApi
RUN dotnet publish -c Release -o /app/out

# Étape 2 : Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copier les fichiers publiés depuis l'étape build
COPY --from=build /app/out .

# Exposer le port (Render utilisera automatiquement la variable PORT)
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 8080

# Démarrer l'application
ENTRYPOINT ["dotnet", "EcommerceApi.dll"]
