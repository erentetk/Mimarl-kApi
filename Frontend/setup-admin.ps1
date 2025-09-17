# Mimarlik Admin Panel Setup Script
Write-Host \"Installing Mimarlik Admin Panel Dependencies...\" -ForegroundColor Green

$adminPath = \"c:\\Users\\Eren\\Desktop\\Mimarlık\\Frontend\\mimarlik-admin\"
Set-Location $adminPath

Write-Host \"Current directory: $(Get-Location)\" -ForegroundColor Yellow
Write-Host \"Installing dependencies...\" -ForegroundColor Yellow

try {
    npm install
    if ($LASTEXITCODE -ne 0) {
        throw \"NPM install failed\"
    }
    
    Write-Host \"Installing additional dependencies...\" -ForegroundColor Yellow
    npm install antd @ant-design/icons react-router-dom react-query axios react-hook-form @hookform/resolvers zod react-beautiful-dnd react-dropzone zustand react-image-crop dayjs
    
    Write-Host \"Installing dev dependencies...\" -ForegroundColor Yellow
    npm install -D @types/react-beautiful-dnd tailwindcss autoprefixer postcss
    
    Write-Host \"Setting up Tailwind CSS...\" -ForegroundColor Yellow
    npx tailwindcss init -p
    
    Write-Host \"Setup complete!\" -ForegroundColor Green
    Write-Host \"\"
    Write-Host \"To start the development server:\" -ForegroundColor Cyan
    Write-Host \"cd Frontend\\mimarlik-admin\" -ForegroundColor White
    Write-Host \"npm run dev\" -ForegroundColor White
}
catch {
    Write-Host \"Setup failed: $_\" -ForegroundColor Red
    exit 1
}

Read-Host \"Press Enter to continue\"