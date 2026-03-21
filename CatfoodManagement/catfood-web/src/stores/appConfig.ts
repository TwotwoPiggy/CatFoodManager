import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { getSettings, getOcrPrompts, type OcrPrompt } from '@/utils/bridge'

const CACHE_KEY = 'app_config_cache'
const CACHE_DURATION = 5 * 60 * 1000

interface AppConfigCache {
  platformFolders: Record<string, string>
  ocrPrompts: OcrPrompt[]
  timestamp: number
}

export const useAppConfigStore = defineStore('appConfig', () => {
  const platformFolders = ref<Record<string, string>>({})
  const ocrPrompts = ref<OcrPrompt[]>([])
  const loading = ref(false)
  const lastFetchTime = ref(0)

  const folderOptions = computed(() => {
    return Object.entries(platformFolders.value).map(([name, path]) => ({
      label: name,
      value: path
    }))
  })

  const defaultPrompt = computed(() => {
    return ocrPrompts.value.find(p => p.IsDefault) || ocrPrompts.value[0] || null
  })

  const defaultPromptContent = computed(() => {
    return defaultPrompt.value?.Content || ''
  })

  function loadFromCache(): boolean {
    try {
      const cacheStr = localStorage.getItem(CACHE_KEY)
      if (!cacheStr) return false

      const cache = JSON.parse(cacheStr) as AppConfigCache
      if (Date.now() - cache.timestamp > CACHE_DURATION) return false

      platformFolders.value = cache.platformFolders
      ocrPrompts.value = cache.ocrPrompts
      lastFetchTime.value = cache.timestamp
      return true
    } catch {
      return false
    }
  }

  function saveToCache() {
    try {
      const cache: AppConfigCache = {
        platformFolders: platformFolders.value,
        ocrPrompts: ocrPrompts.value,
        timestamp: Date.now()
      }
      localStorage.setItem(CACHE_KEY, JSON.stringify(cache))
      lastFetchTime.value = cache.timestamp
    } catch (error) {
      console.error('Failed to save cache:', error)
    }
  }

  async function fetchPlatformFolders(forceRefresh: boolean = false) {
    if (!forceRefresh && Object.keys(platformFolders.value).length > 0) {
      return
    }

    try {
      const result = await getSettings()
      if (result.Success && result.Data?.App?.PlatformFolders) {
        platformFolders.value = result.Data.App.PlatformFolders
      }
    } catch (error) {
      console.error('Failed to fetch platform folders:', error)
    }
  }

  async function fetchOcrPrompts(forceRefresh: boolean = false) {
    if (!forceRefresh && ocrPrompts.value.length > 0) {
      return
    }

    try {
      const result = await getOcrPrompts()
      if (result.Success && result.Data) {
        ocrPrompts.value = result.Data
      }
    } catch (error) {
      console.error('Failed to fetch OCR prompts:', error)
    }
  }

  async function fetchAll(forceRefresh: boolean = false) {
    if (!forceRefresh && loadFromCache()) {
      return
    }

    loading.value = true
    try {
      await Promise.all([
        fetchPlatformFolders(forceRefresh),
        fetchOcrPrompts(forceRefresh)
      ])
      saveToCache()
    } finally {
      loading.value = false
    }
  }

  function clearCache() {
    localStorage.removeItem(CACHE_KEY)
    platformFolders.value = {}
    ocrPrompts.value = []
    lastFetchTime.value = 0
  }

  return {
    platformFolders,
    ocrPrompts,
    loading,
    lastFetchTime,
    folderOptions,
    defaultPrompt,
    defaultPromptContent,
    fetchPlatformFolders,
    fetchOcrPrompts,
    fetchAll,
    clearCache
  }
})
