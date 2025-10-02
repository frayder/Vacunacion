# Publicación en IIS - Sistema de Anulación de Facturas

## 📋 Requisitos Previos

### 1. IIS Instalado
Asegúrese de que IIS esté instalado con las siguientes características:
- IIS-WebServerRole
- IIS-WebServer
- IIS-CommonHttpFeatures
- IIS-HttpErrors
- IIS-HttpLogging
- IIS-RequestFiltering
- IIS-StaticContent
- IIS-WebServerManagementTools
- IIS-ApplicationInit
- IIS-ISAPIExtensions
- IIS-ISAPIFilter
- IIS-NetFxExtensibility
- IIS-ASPNET
- IIS-NetFxExtensibility45
- IIS-ASPNET45

**Comando de PowerShell para instalar IIS:**
```powershell
Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole, IIS-WebServer, IIS-CommonHttpFeatures, IIS-HttpErrors, IIS-HttpLogging, IIS-RequestFiltering, IIS-StaticContent, IIS-WebServerManagementTools, IIS-ApplicationInit, IIS-ISAPIExtensions, IIS-ISAPIFilter, IIS-NetFxExtensibility, IIS-ASPNET, IIS-NetFxExtensibility45, IIS-ASPNET45
```

### 2. ASP.NET Core Hosting Bundle
Descargue e instale el ASP.NET Core Hosting Bundle desde:
https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-9.0.0-windows-hosting-bundle-installer

### 3. SQL Server LocalDB
Instale SQL Server Express LocalDB desde:
https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb

## 🚀 Publicación Automática

### Opción 1: Script Automático (Recomendado)
1. Abra PowerShell como **Administrador**
2. Navegue al directorio del proyecto:
   ```powershell
   cd "D:\INNOVATIONANALYTICS\Anulacion Facturas"
   ```
3. Configure LocalDB (opcional):
   ```powershell
   .\Setup-LocalDB.ps1
   ```
4. Ejecute el script de publicación:
   ```powershell
   .\Publish-To-IIS.ps1
   ```

### Opción 2: Publicación Manual

#### Paso 1: Publicar la aplicación
```powershell
cd "D:\INNOVATIONANALYTICS\Anulacion Facturas"
dotnet publish --configuration Release --output "C:\inetpub\wwwroot\AnulacionFacturas" --runtime win-x64 --self-contained false
```

#### Paso 2: Crear Application Pool
1. Abra **Administrador de IIS**
2. Expanda el servidor
3. Haga clic derecho en **Application Pools** → **Add Application Pool**
4. Nombre: `AnulacionFacturasPool`
5. .NET CLR Version: `No Managed Code`
6. Process Model → Identity: `ApplicationPoolIdentity`

#### Paso 3: Crear Sitio Web
1. En Administrador de IIS, haga clic derecho en **Sites** → **Add Website**
2. Site name: `AnulacionFacturas`
3. Application pool: `AnulacionFacturasPool`
4. Physical path: `C:\inetpub\wwwroot\AnulacionFacturas`
5. Port: `8080` (o el puerto que prefiera)

#### Paso 4: Configurar Permisos
1. Haga clic derecho en la carpeta `C:\inetpub\wwwroot\AnulacionFacturas`
2. **Propiedades** → **Seguridad** → **Editar**
3. **Agregar** → Escriba `IIS APPPOOL\AnulacionFacturasPool`
4. Otorgue permisos de **Control total**

## 🔧 Configuración de Base de Datos

### Para desarrollo/local:
La aplicación está configurada para usar LocalDB automáticamente.

### Para producción:
1. Actualice `appsettings.Production.json` con la cadena de conexión correcta
2. Asegúrese de que el servidor SQL esté accesible desde IIS
3. Configure los permisos de base de datos para el usuario del pool de aplicaciones

## 🌐 Acceder a la Aplicación

Después de la publicación, acceda a:
```
http://localhost:8080
```

### Credenciales de prueba:
- **Usuario:** `admin`
- **Contraseña:** `admin123`

## 🔍 Solución de Problemas

### Error 500.19 - Internal Server Error
- Verifique que ASP.NET Core Hosting Bundle esté instalado
- Confirme que el web.config esté presente en la carpeta de publicación

### Error de conexión a base de datos
- Ejecute `Setup-LocalDB.ps1` para configurar LocalDB
- Verifique la cadena de conexión en `appsettings.Production.json`

### Error 403 Forbidden
- Verifique los permisos de la carpeta para `IIS APPPOOL\AnulacionFacturasPool`
- Asegúrese de que el Application Pool esté ejecutándose

### Error 502.5 - Process Failure
- Verifique que .NET 9.0 esté instalado en el servidor
- Confirme que la aplicación se publicó correctamente
- Revise los logs en la carpeta `logs` dentro del directorio de publicación

## 📊 Monitoreo

### Logs de aplicación:
Los logs se generan en: `C:\inetpub\wwwroot\AnulacionFacturas\logs\`

### Logs de IIS:
- Ubicación: `C:\inetpub\logs\LogFiles\`
- También disponibles en **Visor de eventos** → **Registros de Windows** → **Sistema**

## 🔄 Actualizaciones

Para actualizar la aplicación:
1. Detenga el sitio web en IIS
2. Vuelva a ejecutar `dotnet publish`
3. Inicie el sitio web nuevamente

## 📞 Soporte

Si encuentra problemas:
1. Revise los logs de aplicación e IIS
2. Verifique la configuración de IIS
3. Confirme que todos los requisitos previos estén instalados
4. Verifique la conectividad a la base de datos

---

**¡La aplicación está lista para producción!** 🎉