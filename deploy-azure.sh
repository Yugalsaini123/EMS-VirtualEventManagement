#!/bin/bash
# Azure Deployment Script for EMS Virtual Event Management System
# This script automates the entire deployment process to Azure

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║     EMS Azure Deployment Script (Linux/Mac)              ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"
echo

# ============================================================================
# CONFIGURATION
# ============================================================================

read -p "Enter Azure Resource Group name (default: ems-resource-group): " RESOURCE_GROUP
RESOURCE_GROUP=${RESOURCE_GROUP:-ems-resource-group}

read -p "Enter Azure location (default: eastus): " LOCATION
LOCATION=${LOCATION:-eastus}

read -p "Enter Registry name (no special chars, default: emsregistry$(date +%s | tail -c 5)): " REGISTRY_NAME
REGISTRY_NAME=${REGISTRY_NAME:-emsregistry$(date +%s | tail -c 5)}

read -p "Enter SQL Server name (default: ems-sql-$(date +%s | tail -c 5)): " SQL_SERVER_NAME
SQL_SERVER_NAME=${SQL_SERVER_NAME:-ems-sql-$(date +%s | tail -c 5)}

read -p "Enter SQL Admin username (default: sqladmin): " SQL_ADMIN_USER
SQL_ADMIN_USER=${SQL_ADMIN_USER:-sqladmin}

read -sp "Enter SQL Admin password: " SQL_ADMIN_PASSWORD
echo

read -p "Use local SQL Server? (y/n, default: n): " USE_LOCAL_SQL
USE_LOCAL_SQL=${USE_LOCAL_SQL:-n}

# ============================================================================
# STEP 1: Login to Azure
# ============================================================================

echo -e "${YELLOW}Step 1: Logging in to Azure...${NC}"
az login

# ============================================================================
# STEP 2: Create Resource Group
# ============================================================================

echo -e "${YELLOW}Step 2: Creating Azure Resource Group...${NC}"
az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION"

echo -e "${GREEN}✅ Resource Group created: $RESOURCE_GROUP${NC}"

# ============================================================================
# STEP 3: Create Container Registry
# ============================================================================

echo -e "${YELLOW}Step 3: Creating Azure Container Registry...${NC}"
az acr create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$REGISTRY_NAME" \
  --sku Basic \
  --admin-enabled true

echo -e "${GREEN}✅ Container Registry created: $REGISTRY_NAME${NC}"

# Get ACR credentials
REGISTRY_URL="$REGISTRY_NAME.azurecr.io"
ACR_USERNAME=$(az acr credential show --resource-group "$RESOURCE_GROUP" --name "$REGISTRY_NAME" --query username --output tsv)
ACR_PASSWORD=$(az acr credential show --resource-group "$RESOURCE_GROUP" --name "$REGISTRY_NAME" --query passwords[0].value --output tsv)

echo "Registry URL: $REGISTRY_URL"
echo "Username: $ACR_USERNAME"

# ============================================================================
# STEP 4: Create SQL Database (if not using local)
# ============================================================================

if [ "$USE_LOCAL_SQL" = "n" ] || [ "$USE_LOCAL_SQL" = "N" ]; then
  echo -e "${YELLOW}Step 4: Creating Azure SQL Server...${NC}"
  
  az sql server create \
    --name "$SQL_SERVER_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --location "$LOCATION" \
    --admin-user "$SQL_ADMIN_USER" \
    --admin-password "$SQL_ADMIN_PASSWORD"
  
  echo -e "${YELLOW}Creating SQL Database...${NC}"
  az sql db create \
    --resource-group "$RESOURCE_GROUP" \
    --server "$SQL_SERVER_NAME" \
    --name EMSDatabase \
    --service-objective S0
  
  echo -e "${YELLOW}Configuring SQL Firewall...${NC}"
  az sql server firewall-rule create \
    --resource-group "$RESOURCE_GROUP" \
    --server "$SQL_SERVER_NAME" \
    --name AllowAzureServices \
    --start-ip-address 0.0.0.0 \
    --end-ip-address 0.0.0.0
  
  SQL_FQDN="$SQL_SERVER_NAME.database.windows.net"
  CONNECTION_STRING="Server=tcp:$SQL_FQDN,1433;Initial Catalog=EMSDatabase;Persist Security Info=False;User ID=$SQL_ADMIN_USER;Password=$SQL_ADMIN_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  
  echo -e "${GREEN}✅ SQL Database created: $SQL_SERVER_NAME${NC}"
else
  CONNECTION_STRING="Server=host.docker.internal,1433;Database=EMSDatabase;User Id=sa;Password=Admin@12345!;Encrypt=false;TrustServerCertificate=true;"
  echo -e "${GREEN}✅ Using local SQL Server${NC}"
fi

# ============================================================================
# STEP 5: Build and Push Docker Images
# ============================================================================

echo -e "${YELLOW}Step 5: Building Docker images...${NC}"

docker build -t ems-api:latest -f EMS.API/Dockerfile . || exit 1
docker build -t ems-frontend:latest -f EMS.Frontend/Dockerfile . || exit 1

echo -e "${YELLOW}Tagging images for ACR...${NC}"

