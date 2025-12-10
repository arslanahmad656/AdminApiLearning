<#
    NuGet Upgrade Script for .NET Solutions (Project-by-Project Mode)
    -----------------------------------------------------------------

    Uses the built-in .NET SDK command:
        dotnet list <project> package --outdated

    This avoids MSBuild evaluation bugs in dotnet-outdated-tool.

    PREREQUISITES:
      - PowerShell 5.0+
      - .NET SDK installed
      - A valid .sln file

    USAGE:
       Dry run (detect only):
           .\upgrade-nuget.ps1 -SolutionPath "C:\Path\MySolution.sln" -DryRun

       Perform real upgrades:
           .\upgrade-nuget.ps1 -SolutionPath "C:\Path\MySolution.sln"

    NOTES:
      - Script stops on any error.
      - Logs contain full verbose output of all commands.
      - Upgrades only packages in projects referenced by the solution.
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$SolutionPath,

    [switch]$DryRun,

    [string]$LogFile = "nuget-upgrade-log.txt"
)

$ErrorActionPreference = "Stop"

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "   NuGet Upgrade Script (B2 Mode)     " -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan


# ---------------------------------------------------------------
# Pre-Req Checks
# ---------------------------------------------------------------
if ($PSVersionTable.PSVersion.Major -lt 5) {
    Write-Host "ERROR: PowerShell 5.0 or later required." -ForegroundColor Red
    exit 1
}

try {
    $dotnetVersion = dotnet --version
} catch {
    Write-Host "ERROR: .NET SDK not detected." -ForegroundColor Red
    exit 1
}
Write-Host "Detected .NET SDK: $dotnetVersion" -ForegroundColor Green


# ---------------------------------------------------------------
# Validate solution path
# ---------------------------------------------------------------
if (-not (Test-Path $SolutionPath)) {
    Write-Host "ERROR: Solution file not found." -ForegroundColor Red
    exit 1
}

if (-not $SolutionPath.ToLower().EndsWith(".sln")) {
    Write-Host "ERROR: Path is not a .sln file." -ForegroundColor Red
    exit 1
}

$SolutionDir = Split-Path $SolutionPath -Parent
Write-Host "`nSolution directory: $SolutionDir" -ForegroundColor DarkCyan


# ---------------------------------------------------------------
# Extract all project paths from the .sln
# ---------------------------------------------------------------
Write-Host "`nExtracting project list from solution..." -ForegroundColor Cyan

$projectLines = Select-String -Path $SolutionPath -Pattern 'Project\(".*"\)\s=\s".*?",\s*"(.*?\.csproj)"'
$projectPaths = @()

foreach ($line in $projectLines) {

    $relativePath = $line.Matches.Groups[1].Value
    $absolute = Resolve-Path -Path (Join-Path $SolutionDir $relativePath) -ErrorAction SilentlyContinue

    if ($absolute) {
        $projectPaths += $absolute.Path
    }
    else {
        Write-Host "WARNING: Project missing: $relativePath" -ForegroundColor Yellow
    }
}

if ($projectPaths.Count -eq 0) {
    Write-Host "ERROR: No .csproj files found in solution." -ForegroundColor Red
    exit 1
}

Write-Host "Found $($projectPaths.Count) project(s)." -ForegroundColor Green


# ---------------------------------------------------------------
# Prepare log file
# ---------------------------------------------------------------
if (Test-Path $LogFile) { Remove-Item $LogFile }

"NuGet Upgrade Log - $(Get-Date)" | Out-File $LogFile
"DryRun: $DryRun" | Out-File $LogFile -Append
"Solution: $SolutionPath" | Out-File $LogFile -Append
"======================================" | Out-File $LogFile -Append


# ---------------------------------------------------------------
# Track upgrades
# ---------------------------------------------------------------
$Upgrades = @()

# ---------------------------------------------------------------
# Process each project
# ---------------------------------------------------------------
foreach ($proj in $projectPaths) {

    Write-Host "`nProcessing project: $proj" -ForegroundColor Yellow
    "`nProject: $proj" | Out-File $LogFile -Append

    # RUN OUTDATED CHECK
    Write-Host "Running: dotnet list package --outdated (verbose)..." -ForegroundColor DarkGray

    $arguments = @(
        "list", $proj, "package",
        "--outdated",
        "--include-transitive"
    )

    $output = dotnet $arguments 2>&1

    # STOP ON ERROR
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`n❌ ERROR: dotnet list package FAILED for:" -ForegroundColor Red
        Write-Host $proj -ForegroundColor Red
        Write-Host "`nOutput:" -ForegroundColor Red
        Write-Host $output -ForegroundColor Red
        exit 1
    }

    # Log raw output
    "----- dotnet list output for $proj -----" | Out-File $LogFile -Append
    $output | Out-File $LogFile -Append


    # PARSE OUTDATED PACKAGES
    foreach ($line in $output) {
        if ($line -match "(\S+)\s+(\d+[\w\.-]*)\s+(\d+[\w\.-]*)\s+(\d+[\w\.-]*)") {

            $pkgName = $matches[1]
            $reqVer  = $matches[2]
            $resVer  = $matches[3]
            $latest  = $matches[4]

            if ($reqVer -ne $latest) {
                $Upgrades += [PSCustomObject]@{
                    Project    = Split-Path $proj -Leaf
                    Package    = $pkgName
                    Current    = $reqVer
                    UpdatedTo  = $latest
                }

                if (-not $DryRun) {
                    Write-Host "Upgrading $pkgName from $reqVer → $latest" -ForegroundColor Green

                    $addArgs = @(
                        "add", $proj, "package", $pkgName,
                        "--version", $latest
                    )

                    $addOutput = dotnet $addArgs 2>&1

                    if ($LASTEXITCODE -ne 0) {
                        Write-Host "`n❌ ERROR upgrading package $pkgName" -ForegroundColor Red
                        Write-Host $addOutput -ForegroundColor Red
                        exit 1
                    }

                    # Log output
                    $addOutput | Out-File $LogFile -Append
                }
            }
        }
    }
}


# ---------------------------------------------------------------
# Summary
# ---------------------------------------------------------------
Write-Host "`n======================================" -ForegroundColor Cyan
Write-Host " Summary of Upgraded Packages " -ForegroundColor Magenta
Write-Host "======================================" -ForegroundColor Cyan

if ($Upgrades.Count -gt 0) {
    $Upgrades | Format-Table -AutoSize

    "`nSummary:" | Out-File $LogFile -Append
    $Upgrades | Out-File $LogFile -Append
}
else {
    Write-Host "🎉 All packages were up-to-date." -ForegroundColor Green
}


# ---------------------------------------------------------------
# Final
# ---------------------------------------------------------------
if ($DryRun) {
    Write-Host "`nDRY RUN COMPLETE — No changes applied." -ForegroundColor Yellow
}
else {
    Write-Host "`nALL UPGRADES COMPLETED SUCCESSFULLY." -ForegroundColor Green
}

Write-Host "Full log written to: $LogFile" -ForegroundColor Cyan
Write-Host "======================================"
