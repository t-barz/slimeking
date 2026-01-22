# Fix FastScriptReload DLL conflict
# This script replaces outdated System.Collections.Immutable.dll files
# that are causing version conflicts with Microsoft.CodeAnalysis

$projectRoot = "G:\GameDev\slimeking\game\theslimeking"
$sourceDll = "$projectRoot\Library\PackageCache\org.nuget.system.collections.immutable@1cb871d2ec81\lib\netstandard2.0\System.Collections.Immutable.dll"

$targets = @(
    "$projectRoot\Assets\FastScriptReload\Plugins\Roslyn\2019+\System.Collections.Immutable.dll",
    "$projectRoot\Assets\FastScriptReload\Plugins\Roslyn\2021+\System.Collections.Immutable.dll"
)

Write-Host "Checking if Unity is running..."
$unityProcess = Get-Process -Name "Unity" -ErrorAction SilentlyContinue

if ($unityProcess) {
    Write-Host "ERROR: Unity is still running. Please close Unity first!" -ForegroundColor Red
    Write-Host "Close Unity and run this script again." -ForegroundColor Yellow
    exit 1
}

Write-Host "Unity is not running. Proceeding with DLL replacement..." -ForegroundColor Green

foreach ($target in $targets) {
    if (Test-Path $target) {
        try {
            # Backup original
            $backup = "$target.backup"
            if (-not (Test-Path $backup)) {
                Copy-Item -Path $target -Destination $backup -Force
                Write-Host "Backed up: $backup" -ForegroundColor Cyan
            }
            
            # Replace with new version
            Copy-Item -Path $sourceDll -Destination $target -Force
            
            # Verify
            $version = [System.Reflection.Assembly]::LoadFrom($target).GetName().Version
            Write-Host "Updated: $target -> Version $version" -ForegroundColor Green
        }
        catch {
            Write-Host "ERROR updating $target : $_" -ForegroundColor Red
        }
    }
    else {
        Write-Host "WARNING: File not found: $target" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Done! You can now open Unity again." -ForegroundColor Green
Write-Host "The compilation error should be resolved." -ForegroundColor Green
