#!/bin/bash

# EMS Azure Deployment Script
# This script automates the deployment of EMS application to Azure

set -e

# Color output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
RESOURCE_GROUP="ems-rg"
REGISTRY_NAME="emsacr"
LOCATION="eastus"
ACR_LOGIN_SERVER="${REGISTRY_NAME}.azurecr.io"
CONTAINER_GROUP_NAME="ems-app"
IMAGE_API="${ACR_LOGIN_SERVER}/ems-api:latest"
IMAGE_FRONTEND="${ACR_LOGIN_SERVER}/ems-frontend:latest"

# Functions
print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Step 1: Check prerequisites
check_prerequisites() {
    print_info "Checking prerequisites..."
    
    # Check Azure CLI
    if ! command -v az &> /dev/null; then
        print_error "Azure CLI is not installed"
        exit 1
    fi
    
    # Check Docker
    if ! command -v docker &> /dev/null; then
        print_error "Docker is not installed"
        exit 1
    fi
    
    print_info "Prerequisites check passed"
}

# Step 2: Azure login
azure_login() {
    print_info "Logging into Azure..."
    az login
    
    print_info "Setting subscription..."
    SUBSCRIPTION_ID=$(az account show --query id -o tsv)
    print_info "Using subscription: $SUBSCRIPTION_ID"
}

# Step 3: Create resource group
create_resource_group() {
    print_info "Creating resource group: $RESOURCE_GROUP"
    
    if az group exists --name $RESOURCE_GROUP | grep -q true; then
        print_warning "Resource group already exists"
    else
        az group create --name $RESOURCE_GROUP --location $LOCATION
        print_info "Resource group created successfully"
    fi
}

# Step 4: Create Azure Container Registry
create_container_registry() {
    print_info "Creating container registry: $REGISTRY_NAME"
    
    if az acr show --name $REGISTRY_NAME --resource-group $RESOURCE_GROUP &> /dev/null; then
        print_warning "Container registry already exists"
    else
        az acr create \
            --resource-group $RESOURCE_GROUP \
            --name $REGISTRY_NAME \
            --sku Basic \
            --admin-enabled true
        print_info "Container registry created successfully"
    fi
    
    # Get admin credentials
    print_info "Getting ACR credentials..."
    REGISTRY_USERNAME=$(az acr credential show --name $REGISTRY_NAME --query username -o tsv)
    REGISTRY_PASSWORD=$(az acr credential show --name $REGISTRY_NAME --query passwords[0].value -o tsv)
    
    print_info "ACR URL: $ACR_LOGIN_SERVER"
}

# Step 5: Build and push images
build_and_push_images() {
    print_info "Building Docker images..."
    
    # Build images
    docker build -t ems-api:latest -f EMS.API/Dockerfile . || {
        print_error "Failed to build API image"
        exit 1
    }
    
    docker build -t ems-frontend:latest -f EMS.Frontend/Dockerfile . || {
        print_error "Failed to build frontend image"
        exit 1
    }
    
    print_info "Logging into Azure Container Registry..."
    az acr login --name $REGISTRY_NAME
    
    print_info "Tagging and pushing images to ACR..."
    
    # Tag and push API
    docker tag ems-api:latest $IMAGE_API
    docker push $IMAGE_API
    print_info "API image pushed: $IMAGE_API"
    
    # Tag and push Frontend
    docker tag ems-frontend:latest $IMAGE_FRONTEND
    docker push $IMAGE_FRONTEND
    print_info "Frontend image pushed: $IMAGE_FRONTEND"
}

# Step 6: Deploy to Azure Container Instances
deploy_to_aci() {
    print_info "Deploying to Azure Container Instances..."
    
    # Check if container group already exists
    if az container show --resource-group $RESOURCE_GROUP --name $CONTAINER_GROUP_NAME &> /dev/null; then
        print_warning "Container group already exists. Deleting..."
        az container delete --resource-group $RESOURCE_GROUP --name $CONTAINER_GROUP_NAME --yes
    fi
    
    print_info "Creating container group..."
    az container create \
        --resource-group $RESOURCE_GROUP \
        --name $CONTAINER_GROUP_NAME \
        --image $IMAGE_API \
        --registry-login-server $ACR_LOGIN_SERVER \
        --registry-username $REGISTRY_USERNAME \
        --registry-password $REGISTRY_PASSWORD \
        --dns-name-label ems-app \
        --ports 80 5000 \
        --cpu 1.5 \
        --memory 2.0 \
        --environment-variables \
            ASPNETCORE_ENVIRONMENT=Production \
            ASPNETCORE_URLS="http://+:5000" \
            CORS__AllowedOrigins="*" \
        --protocol TCP
    
    print_info "Container deployed successfully"
}

# Step 7: Get deployment details
get_deployment_info() {
    print_info "Getting deployment information..."
    
    FQDN=$(az container show \
        --resource-group $RESOURCE_GROUP \
        --name $CONTAINER_GROUP_NAME \
        --query ipAddress.fqdn \
        -o tsv)
    
    IP=$(az container show \
        --resource-group $RESOURCE_GROUP \
        --name $CONTAINER_GROUP_NAME \
        --query ipAddress.ip \
        -o tsv)
    
    print_info "✅ Deployment Complete!"
    echo -e "\n${GREEN}Deployment Information:${NC}"
    echo "  API FQDN: http://${FQDN}:5000"
    echo "  API Health: http://${FQDN}:5000/health"
    echo "  Frontend: http://${FQDN}:80"
    echo "  Public IP: ${IP}"
    echo ""
}

# Main execution
main() {
    print_info "Starting EMS Azure Deployment"
    echo ""
    
    check_prerequisites
    azure_login
    create_resource_group
    create_container_registry
    build_and_push_images
    deploy_to_aci
    get_deployment_info
    
    print_info "Deployment script completed successfully!"
}

# Run main function
main
