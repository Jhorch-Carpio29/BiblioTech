# Etapa 1: Compilación y restauración de dependencias
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivos .csproj de cada capa para aprovechar la caché de Docker
COPY ["BiblioTech.API/BiblioTech.API.csproj", "BiblioTech.API/"]
COPY ["BiblioTech.Application/BiblioTech.Application.csproj", "BiblioTech.Application/"]
COPY ["BiblioTech.Domain/BiblioTech.Domain.csproj", "BiblioTech.Domain/"]
COPY ["BiblioTech.Infrastructure/BiblioTech.Infrastructure.csproj", "BiblioTech.Infrastructure/"]

# Restaurar paquetes de la capa principal
RUN dotnet restore "BiblioTech.API/BiblioTech.API.csproj"

# Copiar todo el código restante de la solución
COPY . .
WORKDIR "/src/BiblioTech.API"

# Publicar la aplicación en modo Release optimizado
RUN dotnet publish "BiblioTech.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Entorno de ejecución ligero
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Forzar al contenedor a escuchar el puerto dinámico de Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "BiblioTech.API.dll"]