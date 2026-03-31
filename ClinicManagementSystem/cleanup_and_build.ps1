
$ErrorActionPreference = "Continue"
Get-Process | Where-Object { $_.Modules.FileName -like "*ClinicManagementSystem*" } | Stop-Process -Force -ErrorAction SilentlyContinue
Stop-Process -Name "dotnet" -Force -ErrorAction SilentlyContinue
dotnet build
