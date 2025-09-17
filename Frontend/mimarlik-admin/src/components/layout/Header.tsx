'use client'

import { useState } from 'react'
import { Menu, Bell, User, Globe } from 'lucide-react'

interface HeaderProps {
  onToggleSidebar: () => void
}

export default function Header({ onToggleSidebar }: HeaderProps) {
  const [currentLanguage, setCurrentLanguage] = useState('tr')

  const languages = [
    { value: 'tr', label: 'Türkçe' },
    { value: 'en', label: 'English' },
    { value: 'de', label: 'Deutsch' },
  ]

  return (
    <header className="bg-white border-b border-gray-200 px-6 py-4">
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-4">
          <button
            onClick={onToggleSidebar}
            className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
          >
            <Menu className="w-5 h-5" />
          </button>
          <h1 className="text-xl font-semibold text-gray-800">
            Admin Panel
          </h1>
        </div>

        <div className="flex items-center space-x-4">
          {/* Language Selector */}
          <div className="flex items-center space-x-2">
            <Globe className="w-4 h-4 text-gray-500" />
            <select
              value={currentLanguage}
              onChange={(e) => setCurrentLanguage(e.target.value)}
              className="border border-gray-300 rounded-md px-3 py-1 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {languages.map(lang => (
                <option key={lang.value} value={lang.value}>
                  {lang.label}
                </option>
              ))}
            </select>
          </div>
          
          {/* Notifications */}
          <button className="relative p-2 hover:bg-gray-100 rounded-lg transition-colors">
            <Bell className="w-5 h-5" />
            <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
              3
            </span>
          </button>

          {/* User Menu */}
          <button className="flex items-center space-x-2 p-2 hover:bg-gray-100 rounded-lg transition-colors">
            <User className="w-5 h-5" />
            <span className="text-sm font-medium">Admin</span>
          </button>
        </div>
      </div>
    </header>
  )
}