docker tag ems-api:latest "$REGISTRY_URL/ems-api:latest"
docker tag ems-api:latest "$REGISTRY_URL/ems-api:v1"
docker tag ems-frontend:latest "$REGISTRY_URL/ems-frontend:latest"
docker tag ems-frontend:latest "$REGISTRY_URL/ems-frontend:v1"

echo -e "${YELLOW}Logging in to Azure Container Registry...${NC}"

az acr login --name "$REGISTRY_NAME"

echo -e "${YELLOW}Pushing images to ACR...${NC}"

docker push "$REGISTRY_URL/ems-api:latest"
docker push "$REGISTRY_URL/ems-api:v1"
docker push "$REGISTRY_URL/ems-frontend:latest"
docker push "$REGISTRY_URL/ems-frontend:v1"

echo -e "${GREEN}✅ Docker images pushed to ACR${NC}"

# ============================================================================
# STEP 6: Deploy API Container
# ============================================================================

echo -e "${YELLOW}Step 6: Deploying API Container...${NC}"

API_DNS_LABEL="ems-api-$(date +%s | tail -c 5)"

az container create \
  --resource-group "$RESOURCE_GROUP" \
  --name ems-api-container \
  --image "$REGISTRY_URL/ems-api:latest" \
  --dns-name-label "$API_DNS_LABEL" \
  --ports 5000 \
  --environment-variables \
    ASPNETCORE_URLS="http://+:5000" \
    ASPNETCORE_ENVIRONMENT="Production" \
    "ConnectionStrings__DefaultConnection=$CONNECTION_STRING" \
    Jwt__Secret="sdgkjhgeasrdgtfhyjgbvxcgjmhtfdrsdhtfjgkhfsdgjkmhngfjygkujhdgrsdhfkfsfh" \
    Jwt__Issuer="EMSApi" \
    Jwt__Audience="EMSApiUsers" \
    Jwt__ExpirationMinutes="60" \
  --registry-login-server "$REGISTRY_URL" \
  --registry-username "$ACR_USERNAME" \
  --registry-password "$ACR_PASSWORD" \
  --cpu 1 \
  --memory 1

echo -e "${GREEN}✅ API Container deployed${NC}"

# Get API endpoint
API_FQDN=$(az container show \
  --resource-group "$RESOURCE_GROUP" \
  --name ems-api-container \
  --query ipAddress.fqdn \
  --output tsv)

echo "API FQDN: $API_FQDN:5000"

# ============================================================================
# STEP 7: Deploy Frontend Container
# ============================================================================

echo -e "${YELLOW}Step 7: Deploying Frontend Container...${NC}"

FRONTEND_DNS_LABEL="ems-frontend-$(date +%s | tail -c 5)"

az container create \
  --resource-group "$RESOURCE_GROUP" \
  --name ems-frontend-container \
  --image "$REGISTRY_URL/ems-frontend:latest" \
  --dns-name-label "$FRONTEND_DNS_LABEL" \
  --ports 80 \
  --environment-variables \
    API_URL="http://$API_FQDN:5000" \
    ASPNETCORE_ENVIRONMENT="Production" \
  --registry-login-server "$REGISTRY_URL" \
  --registry-username "$ACR_USERNAME" \
  --registry-password "$ACR_PASSWORD" \
  --cpu 1 \
  --memory 1

echo -e "${GREEN}✅ Frontend Container deployed${NC}"

# ============================================================================
# STEP 8: Verification
# ============================================================================

echo -e "${YELLOW}Step 8: Verifying Deployment...${NC}"

FRONTEND_FQDN=$(az container show \
  --resource-group "$RESOURCE_GROUP" \
  --name ems-frontend-container \
  --query ipAddress.fqdn \
  --output tsv)

sleep 10

echo -e "${YELLOW}Container Status:${NC}"
az container list \
  --resource-group "$RESOURCE_GROUP" \
  --query "[].{Name:name, Status:instanceView.state, FQDN:ipAddress.fqdn}" \
  --output table

# ============================================================================
# DEPLOYMENT COMPLETE
# ============================================================================

echo
echo -e "${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║           🎉 Azure Deployment Complete! 🎉               ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"
echo
echo -e "${YELLOW}📊 Deployment Summary:${NC}"
echo "  Registry: $REGISTRY_NAME"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Location: $LOCATION"
echo
echo -e "${YELLOW}🌐 Access Your Application:${NC}"
echo "  Frontend: ${GREEN}http://$FRONTEND_FQDN${NC}"
echo "  API: ${GREEN}http://$API_FQDN:5000${NC}"
echo "  API Health: ${GREEN}http://$API_FQDN:5000/health${NC}"
echo "  Swagger: ${GREEN}http://$API_FQDN:5000/swagger${NC}"
echo
echo -e "${YELLOW}💾 Saved Configuration:${NC}"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  Registry: $REGISTRY_NAME"
echo "  SQL Server: $SQL_SERVER_NAME"
echo
echo -e "${YELLOW}📝 Next Steps:${NC}"
echo "  1. Test the application at: http://$FRONTEND_FQDN"
echo "  2. Monitor logs: az container logs -g $RESOURCE_GROUP -n ems-api-container"
echo "  3. Configure custom domain and SSL/TLS"
echo "  4. Set up Application Gateway for HTTPS"
echo
