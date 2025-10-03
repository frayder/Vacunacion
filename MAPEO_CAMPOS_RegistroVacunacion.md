# Mapeo de Campos: Modelo vs Vista - RegistroVacunacion

## Campos Obligatorios del Modelo

### ✅ Campos ya mapeados correctamente:

| Campo del Modelo | Campo(s) de la Vista | Paso | Estado |
|------------------|---------------------|------|---------|
| `Consecutivo` | Auto-generado | - | ✅ OK |
| `NombresApellidos` | `PrimerNombre` + `SegundoNombre` + `PrimerApellido` + `SegundoApellido` | 1 | ✅ OK |
| `TipoDocumento` | `TipoDocumento` | 1 | ✅ OK |
| `NumeroDocumento` | `Documento` | 1 | ✅ OK |
| `FechaNacimiento` | `FechaNacimiento` | 1 | ✅ OK |
| `Genero` | `Genero` (radio buttons) | 2 | ✅ OK |
| `Vacuna` | `VacunaSeleccionada` | 6 | ✅ OK |
| `NumeroDosis` | `NumeroDosis` | 6 | ✅ OK |
| `FechaAplicacion` | `FechaAplicacion` | 6 | ✅ OK |

### 📋 Campos Opcionales Mapeados:

| Campo del Modelo | Campo(s) de la Vista | Paso | Estado |  
|------------------|---------------------|------|---------|
| `Telefono` | `Telefono` | 2 | ✅ OK |
| `Direccion` | `Direccion` | 2 | ✅ OK |
| `AseguradoraId` | `AseguradoraId` | 2 | ✅ OK |
| `RegimenAfiliacionId` | `RegimenAfiliacionId` | 2 | ✅ OK |
| `PertenenciaEtnicaId` | `PertenenciaEtnicaId` | 2 | ✅ OK |
| `CentroAtencionId` | `CentroAtencionId` | 4 | ✅ OK |
| `CondicionUsuariaId` | `CondicionUsuariaId` | 4 | ✅ OK |
| `TipoCarnetId` | `TipoCarnet` | 6 | ✅ OK |
| `Lote` | `LoteVacuna` | 6 | ✅ OK |
| `Laboratorio` | `Laboratorio` | 6 | ✅ OK |
| `ViaAdministracion` | `ViaAdministracion` | 6 | ✅ OK |
| `SitioAplicacion` | `SitioAplicacion` | 6 | ✅ OK |
| `Vacunador` | `Vacunador` | 7 | ✅ OK |
| `RegistroProfesional` | `RegistroProfesional` | 7 | ✅ OK |
| `Observaciones` | `ObservacionesVacuna` | 6 | ✅ OK |
| `NotasFinales` | `NotasFinales` | 7 | ✅ OK |

## Estructura por Pasos del Formulario

### Paso 1: Datos Básicos (_DatosBasicos.cshtml)
```html
<!-- Campos Obligatorios -->
TipoDocumento (select)
Documento (input text) 
PrimerNombre (input text)
PrimerApellido (input text)
FechaNacimiento (input date)

<!-- Campos Opcionales -->
SegundoNombre (input text)
SegundoApellido (input text)
```

### Paso 2: Datos Complementarios (_DatosComplementarios.cshtml)
```html
<!-- Campos Obligatorios -->
Genero (radio: FEMENINO/MASCULINO)

<!-- Campos Opcionales -->
Telefono (input text)
Direccion (input text)
AseguradoraId (select)
RegimenAfiliacionId (select)
PertenenciaEtnicaId (select)
```

### Paso 4: Condición Usuario (_CondicionUsuario.cshtml)
```html
<!-- Campos Opcionales -->
CentroAtencionId (select)
CondicionUsuariaId (select)
```

### Paso 6: Esquema Vacunación (_EsquemaVacunacion.cshtml)
```html
<!-- Campos Obligatorios -->
VacunaSeleccionada (select)
NumeroDosis (select)
FechaAplicacion (input date)

<!-- Campos Opcionales -->
TipoCarnet (select)
LoteVacuna (input text)
Laboratorio (input text)
ViaAdministracion (select)
SitioAplicacion (select)
ObservacionesVacuna (textarea)
```

### Paso 7: Responsable (_Responsable.cshtml)
```html
<!-- Campos Opcionales -->
Vacunador (input text)
RegistroProfesional (input text)
NotasFinales (textarea)
```

## Validaciones Implementadas

### En el Controlador:
```csharp
// Campos obligatorios validados:
- NombresApellidos (construido desde PrimerNombre + SegundoNombre + PrimerApellido + SegundoApellido)
- TipoDocumento
- NumeroDocumento  
- Genero
- Vacuna (desde VacunaSeleccionada)
- NumeroDosis
```

### En la Vista (JavaScript):
- Validación de campos obligatorios en cada paso
- Almacenamiento en localStorage paso a paso
- Restauración de datos al navegar entre pasos

## Problemas Solucionados

1. **❌ Error DBNull**: Reemplazado con SqlParameter para manejo correcto de valores nulos
2. **❌ Campos no mapeados**: Corregido mapeo entre nombres de campos del modelo y la vista
3. **❌ Validaciones inconsistentes**: Agregadas validaciones detalladas con información de debug

## Debugging Habilitado

El controlador ahora incluye logging detallado que muestra:
- Todos los campos recibidos del formulario
- Validaciones que fallan con información específica
- Construcción del nombre completo paso a paso
- Mapeo de cada campo individual

## Próximos Pasos

1. **Ejecutar el script SQL** para crear la tabla `RegistroVacunacion`
2. **Probar el formulario** paso a paso
3. **Usar el botón Debug** para verificar que los datos se almacenan correctamente
4. **Revisar la consola del servidor** para ver los logs de debugging si hay errores

## Comandos para Testing

```javascript
// En la consola del navegador, ejecutar después de completar algunos pasos:
debugStoredData(); // Ver datos almacenados en localStorage

// Verificar que los campos obligatorios tienen valores:
console.log('PrimerNombre:', localStorage.getItem('stepData_1'));
console.log('Genero:', localStorage.getItem('stepData_2')); 
console.log('Vacuna:', localStorage.getItem('stepData_6'));
```

## Estructura SQL de la Tabla

La tabla `RegistroVacunacion` debe crearse con el script proporcionado:
- `CreateRegistroVacunacionTable.sql` - Crear tabla completa
- `VerificarTablaRegistroVacunacion.sql` - Verificar creación

Una vez creada la tabla y con el mapeo corregido, el formulario debería guardar exitosamente todos los datos.