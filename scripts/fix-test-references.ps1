# Script para corregir ProjectReference en proyectos de test

Write-Host "Corrigiendo referencias de proyectos de test..." -ForegroundColor Cyan

$baseDir = "C:\Proyectos\CitasMedicas"

# Definir correcciones necesarias
$correcciones = @(
    @{
        Archivo = "$baseDir\tests\Citas.Tests\Citas.Tests.csproj"
        Buscar = '<ProjectReference Include="..\..\..\Citas.Domain\Citas.Domain.csproj">'
        Reemplazar = '<ProjectReference Include="..\..\src\Citas\Citas.Domain\Citas.Domain.csproj">'
    },
    @{
        Archivo = "$baseDir\tests\Recetas.Tests\Recetas.Tests.csproj"
        Buscar = '<ProjectReference Include="..\..\..\Recetas.Domain\Recetas.Domain.csproj">'
        Reemplazar = '<ProjectReference Include="..\..\src\Recetas\Recetas.Domain\Recetas.Domain.csproj">'
    },
    @{
        Archivo = "$baseDir\tests\Personas.Pruebas\Personas.Pruebas.csproj"
        Buscar = '<ProjectReference Include="..\Personas.Domain\Personas.Domain.csproj">'
        Reemplazar = '<ProjectReference Include="..\..\src\Personas\Personas.Domain\Personas.Domain.csproj">'
    }
)

foreach ($corr in $correcciones) {
    if (Test-Path $corr.Archivo) {
        Write-Host "Corrigiendo: $($corr.Archivo)" -ForegroundColor Yellow
        $content = Get-Content $corr.Archivo -Raw
        $content = $content -replace [regex]::Escape($corr.Buscar), $corr.Reemplazar
        Set-Content -Path $corr.Archivo -Value $content -Encoding UTF8 -NoNewline
        Write-Host "  [OK] Corregido" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Correccion completada. Restaura paquetes NuGet y recompila." -ForegroundColor Green
