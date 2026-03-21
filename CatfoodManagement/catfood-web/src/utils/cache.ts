const CACHE_PREFIX = 'catfood_cache_'
const DEFAULT_EXPIRY = 24 * 60 * 60 * 1000

interface CacheItem<T> {
  data: T
  timestamp: number
  expiry: number
}

export const useCache = <T>(key: string, defaultExpiry = DEFAULT_EXPIRY) => {
  const cacheKey = `${CACHE_PREFIX}${key}`
  
  const get = (): T | null => {
    try {
      const itemStr = localStorage.getItem(cacheKey)
      if (!itemStr) return null
      
      const item: CacheItem<T> = JSON.parse(itemStr)
      const now = Date.now()
      
      if (now > item.timestamp + item.expiry) {
        localStorage.removeItem(cacheKey)
        return null
      }
      
      return item.data
    } catch (error) {
      console.error(`Cache get error for key "${key}":`, error)
      return null
    }
  }
  
  const set = (data: T, expiry = defaultExpiry): void => {
    try {
      const item: CacheItem<T> = {
        data,
        timestamp: Date.now(),
        expiry
      }
      localStorage.setItem(cacheKey, JSON.stringify(item))
    } catch (error) {
      console.error(`Cache set error for key "${key}":`, error)
    }
  }
  
  const remove = (): void => {
    localStorage.removeItem(cacheKey)
  }
  
  const clear = (): void => {
    const keysToRemove: string[] = []
    for (let i = 0; i < localStorage.length; i++) {
      const k = localStorage.key(i)
      if (k?.startsWith(CACHE_PREFIX)) {
        keysToRemove.push(k)
      }
    }
    keysToRemove.forEach(k => localStorage.removeItem(k))
  }
  
  const getOrFetch = async (fetcher: () => Promise<T>, expiry = defaultExpiry): Promise<T> => {
    const cached = get()
    if (cached !== null) {
      return cached
    }
    
    const data = await fetcher()
    set(data, expiry)
    return data
  }
  
  return {
    get,
    set,
    remove,
    clear,
    getOrFetch
  }
}

export const CacheKeys = {
  MODELS: 'models',
  SETTINGS: 'settings',
  BRANDS: 'brands',
  USER_PREFERENCES: 'user_preferences'
} as const

export const createModelCache = () => useCache<string[]>(CacheKeys.MODELS, 7 * 24 * 60 * 60 * 1000)
export const createSettingsCache = () => useCache<Record<string, any>>(CacheKeys.SETTINGS, 24 * 60 * 60 * 1000)
export const createBrandsCache = () => useCache<any[]>(CacheKeys.BRANDS, 60 * 60 * 1000)
