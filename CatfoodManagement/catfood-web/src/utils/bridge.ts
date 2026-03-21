import type { PagedResult, ApiResponse, TaskItem, TaskConfiguration } from '@/types'

declare global {
  interface Window {
    CefSharp: {
      BindObjectAsync: (name: string) => Promise<void>
    }
    catFoodApi: {
      getCatFoods: (page: number, pageSize: number, searchKey?: string) => Promise<string>
      updateCatFood: (id: number, field: string, value: any) => Promise<string>
      deleteCatFood: (id: number) => Promise<string>
      viewImage: (picturePath: string) => void
      uploadImage: (id: number, imagePath: string) => Promise<string>
    }
    brandApi: {
      getBrands: (searchKey?: string) => Promise<string>
      addBrand: (name: string) => Promise<string>
      updateBrand: (id: number, name: string) => Promise<string>
      deleteBrand: (id: number) => Promise<string>
    }
    bestPriceApi: {
      getBestPrices: (page: number, pageSize: number, searchKey?: string) => Promise<string>
      addBestPrice: (dto: any) => Promise<string>
      updateBestPrice: (id: number, field: string, value: any) => Promise<string>
      deleteBestPrice: (id: number) => Promise<string>
    }
    ocrApi: {
      validateModelAsync: () => Promise<string>
      syncFromPicturesAsync: (folderPath: string, promptText: string) => Promise<string>
      selectFolderAsync: () => Promise<string>
      getModelsAsync: (apiKey?: string) => Promise<string>
      clearModelsCache: (apiKey?: string) => string
    }
    ocrPromptApi: {
      getDefaultAsync: () => Promise<string>
      getByIdAsync: (id: number) => Promise<string>
      getAllAsync: () => Promise<string>
      createAsync: (name: string, content: string, isDefault?: boolean, description?: string) => Promise<string>
      updateAsync: (id: number, name: string, content: string, isDefault: boolean, description?: string) => Promise<string>
      deleteAsync: (id: number) => Promise<string>
      setDefaultAsync: (id: number) => Promise<string>
    }
    settingsApi: {
      getSettings: () => Promise<string>
      saveSettings: (settingsJson: string) => Promise<string>
    }
    taskApi: {
      getTasksAsync: (page: number, pageSize: number, status?: number, type?: number) => Promise<string>
      getTaskByIdAsync: (id: number) => Promise<string>
      createTaskAsync: (type: number, name: string, parameters: string, description?: string, priority?: number) => Promise<string>
      cancelTaskAsync: (id: number) => Promise<string>
      retryTaskAsync: (id: number) => Promise<string>
      deleteTaskAsync: (id: number) => Promise<string>
      terminateTaskAsync: (id: number) => Promise<string>
      getTaskConfigurationAsync: () => Promise<string>
      updateTaskConfigurationAsync: (configuration: string) => Promise<string>
      getRunningTaskCountAsync: () => Promise<string>
      getQueueLengthAsync: () => Promise<string>
    }
  }
}

let cefSharpReady = false
let cefSharpResolve: (() => void) | null = null

const cefSharpReadyPromise = new Promise<void>((resolve) => {
  cefSharpResolve = resolve
})

export function setCefSharpReady(): void {
  cefSharpReady = true
  if (cefSharpResolve) cefSharpResolve()
}

(window as any).setCefSharpReady = setCefSharpReady

export async function waitForCefSharp(): Promise<void> {
  if (cefSharpReady) return
  await cefSharpReadyPromise
}

export function isCefSharpReady(): boolean {
  return cefSharpReady || (
    (typeof window.catFoodApi !== 'undefined' && typeof window.catFoodApi.getCatFoods === 'function') ||
    (typeof window.bestPriceApi !== 'undefined' && typeof window.bestPriceApi.getBestPrices === 'function') ||
    (typeof window.settingsApi !== 'undefined' && typeof window.settingsApi.getSettings === 'function')
  )
}

export async function getCatFoods(
  page: number,
  pageSize: number,
  searchKey?: string
): Promise<PagedResult<any>> {
  if (!isCefSharpReady()) {
    console.warn('CefSharp not ready, returning empty result')
    return { Data: [], Total: 0, Page: page, PageSize: pageSize }
  }
  try {
    const result = await window.catFoodApi.getCatFoods(page, pageSize, searchKey)
    console.log('GetCatFoods raw result:', result)
    const parsed = JSON.parse(result)
    console.log('GetCatFoods parsed result:', parsed)
    return parsed
  } catch (error) {
    console.error('GetCatFoods error:', error)
    throw error
}
}

