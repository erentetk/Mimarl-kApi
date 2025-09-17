'use client'

import { useState, useCallback, useEffect } from 'react'
import { useDropzone } from 'react-dropzone'
import { Upload, X, ImageIcon, CheckCircle, Settings, Star } from 'lucide-react'

interface Photo {
  id: number
  fileName: string
  filePath: string
  altText: string
  caption?: string
  projectId?: number
  isHomepageSlider: boolean
  status: number
  fileSize: number
}

interface Project {
  id: number
  title: string
}

interface PhotoUploadProps {
  onPhotosUploaded?: (photos: Photo[]) => void
  projectId?: number
}

interface PhotoSettings {
  altText: string
  caption: string
  description: string
  projectId: number | ''
  status: number
  sortOrder: number
  isHomepageSlider: boolean
  sliderText: string
  addWatermark: boolean
}

export default function PhotoUpload({ onPhotosUploaded, projectId }: PhotoUploadProps) {
  const [files, setFiles] = useState<File[]>([])
  const [projects, setProjects] = useState<Project[]>([])
  const [uploading, setUploading] = useState(false)
  const [uploadProgress, setUploadProgress] = useState(0)
  const [showSettings, setShowSettings] = useState(true) // Always show settings
  const [photoSettings, setPhotoSettings] = useState<PhotoSettings>({
    altText: '',
    caption: '',
    description: '',
    projectId: projectId || '',
    status: 1, // Published
    sortOrder: 0,
    isHomepageSlider: false,
    sliderText: '',
    addWatermark: false
  })

  // Load projects on component mount
  useEffect(() => {
    const loadProjects = async () => {
      try {
        const response = await fetch('http://localhost:5187/api/projects')
        if (response.ok) {
          const projectsData = await response.json()
          setProjects(projectsData)
        }
      } catch (error) {
        console.error('Error loading projects:', error)
      }
    }
    
    loadProjects()
  }, [])

  const onDrop = useCallback((acceptedFiles: File[]) => {
    // Only accept image files
    const imageFiles = acceptedFiles.filter(file => file.type.startsWith('image/'))
    setFiles(prev => [...prev, ...imageFiles])
  }, [])

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: {
      'image/*': ['.jpeg', '.jpg', '.png', '.gif', '.webp']
    },
    multiple: true
  })

  const removeFile = (index: number) => {
    setFiles(prev => prev.filter((_, i) => i !== index))
  }

  const getFilePreview = (file: File) => {
    return URL.createObjectURL(file)
  }

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 Bytes'
    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
  }

  const uploadPhotos = async () => {
    if (files.length === 0) return

    setUploading(true)
    setUploadProgress(0)

    try {
      const uploadedPhotos: Photo[] = []
      
      for (let i = 0; i < files.length; i++) {
        const file = files[i]
        const formData = new FormData()
        
        // API fields in PascalCase as required by backend
        formData.append('File', file)
        formData.append('AltText', photoSettings.altText || file.name.split('.')[0])
        formData.append('Caption', photoSettings.caption)
        formData.append('Description', photoSettings.description)
        formData.append('Status', photoSettings.status.toString())
        formData.append('SortOrder', (photoSettings.sortOrder + i).toString())
        formData.append('IsHomepageSlider', photoSettings.isHomepageSlider.toString())
        formData.append('SliderText', photoSettings.sliderText)
        formData.append('AddWatermark', photoSettings.addWatermark.toString())
        
        if (photoSettings.projectId) {
          formData.append('ProjectId', photoSettings.projectId.toString())
        }

        const response = await fetch('http://localhost:5187/api/photos', {
          method: 'POST',
          body: formData
        })

        if (response.ok) {
          const uploadedPhoto = await response.json()
          uploadedPhotos.push(uploadedPhoto)
        } else {
          throw new Error(`Failed to upload ${file.name}`)
        }

        // Update progress
        setUploadProgress(Math.round(((i + 1) / files.length) * 100))
      }

      // Success
      if (onPhotosUploaded) {
        onPhotosUploaded(uploadedPhotos)
      }
      
      setFiles([])
      // Reset settings for next upload
      setPhotoSettings(prev => ({ ...prev, altText: '', caption: '', description: '', sliderText: '' }))
      setShowSettings(false)
      
    } catch (error) {
      console.error('Upload error:', error)
      alert('Fotoğraf yükleme hatası: ' + error)
    } finally {
      setUploading(false)
      setUploadProgress(0)
    }
  }

  return (
    <div className="bg-white p-6 rounded-lg shadow-sm border">
      <h3 className="text-lg font-semibold mb-4 flex items-center">
        <Upload className="w-5 h-5 mr-2" />
        Fotoğraf Yükle
      </h3>
      
      {/* Drag & Drop Area */}
      <div
        {...getRootProps()}
        className={`border-2 border-dashed rounded-lg p-8 text-center cursor-pointer transition-all duration-200 ${
          isDragActive 
            ? 'border-blue-500 bg-blue-50' 
            : 'border-gray-300 bg-gray-50 hover:border-blue-400 hover:bg-blue-50'
        }`}
      >
        <input {...getInputProps()} />
        
        <ImageIcon className="w-12 h-12 mx-auto mb-4 text-gray-400" />
        
        {isDragActive ? (
          <p className="text-blue-600 font-medium">
            Fotoğrafları buraya bırakın...
          </p>
        ) : (
          <div>
            <p className="text-gray-600 mb-2">
              <span className="font-medium">Tıklayın</span> veya fotoğrafları buraya sürükleyin
            </p>
            <p className="text-sm text-gray-500">
              PNG, JPG, GIF, WebP - Maksimum 10MB
            </p>
          </div>
        )}
      </div>

      {/* Photo Settings Form */}
      {files.length > 0 && (
        <div className="mt-6 bg-gray-50 p-4 rounded-lg">
          <div className="flex items-center justify-between mb-4">
            <h4 className="font-semibold text-gray-900 flex items-center">
              <Settings className="w-4 h-4 mr-2" />
              Fotoğraf Ayarları
            </h4>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Alt Text */}
            <div>
              <label className="block text-sm font-semibold text-gray-800 mb-1">
                Alt Text
              </label>
              <input
                type="text"
                value={photoSettings.altText}
                onChange={(e) => setPhotoSettings(prev => ({ ...prev, altText: e.target.value }))}
                placeholder="Fotoğraf açıklaması"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 font-medium"
              />
            </div>

            {/* Caption */}
            <div>
              <label className="block text-sm font-semibold text-gray-800 mb-1">
                Başlık
              </label>
              <input
                type="text"
                value={photoSettings.caption}
                onChange={(e) => setPhotoSettings(prev => ({ ...prev, caption: e.target.value }))}
                placeholder="Fotoğraf başlığı"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 font-medium"
              />
            </div>

            {/* Description */}
            <div className="md:col-span-2">
              <label className="block text-sm font-semibold text-gray-800 mb-1">
                Açıklama
              </label>
              <textarea
                value={photoSettings.description}
                onChange={(e) => setPhotoSettings(prev => ({ ...prev, description: e.target.value }))}
                placeholder="Detaylı açıklama"
                rows={3}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 font-medium"
              />
            </div>

            {/* Project Selection */}
            <div>
              <label className="block text-sm font-semibold text-gray-800 mb-1">
                Proje
              </label>
              <select
                value={photoSettings.projectId}
                onChange={(e) => setPhotoSettings(prev => ({ ...prev, projectId: e.target.value ? parseInt(e.target.value) : '' }))}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 font-medium"
              >
                <option value="">Proje seçin</option>
                {projects.map((project) => (
                  <option key={project.id} value={project.id}>
                    {project.title}
                  </option>
                ))}
              </select>
            </div>

            {/* Status */}
            <div>
              <label className="block text-sm font-semibold text-gray-800 mb-1">
                Durum
              </label>
              <select
                value={photoSettings.status}
                onChange={(e) => setPhotoSettings(prev => ({ ...prev, status: parseInt(e.target.value) }))}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 font-medium"
              >
                <option value={1}>Yayınlandı</option>
                <option value={2}>Gizli</option>
                <option value={3}>Taslak</option>
              </select>
            </div>

            {/* Sort Order */}
            <div>
              <label className="block text-sm font-semibold text-gray-800 mb-1">
                Sıralama
              </label>
              <input
                type="number"
                value={photoSettings.sortOrder}
                onChange={(e) => setPhotoSettings(prev => ({ ...prev, sortOrder: parseInt(e.target.value) || 0 }))}
                placeholder="Sıra numarası"
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 font-medium"
              />
            </div>

            {/* Homepage Slider */}
            <div className="flex items-center space-x-2">
              <input
                type="checkbox"
                id="isHomepageSlider"
                checked={photoSettings.isHomepageSlider}
                onChange={(e) => setPhotoSettings(prev => ({ ...prev, isHomepageSlider: e.target.checked }))}
                className="w-4 h-4 text-blue-600 rounded focus:ring-blue-500"
              />
              <label htmlFor="isHomepageSlider" className="text-sm font-semibold text-gray-800 flex items-center">
                <Star className="w-4 h-4 mr-1" />
                Ana Sayfa Slider'ında Göster
              </label>
            </div>

            {/* Slider Text */}
            {photoSettings.isHomepageSlider && (
              <div>
                <label className="block text-sm font-semibold text-gray-800 mb-1">
                  Slider Metni
                </label>
                <input
                  type="text"
                  value={photoSettings.sliderText}
                  onChange={(e) => setPhotoSettings(prev => ({ ...prev, sliderText: e.target.value }))}
                  placeholder="Slider üzerinde gösterilecek metin"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-900 font-medium"
                />
              </div>
            )}

            {/* Add Watermark */}
            <div className="flex items-center space-x-2">
              <input
                type="checkbox"
                id="addWatermark"
                checked={photoSettings.addWatermark}
                onChange={(e) => setPhotoSettings(prev => ({ ...prev, addWatermark: e.target.checked }))}
                className="w-4 h-4 text-blue-600 rounded focus:ring-blue-500"
              />
              <label htmlFor="addWatermark" className="text-sm font-semibold text-gray-800">
                Filigran Ekle
              </label>
            </div>
          </div>
        </div>
      )}

      {/* Selected Files */}
      {files.length > 0 && (
        <div className="mt-6">
          <h4 className="font-medium mb-3">{files.length} dosya seçildi</h4>
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
            {files.map((file, index) => (
              <div key={index} className="relative group">
                <img
                  src={getFilePreview(file)}
                  alt={file.name}
                  className="w-full h-24 object-cover rounded border"
                />
                <button
                  onClick={() => removeFile(index)}
                  className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-6 h-6 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity"
                >
                  <X className="w-4 h-4" />
                </button>
                <div className="absolute bottom-0 left-0 right-0 bg-black bg-opacity-75 text-white text-xs p-1 truncate">
                  {file.name}
                </div>
                <div className="mt-1 text-xs text-gray-500">
                  {formatFileSize(file.size)}
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Upload Progress */}
      {uploading && (
        <div className="mt-4">
          <div className="bg-gray-200 rounded-full h-3">
            <div 
              className="bg-blue-600 h-3 rounded-full transition-all duration-300"
              style={{ width: `${uploadProgress}%` }}
            ></div>
          </div>
          <p className="text-sm text-gray-600 mt-1">
            Yükleniyor: {uploadProgress}%
          </p>
        </div>
      )}

      {/* Upload Button */}
      {files.length > 0 && !uploading && (
        <div className="mt-4 flex justify-end">
          <button
            onClick={uploadPhotos}
            className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
          >
            <Upload className="w-4 h-4" />
            <span>{files.length} Fotoğrafı Yükle</span>
          </button>
        </div>
      )}
    </div>
  )
}