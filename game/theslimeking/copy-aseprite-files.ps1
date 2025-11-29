# Script para copiar imagens JPG e PNG
# Busca todos os arquivos .jpg e .png em G:\GameDev e copia SOMENTE OS ARQUIVOS para G:/OtherImg

$sourceRoot = "G:\GameDev"
$destinationFolder = "G:\OtherImg"

# Criar pasta de destino se não existir
if (-not (Test-Path -Path $destinationFolder)) {
    Write-Host "Criando pasta de destino: $destinationFolder"
    New-Item -ItemType Directory -Path $destinationFolder -Force | Out-Null
}

Write-Host "Buscando arquivos .jpg e .png em $sourceRoot..."
Write-Host ""

# Buscar arquivos .jpg e .png
$files = Get-ChildItem -Path $sourceRoot -Include *.jpg,*.jpeg,*.png -Recurse -ErrorAction SilentlyContinue

$totalFiles = $files.Count
Write-Host "Encontrados $totalFiles arquivos"
Write-Host ""

if ($totalFiles -eq 0) {
    Write-Host "Nenhum arquivo encontrado."
    exit
}

# Copiar apenas os arquivos (sem estrutura de pastas)
$counter = 0
foreach ($file in $files) {
    $counter++
    $destPath = Join-Path -Path $destinationFolder -ChildPath $file.Name
    
    Write-Host "[$counter/$totalFiles] Copiando: $($file.Name)"
    Copy-Item -Path $file.FullName -Destination $destPath -Force
}

Write-Host ""
Write-Host "Concluído! $totalFiles arquivos copiados para $destinationFolder"
