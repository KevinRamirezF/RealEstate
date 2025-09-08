# RealEstate Documentation Helper Script
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("start", "build", "serve", "deploy", "install", "clean", "help")]
    [string]$Command
)

function Show-Help {
    Write-Host @"
RealEstate Documentation Helper Script

Usage: ./docs.ps1 <command>

Commands:
  install   Install npm dependencies
  start     Start development server (http://localhost:3000)
  build     Build static site for production
  serve     Serve built site locally
  deploy    Deploy to GitHub Pages
  clean     Clear Docusaurus cache and node_modules
  help      Show this help message

Examples:
  ./docs.ps1 install    # First time setup
  ./docs.ps1 start      # Start development
  ./docs.ps1 build      # Build for production

"@ -ForegroundColor Cyan
}

function Invoke-DocsCommand {
    param([string]$cmd)
    
    Write-Host "📚 RealEstate Documentation - $cmd" -ForegroundColor Yellow
    Write-Host "Working directory: $(Get-Location)" -ForegroundColor Gray
    
    try {
        switch ($cmd) {
            "install" {
                Write-Host "📦 Installing dependencies..." -ForegroundColor Green
                npm install
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "✅ Dependencies installed successfully!" -ForegroundColor Green
                } else {
                    Write-Host "❌ Installation failed!" -ForegroundColor Red
                }
            }
            
            "start" {
                Write-Host "🚀 Starting development server..." -ForegroundColor Green
                Write-Host "The site will be available at: http://localhost:3000" -ForegroundColor Cyan
                npm run start
            }
            
            "build" {
                Write-Host "🏗️ Building static site..." -ForegroundColor Green
                npm run build
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "✅ Build completed! Files are in the 'build' directory." -ForegroundColor Green
                } else {
                    Write-Host "❌ Build failed!" -ForegroundColor Red
                }
            }
            
            "serve" {
                Write-Host "🌐 Serving built site..." -ForegroundColor Green
                if (!(Test-Path "build")) {
                    Write-Host "❌ No build directory found. Run './docs.ps1 build' first." -ForegroundColor Red
                    return
                }
                npm run serve
            }
            
            "deploy" {
                Write-Host "🚀 Deploying to GitHub Pages..." -ForegroundColor Green
                npm run deploy
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "✅ Deployment completed!" -ForegroundColor Green
                } else {
                    Write-Host "❌ Deployment failed!" -ForegroundColor Red
                }
            }
            
            "clean" {
                Write-Host "🧹 Cleaning cache and dependencies..." -ForegroundColor Yellow
                if (Test-Path "node_modules") {
                    Remove-Item -Recurse -Force "node_modules"
                    Write-Host "  Removed node_modules" -ForegroundColor Gray
                }
                if (Test-Path ".docusaurus") {
                    Remove-Item -Recurse -Force ".docusaurus"
                    Write-Host "  Removed .docusaurus cache" -ForegroundColor Gray
                }
                if (Test-Path "build") {
                    Remove-Item -Recurse -Force "build"
                    Write-Host "  Removed build directory" -ForegroundColor Gray
                }
                Write-Host "✅ Cleanup completed! Run './docs.ps1 install' to reinstall dependencies." -ForegroundColor Green
            }
            
            "help" {
                Show-Help
            }
            
            default {
                Write-Host "❌ Unknown command: $cmd" -ForegroundColor Red
                Show-Help
            }
        }
    }
    catch {
        Write-Host "❌ Error executing command: $_" -ForegroundColor Red
    }
}

# Check if we're in the docs directory
if (!(Test-Path "package.json")) {
    Write-Host "❌ No package.json found. Please run this script from the docs directory." -ForegroundColor Red
    exit 1
}

# Check if Node.js is installed
try {
    $nodeVersion = node --version
    Write-Host "Node.js version: $nodeVersion" -ForegroundColor Gray
} catch {
    Write-Host "❌ Node.js is not installed or not in PATH. Please install Node.js first." -ForegroundColor Red
    exit 1
}

# Execute the command
Invoke-DocsCommand -cmd $Command