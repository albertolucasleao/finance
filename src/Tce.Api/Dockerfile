# ----------------------------
# BUILD STAGE
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia tudo
COPY . .

# Vai até o projeto da API
WORKDIR /src/src/Tce.Api

# Restaura dependências
RUN dotnet restore

# Publica aplicação
RUN dotnet publish -c Release -o /app/publish

# ----------------------------
# RUNTIME STAGE
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copia do build
COPY --from=build /app/publish .

# Porta padrão do ASP.NET no container
EXPOSE 8080

ENTRYPOINT ["dotnet", "Tce.Api.dll"]