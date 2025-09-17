'use client'

import { useState, useEffect } from 'react'
import { 
  Image, 
  Upload, 
  Search, 
  Filter, 
  Grid3X3, 
  List, 
  Edit, 
  Trash2, 
  Eye, 
  Download,
  MoreVertical,
  Plus,
  X,
  Star,
  StarOff
} from 'lucide-react'
import PhotoUpload from '../../../components/PhotoUpload'

interface Photo {
  id: number
  fileName: string
  filePath: string
  altText: string
  caption?: string
  projectId?: number
  projectTitle?: string
  isHomepageSlider: boolean
  status: number
  fileSize: number
  uploadDate?: string
  dimensions?: string
  url?: string
}

interface Project {
  id: number
  title: string
}

export default function PhotosPage() {
  const [photos, setPhotos] = useState<Photo[]>([])
  const [projects, setProjects] = useState<Project[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedProject, setSelectedProject] = useState<number | null>(null)
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid')
  const [showUpload, setShowUpload] = useState(false)
  const [selectedPhotos, setSelectedPhotos] = useState<number[]>([])
  const [showPreview, setShowPreview] = useState<Photo | null>(null)

  // Load photos and projects
  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        
        // Test API connection first
        try {
          const healthCheck = await fetch('http://localhost:5187/api/health')
          if (!healthCheck.ok) {
            throw new Error('API health check failed')
          }
        } catch (healthError) {
          console.warn('API health check failed, continuing with photo fetch...')
        }

        // Load photos with error handling
        try {
          const photosResponse = await fetch('http://localhost:5187/api/photos')
          if (photosResponse.ok) {
            const photosData = await photosResponse.json()
            // Handle both array response and object with data property
            const photosList = Array.isArray(photosData) ? photosData : (photosData.data || [])
            
            // Add mock upload date if not present and fix file paths
            const photosWithDates = photosList.map((photo: any) => {
              // Debug: log the photo object to see the structure
              console.log('Photo from API:', photo)
              
              return {
                ...photo,
                uploadDate: photo.uploadDate || photo.createdAt || new Date().toISOString(),
                dimensions: photo.dimensions || `${photo.width || 1920}x${photo.height || 1080}`,
                // Ensure filePath starts with / if it doesn't already
                filePath: photo.filePath?.startsWith('/') ? photo.filePath : `/uploads/photos/${photo.fileName || photo.id}.jpg`
              }
            })
            
            setPhotos(photosWithDates)
            console.log('Processed photos:', photosWithDates)
          } else {
            console.error('Photos API response not ok:', photosResponse.status)
            setPhotos([])
          }
        } catch (photosError) {
          console.error('Error loading photos:', photosError)
          setPhotos([])
        }

        // Load projects with error handling
        try {
          const projectsResponse = await fetch('http://localhost:5187/api/projects')
          if (projectsResponse.ok) {
            const projectsData = await projectsResponse.json()
            const projectsList = Array.isArray(projectsData) ? projectsData : (projectsData.data || [])
            setProjects(projectsList)
          } else {
            console.error('Projects API response not ok:', projectsResponse.status)
            setProjects([])
          }
        } catch (projectsError) {
          console.error('Error loading projects:', projectsError)
          setProjects([])
        }
        
      } catch (error) {
        console.error('Error loading data:', error)
        setPhotos([])
        setProjects([])
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  // Filter photos based on search and project
  const filteredPhotos = photos.filter(photo => {
    const matchesSearch = photo.fileName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         photo.altText.toLowerCase().includes(searchTerm.toLowerCase())
    const matchesProject = selectedProject === null || photo.projectId === selectedProject
    return matchesSearch && matchesProject
  })

  const handlePhotoUpload = (uploadedPhotos: Photo[]) => {
    // Add missing properties to match interface
    const photosWithMetadata = uploadedPhotos.map(photo => ({
      ...photo,
      uploadDate: new Date().toISOString(),
      dimensions: '1920x1080' // Default dimensions
    }))
    setPhotos(prev => [...photosWithMetadata, ...prev])
    setShowUpload(false)
  }

  const handleDeletePhoto = async (photoId: number) => {
    if (!confirm('Bu fotoğrafı silmek istediğinizden emin misiniz?')) return

    try {
      const response = await fetch(`http://localhost:5187/api/photos/${photoId}`, {
        method: 'DELETE'
      })

      if (response.ok) {
        setPhotos(prev => prev.filter(p => p.id !== photoId))
        setSelectedPhotos(prev => prev.filter(id => id !== photoId))
      }
    } catch (error) {
      console.error('Error deleting photo:', error)
      alert('Fotoğraf silme hatası')
    }
  }

  const handleToggleSlider = async (photoId: number, isSlider: boolean) => {
    try {
      const response = await fetch(`http://localhost:5187/api/photos/${photoId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          IsHomepageSlider: isSlider
        })
      })

      if (response.ok) {
        setPhotos(prev => prev.map(p => 
          p.id === photoId ? { ...p, isHomepageSlider: isSlider } : p
        ))
      }
    } catch (error) {
      console.error('Error updating photo:', error)
    }
  }

  const handleSelectPhoto = (photoId: number) => {
    setSelectedPhotos(prev => {
      if (prev.includes(photoId)) {
        return prev.filter(id => id !== photoId)
      } else {
        return [...prev, photoId]
      }
    })
  }

  const handleBulkDelete = async () => {
    if (selectedPhotos.length === 0) return
    if (!confirm(`${selectedPhotos.length} fotoğrafı silmek istediğinizden emin misiniz?`)) return

    try {
      await Promise.all(
        selectedPhotos.map(photoId =>
          fetch(`http://localhost:5187/api/photos/${photoId}`, { method: 'DELETE' })
        )
      )

      setPhotos(prev => prev.filter(p => !selectedPhotos.includes(p.id)))
      setSelectedPhotos([])
    } catch (error) {
      console.error('Error deleting photos:', error)
      alert('Fotoğraf silme hatası')
    }
  }

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 Bytes'
    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
  }

  const getProjectTitle = (projectId?: number) => {
    if (!projectId) return 'Proje atanmamış'
    const project = projects.find(p => p.id === projectId)
    return project?.title || `Proje #${projectId}`
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span className="ml-3 text-gray-600">Fotoğraflar yükleniyor...</span>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Fotoğraf Galerisi</h1>
          <p className="text-gray-600 mt-2">Tüm proje fotoğraflarınızı yönetin ve düzenleyin</p>
        </div>
        <button
          onClick={() => setShowUpload(!showUpload)}
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
        >
          <Plus className="w-4 h-4" />
          <span>Fotoğraf Ekle</span>
        </button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div className="bg-white p-6 rounded-lg shadow-sm border">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Toplam Fotoğraf</p>
              <p className="text-2xl font-bold text-gray-900">{photos.length}</p>
            </div>
            <Image className="w-8 h-8 text-blue-600" />
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-sm border">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Slider Fotoğrafları</p>
              <p className="text-2xl font-bold text-gray-900">
                {photos.filter(p => p.isHomepageSlider).length}
              </p>
            </div>
            <Star className="w-8 h-8 text-yellow-600" />
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-sm border">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Toplam Boyut</p>
              <p className="text-2xl font-bold text-gray-900">
                {photos.length > 0 
                  ? formatFileSize(photos.reduce((total, photo) => total + (photo.fileSize || 0), 0))
                  : '0 Bytes'
                }
              </p>
            </div>
            <Upload className="w-8 h-8 text-green-600" />
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-sm border">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Filtrelenen</p>
              <p className="text-2xl font-bold text-gray-900">{filteredPhotos.length}</p>
            </div>
            <Filter className="w-8 h-8 text-purple-600" />
          </div>
        </div>
      </div>

      {/* Upload Modal */}
      {showUpload && (
        <div className="fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-4xl max-h-full overflow-auto w-full">
            <div className="flex items-center justify-between p-4 border-b">
              <h3 className="text-lg font-semibold">Yeni Fotoğraf Yükle</h3>
              <button
                onClick={() => setShowUpload(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X className="w-6 h-6" />
              </button>
            </div>
            <div className="p-4">
              <PhotoUpload onPhotosUploaded={handlePhotoUpload} />
            </div>
          </div>
        </div>
      )}

      {/* Search and Filters */}
      <div className="bg-white p-6 rounded-lg shadow-sm border">
        <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between space-y-4 lg:space-y-0">
          {/* Search */}
          <div className="relative flex-1 max-w-md">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
            <input
              type="text"
              placeholder="Fotoğraf ara..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10 pr-4 py-2 border border-gray-300 rounded-lg w-full focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
          </div>

          <div className="flex items-center space-x-4">
            {/* Project Filter */}
            <select
              value={selectedProject || ''}
              onChange={(e) => setSelectedProject(e.target.value ? parseInt(e.target.value) : null)}
              className="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">Tüm Projeler</option>
              {projects.map(project => (
                <option key={project.id} value={project.id}>
                  {project.title}
                </option>
              ))}
            </select>

            {/* View Mode Toggle */}
            <div className="flex bg-gray-100 rounded-lg p-1">
              <button
                onClick={() => setViewMode('grid')}
                className={`p-2 rounded ${viewMode === 'grid' ? 'bg-white shadow-sm' : ''}`}
              >
                <Grid3X3 className="w-4 h-4" />
              </button>
              <button
                onClick={() => setViewMode('list')}
                className={`p-2 rounded ${viewMode === 'list' ? 'bg-white shadow-sm' : ''}`}
              >
                <List className="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>

        {/* Bulk Actions */}
        {selectedPhotos.length > 0 && (
          <div className="mt-4 flex items-center justify-between bg-blue-50 p-3 rounded-lg">
            <span className="text-sm text-blue-800">
              {selectedPhotos.length} fotoğraf seçildi
            </span>
            <div className="space-x-2">
              <button
                onClick={handleBulkDelete}
                className="bg-red-600 text-white px-3 py-1 rounded text-sm hover:bg-red-700 transition-colors"
              >
                Seçilenleri Sil
              </button>
              <button
                onClick={() => setSelectedPhotos([])}
                className="bg-gray-600 text-white px-3 py-1 rounded text-sm hover:bg-gray-700 transition-colors"
              >
                Seçimi Temizle
              </button>
            </div>
          </div>
        )}
      </div>

      {/* Photos Grid/List */}
      {filteredPhotos.length === 0 ? (
        <div className="bg-white p-12 rounded-lg shadow-sm border text-center">
          <Image className="w-16 h-16 mx-auto mb-4 text-gray-400" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">Fotoğraf bulunamadı</h3>
          <p className="text-gray-600 mb-4">
            {searchTerm || selectedProject 
              ? 'Arama kriterlerinizi değiştirmeyi deneyin'
              : 'Henüz hiç fotoğraf yüklenmemiş'
            }
          </p>
          <button
            onClick={() => setShowUpload(true)}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
          >
            İlk Fotoğrafınızı Yükleyin
          </button>
        </div>
      ) : viewMode === 'grid' ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
          {filteredPhotos.map((photo) => (
            <div key={photo.id} className="bg-white rounded-lg shadow-sm border overflow-hidden group hover:shadow-md transition-shadow">
              <div className="relative">
                <img
                  src={photo.url || `http://localhost:5187${photo.filePath}`}
                  alt={photo.altText}
                  className="w-full h-48 object-cover"
                  onError={(e) => {
                    // If image fails to load, try different path formats
                    const target = e.currentTarget
                    const currentSrc = target.src
                    console.log('Image failed to load:', currentSrc, 'for photo:', photo)
                    
                    if (!currentSrc.includes('/api/photos/') && !currentSrc.includes('data:image')) {
                      // Try API endpoint
                      target.src = `http://localhost:5187/api/photos/${photo.id}/image`
                    } else if (currentSrc.includes('/api/photos/')) {
                      // Try base64 placeholder
                      target.src = `data:image/svg+xml;base64,${btoa('<svg xmlns="http://www.w3.org/2000/svg" width="400" height="300" viewBox="0 0 400 300"><rect width="400" height="300" fill="#f3f4f6"/><text x="200" y="150" text-anchor="middle" fill="#9ca3af" font-family="Arial" font-size="16">Görsel Yüklenemedi</text></svg>')}`
                    }
                  }}
                  onLoad={() => {
                    // Image loaded successfully
                  }}
                />
                
                {/* Overlay Actions */}
                <div className="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-50 transition-all duration-200 flex items-center justify-center opacity-0 group-hover:opacity-100">
                  <div className="flex space-x-2">
                    <button
                      onClick={() => setShowPreview(photo)}
                      className="bg-white text-gray-800 p-2 rounded-full hover:bg-gray-100 transition-colors"
                    >
                      <Eye className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => handleToggleSlider(photo.id, !photo.isHomepageSlider)}
                      className={`p-2 rounded-full transition-colors ${
                        photo.isHomepageSlider 
                          ? 'bg-yellow-500 text-white hover:bg-yellow-600' 
                          : 'bg-white text-gray-800 hover:bg-gray-100'
                      }`}
                    >
                      {photo.isHomepageSlider ? <Star className="w-4 h-4" /> : <StarOff className="w-4 h-4" />}
                    </button>
                    <button
                      onClick={() => handleDeletePhoto(photo.id)}
                      className="bg-red-500 text-white p-2 rounded-full hover:bg-red-600 transition-colors"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>

                {/* Selection Checkbox */}
                <div className="absolute top-2 left-2">
                  <input
                    type="checkbox"
                    checked={selectedPhotos.includes(photo.id)}
                    onChange={() => handleSelectPhoto(photo.id)}
                    className="w-4 h-4 text-blue-600 rounded focus:ring-blue-500"
                  />
                </div>

                {/* Slider Badge */}
                {photo.isHomepageSlider && (
                  <div className="absolute top-2 right-2">
                    <div className="bg-yellow-500 text-white px-2 py-1 rounded-full text-xs font-medium flex items-center">
                      <Star className="w-3 h-3 mr-1" />
                      Slider
                    </div>
                  </div>
                )}
              </div>

              <div className="p-4">
                <h4 className="font-medium text-gray-900 truncate" title={photo.fileName}>
                  {photo.fileName}
                </h4>
                <p className="text-sm text-gray-600 mt-1 truncate" title={getProjectTitle(photo.projectId)}>
                  {getProjectTitle(photo.projectId)}
                </p>
                <div className="flex items-center justify-between mt-2 text-xs text-gray-500">
                  <span>{formatFileSize(photo.fileSize)}</span>
                  {photo.dimensions && <span>{photo.dimensions}</span>}
                </div>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <div className="bg-white rounded-lg shadow-sm border overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    <input
                      type="checkbox"
                      checked={selectedPhotos.length === filteredPhotos.length && filteredPhotos.length > 0}
                      onChange={(e) => {
                        if (e.target.checked) {
                          setSelectedPhotos(filteredPhotos.map(p => p.id))
                        } else {
                          setSelectedPhotos([])
                        }
                      }}
                      className="w-4 h-4 text-blue-600 rounded focus:ring-blue-500"
                    />
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Önizleme
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Dosya Adı
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Proje
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Boyut
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Durum
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    İşlemler
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredPhotos.map((photo) => (
                  <tr key={photo.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <input
                        type="checkbox"
                        checked={selectedPhotos.includes(photo.id)}
                        onChange={() => handleSelectPhoto(photo.id)}
                        className="w-4 h-4 text-blue-600 rounded focus:ring-blue-500"
                      />
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <img
                        src={photo.url || `http://localhost:5187${photo.filePath}`}
                        alt={photo.altText}
                        className="w-12 h-12 object-cover rounded"
                        onError={(e) => {
                          const target = e.currentTarget
                          const currentSrc = target.src
                          console.log('List image failed to load:', currentSrc)
                          
                          if (!currentSrc.includes('/api/photos/') && !currentSrc.includes('data:image')) {
                            target.src = `http://localhost:5187/api/photos/${photo.id}/image`
                          } else if (currentSrc.includes('/api/photos/')) {
                            target.src = `data:image/svg+xml;base64,${btoa('<svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 48 48"><rect width="48" height="48" fill="#f3f4f6"/><text x="24" y="24" text-anchor="middle" fill="#9ca3af" font-family="Arial" font-size="10">IMG</text></svg>')}`
                          }
                        }}
                      />
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm font-medium text-gray-900">{photo.fileName}</div>
                      <div className="text-sm text-gray-500">{photo.altText}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {getProjectTitle(photo.projectId)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {formatFileSize(photo.fileSize)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center space-x-2">
                        {photo.isHomepageSlider && (
                          <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800">
                            <Star className="w-3 h-3 mr-1" />
                            Slider
                          </span>
                        )}
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                          photo.status === 1 ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                        }`}>
                          {photo.status === 1 ? 'Yayında' : 'Taslak'}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex items-center justify-end space-x-2">
                        <button
                          onClick={() => setShowPreview(photo)}
                          className="text-blue-600 hover:text-blue-900"
                        >
                          <Eye className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => handleToggleSlider(photo.id, !photo.isHomepageSlider)}
                          className="text-yellow-600 hover:text-yellow-900"
                        >
                          {photo.isHomepageSlider ? <Star className="w-4 h-4" /> : <StarOff className="w-4 h-4" />}
                        </button>
                        <button
                          onClick={() => handleDeletePhoto(photo.id)}
                          className="text-red-600 hover:text-red-900"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* Photo Preview Modal */}
      {showPreview && (
        <div className="fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-4xl max-h-full overflow-hidden">
            <div className="flex items-center justify-between p-4 border-b">
              <h3 className="text-lg font-semibold">{showPreview.fileName}</h3>
              <button
                onClick={() => setShowPreview(null)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X className="w-6 h-6" />
              </button>
            </div>
            <div className="p-4">
              <img
                src={showPreview.url || `http://localhost:5187${showPreview.filePath}`}
                alt={showPreview.altText}
                className="w-full h-auto max-h-96 object-contain"
                onError={(e) => {
                  const target = e.currentTarget
                  const currentSrc = target.src
                  console.log('Modal image failed to load:', currentSrc)
                  
                  if (!currentSrc.includes('/api/photos/') && !currentSrc.includes('data:image')) {
                    target.src = `http://localhost:5187/api/photos/${showPreview.id}/image`
                  } else if (currentSrc.includes('/api/photos/')) {
                    target.src = `data:image/svg+xml;base64,${btoa('<svg xmlns="http://www.w3.org/2000/svg" width="400" height="300" viewBox="0 0 400 300"><rect width="400" height="300" fill="#f3f4f6"/><text x="200" y="150" text-anchor="middle" fill="#9ca3af" font-family="Arial" font-size="16">Görsel Yüklenemedi</text></svg>')}`
                  }
                }}
              />
              <div className="mt-4 grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="font-medium">Dosya Adı:</span> {showPreview.fileName}
                </div>
                <div>
                  <span className="font-medium">Boyut:</span> {formatFileSize(showPreview.fileSize)}
                </div>
                <div>
                  <span className="font-medium">Proje:</span> {getProjectTitle(showPreview.projectId)}
                </div>
                <div>
                  <span className="font-medium">Alt Text:</span> {showPreview.altText}
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}