export async function updateCatFood(
  id: number,
  field: string,
  value: any
): Promise<ApiResponse> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  const result = await window.catFoodApi.updateCatFood(id, field, value)
  return JSON.parse(result)
}

export function viewImage(picturePath: string): void {
  if (isCefSharpReady()) {
    window.catFoodApi.viewImage(picturePath)
  }
}

export async function getBrands(searchKey?: string): Promise<ApiResponse> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  const result = await window.brandApi.getBrands(searchKey)
  return JSON.parse(result)
}

export async function addBrand(name: string): Promise<ApiResponse> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  const result = await window.brandApi.addBrand(name)
  return JSON.parse(result)
}

export async function updateBrand(id: number, name: string): Promise<ApiResponse> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  const result = await window.brandApi.updateBrand(id, name)
  return JSON.parse(result)
}

export async function deleteBrand(id: number): Promise<ApiResponse> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  const result = await window.brandApi.deleteBrand(id)
  return JSON.parse(result)
}

export async function getBestPrices(
  page: number,
  pageSize: number,
  searchKey?: string
): Promise<PagedResult<any>> {
  if (!isCefSharpReady()) {
    return { Data: [], Total: 0, Page: page, PageSize: pageSize }
  }
  const result = await window.bestPriceApi.getBestPrices(page, pageSize, searchKey)
  return JSON.parse(result)
}

export async function addBestPrice(dto: any): Promise<ApiResponse> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  const result = await window.bestPriceApi.addBestPrice(dto)
  return JSON.parse(result)
}

export async function updateBestPrice(
  id: number,
  field: string,
  value: any
): Promise<ApiResponse> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  const result = await window.bestPriceApi.updateBestPrice(id, field, value)
  return JSON.parse(result)
}

export async function deleteBestPrice(id: number): Promise<ApiResponse> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  const result = await window.bestPriceApi.deleteBestPrice(id)
  return JSON.parse(result)
}

export async function validateModel(): Promise<{ Success: boolean; Message?: string }> {
  if (isCefSharpReady()) {
    const result = await window.ocrApi.validateModelAsync()
    return JSON.parse(result)
  }
  return { Success: false, Message: 'CefSharp not ready' }
}

export interface ModelInfo {
  Name: string
  DisplayName: string
}

interface ModelCache {
  apiKeyHash: string
  timestamp: number
  data: ModelInfo[]
}

const MODEL_CACHE_KEY = 'gemini_models_cache'
const MODEL_CACHE_DURATION = 30 * 60 * 1000

function hashApiKey(apiKey: string): string {
  let hash = 0
  for (let i = 0; i < apiKey.length; i++) {
    const char = apiKey.charCodeAt(i)
    hash = ((hash << 5) - hash) + char
    hash = hash & hash
  }
  return Math.abs(hash).toString(16).padStart(8, '0')
}

function getCachedModels(apiKey: string): ModelInfo[] | null {
  try {
    const cacheStr = localStorage.getItem(MODEL_CACHE_KEY)
    if (!cacheStr) return null

    const cache = JSON.parse(cacheStr) as ModelCache
    const hash = hashApiKey(apiKey)

    if (cache.apiKeyHash !== hash) return null
    if (Date.now() - cache.timestamp > MODEL_CACHE_DURATION) return null

    return cache.data
  } catch {
    return null
  }
}

function setCachedModels(apiKey: string, data: ModelInfo[]): void {
  try {
    const cache: ModelCache = {
      apiKeyHash: hashApiKey(apiKey),
      timestamp: Date.now(),
      data
    }
    localStorage.setItem(MODEL_CACHE_KEY, JSON.stringify(cache))
  } catch (error) {
    console.error('Failed to cache models:', error)
  }
}

export function clearLocalModelsCache(): void {
  localStorage.removeItem(MODEL_CACHE_KEY)
}

