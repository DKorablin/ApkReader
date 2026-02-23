# Generate Code Coverage Report Locally
# This script generates code coverage reports using coverlet without external dependencies
# It works on Windows with PowerShell

param(
	[String]$Configuration = "Debug",
	[String]$Framework = "net8",
	[String]$OutputPath = ".coverage",
	[Switch]$SkipTests = $false
)

# Define paths - script is in .github/scripts, so we need to go up two levels to repo root
$ScriptDirectory = if ($MyInvocation.MyCommandPath) {
	Split-Path -Parent $MyInvocation.MyCommandPath
} else {
	$PSScriptRoot
}

# Fallback to current directory if both are empty
if (-not $ScriptDirectory) {
	$ScriptDirectory = Get-Location
}

$SolutionRoot = Split-Path -Parent (Split-Path -Parent $ScriptDirectory)
$TestProjectPath = Join-Path $SolutionRoot "ApkReader.Tests"

# Create output directory if it doesn't exist
$CoverageOutputPath = if ([System.IO.Path]::IsPathRooted($OutputPath)) {
	$OutputPath.TrimEnd('\').TrimEnd('/')
} else {
	Join-Path $SolutionRoot $OutputPath
}

# Create output directory if it doesn't exist
if (-not (Test-Path $CoverageOutputPath)) {
	New-Item -ItemType Directory -Path $CoverageOutputPath | Out-Null
}

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Code Coverage Report Generation" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Framework: $Framework" -ForegroundColor Yellow
Write-Host "Output Path: $CoverageOutputPath" -ForegroundColor Yellow
Write-Host "Solution Root: $SolutionRoot" -ForegroundColor Yellow
Write-Host ""

# Run tests with coverage collection using coverlet
if (-not $SkipTests) {
	Write-Host "Running tests with coverage collection..." -ForegroundColor Green
	$CoverletArgs = @(
		"test",
		$TestProjectPath,
		"--configuration", $Configuration,
		"--framework", $Framework,
		"--no-restore",
		"--logger", "console;verbosity=normal",
		"/p:CollectCoverage=true",
		"/p:CoverletOutputFormat=cobertura%2cjson%2copencover",
		"/p:CoverletOutput=$($CoverageOutputPath)/",
		"/p:Include=`"[ApkReader]*`""
	)

	& dotnet @CoverletArgs
	$TestExitCode = $LASTEXITCODE

	if ($TestExitCode -ne 0) {
		Write-Host "Tests failed with exit code: $TestExitCode" -ForegroundColor Red
		exit $TestExitCode
	}
} else {
	Write-Host "Skipping test execution (SkipTests flag enabled)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Coverage Report Generation" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan

# Check if coverage files were generated
$CoberturaFile = Join-Path $CoverageOutputPath "coverage.$Framework.cobertura.xml"
$JsonFile = Join-Path $CoverageOutputPath "coverage.$Framework.json"
$OpenCoverFile = Join-Path $CoverageOutputPath "coverage.$Framework.opencover.xml"

$FilesGenerated = @()
if (Test-Path $CoberturaFile) {
	$FilesGenerated += "Cobertura XML"
	Write-Host "Cobertura coverage file generated" -ForegroundColor Green
}
if (Test-Path $JsonFile) {
	$FilesGenerated += "JSON Summary"
	Write-Host "JSON summary file generated" -ForegroundColor Green
}
if (Test-Path $OpenCoverFile) {
	$FilesGenerated += "OpenCover XML"
	Write-Host "OpenCover coverage file generated" -ForegroundColor Green
}

# Parse coverage from JSON summary if available
if (Test-Path $JsonFile) {
	Write-Host ""
	Write-Host "Coverage Summary:" -ForegroundColor Cyan
	
	$jsonContent = Get-Content $JsonFile | ConvertFrom-Json
	$coverage = $jsonContent.coverage
	
	Write-Host "  Line Coverage:       $('{0:P2}' -f $coverage)" -ForegroundColor Yellow
	
	# Try to get branch coverage if available
	if ($jsonContent.PSObject.Properties.Name -contains "branch") {
		$branchCoverage = $jsonContent.branch
		Write-Host "  Branch Coverage:     $('{0:P2}' -f $branchCoverage)" -ForegroundColor Yellow
	}
	
	# Summary statistics
	Write-Host ""
	Write-Host "Coverage Report Location:" -ForegroundColor Cyan
	Write-Host "  $CoverageOutputPath" -ForegroundColor Yellow
}

# Generate HTML report using reportgenerator if available
$HtmlReportDir = Join-Path $CoverageOutputPath "html"
$CoberturaFile = Join-Path $CoverageOutputPath "coverage.$Framework.cobertura.xml"

if (Test-Path $CoberturaFile) {
	Write-Host "Generating HTML report using reportgenerator..." -ForegroundColor Green
	try {
		& reportgenerator -reports:$CoberturaFile -targetdir:$HtmlReportDir -reporttypes:Html
		if ($LASTEXITCODE -eq 0) {
			Write-Host "HTML report generated at $HtmlReportDir" -ForegroundColor Green
		} else {
			Write-Host "reportgenerator failed with exit code $LASTEXITCODE" -ForegroundColor Red
		}
	} catch {
		Write-Host "reportgenerator tool not found or failed to run. Please install it with 'dotnet tool install -g dotnet-reportgenerator-globaltool' if needed." -ForegroundColor Yellow
	}
}

Write-Host ""
Write-Host "Files generated:" -ForegroundColor Green
$FilesGenerated | ForEach-Object { Write-Host "  - $_" -ForegroundColor Green }

Write-Host ""
Write-Host "===============================================" -ForegroundColor Green
Write-Host "Coverage report generation complete!" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green

exit 0