FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier seulement les fichiers .csproj qui existent
COPY *.sln ./
COPY Ecommerce.API/*.csproj ./Ecommerce.API/
# Supprimer ou commenter les lignes pour les dossiers manquants
# COPY Core/*.csproj ./Core/
# COPY Infrastructure/*.csproj ./Infrastructure/
RUN dotnet restore

COPY . .

WORKDIR /Ecommerce.API
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Ecommerce.API.dll"]
