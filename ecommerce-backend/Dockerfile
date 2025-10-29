FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier le fichier solution
COPY *.sln ./

# Copier les fichiers .csproj avec la structure RÉELLE de votre projet
COPY Ecommerce.API/*.csproj ./Ecommerce.API/
COPY Ecommerce.Domain/*.csproj ./Ecommerce.Domain/
COPY Ecommerce.Application/*.csproj ./Ecommerce.Application/
COPY Ecommerce.Infrastructure/*.csproj ./Ecommerce.Infrastructure/

RUN dotnet restore

COPY . .

WORKDIR /src/Ecommerce.API
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Ecommerce.API.dll"]