export async function getModels(
  apiKey: string,
  forceRefresh: boolean = false
): Promise<{ Success: boolean; Data?: ModelInfo[]; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }

  if (!forceRefresh) {
    const cached = getCachedModels(apiKey)
    if (cached) {
      console.log('Returning cached models')
      return { Success: true, Data: cached }
    }
  }

  try {
    const result = await window.ocrApi.getModelsAsync(apiKey)
    const parsed = JSON.parse(result)
    if (parsed.Success && parsed.Data) {
      setCachedModels(apiKey, parsed.Data)
    }
    return parsed
  } catch (error) {
    console.error('GetModels error:', error)
    return { Success: false, Message: 'Failed to get models' }
  }
}

export function clearModelsCache(apiKey?: string): { Success: boolean; Message?: string } {
  clearLocalModelsCache()

  if (isCefSharpReady()) {
    try {
      const result = window.ocrApi.clearModelsCache(apiKey)
      return JSON.parse(result)
    } catch (error) {
      console.error('ClearModelsCache error:', error)
      return { Success: false, Message: 'Failed to clear backend cache' }
    }
  }

  return { Success: true, Message: 'Local cache cleared' }
}

export async function syncFromPictures(
  folderPath: string,
  promptText: string
): Promise<{ Success: boolean; Count: number; Data: any[]; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Count: 0, Data: [], Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.ocrApi.syncFromPicturesAsync(folderPath, promptText)
    const parsed = JSON.parse(result)
    return parsed
  } catch (error) {
    console.error('SyncFromPictures error:', error)
    throw error
  }
}

export async function selectFolder(): Promise<string> {
  if (!isCefSharpReady()) {
    return ''
  }
  return await window.ocrApi.selectFolderAsync()
}

export interface AppSettings {
  AI: {
    ApiKey: string
    ModelName: string
    RPM: number
    TPM: number
    RPD: number
    Proxy: {
      Enabled: boolean
      Address: string
    }
  }
  Database: {
    ConnectionString: string
  }
  App: {
    PlatformFolders: Record<string, string>
  }
}

export async function getSettings(): Promise<{ Success: boolean; Data?: AppSettings; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.settingsApi.getSettings()
    return JSON.parse(result)
  } catch (error) {
    console.error('GetSettings error:', error)
    return { Success: false, Message: 'Failed to get settings' }
  }
}

export async function saveSettings(settings: AppSettings): Promise<{ Success: boolean; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const settingsJson = JSON.stringify(settings)
    const result = await window.settingsApi.saveSettings(settingsJson)
    return JSON.parse(result)
  } catch (error) {
    console.error('SaveSettings error:', error)
    return { Success: false, Message: 'Failed to save settings' }
  }
}

export interface OcrPrompt {
  Id: number
  Name: string
  Content: string
  IsDefault: boolean
  Description?: string
  CreatedAt: string
  UpdatedAt?: string
}

export async function getOcrPromptDefault(): Promise<{ Success: boolean; Data?: OcrPrompt; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.ocrPromptApi.getDefaultAsync()
    return JSON.parse(result)
  } catch (error) {
    console.error('GetOcrPromptDefault error:', error)
    return { Success: false, Message: 'Failed to get default prompt' }
  }
}

export async function getOcrPromptById(id: number): Promise<{ Success: boolean; Data?: OcrPrompt; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.ocrPromptApi.getByIdAsync(id)
    return JSON.parse(result)
  } catch (error) {
    console.error('GetOcrPromptById error:', error)
    return { Success: false, Message: 'Failed to get prompt' }
  }
}

export async function getOcrPrompts(): Promise<{ Success: boolean; Data?: OcrPrompt[]; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.ocrPromptApi.getAllAsync()
    return JSON.parse(result)
  } catch (error) {
    console.error('GetOcrPrompts error:', error)
    return { Success: false, Message: 'Failed to get prompts' }
  }
}

export async function createOcrPrompt(
  name: string,
  content: string,
  isDefault: boolean = false,
  description?: string
): Promise<{ Success: boolean; Data?: OcrPrompt; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.ocrPromptApi.createAsync(name, content, isDefault, description)
    return JSON.parse(result)
  } catch (error) {
    console.error('CreateOcrPrompt error:', error)
    return { Success: false, Message: 'Failed to create prompt' }
  }
}

export async function updateOcrPrompt(
  id: number,
  name: string,
  content: string,
  isDefault: boolean,
  description?: string
): Promise<{ Success: boolean; Data?: OcrPrompt; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.ocrPromptApi.updateAsync(id, name, content, isDefault, description)
    return JSON.parse(result)
  } catch (error) {
    console.error('UpdateOcrPrompt error:', error)
    return { Success: false, Message: 'Failed to update prompt' }
  }
}

