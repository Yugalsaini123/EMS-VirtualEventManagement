# EMS Azure Deployment Script (PowerShell)
# This script automates the deployment of EMS application to Azure

param(
    [string]$ResourceGroup = "ems-rg",
    [string]$RegistryName = "emsacr",
    [string]$Location = "eastus",
    [string]$ContainerGroupName = "ems-app",
    [string]$EnvironmentName = "Production"
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Configuration
$ACRLoginServer = "$RegistryName.azurecr.io"
$ImageAPI = "$ACRLoginServer/ems-api:latest"
$ImageFrontend = "$ACRLoginServer/ems-frontend:latest"

# Helper Functions
function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Green
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

# Step 1: Check Prerequisites
function Check-Prerequisites {
    Write-Info "Checking prerequisites..."
    
    # Check Azure CLI
    $azCli = Get-Command az -ErrorAction SilentlyContinue
    if (-not $azCli) {
        Write-Error "Azure CLI is not installed. Download from: https://aka.ms/azurecli"
        exit 1
    }
    
    # Check Docker
    $docker = Get-Command docker -ErrorAction SilentlyContinue
    if (-not $docker) {
        Write-Error "Docker is not installed. Download from: https://www.docker.com/products/docker-desktop"
        exit 1
    }
    
    Write-Info "Prerequisites check passed"
}

# Step 2: Azure Login
function Login-Azure {
    Write-Info "Logging into Azure..."
    
    # Check if already logged in
    $account = az account show 2>$null
    if (-not $account) {
        az login
    }
    
    $subscriptionId = az account show --query id -o tsv
    Write-Info "Using subscription: $subscriptionId"
    
    return $subscriptionId
}

# Step 3: Create Resource Group
function Create-ResourceGroup {
    Write-Info "Creating resource group: $ResourceGroup in $Location"
    
    $exists = az group exists --name $ResourceGroup | ConvertFrom-Json
    
    if ($exists) {
        Write-Warning "Resource group already exists"
    } else {
        az group create --name $ResourceGroup --location $Location
        Write-Info "Resource group created successfully"
    }
}

# Step 4: Create Container Registry
function Create-ContainerRegistry {
    Write-Info "Creating container registry: $RegistryName"
    
    try {
        $registry = az acr show --name $RegistryName --resource-group $ResourceGroup --query name -o tsv 2>$null
        if ($registry) {
            Write-Warning "Container registry already exists"
        }
    } catch {
        Write-Info "Container registry does not exist. Creating..."
        az acr create `
            --resource-group $ResourceGroup `
            --name $RegistryName `
            --sku Basic `
            --admin-enabled true
        Write-Info "Container registry created successfully"
    }
    
    # Get admin credentials
    Write-Info "Getting ACR credentials..."
    $username = az acr credential show --name $RegistryName --query username -o tsv
    $password = az acr credential show --name $RegistryName --query passwords[0].value -o tsv
    
    Write-Info "ACR Login Server: $ACRLoginServer"
    
    return @{
        Username = $username
        Password = $password
    }
}

# Step 5: Build and Push Images
function Build-AndPush-Images {
    param([hashtable]$Credentials)
    
    Write-Info "Building Docker images..."
    
    # Build API image
    Write-Info "Building API image..."
    docker build -t ems-api:latest -f EMS.API/Dockerfile .
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to build API image"
        exit 1
    }
    
    # Build Frontend image
    Write-Info "Building Frontend image..."
    docker build -t ems-frontend:latest -f EMS.Frontend/Dockerfile .
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to build frontend image"
        exit 1
    }
    
    # Login to ACR
    Write-Info "Logging into Azure Container Registry..."
    az acr login --name $RegistryName
    
    # Push images
    Write-Info "Pushing images to ACR..."
    
    # Tag and push API
    docker tag ems-api:latest $ImageAPI
    docker push $ImageAPI
    Write-Info "API image pushed: $ImageAPI"
    
    # Tag and push Frontend
    docker tag ems-frontend:latest $ImageFrontend
    docker push $ImageFrontend
    Write-Info "Frontend image pushed: $ImageFrontend"
}

# Step 6: Deploy to ACI
function Deploy-ToACI {
    param([hashtable]$Credentials)
    
    Write-Info "Deploying to Azure Container Instances..."
    
    # Check if container group exists
    try {
        $container = az container show --resource-group $ResourceGroup --name $ContainerGroupName 2>$null
        if ($container) {
            Write-Warning "Container group already exists. Deleting..."
            az container delete --resource-group $ResourceGroup --name $ContainerGroupName --yes
        }
    } catch {
        # Container doesn't exist, continue
    }
    
    Write-Info "Creating container group..."
    az container create `
        --resource-group $ResourceGroup `
        --name $ContainerGroupName `
        --image $ImageAPI `
        --registry-login-server $ACRLoginServer `
        --registry-username $Credentials.Username `
        --registry-password $Credentials.Password `
        --dns-name-label ems-app `
        --ports 80 5000 `
        --cpu 1.5 `
        --memory 2.0 `
        --environment-variables `
            ASPNETCORE_ENVIRONMENT=$EnvironmentName `
            ASPNETCORE_URLS="http://+:5000" `
            CORS__AllowedOrigins="*" `
        --protocol TCP
    
    Write-Info "Container deployed successfully"
}

# Step 7: Get Deployment Information
function Get-DeploymentInfo {
    Write-Info "Getting deployment information..."
    
    $fqdn = az container show `
        --resource-group $ResourceGroup `
        --name $ContainerGroupName `
        --query ipAddress.fqdn `
        -o tsv
    
    $ip = az container show `
        --resource-group $ResourceGroup `
        --name $ContainerGroupName `
        --query ipAddress.ip `
        -o tsv
    
    Write-Info "✅ Deployment Complete!"
    Write-Host ""
    Write-Host "Deployment Information:" -ForegroundColor Green
    Write-Host "  API FQDN: http://${fqdn}:5000"
    Write-Host "  API Health: http://${fqdn}:5000/health"
    Write-Host "  Frontend: http://${fqdn}:80"
    Write-Host "  Public IP: ${ip}"
    Write-Host ""
}

# Step 8: Cleanup
function Cleanup-Resources {
    Write-Info "Cleaning up resources..."
    
    Write-Info "Removing container group..."
    az container delete --resource-group $ResourceGroup --name $ContainerGroupName --yes
    
    Write-Info "Removing container registry..."
    az acr delete --name $RegistryName --resource-group $ResourceGroup --yes
    
    Write-Info "Removing resource group..."
    az group delete --name $ResourceGroup --yes
    
    Write-Info "Cleanup completed"
}

# Main Execution
function Main {
    Write-Info "Starting EMS Azure Deployment"
    Write-Host ""
    
    try {
        Check-Prerequisites
        Login-Azure
        Create-ResourceGroup
        $credentials = Create-ContainerRegistry
        Build-AndPush-Images $credentials
        Deploy-ToACI $credentials
        Get-DeploymentInfo
        
        Write-Info "Deployment script completed successfully!"
    } catch {
        Write-Error "Deployment failed: $_"
        exit 1
    }
}

# Parse command line arguments
$cleanupMode = $PSBoundParameters.ContainsKey('Cleanup')

if ($cleanupMode) {
    Write-Warning "Running in cleanup mode. This will remove all resources."
    $confirm = Read-Host "Are you sure? (yes/no)"
    if ($confirm -eq "yes") {
        Cleanup-Resources
    } else {
        Write-Info "Cleanup cancelled"
    }
} else {
    Main
}
