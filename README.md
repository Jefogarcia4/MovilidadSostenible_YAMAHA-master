# MovilidadSostenible_YAMAHA

Aplicaci�n ASP.NET Core (.NET 8) con Razor Pages/Controllers para la encuesta de Movilidad Sostenible. Este repositorio est� preparado para ejecutarse en contenedores Docker con soporte de HTTPS y variables de entorno para secretos.

## Requisitos

- Docker Desktop 4.x o superior.
- .NET 8 SDK (opcional, solo si deseas ejecutar sin contenedor).
- Visual Studio 2022 (opcional). Soporta perfil __Docker__ para depuraci�n.
- Acceso a MySQL (AWS RDS) y variables de entorno configuradas.

## Arquitectura de ejecuci�n

- Imagen base: mcr.microsoft.com/dotnet/aspnet:8.0
- Puertos expuestos:
  - HTTP: 8080
  - HTTPS: 8443
- Certificado HTTPS: montado como PFX dentro del contenedor en /https/aspnetapp.pfx (configurable).
- Configuraci�n de conexi�n y secretos: v�a variables de entorno (secci�n AmazonSecret).

## Archivos clave

- Dockerfile: construcci�n y runtime (HTTP/HTTPS).
- docker-compose.yml: orquesta variables y certificado.
- .dockerignore: ignora artefactos locales.
- Properties/launchSettings.json: perfil __Docker__ para Visual Studio.
- Program.cs: lee configuraci�n AmazonSecret y configura la canalizaci�n HTTP/HTTPS.

## Configuraci�n de variables (.env)

Crea un archivo `.env` en la ra�z (no lo subas a control de c�digo) con:

Importante:
- No compartas este archivo. A�adido ya a `.dockerignore`.
- Si alguna variable falta, Docker Compose mostrar� advertencias y podr�a fallar el bind del volumen con �invalid spec: :/https:ro�.

# Certificado HTTPS
HTTPS_CERT_PATH=C:\Users\TU_USUARIO\.aspnet\https
# En macOS/Linux: /Users/TU_USUARIO/.aspnet/https � /home/TU_USUARIO/.aspnet/https
HTTPS_CERT_PASSWORD=TU_PASSWORD

# Amazon Secret / RDS (ajusta seg�n tu entorno)
AMZ_SERVERNAME=turds.cluster-xxx.us-east-1.rds.amazonaws.com
AMZ_DATABASE=NombreBD
AMZ_AWS_ACCESS_KEY_ID=AKIAxxxxxxxxxxxxxxx
AMZ_AWS_SECRET_ACCESS_KEY=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
AMZ_SECRET_NAME=nombre/del/secret
AMZ_REGION=us-east-1

docker compose --env-file .env config

docker build -t movilidadsostenible-yamaha:latest .

docker run --name movilidad-web -d `
  -p 8080:8080 -p 8443:8443 `
  -e ASPNETCORE_URLS="http://+:8080;https://+:8443" `
  -e ASPNETCORE_Kestrel__Certificates__Default__Path="/https/aspnetapp.pfx" `
  -e ASPNETCORE_Kestrel__Certificates__Default__Password="$env:HTTPS_CERT_PASSWORD" `
  -e AmazonSecret__servername="$env:AMZ_SERVERNAME" `
  -e AmazonSecret__database="$env:AMZ_DATABASE" `
  -e AmazonSecret__aws_access_key_id="$env:AMZ_AWS_ACCESS_KEY_ID" `
  -e AmazonSecret__aws_secret_access_key="$env:AMZ_AWS_SECRET_ACCESS_KEY" `
  -e AmazonSecret__secretName="$env:AMZ_SECRET_NAME" `
  -e AmazonSecret__region="${env:AMZ_REGION}" `
  -v "$env:USERPROFILE\.aspnet\https:/https:ro" `
  movilidadsostenible-yamaha:latest