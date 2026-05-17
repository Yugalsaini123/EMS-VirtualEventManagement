# SETUP.md - Local Development Setup Guide

Live Demo: [EMS - Virtual Event Management System](http://ems-frontend-28551.centralindia.azurecontainer.io/)

## 📋 Prerequisites

### Required Software
- **Docker Desktop** - v20.10+ ([Download](https://www.docker.com/products/docker-desktop))
- **SQL Server 2022** - Local installation or Docker image
- **Git** - v2.30+ ([Download](https://git-scm.com/))
- **.NET SDK** - v8.0 (optional, for local development)
- **Node.js** - v18+ (optional, for frontend development)

### System Requirements
- Windows 10/11 or macOS or Linux
- 8GB RAM minimum
- 50GB disk space
- Ports 80, 5000, 1433 available

---

## 🚀 Quick Start (5 Minutes)

### Step 1: Clone Repository
```bash
git clone [https://github.com/Yugalsaini123/EMS-VirtualEventManagement.git]
cd EMS
```

### Step 2: Start Local Stack
```bash
docker-compose up -d
```

### Step 3: Verify
```bash
docker-compose ps
# Both ems-api and ems-frontend should show (healthy)
```

### Step 4: Access
- **Frontend**: http://localhost
- **API**: http://localhost:5000/health

---

## 📝 Detailed Setup

### 1. Install Docker Desktop

#### Windows/macOS
1. Download from https://www.docker.com/products/docker-desktop
2. Run installer and follow prompts
3. Restart computer
4. Verify: `docker --version`

#### Linux
```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install docker.io docker-compose
sudo usermod -aG docker $USER
```

### 2. Install SQL Server

#### Option A: Local Installation
- Download SQL Server 2022 Developer Edition
- Install with default settings
- Note connection: `localhost,1433` or `127.0.0.1,1433`
- Username: `sa`
- Password: Set during installation

#### Option B: Docker Container
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin@12345" \
  -p 1433:1433 \
  -d \
  mcr.microsoft.com/mssql/server:2022-latest
```

### 3. Clone Repository
```bash
git clone [https://github.com/Yugalsaini123/EMS-VirtualEventManagement.git]
cd EMS
git checkout develop  # or main, depending on your structure
```

### 4. Configure Environment

#### Create .env.local
```bash
# Copy and edit
cp .env.example .env.local
```

#### Edit .env.local with your values
```
SQL_SERVER=localhost,1433
DATABASE_NAME=EMSDatabase
SA_PASSWORD=Admin@12345
JWT_SECRET=your-super-secret-key-min-32-chars
JWT_ISSUER=EMSApi
JWT_AUDIENCE=EMSApiUsers
JWT_EXPIRATION_MINUTES=60
API_URL=http://localhost:5000
API_PORT=5000
FRONTEND_PORT=80
ANGULAR_ENVIRONMENT=development
```

### 5. Create Database

```bash
# Using Docker Compose (automatic)
docker-compose up -d

# OR manually with SQL Server
sqlcmd -S localhost,1433 -U sa -P Admin@12345 -Q "CREATE DATABASE EMSDatabase"
```

---

## 🐳 Running with Docker Compose

### Start Services
```bash
# Start all services
docker-compose up -d

# Watch logs in real-time
docker-compose logs -f
```

### Check Status
```bash
docker-compose ps
```

### View Logs
```bash
# All services
docker-compose logs

# Specific service
docker-compose logs api
docker-compose logs frontend

# Follow logs
docker-compose logs -f api
```

### Restart Services
```bash
# Restart specific service
docker-compose restart api
docker-compose restart frontend

# Rebuild and restart
docker-compose up -d --build
```

### Stop Services
```bash
# Stop all
docker-compose down

# Stop and remove volumes
docker-compose down -v

# Stop specific service
docker-compose stop api
```

---

## 💻 Local Development (Without Docker)

### Backend Setup

#### 1. Prerequisites
```bash
dotnet --version  # Should be 8.0+
```

#### 2. Navigate to API
```bash
cd EMS.API
```

#### 3. Install Dependencies
```bash
dotnet restore
```

#### 4. Apply Database Migrations
```bash
cd ../EMS.DAL
dotnet ef database update
cd ../EMS.API
```

#### 5. Run API
```bash
dotnet run
# API runs on: http://localhost:5000
```

### Frontend Setup

#### 1. Prerequisites
```bash
node --version   # Should be v18+
npm --version    # Should be v9+
```

#### 2. Navigate to Frontend
```bash
cd EMS.Frontend
```

#### 3. Install Dependencies
```bash
npm install
```

#### 4. Start Development Server
```bash
npm start
# Frontend runs on: http://localhost:4200
```

---

## ✔️ Verification Checklist

### Local Docker Setup
- [ ] Docker running: `docker ps`
- [ ] Docker Compose installed: `docker-compose --version`
- [ ] SQL Server accessible: `sqlcmd -S localhost,1433 -U sa`
- [ ] Containers started: `docker-compose ps` (shows healthy)
- [ ] API responding: `curl http://localhost:5000/health`
- [ ] Frontend loading: `curl http://localhost`

### Development Environment
- [ ] Git configured: `git config --global user.name`
- [ ] Repository cloned: `ls -la` shows EMS directory
- [ ] .env.local created: `ls -la .env.local`
- [ ] All services running: `docker-compose ps`

---

## 🔧 Configuration Files

### docker-compose.yml
Located in project root
- Defines API service
- Defines Frontend service
- Creates network
- Configures health checks
- Maps ports

### .env.local
Local environment variables
- Database connection
- JWT secrets
- API URLs
- Port configurations

### appsettings.json
Backend configuration
- Database configuration
- JWT settings
- CORS settings
- Logging configuration

### angular.json
Frontend configuration
- Build settings
- Development settings
- Production settings

---

## 🐛 Troubleshooting

### Issue: "Port 80 already in use"
```bash
# Find process using port 80
netstat -ano | findstr :80

# Option 1: Change port in docker-compose.yml (80:80 → 8080:80)
# Option 2: Kill process (Windows PowerShell as Admin)
Stop-Process -Id <PID> -Force
```

### Issue: "Cannot connect to SQL Server"
```bash
# Verify SQL Server running
sqlcmd -S localhost,1433 -U sa -P Admin@12345 -Q "SELECT 1"

# Check connection string in docker-compose.yml
# Should be: host.docker.internal,1433 (for local SQL Server)
```

### Issue: "API container not starting"
```bash
# Check logs
docker logs ems-api

# Common causes:
# 1. SQL Server not running
# 2. Wrong password in connection string
# 3. Database doesn't exist
```

### Issue: "Frontend showing blank page"
```bash
# Check Nginx
docker logs ems-frontend

# Verify API endpoint
docker exec ems-frontend curl http://localhost/

# Check if API accessible
curl http://localhost:5000/health
```

### Issue: "Database migrations failed"
```bash
# Reset database
sqlcmd -S localhost,1433 -U sa -P Admin@12345 \
  -Q "DROP DATABASE EMSDatabase; CREATE DATABASE EMSDatabase"

# Restart containers
docker-compose restart api
```

---

## 📚 Common Commands

### Docker
```bash
docker-compose up -d                # Start services
docker-compose down                 # Stop services
docker-compose ps                   # List containers
docker-compose logs -f api          # View logs
docker exec ems-api curl localhost:5000/health  # Test API
```

### Database
```bash
sqlcmd -S localhost,1433 -U sa -P Admin@12345
# Inside sqlcmd:
> SELECT name FROM sys.databases;
> USE EMSDatabase;
> SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;
> GO
```

### Git
```bash
git clone [https://github.com/Yugalsaini123/EMS-VirtualEventManagement.git]          # Clone repo
git checkout -b feature  # Create branch
git add .               # Stage changes
git commit -m "message" # Commit
git push origin feature # Push to GitHub
```

---

## ✨ Next Steps

1. **Verify Setup**: Run all checks in verification checklist
2. **Explore Code**: Navigate EMS.API, EMS.Frontend directories
3. **Test API**: Open http://localhost:5000/swagger
4. **Test Frontend**: Open http://localhost in browser
5. **Make Changes**: Create a branch and commit
6. **Push**: Push to GitHub

---

## 📖 Documentation

- **README.md** - Project overview
- **DEPLOYMENT.md** - Deploy to production
- **ARCHITECTURE.md** - System design
- **API_DOCUMENTATION.md** - API reference
- **GITHUB_GUIDE.md** - Git workflow

---

**Status**: ✅ Ready for Development

**Last Updated**: May 17, 2026
