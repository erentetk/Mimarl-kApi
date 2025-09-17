'use client'

import { useState, useEffect } from 'react'
import Link from 'next/link'
import { usePathname } from 'next/navigation'
import { 
  LayoutDashboard,
  FolderOpen,
  Building,
  Image,
  Presentation,
  Globe,
  FileText,
  Search,
  ChevronLeft,
  ChevronRight
} from 'lucide-react'

interface SidebarProps {
  collapsed: boolean
}

const menuItems = [
  { id: 'dashboard', label: 'Dashboard', icon: LayoutDashboard, path: '/admin' },
  { id: 'categories', label: 'Categories', icon: FolderOpen, path: '/admin/categories' },
  { id: 'projects', label: 'Projects', icon: Building, path: '/admin/projects' },
  { id: 'photos', label: 'Photos', icon: Image, path: '/admin/photos' },
  { id: 'slider', label: 'Homepage Slider', icon: Presentation, path: '/admin/slider' },
  { id: 'languages', label: 'Languages', icon: Globe, path: '/admin/languages' },
  { id: 'translations', label: 'Translations', icon: FileText, path: '/admin/translations' },
  { id: 'seo', label: 'SEO Management', icon: Search, path: '/admin/seo' },
]

export default function Sidebar({ collapsed }: SidebarProps) {
  const pathname = usePathname()
  const [isClient, setIsClient] = useState(false)

  useEffect(() => {
    setIsClient(true)
  }, [])

  return (
    <div className={`fixed left-0 top-0 h-full bg-white shadow-lg transition-all duration-300 z-40 ${
      collapsed ? 'w-16' : 'w-64'
    }`}>
      {/* Header */}
      <div className="p-4 border-b border-gray-200">
        <div className="flex items-center">
          <div className="text-2xl font-bold text-blue-600">
            {collapsed ? 'M' : 'Mimarlık Admin'}
          </div>
        </div>
      </div>

      {/* Navigation */}
      <nav className="mt-4">
        <ul className="space-y-1">
          {menuItems.map((item) => {
            const IconComponent = item.icon
            const isActive = isClient && pathname === item.path
            
            return (
              <li key={item.id}>
                <Link
                  href={item.path}
                  className={`flex items-center px-4 py-3 transition-colors duration-200 ${
                    isActive 
                      ? 'bg-blue-50 text-blue-600 border-r-2 border-blue-600' 
                      : 'text-gray-700 hover:bg-blue-50 hover:text-blue-600'
                  }`}
                  title={collapsed ? item.label : ''}
                >
                  <IconComponent className="w-5 h-5 mr-3" />
                  {!collapsed && (
                    <span className="font-medium">{item.label}</span>
                  )}
                </Link>
              </li>
            )
          })}
        </ul>
      </nav>
    </div>
  )
}