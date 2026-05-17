# DEPLOYMENT.md - Production Deployment Guide

Live Demo: [EMS - Virtual Event Management System](http://ems-frontend-28551.centralindia.azurecontainer.io/)

## 🚀 Overview

This guide covers deploying the EMS application to Azure using Docker containers and Azure Container Instances (ACI).

---

## 📋 Prerequisites

- Azure subscription (active)
- Azure CLI installed (`az --version`)
- Docker Desktop installed
- Local application built and tested
- GitHub repository created and populated

---

## 🐳 Docker Deployment (Local)

### Build Docker Images

```bash
# Navigate to project root
cd c:\EMS_Project

# Build API image
docker build -t ems-api:latest -f EMS.API/Dockerfile .

# Build Frontend image
docker build -t ems-frontend:latest -f EMS.Frontend/Dockerfile .

# Verify images
docker images | findstr ems-
```

### Deploy Locally with Docker Compose

```bash
# Start services
docker-compose up -d

# Check status
docker-compose ps
# Both should show (healthy)

# View logs
docker-compose logs -f api
docker-compose logs -f frontend
```

### Access Local Application
- Frontend: http://localhost
- API: http://localhost:5000/health

---

## ☁️ Azure Deployment

### Step 1: Setup Azure Resources

```powershell
# Login to Azure
az login

# Create resource group
az group create --name ems-resource-group --location centralindia

# Create container registry
az acr create `
  --resource-group ems-resource-group `
  --name emsregistry$(Get-Random -Min 1000 -Max 9999) `
  --sku Basic `
  --admin-enabled true
```

### Step 2: Push Images to Azure Container Registry

```powershell
# Get registry credentials
$REGISTRY_NAME = "emsregistry1234"  # Your registry name
$REGISTRY_URL = "$REGISTRY_NAME.azurecr.io"
$ACR_USERNAME = az acr credential show --name $REGISTRY_NAME `
  --query username --output tsv
$ACR_PASSWORD = az acr credential show --name $REGISTRY_NAME `
  --query "passwords[0].value" --output tsv

# Login to ACR
az acr login --name $REGISTRY_NAME

# Tag images
docker tag ems-api:latest "$REGISTRY_URL/ems-api:latest"
docker tag ems-frontend:latest "$REGISTRY_URL/ems-frontend:latest"

# Push images
docker push "$REGISTRY_URL/ems-api:latest"
docker push "$REGISTRY_URL/ems-frontend:latest"
```

### Step 3: Deploy API Container

```powershell
az container create `
  --resource-group ems-resource-group `
  --name ems-api-container `
  --image "$REGISTRY_URL/ems-api:latest" `
  --dns-name-label ems-api-28551 `
  --ports 5000 `
  --environment-variables `
    ASPNETCORE_URLS="http://+:5000" `
    ASPNETCORE_ENVIRONMENT="Production" `
    "ConnectionStrings__DefaultConnection=Server=your-sql-server;Database=EMSDatabase;User Id=sa;Password=your-password;" `
    Jwt__Secret="your-secret-key" `
  --registry-login-server $REGISTRY_URL `
  --registry-username $ACR_USERNAME `
  --registry-password $ACR_PASSWORD `
  --cpu 1 `
  --memory 1
```

### Step 4: Deploy Frontend Container

```powershell
# Get API endpoint
$API_FQDN = az container show `
  --resource-group ems-resource-group `
  --name ems-api-container `
  --query ipAddress.fqdn --output tsv

az container create `
  --resource-group ems-resource-group `
  --name ems-frontend-container `
  --image "$REGISTRY_URL/ems-frontend:latest" `
  --dns-name-label ems-frontend-28551 `
  --ports 80 `
  --environment-variables `
    API_URL="http://$API_FQDN:5000" `
  --registry-login-server $REGISTRY_URL `
  --registry-username $ACR_USERNAME `
  --registry-password $ACR_PASSWORD `
  --cpu 1 `
  --memory 1
```

### Step 5: Access Live Application

```powershell
# Get endpoints
az container list --resource-group ems-resource-group `
  --query "[].{Name:name, Status:instanceView.state, URL:ipAddress.fqdn}"

# Test
curl http://ems-frontend-28551.centralindia.azurecontainer.io/
curl http://ems-api-28551.centralindia.azurecontainer.io:5000/health
```

---

## 🔒 Production Security

### Environment Variables (NOT hardcoded secrets)
```
✅ DO: Use environment variables for secrets
✅ DO: Store secrets in Azure Key Vault
✅ DO: Use Service Principal for authentication
❌ DON'T: Hardcode credentials in code
❌ DON'T: Commit .env files to GitHub
```

### Dockerfile Security
```dockerfile
# ✅ DO: Use specific base image versions
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

# ✅ DO: Run as non-root user
RUN useradd -m -u 1000 appuser
USER appuser

# ✅ DO: Mark read-only where possible
RUN chmod -R 555 /app

# ❌ DON'T: Use latest tag
# FROM mcr.microsoft.com/dotnet/aspnet:latest
```

### API Security
- Enable HTTPS/TLS
- CORS restricted to your domain
- Rate limiting enabled
- Input validation on all endpoints
- SQL injection prevention (parameterized queries)

---

## 📊 Monitoring & Logs

### View Container Logs
```powershell
# API logs
az container logs --resource-group ems-resource-group --name ems-api-container

# Frontend logs
az container logs --resource-group ems-resource-group --name ems-frontend-container

# Real-time logs
az container attach --resource-group ems-resource-group --name ems-api-container
```

### Container Status
```powershell
az container show --resource-group ems-resource-group --name ems-api-container `
  --query "{Name:name, State:instanceView.state, IP:ipAddress.fqdn}"
```

---

## 🔄 Updates & Redeployment

### Update Application

```bash
# Make code changes and commit
git add .
git commit -m "fix: update feature"
git push origin feature-branch
```

### Rebuild and Push Images

```powershell
# Rebuild locally
docker build -t ems-api:v2 -f EMS.API/Dockerfile .
docker build -t ems-frontend:v2 -f EMS.Frontend/Dockerfile .

# Push to registry
docker tag ems-api:v2 $REGISTRY_URL/ems-api:v2
docker push $REGISTRY_URL/ems-api:v2

docker tag ems-frontend:v2 $REGISTRY_URL/ems-frontend:v2
docker push $REGISTRY_URL/ems-frontend:v2
```

### Update Container with New Image

```powershell
# Delete old container
az container delete --resource-group ems-resource-group `
  --name ems-api-container --yes

# Create new container with v2 image
az container create `
  --resource-group ems-resource-group `
  --name ems-api-container `
  --image "$REGISTRY_URL/ems-api:v2" `
  ... (same parameters)
```

---

## 🧹 Cleanup

### Delete All Resources (Cost-saving)

```powershell
# Delete entire resource group (all resources)
az group delete --name ems-resource-group --yes

# Or delete specific resources
az container delete --resource-group ems-resource-group `
  --name ems-api-container --yes

az container delete --resource-group ems-resource-group `
  --name ems-frontend-container --yes

az acr delete --resource-group ems-resource-group `
  --name emsregistry1234 --yes
```

---

## 🆘 Troubleshooting

### Container Won't Start
```powershell
# Check logs
az container logs -g ems-resource-group -n ems-api-container

# Common issues:
# - Wrong connection string
# - SQL Server not accessible
# - Invalid environment variables
# - Port in use
```

### Frontend Can't Reach API
```powershell
# Verify both containers running
az container list -g ems-resource-group

# Check API is healthy
curl http://ems-api-28551.centralindia.azurecontainer.io:5000/health

# Verify API_URL in frontend env variables
```

### Database Connection Failed
```powershell
# Verify connection string
az container show -g ems-resource-group -n ems-api-container `
  --query "containers[0].environmentVariables"

# Test SQL Server access
# (Depends on whether using Azure SQL or local)
```

---

## 📋 Deployment Checklist

- [ ] Local application built and tested
- [ ] Docker images built successfully
- [ ] GitHub repository created with code
- [ ] Azure subscription active
- [ ] Azure CLI installed and logged in
- [ ] Resource group created
- [ ] Container Registry created
- [ ] Images pushed to ACR
- [ ] API container deployed
- [ ] Frontend container deployed
- [ ] Both containers healthy
- [ ] API responding to health check
- [ ] Frontend loads in browser
- [ ] Frontend can reach API
- [ ] Live URL documented
- [ ] Costs monitored
- [ ] Backups configured (if using Azure SQL)

---



---

## 📞 Support Resources

- [Azure CLI Documentation](https://learn.microsoft.com/cli/azure/)
- [Azure Container Instances](https://learn.microsoft.com/azure/container-instances/)
- [Docker Documentation](https://docs.docker.com/)
- [ASP.NET Core Deployment](https://learn.microsoft.com/aspnet/core/host-and-deploy/)

---

**Status**: ✅ Production Ready

**Live URL**: http://ems-frontend-28551.centralindia.azurecontainer.io/

**Last Updated**: May 17, 2026
