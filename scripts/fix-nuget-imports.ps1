# Script para corregir rutas de imports de paquetes NuGet

Write-Host "Corrigiendo rutas de imports de NuGet..." -ForegroundColor Cyan

$baseDir = "C:\Proyectos\CitasMedicas"

# Proyectos en src/Microservicio/Proyecto/ necesitan ..\..\..\packages (3 niveles)
$proyectosSrc = @(
    "$baseDir\src\Personas\Personas.Infrastructure\Personas.Infrastructure.csproj",
    "$baseDir\src\Citas\Citas.API\Citas.API.csproj",
    "$baseDir\src\Personas\Personas.API\Personas.API.csproj",
    "$baseDir\src\Recetas\Recetas.API\Recetas.API.csproj"
)

foreach ($proyecto in $proyectosSrc) {
    if (Test-Path $proyecto) {
        Write-Host "Corrigiendo: $(Split-Path $proyecto -Leaf)" -ForegroundColor Yellow
        $content = Get-Content $proyecto -Raw
        
        # Corregir rutas de 4 niveles a 3 niveles
        $content = $content -replace '\.\.\\\.\.\\\.\.\\\.\.\\packages', '..\..\..\packages'
        
        Set-Content -Path $proyecto -Value $content -Encoding UTF8 -NoNewline
        Write-Host "  [OK] Corregido" -ForegroundColor Green
    }
}

# Proyectos en tests/ necesitan ..\..\packages (2 niveles)
$proyectosTest = @(
    "$baseDir\tests\Personas.Pruebas\Personas.Pruebas.csproj"
)

foreach ($proyecto in $proyectosTest) {
    if (Test-Path $proyecto) {
        Write-Host "Corrigiendo: $(Split-Path $proyecto -Leaf)" -ForegroundColor Yellow
        $content = Get-Content $proyecto -Raw
        
        # Corregir rutas de 3 niveles a 2 niveles
        $content = $content -replace '\.\.\\\.\.\\\.\.\\packages', '..\..\packages'
        
        Set-Content -Path $proyecto -Value $content -Encoding UTF8 -NoNewline
        Write-Host "  [OK] Corregido" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "[EXITO] Correccion completada. Recarga la solucion en Visual Studio." -ForegroundColor Green
