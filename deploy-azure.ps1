# Azure Deployment Script for EMS Virtual Event Management System (Windows PowerShell)
# This script automates the entire deployment process to Azure

param(
    [string]$ResourceGroup = "ems-resource-group",
    [string]$Location = "centralindia",
    [string]$RegistryName = "",
    [string]$SqlServerName = "",
    [string]$SqlAdminUser = "sqladmin",
    [string]$SqlAdminPassword = "",
    [switch]$UseLocalSql = $false
)

# Generate random suffix if names not provided
$RandomSuffix = Get-Random -Minimum 10000 -Maximum 99999
if (-not $RegistryName) { $RegistryName = "emsregistry$RandomSuffix" }
if (-not $SqlServerName) { $SqlServerName = "ems-sql-$RandomSuffix" }
if (-not $SqlAdminPassword) { 
    $SqlAdminPassword = Read-Host -AsSecureString -Prompt "Enter SQL Admin password"
    $SqlAdminPassword = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($SqlAdminPassword))
}

Write-Host "`n============================================================" -ForegroundColor Green
Write-Host "     EMS Azure Deployment Script (Windows PowerShell)      " -ForegroundColor Green
Write-Host "============================================================`n" -ForegroundColor Green

# ============================================================================
# STEP 1: Login to Azure
# ============================================================================

Write-Host "Step 1: Logging in to Azure..." -ForegroundColor Yellow
az login

# ============================================================================
# STEP 2: Create Resource Group
# ============================================================================

Write-Host "Step 2: Creating Azure Resource Group..." -ForegroundColor Yellow
az group create `
  --name $ResourceGroup `
  --location $Location

Write-Host "[OK] Resource Group created: $ResourceGroup`n" -ForegroundColor Green

# ============================================================================
# STEP 3: Create Container Registry
# ============================================================================

Write-Host "Step 3: Creating Azure Container Registry..." -ForegroundColor Yellow
az acr create `
  --resource-group $ResourceGroup `
  --name $RegistryName `
  --sku Basic `
  --admin-enabled true

Write-Host "[OK] Container Registry created: $RegistryName`n" -ForegroundColor Green

# Get ACR credentials
$RegistryUrl = "$RegistryName.azurecr.io"
$AcrUsername = az acr credential show `
  --resource-group $ResourceGroup `
  --name $RegistryName `
  --query username `
  --output tsv

$AcrPassword = az acr credential show `
  --resource-group $ResourceGroup `
  --name $RegistryName `
  --query "passwords[0].value" `
  --output tsv

Write-Host "Registry URL: $RegistryUrl"
Write-Host "Username: $AcrUsername`n"

# ============================================================================
# STEP 4: Create SQL Database (if not using local)
# ============================================================================

if (-not $UseLocalSql) {
    Write-Host "Step 4: Creating Azure SQL Server..." -ForegroundColor Yellow
    
    az sql server create `
      --name $SqlServerName `
      --resource-group $ResourceGroup `
      --location $Location `
      --admin-user $SqlAdminUser `
      --admin-password $SqlAdminPassword
    
    Write-Host "Creating SQL Database..." -ForegroundColor Yellow
    az sql db create `
      --resource-group $ResourceGroup `
      --server $SqlServerName `
      --name EMSDatabase `
      --service-objective S0
    
    Write-Host "Configuring SQL Firewall..." -ForegroundColor Yellow
    az sql server firewall-rule create `
      --resource-group $ResourceGroup `
      --server $SqlServerName `
      --name AllowAzureServices `
      --start-ip-address 0.0.0.0 `
      --end-ip-address 0.0.0.0
    
    $SqlFqdn = "$SqlServerName.database.windows.net"
    $ConnectionString = "Server=tcp:$SqlFqdn,1433;Initial Catalog=EMSDatabase;Persist Security Info=False;User ID=$SqlAdminUser;Password=$SqlAdminPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    
    Write-Host "[OK] SQL Database created: $SqlServerName`n" -ForegroundColor Green
}
else {
    $ConnectionString = "Server=host.docker.internal,1433;Database=EMSDatabase;User Id=sa;Password=Admin@12345!;Encrypt=false;TrustServerCertificate=true;"
    Write-Host "[OK] Using local SQL Server`n" -ForegroundColor Green
}

# ============================================================================
# STEP 5: Build and Push Docker Images
# ============================================================================

Write-Host "Step 5: Building Docker images..." -ForegroundColor Yellow

docker build -t ems-api:latest -f EMS.API/Dockerfile . 
if ($LASTEXITCODE -ne 0) { exit 1 }

docker build -t ems-frontend:latest -f EMS.Frontend/Dockerfile .
if ($LASTEXITCODE -ne 0) { exit 1 }

Write-Host "Tagging images for ACR..." -ForegroundColor Yellow

docker tag ems-api:latest "$RegistryUrl/ems-api:latest"
docker tag ems-api:latest "$RegistryUrl/ems-api:v1"
docker tag ems-frontend:latest "$RegistryUrl/ems-frontend:latest"
docker tag ems-frontend:latest "$RegistryUrl/ems-frontend:v1"

