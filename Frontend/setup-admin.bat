@echo off
echo Installing Mimarlik Admin Panel Dependencies...
cd /d \"c:\Users\Eren\Desktop\Mimarlık\Frontend\mimarlik-admin\"

echo Current directory: %CD%
echo Installing dependencies...

npm install
if %ERRORLEVEL% NEQ 0 (
    echo Failed to install dependencies
    pause
    exit /b 1
)

echo Installing additional dependencies...
npm install antd @ant-design/icons react-router-dom react-query axios react-hook-form @hookform/resolvers zod react-beautiful-dnd react-dropzone zustand react-image-crop dayjs

echo Installing dev dependencies...
npm install -D @types/react-beautiful-dnd tailwindcss autoprefixer postcss

echo Setting up Tailwind CSS...
npx tailwindcss init -p

echo Setup complete!
echo.
echo To start the development server:
echo cd Frontend\mimarlik-admin
echo npm run dev
pause