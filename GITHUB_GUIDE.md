# GITHUB_GUIDE.md - Git Workflow & Version Control

## 🚀 Initial GitHub Setup

### Step 1: Create GitHub Repository

1. Go to https://github.com/new
2. Fill in repository details:
   - **Repository name**: `EMS-VirtualEventManagement` (or your choice)
   - **Description**: `Virtual Event Management System with Docker & Azure Deployment`
   - **Visibility**: Public (for submission) or Private
   - **Initialize**: Don't initialize (we have existing code)
3. Click **Create Repository**

### Step 2: Get Your Repository URL
```
https://github.com/YOUR_USERNAME/EMS-VirtualEventManagement.git
```

---

## 🔧 Push Existing Code to GitHub

### Step 1: Navigate to Project
```bash
cd c:\EMS_Project
```

### Step 2: Initialize Git (if not already done)
```bash
# Check if git initialized
git status

# If not initialized:
git init
git config user.name "Your Name"
git config user.email "your.email@example.com"
```

### Step 3: Create .gitignore
```bash
# Use provided .gitignore or create new one
```

### Step 4: Add Files (Excluding Unnecessary Ones)
```bash
# Add all necessary files
git add .

# Exclude specific files (make sure .gitignore handles these)
# node_modules/, bin/, obj/, dist/, .vs/, etc.
```

### Step 5: Create Initial Commit
```bash
git commit -m "initial: Add EMS project - Virtual Event Management System

- ASP.NET Core 8.0 API with clean architecture
- Angular 17+ frontend with responsive UI
- SQL Server database with Entity Framework migrations
- Docker containerization for API and frontend
- Nginx reverse proxy configuration
- Ready for Azure deployment (ACR + ACI)

Deployment: http://ems-frontend-28551.centralindia.azurecontainer.io/"
```

### Step 6: Add Remote and Push
```bash
# Add remote repository
git remote add origin https://github.com/YOUR_USERNAME/EMS-VirtualEventManagement.git

# Verify remote
git remote -v

# Push to GitHub
git branch -M main
git push -u origin main
```

---

## 🌿 Branching Strategy

### Main Branches

#### main (Production)
- Stable, tested code
- Only merge from release branches
- Always deployable

#### develop (Development)
- Integration branch
- Merges from feature branches
- Pre-production testing

### Feature Branches

Create feature branches from `develop`:

```bash
# Create and checkout feature branch
git checkout -b feature/user-authentication

# Work on feature
git add .
git commit -m "feat: implement JWT authentication

- Add AuthService for login/register
- Create JWT token generation
- Add authentication middleware
- Implement password hashing with bcrypt"

# Push to GitHub
git push origin feature/user-authentication
```

### Branch Naming Convention

```
feature/short-description      # New feature
fix/bug-description           # Bug fix
docs/documentation-update     # Documentation
refactor/code-improvement     # Code refactor
```

---

## 📝 Commit Message Convention

### Format
```
<type>(<scope>): <subject>

<body>

<footer>
```

### Examples

**Feature**:
```
feat(auth): add JWT authentication

- Implement JWT token generation
- Create login endpoint
- Add password hashing with bcrypt
- Implement token refresh mechanism

Closes #123
```

**Bug Fix**:
```
fix(api): resolve database connection timeout

The API was failing to connect to SQL Server on startup.
Changed connection string to use TCP/IP instead of named pipes.

Fixes #456
```

**Documentation**:
```
docs: update README with deployment instructions

Added comprehensive guide for Azure deployment
including step-by-step instructions and troubleshooting.
```

### Type Prefixes
- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation changes
- **style**: Formatting, missing semicolons, etc.
- **refactor**: Code refactoring
- **test**: Adding or updating tests
- **chore**: Dependencies, build scripts, etc.
- **perf**: Performance improvements

---

## 📋 .gitignore Template

Create `.gitignore` in project root:

```
# .NET
bin/
obj/
*.dll
*.exe
*.pdb
*.user
*.suo
.vs/
.vsconfig

# Node.js / Angular
node_modules/
dist/
.angular/
npm-debug.log
yarn-debug.log

# Environment & Secrets
.env
.env.local
.env.production
appsettings.local.json

# IDE
.vscode/
.idea/
*.swp
*.swo

# OS
.DS_Store
Thumbs.db

# Logs
*.log

# Temporary files
*.tmp
~*
```

---

## 🔄 Pull Request Workflow

### Create Feature Branch
```bash
git checkout -b feature/new-feature
# Make changes
git add .
git commit -m "feat: add new feature"
git push origin feature/new-feature
```

### Create Pull Request on GitHub
1. Go to repository on GitHub
2. Click **Compare & pull request**
3. Fill in PR details:
   - **Title**: Clear, descriptive title
   - **Description**: What changed, why, how to test
   - **Reviewers**: Assign team members
   - **Labels**: feat, bug, docs, etc.
4. Click **Create Pull Request**

### PR Template

