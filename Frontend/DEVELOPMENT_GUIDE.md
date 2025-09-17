# Mimarlik Admin Panel Development Guide

## Project Structure Created

```
Frontend/
└── mimarlik-admin/
    ├── src/
    │   ├── types/
    │   │   └── api.ts                    ✅ Created
    │   ├── services/
    │   │   ├── categoryService.ts        ✅ Created
    │   │   ├── projectService.ts         ✅ Created
    │   │   └── photoService.ts           ✅ Created
    │   └── components/
    │       └── Layout/
    │           ├── AdminLayout.tsx       🔄 In Progress
    │           └── Sidebar.tsx           🔄 In Progress
    └── setup-admin.bat                   ✅ Created
```

## Next Steps

1. **Run Setup Script**: Execute `setup-admin.bat` to install all dependencies
2. **Complete Layout Components**: Finish Sidebar, Header, and AdminLayout
3. **Implement Core Features**: Based on requirements analysis

## Requirements Implementation Checklist

### ✅ Already Addressed in Backend

- [x] Hierarchical category structure (ParentId field)
- [x] Project content blocks (ContentBlock entity)
- [x] Photo upload with watermark (FileService)
- [x] Translation system (Translation table)
- [x] Content status management (Published/Hidden/Draft)
- [x] SEO fields (metaTitle, metaDescription, metaKeywords)

### 🔄 Admin Panel Features to Implement

#### Core Content Management

- [ ] **Mouse-only interface** - No coding required for updates
- [ ] **Dynamic category creation** - Unlimited nesting with drag & drop
- [ ] **Project management** - Multiple content blocks per project
- [ ] **Photo management** - Upload, watermark, positioning
- [ ] **Photo gallery** - Click to enlarge, navigation between photos
- [ ] **Project isolation** - Only show project-specific photos when editing

#### Content Control

- [ ] **Suspend/Hide projects** - Toggle visibility
- [ ] **Safe deletion** - Remove all related content (photos, descriptions)
- [ ] **Homepage slider** - Select photos for homepage rotation
- [ ] **Independent slider photos** - Add photos not tied to projects

#### Display & Navigation

- [ ] **Responsive project grid** - Up to 15 columns, mobile single column
- [ ] **Photo navigation** - Arrow keys, mouse drag, touch swipe
- [ ] **SEO keyword management** - Per project and photo
- [ ] **Multi-language support** - Unlimited languages, hide/show

#### User Experience

- [ ] **Hide empty descriptions** - Don't show blank photo descriptions
- [ ] **Slider text overlay** - Text on homepage photos
- [ ] **Project preview** - Click photo → show project info + "Go to Project"
- [ ] **Simple admin interface** - WordPress-like usability

## Technology Stack

### Frontend (Admin Panel)

- **React 18** + TypeScript
- **Ant Design** - Comprehensive admin UI components
- **React Router** - Navigation
- **React Query** - API state management
- **React Hook Form** + Zod - Form handling & validation
- **React Beautiful DnD** - Drag & drop for categories/photos
- **React Dropzone** - File upload
- **Zustand** - Global state management
- **Tailwind CSS** - Styling

### Backend Integration

- **C# .NET API** (existing)
- **SQL Server LocalDB** (existing)
- **Entity Framework Core** (existing)

## Development Priority

1. **Phase 1: Foundation**

   - Install dependencies (setup-admin.bat)
   - Complete layout components
   - Setup routing
   - Basic authentication

2. **Phase 2: Core Features**

   - Category tree management
   - Project CRUD operations
   - Photo upload system
   - Translation management

3. **Phase 3: Advanced Features**

   - Drag & drop functionality
   - Slider management
   - SEO optimization
   - Bulk operations

4. **Phase 4: Polish**
   - Mobile responsiveness
   - User experience improvements
   - Performance optimization

## API Endpoints (Existing Backend)

- **Categories**: `/api/categories`
- **Projects**: `/api/projects`
- **Photos**: `/api/photos`
- **Languages**: `/api/languages`
- **Translations**: `/api/translations`
- **Slider**: `/api/slider`

## Installation Instructions

1. Open PowerShell as Administrator
2. Navigate to the project directory
3. Run the setup script:
   ```powershell
   cd "c:\Users\Eren\Desktop\Mimarlık\Frontend"
   .\setup-admin.bat
   ```
4. Once setup is complete, start development:
   ```powershell
   cd mimarlik-admin
   npm run dev
   ```

## Development Notes

- All components use TypeScript for type safety
- API services use native fetch() to avoid external dependencies initially
- UI components will use Ant Design for professional admin interface
- Tailwind CSS for custom styling and responsive design
- Forms use React Hook Form + Zod for validation
