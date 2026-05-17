# Fix Docker Compose Issues - Rebuild and Start

Write-Host "🔧 Fixing EMS Docker Deployment..." -ForegroundColor Cyan
Write-Host ""

# Step 1: Stop and remove everything
Write-Host "1️⃣  Cleaning up existing containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.yml down -v

Write-Host "   ✅ Cleanup complete" -ForegroundColor Green
Start-Sleep -Seconds 3

# Step 2: Rebuild images
Write-Host ""
Write-Host "2️⃣  Building Docker images..." -ForegroundColor Yellow
docker-compose -f docker-compose.yml build --no-cache

if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ Build successful" -ForegroundColor Green
} else {
    Write-Host "   ❌ Build failed" -ForegroundColor Red
    exit 1
}

Start-Sleep -Seconds 3

# Step 3: Start containers
Write-Host ""
Write-Host "3️⃣  Starting containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.yml up -d

Write-Host "   ✅ Containers started" -ForegroundColor Green
Write-Host ""

# Step 4: Monitor status
Write-Host "4️⃣  Monitoring container status..." -ForegroundColor Yellow
Write-Host ""

$attempts = 0
$maxAttempts = 12  # 2 minutes max (12 * 10 seconds)

while ($attempts -lt $maxAttempts) {
    Clear-Host
    Write-Host "Docker Container Status (Attempt $($attempts + 1)/$maxAttempts)" -ForegroundColor Cyan
    Write-Host "============================================" -ForegroundColor Cyan
    
    docker-compose -f docker-compose.yml ps
    
    # Check if all containers are healthy
    $healthy = 0
    $output = docker-compose -f docker-compose.yml ps --format "json" 2>$null
    
    if ($output) {
        try {
            $containers = $output | ConvertFrom-Json
            if ($containers -is [array]) {
                $healthyCount = ($containers | Where-Object { $_.State -eq "running" }).Count
                $totalCount = $containers.Count
                
                if ($healthyCount -eq $totalCount -and $totalCount -gt 0) {
                    Write-Host ""
                    Write-Host "✅ All containers are running!" -ForegroundColor Green
                    break
                }
            }
        } catch {
            # JSON parsing might fail, continue checking
        }
    }
    
    $attempts++
    if ($attempts -lt $maxAttempts) {
        Write-Host ""
        Write-Host "Waiting for containers to stabilize... ($([Math]::Ceiling(($maxAttempts - $attempts) * 10))s remaining)" -ForegroundColor Yellow
        Start-Sleep -Seconds 10
    }
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📊 Final Container Status:" -ForegroundColor Cyan
docker-compose -f docker-compose.yml ps
Write-Host ""

Write-Host "🔗 Access Points:" -ForegroundColor Green
Write-Host "   • Frontend    → http://localhost"
Write-Host "   • API Health  → http://localhost:5000/health"
Write-Host "   • Swagger     → http://localhost:5000/swagger (dev only)"
Write-Host ""

Write-Host "📋 To view logs:" -ForegroundColor Green
Write-Host "   • API Logs:      docker logs ems-api"
Write-Host "   • Frontend Logs: docker logs ems-frontend"
Write-Host "   • DB Logs:       docker logs ems-mssql"
Write-Host ""

Write-Host "✅ Deployment fix complete!" -ForegroundColor Green
