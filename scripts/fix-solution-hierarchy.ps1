# Script para reorganizar la jerarquia del Solution Explorer

Write-Host "Reorganizando jerarquia de carpetas en CitasMedicas.sln..." -ForegroundColor Cyan

$slnPath = "C:\Proyectos\CitasMedicas\CitasMedicas.sln"

# Leer contenido actual
$content = Get-Content $slnPath -Raw

# Definir seccion NestedProjects
$nestedProjects = @"
	GlobalSection(NestedProjects) = preSolution
		{33333333-3333-3333-3333-333333333333} = {11111111-1111-1111-1111-111111111111}
		{44444444-4444-4444-4444-444444444444} = {11111111-1111-1111-1111-111111111111}
		{55555555-5555-5555-5555-555555555555} = {11111111-1111-1111-1111-111111111111}
		{CFB988DF-BFF0-491C-8A47-1233460DB03A} = {33333333-3333-3333-3333-333333333333}
		{E88AF536-543D-4AA8-9564-90C1111C4F47} = {33333333-3333-3333-3333-333333333333}
		{4F3B2953-116F-4AD3-8638-14EBC079615D} = {33333333-3333-3333-3333-333333333333}
		{E2D80E45-A416-47E6-8480-768A28817DD4} = {33333333-3333-3333-3333-333333333333}
		{D8F3E1B2-4C5A-4D6E-8F7A-1B2C3D4E5F6A} = {44444444-4444-4444-4444-444444444444}
		{E9A2B3C4-5D6F-4E7A-9B8C-2D3E4F5A6B7C} = {44444444-4444-4444-4444-444444444444}
		{F1A2B3C4-6D7E-4F8A-9B0C-3D4E5F6A7B8C} = {44444444-4444-4444-4444-444444444444}
		{96BB296C-D341-4DDC-89A4-38E65373C49E} = {44444444-4444-4444-4444-444444444444}
		{A1B2C3D4-5E6F-7A8B-9C0D-1E2F3A4B5C6D} = {55555555-5555-5555-5555-555555555555}
		{B2C3D4E5-6F7A-8B9C-0D1E-2F3A4B5C6D7E} = {55555555-5555-5555-5555-555555555555}
		{C3D4E5F6-7A8B-9C0D-1E2F-3A4B5C6D7E8F} = {55555555-5555-5555-5555-555555555555}
		{EF2C2913-4260-487E-8E69-F2835A9B0C3A} = {55555555-5555-5555-5555-555555555555}
		{8F7E9C4A-1B2D-4E3F-9A5C-7D8E9F1A2B3C} = {22222222-2222-2222-2222-222222222222}
		{9A8B7C6D-5E4F-3A2B-1C0D-9E8F7A6B5C4D} = {22222222-2222-2222-2222-222222222222}
		{8D7E6F5A-4B3C-2A1D-9E0F-1B2C3D4E5F6A} = {22222222-2222-2222-2222-222222222222}
	EndGlobalSection
"@

# Insertar antes de "EndGlobal"
$content = $content -replace '(EndGlobal)', "$nestedProjects`r`n`$1"

# Guardar
Set-Content -Path $slnPath -Value $content -Encoding UTF8 -NoNewline

Write-Host "[OK] Jerarquia reorganizada correctamente" -ForegroundColor Green
Write-Host ""
Write-Host "Recarga la solucion en Visual Studio:" -ForegroundColor Yellow
Write-Host "  1. File -> Close Solution" -ForegroundColor White
Write-Host "  2. File -> Open -> CitasMedicas.sln" -ForegroundColor White
Write-Host ""
Write-Host "Ahora veras la estructura organizada:" -ForegroundColor Cyan
Write-Host "  src/" -ForegroundColor White
Write-Host "    - Personas/" -ForegroundColor White
Write-Host "    - Citas/" -ForegroundColor White
Write-Host "    - Recetas/" -ForegroundColor White
Write-Host "  tests/" -ForegroundColor White