export async function deleteOcrPrompt(id: number): Promise<{ Success: boolean; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.ocrPromptApi.deleteAsync(id)
    return JSON.parse(result)
  } catch (error) {
    console.error('DeleteOcrPrompt error:', error)
    return { Success: false, Message: 'Failed to delete prompt' }
  }
}

export async function setOcrPromptDefault(id: number): Promise<{ Success: boolean; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.ocrPromptApi.setDefaultAsync(id)
    return JSON.parse(result)
  } catch (error) {
    console.error('SetOcrPromptDefault error:', error)
    return { Success: false, Message: 'Failed to set default prompt' }
  }
}

export async function getTasks(
  page: number,
  pageSize: number,
  status?: number,
  type?: number
): Promise<{ Success: boolean; Data?: TaskItem[]; Total?: number; Page?: number; PageSize?: number; TotalPages?: number; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.getTasksAsync(page, pageSize, status, type)
    return JSON.parse(result)
  } catch (error) {
    console.error('GetTasks error:', error)
    return { Success: false, Message: 'Failed to get tasks' }
  }
}

export async function getTaskById(id: number): Promise<{ Success: boolean; Data?: TaskItem; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.getTaskByIdAsync(id)
    return JSON.parse(result)
  } catch (error) {
    console.error('GetTaskById error:', error)
    return { Success: false, Message: 'Failed to get task' }
  }
}

export async function createTask(
  type: number,
  name: string,
  parameters: string,
  description?: string,
  priority?: number
): Promise<{ Success: boolean; Data?: TaskItem; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.createTaskAsync(type, name, parameters, description, priority)
    return JSON.parse(result)
  } catch (error) {
    console.error('CreateTask error:', error)
    return { Success: false, Message: 'Failed to create task' }
  }
}

export async function cancelTask(id: number): Promise<{ Success: boolean; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.cancelTaskAsync(id)
    return JSON.parse(result)
  } catch (error) {
    console.error('CancelTask error:', error)
    return { Success: false, Message: 'Failed to cancel task' }
  }
}

export async function retryTask(id: number): Promise<{ Success: boolean; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.retryTaskAsync(id)
    return JSON.parse(result)
  } catch (error) {
    console.error('RetryTask error:', error)
    return { Success: false, Message: 'Failed to retry task' }
  }
}

export async function deleteTask(id: number): Promise<{ Success: boolean; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.deleteTaskAsync(id)
    return JSON.parse(result)
  } catch (error) {
    console.error('DeleteTask error:', error)
    return { Success: false, Message: 'Failed to delete task' }
  }
}

export async function terminateTask(id: number): Promise<{ Success: boolean; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.terminateTaskAsync(id)
    return JSON.parse(result)
  } catch (error) {
    console.error('TerminateTask error:', error)
    return { Success: false, Message: 'Failed to terminate task' }
  }
}

export async function getTaskConfiguration(): Promise<{ Success: boolean; Data?: TaskConfiguration; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.getTaskConfigurationAsync()
    return JSON.parse(result)
  } catch (error) {
    console.error('GetTaskConfiguration error:', error)
    return { Success: false, Message: 'Failed to get task configuration' }
  }
}

export async function updateTaskConfiguration(config: TaskConfiguration): Promise<{ Success: boolean; Data?: TaskConfiguration; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.updateTaskConfigurationAsync(JSON.stringify(config))
    return JSON.parse(result)
  } catch (error) {
    console.error('UpdateTaskConfiguration error:', error)
    return { Success: false, Message: 'Failed to update task configuration' }
  }
}

export async function getRunningTaskCount(): Promise<{ Success: boolean; Count?: number; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.getRunningTaskCountAsync()
    return JSON.parse(result)
  } catch (error) {
    console.error('GetRunningTaskCount error:', error)
    return { Success: false, Message: 'Failed to get running task count' }
  }
}

export async function getQueueLength(): Promise<{ Success: boolean; Count?: number; Message?: string }> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  try {
    const result = await window.taskApi.getQueueLengthAsync()
    return JSON.parse(result)
  } catch (error) {
    console.error('GetQueueLength error:', error)
    return { Success: false, Message: 'Failed to get queue length' }
  }
}