Write-Host "Logging in to Azure Container Registry..." -ForegroundColor Yellow
az acr login --name $RegistryName

Write-Host "Pushing images to ACR..." -ForegroundColor Yellow

docker push "$RegistryUrl/ems-api:latest"
docker push "$RegistryUrl/ems-api:v1"
docker push "$RegistryUrl/ems-frontend:latest"
docker push "$RegistryUrl/ems-frontend:v1"

Write-Host "[OK] Docker images pushed to ACR`n" -ForegroundColor Green

# ============================================================================
# STEP 6: Deploy API Container
# ============================================================================

Write-Host "Step 6: Deploying API Container..." -ForegroundColor Yellow

$ApiDnsLabel = "ems-api-$RandomSuffix"

az container create `
  --resource-group $ResourceGroup `
  --name ems-api-container `
  --image "$RegistryUrl/ems-api:latest" `
  --dns-name-label $ApiDnsLabel `
  --ports 5000 `
  --os-type Linux `
  --environment-variables `
    ASPNETCORE_URLS="http://+:5000" `
    ASPNETCORE_ENVIRONMENT="Production" `
    "ConnectionStrings__DefaultConnection=$ConnectionString" `
    Jwt__Secret="sdgkjhgeasrdgtfhyjgbvxcgjmhtfdrsdhtfjgkhfsdgjkmhngfjygkujhdgrsdhfkfsfh" `
    Jwt__Issuer="EMSApi" `
    Jwt__Audience="EMSApiUsers" `
    Jwt__ExpirationMinutes="60" `
  --registry-login-server $RegistryUrl `
  --registry-username $AcrUsername `
  --registry-password $AcrPassword `
  --cpu 1 `
  --memory 1

Write-Host "[OK] API Container deployed`n" -ForegroundColor Green

# Get API endpoint
$ApiFqdn = az container show `
  --resource-group $ResourceGroup `
  --name ems-api-container `
  --query ipAddress.fqdn `
  --output tsv

Write-Host "API FQDN: $ApiFqdn`:5000`n"

# ============================================================================
# STEP 7: Deploy Frontend Container
# ============================================================================

Write-Host "Step 7: Deploying Frontend Container..." -ForegroundColor Yellow

$FrontendDnsLabel = "ems-frontend-$RandomSuffix"

az container create `
  --resource-group $ResourceGroup `
  --name ems-frontend-container `
  --image "$RegistryUrl/ems-frontend:latest" `
  --dns-name-label $FrontendDnsLabel `
  --ports 80 `
  --os-type Linux `
  --environment-variables `
    API_URL="http://$ApiFqdn:5000" `
    ASPNETCORE_ENVIRONMENT="Production" `
  --registry-login-server $RegistryUrl `
  --registry-username $AcrUsername `
  --registry-password $AcrPassword `
  --cpu 1 `
  --memory 1

Write-Host "[OK] Frontend Container deployed`n" -ForegroundColor Green

# ============================================================================
# STEP 8: Verification
# ============================================================================

Write-Host "Step 8: Verifying Deployment..." -ForegroundColor Yellow

$FrontendFqdn = az container show `
  --resource-group $ResourceGroup `
  --name ems-frontend-container `
  --query ipAddress.fqdn `
  --output tsv

Start-Sleep -Seconds 10

Write-Host "`nContainer Status:" -ForegroundColor Yellow
az container list `
  --resource-group $ResourceGroup `
  --query "[].{Name:name, Status:instanceView.state, FQDN:ipAddress.fqdn}" `
  --output table

# ============================================================================
# DEPLOYMENT COMPLETE
# ============================================================================

Write-Host "`n============================================================" -ForegroundColor Green
Write-Host "           Azure Deployment Complete!                      " -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green

Write-Host "`nDeployment Summary:" -ForegroundColor Yellow
Write-Host "  Registry: $RegistryName"
Write-Host "  Resource Group: $ResourceGroup"
Write-Host "  Location: $Location"

Write-Host "`nAccess Your Application:" -ForegroundColor Yellow
Write-Host "  Frontend: http://$FrontendFqdn" -ForegroundColor Green
Write-Host "  API: http://$ApiFqdn`:5000" -ForegroundColor Green
Write-Host "  API Health: http://$ApiFqdn`:5000/health" -ForegroundColor Green
Write-Host "  Swagger: http://$ApiFqdn`:5000/swagger" -ForegroundColor Green

Write-Host "`nSaved Configuration:" -ForegroundColor Yellow
Write-Host "  Resource Group: $ResourceGroup"
Write-Host "  Registry: $RegistryName"
Write-Host "  SQL Server: $SqlServerName"

Write-Host "`nNext Steps:" -ForegroundColor Yellow
Write-Host "  1. Test the application at: http://$FrontendFqdn"
Write-Host "  2. Monitor logs: az container logs -g $ResourceGroup -n ems-api-container"
Write-Host "  3. Configure custom domain and SSL/TLS"
Write-Host "  4. Set up Application Gateway for HTTPS"
Write-Host "`n"