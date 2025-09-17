'use client'

import { useState, useEffect } from 'react'
import { FolderOpen, Folder, Edit, Trash2, Plus, ChevronUp, ChevronDown } from 'lucide-react'

interface Category {
  id: number
  title: string
  description?: string
  slug: string
  parentId?: number
  status: number
  sortOrder: number
  children?: Category[]
}

interface CategoryManagerProps {
  onCategoryReorder?: (categories: Category[]) => void
}

export default function CategoryManager({ onCategoryReorder }: CategoryManagerProps) {
  const [categories, setCategories] = useState<Category[]>([])
  const [loading, setLoading] = useState(true)

  // Load categories from API
  useEffect(() => {
    loadCategories()
  }, [])

  const loadCategories = async () => {
    try {
      setLoading(true)
      const response = await fetch('http://localhost:5187/api/categories')
      const result = await response.json()
      const categoriesData = result.data || result || []
      setCategories(categoriesData)
    } catch (error) {
      console.error('Error loading categories:', error)
      // Mock data for development
      setCategories([
        { id: 1, title: 'Residential', description: 'Konut projeleri', slug: 'residential', status: 1, sortOrder: 0 },
        { id: 2, title: 'Commercial', description: 'Ticari projeler', slug: 'commercial', status: 1, sortOrder: 1 },
        { id: 3, title: 'Public Buildings', description: 'Kamu binaları', slug: 'public-buildings', status: 1, sortOrder: 2 },
      ])
    } finally {
      setLoading(false)
    }
  }

  // Move category up in order
  const moveCategoryUp = async (index: number) => {
    if (index === 0) return // Already at top
    
    const newCategories = [...categories]
    const temp = newCategories[index]
    newCategories[index] = newCategories[index - 1]
    newCategories[index - 1] = temp
    
    // Update sort orders
    const updatedCategories = newCategories.map((cat, idx) => ({
      ...cat,
      sortOrder: idx
    }))
    
    setCategories(updatedCategories)
    
    // Notify parent component
    if (onCategoryReorder) {
      onCategoryReorder(updatedCategories)
    }
    
    await updateCategoryOrder(updatedCategories)
  }

  // Move category down in order
  const moveCategoryDown = async (index: number) => {
    if (index === categories.length - 1) return // Already at bottom
    
    const newCategories = [...categories]
    const temp = newCategories[index]
    newCategories[index] = newCategories[index + 1]
    newCategories[index + 1] = temp
    
    // Update sort orders
    const updatedCategories = newCategories.map((cat, idx) => ({
      ...cat,
      sortOrder: idx
    }))
    
    setCategories(updatedCategories)
    
    // Notify parent component
    if (onCategoryReorder) {
      onCategoryReorder(updatedCategories)
    }
    
    await updateCategoryOrder(updatedCategories)
  }

  // Update category order in API
  const updateCategoryOrder = async (updatedCategories: Category[]) => {
    try {
      for (const category of updatedCategories) {
        await fetch(`http://localhost:5187/api/categories/${category.id}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            ...category,
            sortOrder: category.sortOrder
          })
        })
      }
    } catch (error) {
      console.error('Error updating category order:', error)
    }
  }

  const getStatusColor = (status: number) => {
    switch (status) {
      case 1: return 'bg-green-100 text-green-800 border-green-200'
      case 2: return 'bg-gray-100 text-gray-800 border-gray-200'
      case 3: return 'bg-yellow-100 text-yellow-800 border-yellow-200'
      default: return 'bg-gray-100 text-gray-800 border-gray-200'
    }
  }

  const getStatusText = (status: number) => {
    switch (status) {
      case 1: return 'Published'
      case 2: return 'Hidden'
      case 3: return 'Draft'
      default: return 'Unknown'
    }
  }

  if (loading) {
    return (
      <div className="bg-white p-6 rounded-lg shadow-sm border">
        <div className="flex justify-center items-center h-32">
          <div className="text-gray-500">Kategoriler yükleniyor...</div>
        </div>
      </div>
    )
  }

  return (
    <div className="bg-white p-6 rounded-lg shadow-sm border">
      <div className="flex justify-between items-center mb-6">
        <h3 className="text-lg font-semibold flex items-center">
          <FolderOpen className="w-5 h-5 mr-2" />
          Kategori Yönetimi
        </h3>
        <div className="text-sm text-gray-500">
          Butonlar ile sıralayın
        </div>
      </div>

      {categories.length === 0 ? (
        <div className="text-center text-gray-500 py-8">
          <FolderOpen className="w-12 h-12 mx-auto mb-4 text-gray-400" />
          <p>Henüz kategori eklenmemiş</p>
          <button className="mt-4 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2 mx-auto">
            <Plus className="w-4 h-4" />
            <span>İlk Kategoriyi Ekle</span>
          </button>
        </div>
      ) : (
        <div className="space-y-2">
          {categories.map((category, index) => (
            <div
              key={category.id}
              className="p-4 border rounded-lg transition-all duration-200 border-gray-200 hover:border-gray-300 hover:shadow-md"
            >
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-3">
                  {/* Reorder buttons */}
                  <div className="flex flex-col space-y-1">
                    <button
                      onClick={() => moveCategoryUp(index)}
                      disabled={index === 0}
                      className={`p-1 rounded ${
                        index === 0
                          ? 'text-gray-300 cursor-not-allowed'
                          : 'text-gray-600 hover:text-blue-600 hover:bg-blue-50'
                      }`}
                      title="Yukarı taşı"
                    >
                      <ChevronUp className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => moveCategoryDown(index)}
                      disabled={index === categories.length - 1}
                      className={`p-1 rounded ${
                        index === categories.length - 1
                          ? 'text-gray-300 cursor-not-allowed'
                          : 'text-gray-600 hover:text-blue-600 hover:bg-blue-50'
                      }`}
                      title="Aşağı taşı"
                    >
                      <ChevronDown className="w-4 h-4" />
                    </button>
                  </div>
                  
                  {/* Category icon */}
                  <Folder className="w-6 h-6 text-blue-600" />
                  
                  {/* Category info */}
                  <div>
                    <h4 className="font-medium text-gray-900">{category.title}</h4>
                    {category.description && (
                      <p className="text-sm text-gray-600">{category.description}</p>
                    )}
                    <div className="flex items-center space-x-2 mt-1">
                      <span className="text-xs text-gray-500">#{category.sortOrder}</span>
                      <span className="text-xs text-gray-500">•</span>
                      <span className="text-xs text-gray-500">{category.slug}</span>
                    </div>
                  </div>
                </div>

                <div className="flex items-center space-x-3">
                  {/* Status badge */}
                  <span className={`px-2 py-1 text-xs rounded-full border ${getStatusColor(category.status)}`}>
                    {getStatusText(category.status)}
                  </span>
                  
                  {/* Child categories count */}
                  {category.children && category.children.length > 0 && (
                    <span className="bg-blue-100 text-blue-800 px-2 py-1 text-xs rounded-full">
                      {category.children.length} alt kategori
                    </span>
                  )}
                  
                  {/* Action buttons */}
                  <div className="flex space-x-1">
                    <button 
                      className="text-blue-600 hover:text-blue-800 p-1 rounded hover:bg-blue-50"
                      title="Düzenle"
                    >
                      <Edit className="w-4 h-4" />
                    </button>
                    <button 
                      className="text-red-600 hover:text-red-800 p-1 rounded hover:bg-red-50"
                      title="Sil"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Add category button */}
      {categories.length > 0 && (
        <div className="mt-6 flex justify-center">
          <button className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2">
            <Plus className="w-4 h-4" />
            <span>Yeni Kategori Ekle</span>
          </button>
        </div>
      )}
    </div>
  )
}