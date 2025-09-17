'use client'

import { useState, useEffect } from 'react'
import { Building, Image, FolderOpen, Globe, Plus, Upload, Settings, Wifi, WifiOff, AlertCircle } from 'lucide-react'
import CategoryManager from '../../components/CategoryManager'

interface Stats {
  projects: number
  photos: number
  categories: number
  languages: number
}

interface RecentProject {
  id: number
  title: string
  status: string
  description: string
}

interface ApiStatus {
  connected: boolean
  message: string
  lastChecked: Date
}

export default function Dashboard() {
  const [stats, setStats] = useState<Stats>({
    projects: 0,
    photos: 0,
    categories: 0,
    languages: 0
  })

  const [recentProjects, setRecentProjects] = useState<RecentProject[]>([])
  const [apiStatus, setApiStatus] = useState<ApiStatus>({
    connected: false,
    message: 'Bağlantı kontrol ediliyor...',
    lastChecked: new Date()
  })
  const [loading, setLoading] = useState(true)

  // Check API connection and load data
  useEffect(() => {
    const checkApiAndLoadData = async () => {
      try {
        // First check API connection
        const healthResponse = await fetch('http://localhost:5187/api/photos', {
          method: 'GET',
          headers: {
            'Accept': 'application/json',
          },
        })

        if (healthResponse.ok) {
          setApiStatus({
            connected: true,
            message: 'API bağlantısı başarılı',
            lastChecked: new Date()
          })

          // Load real data from API
          const [projectsRes, photosRes, categoriesRes, languagesRes] = await Promise.all([
            fetch('http://localhost:5187/api/projects').catch(() => null),
            fetch('http://localhost:5187/api/photos').catch(() => null),
            fetch('http://localhost:5187/api/categories').catch(() => null),
            fetch('http://localhost:5187/api/languages').catch(() => null)
          ])

          const projectsData = projectsRes?.ok ? await projectsRes.json() : []
          const photosData = photosRes?.ok ? await photosRes.json() : []
          const categoriesData = categoriesRes?.ok ? await categoriesRes.json() : []
          const languagesData = languagesRes?.ok ? await languagesRes.json() : []

          setStats({
            projects: projectsData.length || 0,
            photos: photosData.length || 0,
            categories: categoriesData.length || 0,
            languages: languagesData.length || 0
          })

          // Set recent projects (take first 3)
          const recentProjectsData = projectsData.slice(0, 3).map((project: any) => ({
            id: project.id,
            title: project.title || `Proje ${project.id}`,
            status: project.status === 1 ? 'Published' : project.status === 2 ? 'In Progress' : 'Planning',
            description: project.description || 'Açıklama bulunmuyor'
          }))
          setRecentProjects(recentProjectsData)

        } else {
          throw new Error('API health check failed')
        }
      } catch (error) {
        console.error('API connection error:', error)
        setApiStatus({
          connected: false,
          message: 'API bağlantısı başarısız - localhost:5187',
          lastChecked: new Date()
        })
        
        // Set fallback data when API is not available
        setStats({
          projects: 0,
          photos: 0,
          categories: 0,
          languages: 0
        })
        setRecentProjects([
          { id: 1, title: 'API Bağlantısı Yok', status: 'Error', description: 'API sunucusunu başlatın' }
        ])
      } finally {
        setLoading(false)
      }
    }

    checkApiAndLoadData()
    
    // Refresh API status every 30 seconds
    const interval = setInterval(checkApiAndLoadData, 30000)
    return () => clearInterval(interval)
  }, [])

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Published':
        return 'bg-green-100 text-green-800'
      case 'In Progress':
        return 'bg-blue-100 text-blue-800'
      case 'Planning':
        return 'bg-yellow-100 text-yellow-800'
      default:
        return 'bg-gray-100 text-gray-800'
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span className="ml-3 text-gray-600">Yükleniyor...</span>
      </div>
    )
  }

  return (
    <div className="space-y-8">
      {/* API Status Banner */}
      <div className={`rounded-lg p-4 border ${
        apiStatus.connected 
          ? 'bg-green-50 border-green-200 text-green-800'
          : 'bg-red-50 border-red-200 text-red-800'
      }`}>
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-3">
            {apiStatus.connected ? (
              <Wifi className="w-5 h-5 text-green-600" />
            ) : (
              <WifiOff className="w-5 h-5 text-red-600" />
            )}
            <div>
              <span className="font-medium">{apiStatus.message}</span>
              <div className="text-sm opacity-75 mt-1">
                Son kontrol: {apiStatus.lastChecked.toLocaleTimeString('tr-TR')}
              </div>
            </div>
          </div>
          {!apiStatus.connected && (
            <div className="flex items-center space-x-2 text-sm">
              <AlertCircle className="w-4 h-4" />
              <span>API sunucusunu başlatmayı unutmayın</span>
            </div>
          )}
        </div>
      </div>

      {/* Page Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-2">Mimarlık projelerinizi yönetin ve kontrol edin</p>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div className="bg-white p-6 rounded-lg shadow-sm border">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Toplam Proje</p>
              <p className="text-3xl font-bold text-gray-900">{stats.projects}</p>
            </div>
            <Building className="w-8 h-8 text-blue-600" />
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-sm border">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Toplam Fotoğraf</p>
              <p className="text-3xl font-bold text-gray-900">{stats.photos}</p>
            </div>
            <Image className="w-8 h-8 text-green-600" />
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-sm border">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Kategoriler</p>
              <p className="text-3xl font-bold text-gray-900">{stats.categories}</p>
            </div>
            <FolderOpen className="w-8 h-8 text-purple-600" />
          </div>
        </div>

        <div className="bg-white p-6 rounded-lg shadow-sm border">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Diller</p>
              <p className="text-3xl font-bold text-gray-900">{stats.languages}</p>
            </div>
            <Globe className="w-8 h-8 text-orange-600" />
          </div>
        </div>
      </div>

      {/* Recent Projects */}
      <div className="bg-white p-6 rounded-lg shadow-sm border">
        <h3 className="text-lg font-semibold mb-4">Son Projeler</h3>
        <div className="space-y-3">
          {recentProjects.map((project) => (
            <div key={project.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
              <div className="flex-1">
                <h4 className="font-medium text-gray-900">{project.title}</h4>
                <p className="text-sm text-gray-600">{project.description}</p>
              </div>
              <span className={`px-3 py-1 text-xs rounded-full font-medium ${getStatusColor(project.status)}`}>
                {project.status}
              </span>
            </div>
          ))}
        </div>
      </div>

      {/* Category Manager */}
      <div className="grid grid-cols-1 lg:grid-cols-1 gap-8">
        <CategoryManager onCategoryReorder={(categories) => {
          console.log('Categories reordered:', categories)
        }} />
      </div>

      {/* Quick Actions */}
      <div>
        <h3 className="text-lg font-semibold mb-4">Hızlı Eylemler</h3>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <button className="bg-blue-600 text-white p-6 rounded-lg shadow-sm hover:bg-blue-700 transition-colors group">
            <Plus className="w-8 h-8 mx-auto mb-3 group-hover:scale-110 transition-transform" />
            <div className="font-medium">Yeni Proje</div>
            <div className="text-sm opacity-90 mt-1">Proje oluştur</div>
          </button>

          <button className="bg-green-600 text-white p-6 rounded-lg shadow-sm hover:bg-green-700 transition-colors group">
            <Upload className="w-8 h-8 mx-auto mb-3 group-hover:scale-110 transition-transform" />
            <div className="font-medium">Fotoğraf Yükle</div>
            <div className="text-sm opacity-90 mt-1">Fotoğraf yükle</div>
          </button>

          <button className="bg-purple-600 text-white p-6 rounded-lg shadow-sm hover:bg-purple-700 transition-colors group">
            <Settings className="w-8 h-8 mx-auto mb-3 group-hover:scale-110 transition-transform" />
            <div className="font-medium">Kategori Yönet</div>
            <div className="text-sm opacity-90 mt-1">Kategori düzenle</div>
          </button>
        </div>
      </div>
    </div>
  )
}