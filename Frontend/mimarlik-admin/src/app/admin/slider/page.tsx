'use client'

import { useState, useEffect } from 'react'
import { 
  Presentation, 
  Plus, 
  Edit, 
  Trash2, 
  Eye, 
  Star, 
  Move, 
  X, 
  Save,
  Play,
  Pause,
  ChevronLeft,
  ChevronRight,
  ChevronUp,
  ChevronDown
} from 'lucide-react'

interface SliderPhoto {
  id: number
  fileName: string
  filePath: string
  url: string
  altText: string
  sliderText: string
  sortOrder: number
  projectTitle?: string
}

interface Photo {
  id: number
  fileName: string
  filePath: string
  url: string
  altText: string
  isHomepageSlider: boolean
  projectId?: number
  projectTitle?: string
}

export default function SliderPage() {
  const [sliderPhotos, setSliderPhotos] = useState<SliderPhoto[]>([])
  const [allPhotos, setAllPhotos] = useState<Photo[]>([])
  const [projects, setProjects] = useState<any[]>([])
  const [loading, setLoading] = useState(true)
  const [showAddModal, setShowAddModal] = useState(false)
  const [showPreview, setShowPreview] = useState(false)
  const [editingPhoto, setEditingPhoto] = useState<SliderPhoto | null>(null)
  const [showReorderModal, setShowReorderModal] = useState(false)
  const [editText, setEditText] = useState('')
  
  // Preview states
  const [currentSlide, setCurrentSlide] = useState(0)
  const [isPlaying, setIsPlaying] = useState(true)

  // Load slider photos
  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true)
        
        // Load slider photos
        const sliderResponse = await fetch('http://localhost:5187/api/slider')
        if (sliderResponse.ok) {
          const sliderData = await sliderResponse.json()
          setSliderPhotos(sliderData.sort((a: SliderPhoto, b: SliderPhoto) => a.sortOrder - b.sortOrder))
        }

        // Load all photos for adding to slider
        const allPhotosResponse = await fetch('http://localhost:5187/api/photos')
        if (allPhotosResponse.ok) {
          const allPhotosData = await allPhotosResponse.json()
          setAllPhotos(allPhotosData.filter((photo: Photo) => !photo.isHomepageSlider))
        }

        // Load projects for displaying names
        const projectsResponse = await fetch('http://localhost:5187/api/projects')
        if (projectsResponse.ok) {
          const projectsData = await projectsResponse.json()
          setProjects(projectsData)
        }
      } catch (error) {
        console.error('Error loading data:', error)
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [])

  // Auto-advance slides in preview (faster transitions)
  useEffect(() => {
    if (!showPreview || !isPlaying || sliderPhotos.length === 0) return

    const interval = setInterval(() => {
      setCurrentSlide(prev => (prev + 1) % sliderPhotos.length)
    }, 3000) // Changed from 5000ms to 3000ms for faster transitions

    return () => clearInterval(interval)
  }, [showPreview, isPlaying, sliderPhotos.length])

  const handleAddToSlider = async (photoId: number, sliderText: string = '') => {
    try {
      const response = await fetch(`http://localhost:5187/api/slider/add-photo/${photoId}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ sliderText })
      })

      if (response.ok) {
        // Refresh data
        window.location.reload()
      } else {
        alert('Fotoğraf slider\'a eklenirken hata oluştu')
      }
    } catch (error) {
      console.error('Error adding to slider:', error)
      alert('Fotoğraf slider\'a eklenirken hata oluştu')
    }
  }

  const handleRemoveFromSlider = async (photoId: number) => {
    if (!confirm('Bu fotoğrafı slider\'dan kaldırmak istediğinizden emin misiniz?')) return

    try {
      const response = await fetch(`http://localhost:5187/api/slider/remove-photo/${photoId}`, {
        method: 'POST'
      })

      if (response.ok) {
        setSliderPhotos(prev => prev.filter(p => p.id !== photoId))
      } else {
        alert('Fotoğraf slider\'dan kaldırılırken hata oluştu')
      }
    } catch (error) {
      console.error('Error removing from slider:', error)
      alert('Fotoğraf slider\'dan kaldırılırken hata oluştu')
    }
  }

  const handleUpdateSliderText = async (photoId: number, sliderText: string) => {
    try {
      const response = await fetch(`http://localhost:5187/api/slider/photo/${photoId}/text`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ sliderText })
      })

      if (response.ok) {
        setSliderPhotos(prev => prev.map(p => 
          p.id === photoId ? { ...p, sliderText } : p
        ))
        setEditingPhoto(null)
        setEditText('')
      } else {
        alert('Slider metni güncellenirken hata oluştu')
      }
    } catch (error) {
      console.error('Error updating slider text:', error)
      alert('Slider metni güncellenirken hata oluştu')
    }
  }

  const startEditingText = (photo: SliderPhoto) => {
    setEditingPhoto(photo)
    setEditText(photo.sliderText)
  }

  const saveEditText = () => {
    if (editingPhoto) {
      handleUpdateSliderText(editingPhoto.id, editText)
    }
  }

  const cancelEdit = () => {
    setEditingPhoto(null)
    setEditText('')
  }

  const movePhotoUp = async (index: number) => {
    if (index === 0) return // Already at top
    
    const newPhotos = [...sliderPhotos]
    const temp = newPhotos[index]
    newPhotos[index] = newPhotos[index - 1]
    newPhotos[index - 1] = temp
    
    // Update sort orders
    const updatedPhotos = newPhotos.map((photo, idx) => ({
      ...photo,
      sortOrder: idx
    }))
    
    setSliderPhotos(updatedPhotos)
    await updateSortOrders(updatedPhotos)
  }

  const movePhotoDown = async (index: number) => {
    if (index === sliderPhotos.length - 1) return // Already at bottom
    
    const newPhotos = [...sliderPhotos]
    const temp = newPhotos[index]
    newPhotos[index] = newPhotos[index + 1]
    newPhotos[index + 1] = temp
    
    // Update sort orders
    const updatedPhotos = newPhotos.map((photo, idx) => ({
      ...photo,
      sortOrder: idx
    }))
    
    setSliderPhotos(updatedPhotos)
    await updateSortOrders(updatedPhotos)
  }

  const updateSortOrders = async (photos: SliderPhoto[]) => {
    try {
      await Promise.all(
        photos.map(photo => 
          fetch(`http://localhost:5187/api/photos/${photo.id}`, {
            method: 'PUT',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({
              altText: photo.altText,
              sortOrder: photo.sortOrder,
              isHomepageSlider: true,
              sliderText: photo.sliderText
            })
          })
        )
      )
    } catch (error) {
      console.error('Error updating sort order:', error)
      alert('Sıralama güncellenirken hata oluştu')
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span className="ml-3 text-gray-600">Slider fotoğrafları yükleniyor...</span>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Ana Sayfa Slider Yönetimi</h1>
          <p className="text-gray-600 mt-2">Ana sayfa slider fotoğraflarını yönetin ve düzenleyin</p>
        </div>
        <div className="flex space-x-3">
          <button
            onClick={() => setShowPreview(true)}
            className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center space-x-2"
          >
            <Eye className="w-4 h-4" />
            <span>Önizleme</span>
          </button>
          <button
            onClick={() => setShowReorderModal(true)}
            className="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center space-x-2"
          >
            <Move className="w-4 h-4" />
            <span>Sıralama</span>
          </button>
          <button
            onClick={() => setShowAddModal(true)}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
          >
            <Plus className="w-4 h-4" />
            <span>Fotoğraf Ekle</span>
          </button>
        </div>
      </div>

      {/* Stats Card */}
      <div className="bg-white p-6 rounded-lg shadow-sm border">
        <div className="flex items-center justify-between">
          <div>
            <p className="text-sm font-medium text-gray-600">Toplam Slider Fotoğrafı</p>
            <p className="text-3xl font-bold text-gray-900">{sliderPhotos.length}</p>
          </div>
          <Presentation className="w-12 h-12 text-blue-600" />
        </div>
      </div>

      {/* Slider Photos Grid */}
      {sliderPhotos.length === 0 ? (
        <div className="bg-white p-12 rounded-lg shadow-sm border text-center">
          <Presentation className="w-16 h-16 mx-auto mb-4 text-gray-400" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">Henüz slider fotoğrafı yok</h3>
          <p className="text-gray-600 mb-4">
            Ana sayfa slider'ına fotoğraf ekleyerek başlayın
          </p>
          <button
            onClick={() => setShowAddModal(true)}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
          >
            İlk Fotoğrafı Ekle
          </button>
        </div>
      ) : (
        <div className="bg-white p-6 rounded-lg shadow-sm border">
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-semibold">Slider Fotoğrafları</h3>
            <p className="text-sm text-gray-600">
              Sıralama için "Sıralama" butonunu kullanın
            </p>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {sliderPhotos.map((photo, index) => (
              <div key={photo.id} className="bg-white rounded-lg shadow-sm border overflow-hidden group transition-all duration-200 hover:shadow-md">
                <div className="relative">
                  <img
                    src={photo.url || `http://localhost:5187${photo.filePath}`}
                    alt={photo.altText}
                    className="w-full h-48 object-cover"
                  />
                  
                  {/* Order Badge */}
                  <div className="absolute top-2 right-2 z-20">
                    <div className="bg-blue-600 text-white px-2 py-1 rounded-full text-xs font-medium">
                      #{index + 1}
                    </div>
                  </div>

                  {/* Action Buttons */}
                  <div className="absolute bottom-2 right-2 flex space-x-1 opacity-0 group-hover:opacity-100 transition-opacity duration-200 z-20">
                    <button
                      onClick={() => startEditingText(photo)}
                      className="bg-white text-gray-800 p-1.5 rounded-full hover:bg-gray-100 transition-colors shadow-md"
                      title="Slider metnini düzenle"
                    >
                      <Edit className="w-3 h-3" />
                    </button>
                    <button
                      onClick={() => handleRemoveFromSlider(photo.id)}
                      className="bg-red-500 text-white p-1.5 rounded-full hover:bg-red-600 transition-colors shadow-md"
                      title="Slider'dan kaldır"
                    >
                      <Trash2 className="w-3 h-3" />
                    </button>
                  </div>
                </div>

                <div className="p-4">
                  <h4 className="font-medium text-gray-900 truncate" title={photo.fileName}>
                    {photo.fileName}
                  </h4>
                  <p className="text-sm text-gray-600 mt-1">
                    {photo.sliderText || 'Slider metni yok'}
                  </p>
                  {photo.projectTitle && (
                    <p className="text-xs text-gray-500 mt-1">
                      Proje: {photo.projectTitle}
                    </p>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Add Photo Modal */}
      {showAddModal && (
        <div className="fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-4xl max-h-full overflow-auto w-full">
            <div className="flex items-center justify-between p-4 border-b">
              <h3 className="text-lg font-semibold">Slider\'a Fotoğraf Ekle</h3>
              <button
                onClick={() => setShowAddModal(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X className="w-6 h-6" />
              </button>
            </div>
            <div className="p-4">
              {allPhotos.length === 0 ? (
                <div className="text-center py-8">
                  <p className="text-gray-600">Slider\'a eklenebilecek fotoğraf bulunamadı.</p>
                  <p className="text-sm text-gray-500 mt-2">Tüm fotoğraflar zaten slider\'da olabilir.</p>
                </div>
              ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  {allPhotos.map((photo) => (
                    <div key={photo.id} className="border rounded-lg overflow-hidden hover:shadow-md transition-shadow">
                      <img
                        src={photo.url || `http://localhost:5187${photo.filePath}`}
                        alt={photo.altText}
                        className="w-full h-32 object-cover"
                      />
                      <div className="p-3">
                        <h4 className="font-medium text-sm truncate">{photo.fileName}</h4>
                        {photo.projectId && (
                          <p className="text-xs text-gray-500 mt-1">
                            Proje: {projects.find(p => p.id === photo.projectId)?.title || `Proje #${photo.projectId}`}
                          </p>
                        )}
                        <div className="mt-2">
                          <button
                            onClick={() => {
                              handleAddToSlider(photo.id)
                              setShowAddModal(false)
                            }}
                            className="w-full bg-blue-600 text-white py-1 px-3 rounded text-sm hover:bg-blue-700 transition-colors"
                          >
                            Slider\'a Ekle
                          </button>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Edit Text Modal */}
      {editingPhoto && (
        <div className="fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-lg w-full">
            <div className="flex items-center justify-between p-4 border-b">
              <h3 className="text-lg font-semibold">Slider Metnini Düzenle</h3>
              <button
                onClick={cancelEdit}
                className="text-gray-500 hover:text-gray-700"
              >
                <X className="w-6 h-6" />
              </button>
            </div>
            <div className="p-4">
              <div className="mb-4">
                <img
                  src={editingPhoto.url || `http://localhost:5187${editingPhoto.filePath}`}
                  alt={editingPhoto.altText}
                  className="w-full h-32 object-cover rounded"
                />
              </div>
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Slider Metni
                </label>
                <textarea
                  value={editText}
                  onChange={(e) => setEditText(e.target.value)}
                  placeholder="Slider üzerinde gösterilecek metin..."
                  rows={3}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                />
              </div>
              <div className="flex space-x-3">
                <button
                  onClick={saveEditText}
                  className="flex-1 bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition-colors flex items-center justify-center space-x-2"
                >
                  <Save className="w-4 h-4" />
                  <span>Kaydet</span>
                </button>
                <button
                  onClick={cancelEdit}
                  className="flex-1 bg-gray-600 text-white py-2 px-4 rounded-lg hover:bg-gray-700 transition-colors"
                >
                  İptal
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Preview Modal */}
      {showPreview && (
        <div className="fixed inset-0 bg-black z-50 flex items-center justify-center">
          <div className="relative w-full h-full">
            {/* Close Button */}
            <button
              onClick={() => setShowPreview(false)}
              className="absolute top-4 right-4 z-10 bg-black bg-opacity-50 text-white p-2 rounded-full hover:bg-opacity-75 transition-colors"
            >
              <X className="w-6 h-6" />
            </button>

            {/* Play/Pause Button */}
            <button
              onClick={() => setIsPlaying(!isPlaying)}
              className="absolute top-4 left-4 z-10 bg-black bg-opacity-50 text-white p-2 rounded-full hover:bg-opacity-75 transition-colors"
            >
              {isPlaying ? <Pause className="w-6 h-6" /> : <Play className="w-6 h-6" />}
            </button>

            {sliderPhotos.length > 0 && (
              <>
                {/* Current Slide */}
                <div className="relative w-full h-full">
                  <img
                    src={sliderPhotos[currentSlide]?.url || `http://localhost:5187${sliderPhotos[currentSlide]?.filePath}`}
                    alt={sliderPhotos[currentSlide]?.altText}
                    className="w-full h-full object-cover"
                  />
                  
                  {/* Slider Text Overlay */}
                  {sliderPhotos[currentSlide]?.sliderText && (
                    <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black to-transparent p-8">
                      <h2 className="text-white text-3xl font-bold mb-2">
                        {sliderPhotos[currentSlide].sliderText}
                      </h2>
                    </div>
                  )}
                </div>

                {/* Navigation Arrows */}
                {sliderPhotos.length > 1 && (
                  <>
                    <button
                      onClick={() => setCurrentSlide(prev => prev === 0 ? sliderPhotos.length - 1 : prev - 1)}
                      className="absolute left-4 top-1/2 transform -translate-y-1/2 bg-black bg-opacity-50 text-white p-3 rounded-full hover:bg-opacity-75 transition-colors"
                    >
                      <ChevronLeft className="w-6 h-6" />
                    </button>
                    <button
                      onClick={() => setCurrentSlide(prev => (prev + 1) % sliderPhotos.length)}
                      className="absolute right-4 top-1/2 transform -translate-y-1/2 bg-black bg-opacity-50 text-white p-3 rounded-full hover:bg-opacity-75 transition-colors"
                    >
                      <ChevronRight className="w-6 h-6" />
                    </button>
                  </>
                )}

                {/* Dots Indicator */}
                {sliderPhotos.length > 1 && (
                  <div className="absolute bottom-4 left-1/2 transform -translate-x-1/2 flex space-x-2">
                    {sliderPhotos.map((_, index) => (
                      <button
                        key={index}
                        onClick={() => setCurrentSlide(index)}
                        className={`w-3 h-3 rounded-full transition-colors ${
                          index === currentSlide ? 'bg-white' : 'bg-white bg-opacity-50'
                        }`}
                      />
                    ))}
                  </div>
                )}

                {/* Slide Counter */}
                <div className="absolute bottom-4 right-4 bg-black bg-opacity-50 text-white px-3 py-1 rounded-full text-sm">
                  {currentSlide + 1} / {sliderPhotos.length}
                </div>
              </>
            )}
          </div>
        </div>
      )}

      {/* Reorder Modal */}
      {showReorderModal && (
        <div className="fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-4xl max-h-full overflow-auto w-full">
            <div className="flex items-center justify-between p-4 border-b">
              <h3 className="text-lg font-semibold">Slider Sıralaması</h3>
              <button
                onClick={() => setShowReorderModal(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <X className="w-6 h-6" />
              </button>
            </div>
            <div className="p-4">
              <p className="text-sm text-gray-600 mb-4">Yukarı/Aşağı butonları ile sıralama yapın</p>
              
              <div className="space-y-3">
                {sliderPhotos.map((photo, index) => (
                  <div key={photo.id} className="flex items-center gap-4 p-3 border rounded-lg bg-gray-50">
                    {/* Photo Preview */}
                    <div className="flex-shrink-0">
                      <img
                        src={photo.url || `http://localhost:5187${photo.filePath}`}
                        alt={photo.altText}
                        className="w-16 h-16 object-cover rounded"
                      />
                    </div>
                    
                    {/* Photo Info */}
                    <div className="flex-1 min-w-0">
                      <h4 className="font-medium text-gray-900 truncate">{photo.fileName}</h4>
                      <p className="text-sm text-gray-600 truncate">
                        {photo.sliderText || 'Slider metni yok'}
                      </p>
                      {photo.projectTitle && (
                        <p className="text-xs text-gray-500">
                          Proje: {photo.projectTitle}
                        </p>
                      )}
                    </div>
                    
                    {/* Order Badge */}
                    <div className="flex-shrink-0">
                      <div className="bg-blue-600 text-white px-3 py-1 rounded-full text-sm font-medium">
                        #{index + 1}
                      </div>
                    </div>
                    
                    {/* Move Buttons */}
                    <div className="flex flex-col gap-1">
                      <button
                        onClick={() => movePhotoUp(index)}
                        disabled={index === 0}
                        className={`p-1 rounded transition-colors ${
                          index === 0 
                            ? 'text-gray-300 cursor-not-allowed'
                            : 'text-gray-600 hover:text-blue-600 hover:bg-blue-50'
                        }`}
                        title="Yukarı taşı"
                      >
                        <ChevronUp className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => movePhotoDown(index)}
                        disabled={index === sliderPhotos.length - 1}
                        className={`p-1 rounded transition-colors ${
                          index === sliderPhotos.length - 1
                            ? 'text-gray-300 cursor-not-allowed'
                            : 'text-gray-600 hover:text-blue-600 hover:bg-blue-50'
                        }`}
                        title="Aşağı taşı"
                      >
                        <ChevronDown className="w-4 h-4" />
                      </button>
                    </div>
                  </div>
                ))}
              </div>
              
              <div className="flex justify-end mt-4">
                <button
                  onClick={() => setShowReorderModal(false)}
                  className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
                >
                  Tamamla
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

    </div>
  )
}