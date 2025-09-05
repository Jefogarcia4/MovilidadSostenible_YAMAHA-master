# MovilidadSostenible_YAMAHA

Aplicación ASP.NET Core (.NET 8) con Razor Pages/Controllers para la encuesta de Movilidad Sostenible. Este repositorio está preparado para ejecutarse en contenedores Docker con soporte de HTTPS y variables de entorno para secretos.

## Requisitos

- Docker Desktop 4.x o superior.
- .NET 8 SDK (opcional, solo si deseas ejecutar sin contenedor).
- Visual Studio 2022 (opcional). Soporta perfil __Docker__ para depuración.
- Acceso a MySQL (AWS RDS) y variables de entorno configuradas.

## Arquitectura de ejecución

- Imagen base: mcr.microsoft.com/dotnet/aspnet:8.0
- Puertos expuestos:
  - HTTP: 8080
  - HTTPS: 8443
- Certificado HTTPS: montado como PFX dentro del contenedor en /https/aspnetapp.pfx (configurable).
- Configuración de conexión y secretos: vía variables de entorno (sección AmazonSecret).

## Archivos clave

- Dockerfile: construcción y runtime (HTTP/HTTPS).
- docker-compose.yml: orquesta variables y certificado.
- .dockerignore: ignora artefactos locales.
- Properties/launchSettings.json: perfil __Docker__ para Visual Studio.
- Program.cs: lee configuración AmazonSecret y configura la canalización HTTP/HTTPS.

## Configuración de variables (.env)

Crea un archivo `.env` en la raíz (no lo subas a control de código) con:

Importante:
- No compartas este archivo. Añadido ya a `.dockerignore`.
- Si alguna variable falta, Docker Compose mostrará advertencias y podría fallar el bind del volumen con “invalid spec: :/https:ro”.

# Certificado HTTPS
HTTPS_CERT_PATH=C:\Users\TU_USUARIO\.aspnet\https
# En macOS/Linux: /Users/TU_USUARIO/.aspnet/https ó /home/TU_USUARIO/.aspnet/https
HTTPS_CERT_PASSWORD=TU_PASSWORD

# Amazon Secret / RDS (ajusta según tu entorno)
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