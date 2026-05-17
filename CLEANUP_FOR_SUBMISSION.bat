#!/bin/bash
# CLEANUP_FOR_SUBMISSION.sh
# Run this script to clean up project before submission

echo "╔════════════════════════════════════════════════════════════════╗"
echo "║  EMS PROJECT CLEANUP FOR SUBMISSION                           ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""
echo "This script will remove unnecessary files and keep only what's needed."
echo ""

cd c:\EMS_Project

# Step 1: Remove unnecessary .md files (40+ files)
echo "STEP 1: Removing unnecessary documentation files..."
del "00_START_HERE.md" 2>nul
del "ACTION_PLAN.md" 2>nul
del "ADVANCED_TROUBLESHOOTING.md" 2>nul
del "AZURE_DEPLOYMENT.md" 2>nul
del "AZURE_DEPLOY_START.md" 2>nul
del "AZURE_QUICK_START.md" 2>nul
del "COMPLETE_STATUS_REPORT.md" 2>nul
del "DEPLOYMENT_CHECKLIST.md" 2>nul
del "DEPLOYMENT_GUIDE.md" 2>nul
del "DEPLOYMENT_GUIDE_FINAL.md" 2>nul
del "DEPLOYMENT_READY.md" 2>nul
del "DEPLOYMENT_STATUS.md" 2>nul
del "DEPLOYMENT_SUMMARY.txt" 2>nul
del "DOCKER_COMPOSE_CHANGES.md" 2>nul
del "DOCKER_DEPLOYMENT_FIXED.md" 2>nul
del "DOCKER_FIXES_APPLIED.md" 2>nul
del "DOCKER_TROUBLESHOOTING.md" 2>nul
del "DOCUMENTATION_INDEX.md" 2>nul
del "FILE_INDEX_AND_GUIDE.md" 2>nul
del "FINAL_DEPLOYMENT_SUMMARY.md" 2>nul
del "FINAL_SOLUTION.md" 2>nul
del "FINAL_STATUS.md" 2>nul
del "FIXES_COMPLETE_READY_DEPLOY.md" 2>nul
del "FIX_SUMMARY.md" 2>nul
del "IMPLEMENTATION_SUMMARY.md" 2>nul
del "INDEX.md" 2>nul
del "LOCAL_DATABASE_SETUP.md" 2>nul
del "LOCAL_SETUP_START_HERE.md" 2>nul
del "NGINX_FIX_APPLIED.md" 2>nul
del "POST_DEPLOYMENT_VERIFICATION.md" 2>nul
del "PRE_DEPLOYMENT_CHECKLIST.md" 2>nul
del "PROJECT_REVIEW_GUIDE.md" 2>nul
del "QUICK_FIX_GUIDE.md" 2>nul
del "QUICK_REFERENCE.md" 2>nul
del "QUICK_START_LOCAL.md" 2>nul
del "README_DEPLOYMENT.md" 2>nul
del "README_LOCAL_DEPLOYMENT.md" 2>nul
del "READY_FOR_AZURE_DEPLOYMENT.md" 2>nul
del "READY_TO_DEPLOY.md" 2>nul
del "REAL_SOLUTION.md" 2>nul
del "ROOT_CAUSE_ANALYSIS.md" 2>nul
del "SUBMISSION_PREP_GUIDE.md" 2>nul
del "START_DEPLOYMENT_NOW.md" 2>nul
del "START_HERE.md" 2>nul
del "START_DEPLOYMENT.txt" 2>nul
del "TESTING_GUIDE.md" 2>nul
del "VISUAL_QUICK_GUIDE.md" 2>nul
del "README_GITHUB.md" 2>nul

echo "✓ Removed unnecessary .md files"
echo ""

# Step 2: Remove .txt status files
echo "STEP 2: Removing temporary .txt status files..."
del "00_DEPLOYMENT_COMPLETE.txt" 2>nul
del "DEPLOY-NOW.txt" 2>nul
del "DEPLOYMENT_STATUS.txt" 2>nul
del "EXECUTE_DEPLOYMENT.txt" 2>nul
del "FILES_CLEANUP_PLAN.txt" 2>nul
del "FINAL_STATUS_READY.txt" 2>nul
del "GO-DEPLOY.txt" 2>nul
del "NGINX_FIX_STATUS.txt" 2>nul
del "READY_NOW.txt" 2>nul

echo "✓ Removed temporary .txt files"
echo ""

# Step 3: Remove .bat scripts
echo "STEP 3: Removing temporary .bat scripts..."
del "CLEANUP.bat" 2>nul
del "DEPLOY-FINAL.bat" 2>nul
del "DEPLOY-FINAL-FIXED.bat" 2>nul
del "deploy-fixed.bat" 2>nul
del "fix-deploy.bat" 2>nul
del "fix-frontend.bat" 2>nul

echo "✓ Removed temporary .bat files"
echo ""

# Step 4: List files to keep
echo "STEP 4: Files to keep for submission:"
echo "✓ README.md"
echo "✓ SETUP.md"
echo "✓ DEPLOYMENT.md"
echo "✓ ARCHITECTURE.md"
echo "✓ API_DOCUMENTATION.md"
echo "✓ GITHUB_GUIDE.md"
echo "✓ SUBMISSION_INSTRUCTIONS.md"
echo "✓ SUBMISSION_SUMMARY.md"
echo "✓ SUBMISSION_QUICK_REFERENCE.txt"
echo ""

# Step 5: Remove local env files
echo "STEP 5: Verifying sensitive files are NOT committed..."
if exist ".env" (
    echo "⚠ WARNING: .env file exists (should not be in git)"
)
if exist ".env.production" (
    echo "⚠ WARNING: .env.production file exists (should not be in git)"
)
echo "✓ Only .env.example should be in git"
echo ""

echo "╔════════════════════════════════════════════════════════════════╗"
echo "║  CLEANUP COMPLETE - PROJECT READY FOR GIT PUSH                ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""
echo "Next Steps:"
echo "1. git add ."
echo "2. git commit -m \"initial: Virtual Event Management System\""
echo "3. git push -u origin main"
echo ""
