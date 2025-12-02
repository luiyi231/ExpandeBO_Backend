# Usamos la imagen oficial de .NET 8 para construir
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos el archivo de proyecto y restauramos dependencias
# IMPORTANTE: Asegúrate que el nombre coincida con tu .csproj
COPY ["ExpandeBO_Backend.csproj", "./"]
RUN dotnet restore "ExpandeBO_Backend.csproj"

# Copiamos todo el resto del código
COPY . .
WORKDIR "/src/."

# Construimos la aplicación
RUN dotnet build "ExpandeBO_Backend.csproj" -c Release -o /app/build
RUN dotnet publish "ExpandeBO_Backend.csproj" -c Release -o /app/publish

# Configuración final para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Render asigna el puerto dinámicamente, pero .NET 8 usa 8080 por defecto
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ExpandeBO_Backend.dll"]