```markdown
## Description
Briefly describe what this PR does.

## Type of Change
- [ ] New feature
- [ ] Bug fix
- [ ] Documentation update
- [ ] Refactoring

## Changes Made
- Change 1
- Change 2
- Change 3

## Testing Done
- How was this tested?
- Screenshots/evidence

## Checklist
- [ ] Code follows style guidelines
- [ ] Comments added for complex logic
- [ ] Documentation updated
- [ ] No new warnings generated
- [ ] Tests added/updated
- [ ] All tests passing
```

---

## 🔐 Protecting Sensitive Information

### What NOT to Commit

```
❌ .env files with real credentials
❌ appsettings.Production.json with passwords
❌ API keys or access tokens
❌ Private encryption keys
❌ SSH keys
❌ Database backups
❌ node_modules/ (too large)
❌ build artifacts (bin/, dist/)
```

### What TO Commit

```
✅ .env.example (template only)
✅ .gitignore
✅ Source code
✅ Documentation
✅ Configuration templates
✅ Docker files
✅ CI/CD configuration
```

### If Accidentally Committed

```bash
# Remove file from history
git rm --cached .env
git commit -m "chore: remove env file from tracking"

# Or use BFG Repo-Cleaner for large files
bfg --delete-files .env
```

---

## 📊 GitHub Features

### Issues
- Track bugs and features
- Assign to team members
- Label and organize
- Link to commits/PRs

Example:
```
Issue: Add email verification for registration
Labels: enhancement, auth
Assignee: @teammate
```

### Projects/Kanban Board
- Organize work into sprints
- Track progress visually
- Column: To Do, In Progress, Done

### Releases
- Tag versions
- Document changes
- Attach build artifacts

Example:
```
Version: v1.0.0
Tag: v1.0.0-prod
Date: May 17, 2026
```

### GitHub Pages (Optional)
- Host project documentation
- Automatic deployment from `docs/` folder
- Enable in repository settings

---

## 🔍 Code Review Guidelines

### Reviewer Checklist
- [ ] Code follows conventions
- [ ] Logic is correct and efficient
- [ ] Tests provided
- [ ] Documentation updated
- [ ] No security issues
- [ ] Performance acceptable
- [ ] Error handling included

### Approval Process
```
Developer creates PR
    ↓
Team reviews code
    ↓
Approve or request changes
    ↓
Developer addresses feedback
    ↓
Reapprove if changes acceptable
    ↓
Merge to develop
    ↓
Deploy to staging
    ↓
Test in staging
    ↓
Merge to main
    ↓
Deploy to production
```

---

## 🚀 Deployment Workflow

### Development to Production

```
Feature Branch
    ↓ (Merge PR)
develop branch
    ↓ (Create release branch)
release/v1.0.0
    ↓ (Final testing & fixes)
main branch (Production)
    ↓ (Tag as v1.0.0)
Deploy to Azure
```

### Git Commands for Release

```bash
# Create release branch
git checkout -b release/v1.0.0 develop

# Merge to main
git checkout main
git merge --no-ff release/v1.0.0
git tag -a v1.0.0 -m "Release version 1.0.0"

# Merge back to develop
git checkout develop
git merge --no-ff release/v1.0.0

# Delete release branch
git branch -d release/v1.0.0

# Push everything
git push origin main develop --tags
```

---

## 📈 GitHub Statistics

### Check Repository Stats
```bash
# Number of commits
git log --oneline | wc -l

# Number of contributors
git shortlog -sn

# File statistics
git ls-files | wc -l

# Repository size
git count-objects -v
```

---

## 🔗 Documentation Links

Include in your README:

```markdown
## Repository
- **GitHub**: [Your Repository URL]
- **Issues**: [GitHub Issues URL]
- **Discussions**: [GitHub Discussions URL]

## Quick Links
- [Project Board](https://github.com/your-username/repo/projects/1)
- [Releases](https://github.com/your-username/repo/releases)
- [Wiki](https://github.com/your-username/repo/wiki)
```

---

## ✅ Pre-Push Checklist

Before pushing code:

- [ ] Code builds successfully
- [ ] All tests passing
- [ ] No compiler warnings
- [ ] Code reviewed by team
- [ ] Documentation updated
- [ ] Commit messages clear
- [ ] No sensitive data in code
- [ ] .gitignore properly configured
- [ ] Branch name follows convention

---

## 🆘 Common Git Commands

```bash
# Check status
git status

# Stage files
git add .
git add filename.txt

# Unstage files
git reset filename.txt

# Commit
git commit -m "message"

# Amend last commit
git commit --amend

# View history
git log
git log --oneline
git log --graph --all --oneline

# Create branch
git checkout -b branch-name
git switch -c branch-name

# Switch branch
git checkout branch-name
git switch branch-name

# Delete branch
git branch -d branch-name
git branch -D branch-name

# Merge branches
git merge branch-name

# Rebase branch
git rebase main

# Stash changes
git stash
git stash pop

# Push changes
git push origin branch-name

# Pull changes
git pull origin branch-name

# Reset changes
git reset --hard HEAD~1
```

---

**Status**: ✅ GitHub Ready

**Last Updated**: May 17, 2026
