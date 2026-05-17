# EMS Virtual Event Management System

**A complete Docker-based Virtual Event Management System with Azure Cloud Deployment**

---

## 📋 Project Overview

The EMS (Event Management System) is a modern web application for managing virtual events. It consists of:
- **Backend**: ASP.NET Core 8.0 Web API
- **Frontend**: Angular 17+ SPA with Nginx reverse proxy
- **Database**: SQL Server 2022

---

## 🚀 Quick Start (Local Deployment)

### Prerequisites
- Docker Desktop
- SQL Server 2022 (local instance)
- Ports 80 and 5000 available

### Deploy Locally
```bash
cd c:\EMS_Project
DEPLOY-FINAL-FIXED.bat
```

### Access Application
- Frontend: http://localhost
- API: http://localhost:5000/health
- Swagger: http://localhost:5000/swagger

---

## ☁️ Azure Deployment

For cloud deployment using Azure Container Registry (ACR) and Azure Container Instances (ACI), see [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md)

### Quick Steps
1. Set up Azure resources
2. Push images to ACR
3. Deploy to ACI
4. Access via public URL

---

## 📁 Project Structure

```
EMS.API/
├── Controllers/        # API endpoints
├── Services/          # Business logic
├── Models/           # Data models
└── Dockerfile        # Container definition

EMS.Frontend/
├── src/              # Angular application
├── Dockerfile        # Container definition
└── nginx.conf        # Reverse proxy config

EMS.DAL/
├── Entities/        # Database entities
├── DbContext.cs     # Entity Framework context
└── Migrations/      # Database migrations

docker-compose.yml   # Local orchestration
DEPLOYMENT_GUIDE.md  # Deployment documentation
```

---

## 🔧 Docker Configuration

### Containers
- **API**: Port 5000 (.NET 8.0)
- **Frontend**: Port 80 (Nginx + Angular)
- **Database**: localhost:1433 (local SQL Server)

### Network
- Bridge network: `ems-network`
- Services communicate via DNS names

---

## 📚 Documentation

| File | Purpose |
|------|---------|
| **README.md** | This file - Project overview |
| **DEPLOYMENT_GUIDE.md** | Local Docker deployment |
| **AZURE_DEPLOYMENT.md** | Azure cloud deployment |
| **Dockerfiles** | Container definitions |
| **docker-compose.yml** | Local orchestration |

---

## ✨ Features

✅ User Authentication & Authorization (JWT)
✅ Event Management (Create, Update, Delete, List)
✅ User Registration
✅ Event Participation
✅ RESTful API with Swagger documentation
✅ Responsive Angular UI
✅ Database migrations
✅ Docker containerization
✅ Azure cloud deployment

---

## 🔐 Security

- JWT token-based authentication
- Password hashing with bcrypt
- CORS enabled
- SQL Server encryption
- Non-root container users

---

## 📊 Tech Stack

| Layer | Technology |
|-------|-----------|
| **Frontend** | Angular 17, TypeScript, Nginx |
| **Backend** | ASP.NET Core 8.0, Entity Framework |
| **Database** | SQL Server 2022 |
| **Deployment** | Docker, Azure (ACR/ACI) |
| **Authentication** | JWT, bcrypt |

---

## 🚀 Deployment Methods

### Local Development
```bash
docker-compose up -d
```

### Production (Azure)
```bash
# See AZURE_DEPLOYMENT.md for complete instructions
```

---

## 🧪 Testing

### API Health Check
```bash
curl http://localhost:5000/health
```

### Frontend Verification
```bash
curl http://localhost
# Should return Angular app HTML
```

### Container Status
```bash
docker-compose ps
# Both containers should show (healthy)
```

---

## 📞 Support

### Common Issues

**API not responding**
- Check SQL Server connection
- View logs: `docker logs ems-api`

**Frontend not loading**
- Check Nginx configuration
- View logs: `docker logs ems-frontend`

**SQL Server not accessible**
- Verify SQL Server is running
- Check connection string in docker-compose.yml

---

## 📝 Getting Started

1. **Clone/Navigate to project**
   ```bash
   cd c:\EMS_Project
   ```

2. **For Local Development**
   ```bash
   DEPLOY-FINAL-FIXED.bat
   ```

3. **For Azure Deployment**
   - Follow [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md)

4. **Access Application**
   - Local: http://localhost
   - Azure: [Your Azure public IP]

---

## 🔄 Deployment Architecture

### Local Development
```
Docker Compose
├── Frontend (Port 80)
├── API (Port 5000)
└── Local SQL Server
```

### Azure Production
```
Azure Container Registry (ACR)
├── Frontend Image
├── API Image
└── SQL Azure Database (optional)

Azure Container Instances (ACI)
├── Frontend Container
├── API Container
└── Private Network
```

---

## ✅ Quality Assurance

- ✅ Docker containers run healthchecks
- ✅ API endpoints tested with curl
- ✅ Frontend loads and serves correctly
- ✅ Database connections verified
- ✅ Ready for production deployment

---

## 📖 Next Steps

1. **Local Testing** - Verify everything works locally
2. **Azure Setup** - Create Azure resources
3. **Image Push** - Push Docker images to ACR
4. **Cloud Deployment** - Deploy to ACI
5. **Production Access** - Access via public URL

---

## 📄 License

This project is created for educational purposes.

---

## 👨‍💻 Project Information

- **Type**: Web Application (Docker + Azure)
- **Language**: C# (Backend), TypeScript (Frontend)
- **Database**: SQL Server 2022
- **Deployment**: Docker & Azure

---

**Status**: ✅ Production Ready

Ready to deploy? Follow [DEPLOYMENT_GUIDE.md](./DEPLOYMENT_GUIDE.md) for local or [AZURE_DEPLOYMENT.md](./AZURE_DEPLOYMENT.md) for cloud deployment.
