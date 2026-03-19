import type { PagedResult, ApiResponse } from '@/types'

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
    setCefSharpReady: () => void
  }
}

let cefSharpReady = false
let cefSharpResolve: (() => void) | null = null
const cefSharpReadyPromise = new Promise<void>((resolve) => {
  cefSharpResolve = resolve
})

export function setCefSharpReady(): void {
  cefSharpReady = true
  if (cefSharpResolve) {
    cefSharpResolve()
  }
}

window.setCefSharpReady = setCefSharpReady

export async function waitForCefSharp(): Promise<void> {
  if (cefSharpReady) return
  await cefSharpReadyPromise
}

export function isCefSharpReady(): boolean {
  return cefSharpReady || (
    (typeof window.catFoodApi !== 'undefined' && typeof window.catFoodApi.getCatFoods === 'function') ||
    (typeof window.bestPriceApi !== 'undefined' && typeof window.bestPriceApi.getBestPrices === 'function')
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

export async function deleteCatFood(id: number): Promise<ApiResponse> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  const result = await window.catFoodApi.deleteCatFood(id)
  return JSON.parse(result)
}

export function viewImage(picturePath: string): void {
  if (isCefSharpReady()) {
    window.catFoodApi.viewImage(picturePath)
  }
}

export async function uploadImage(id: number, imagePath: string): Promise<ApiResponse> {
  if (!isCefSharpReady()) {
    return { Success: false, Message: 'CefSharp not ready' }
  }
  const result = await window.catFoodApi.uploadImage(id, imagePath)
  return JSON.parse(result)
}

export async function getBrands(searchKey?: string): Promise<{ Data: any[]; Total: number }> {
  if (!isCefSharpReady()) {
    return { Data: [], Total: 0 }
  }
  const result = await window.brandApi.getBrands(searchKey)
  return JSON.parse(result)
}

export async function addBrand(name: string): Promise<ApiResponse<{ Id: number }>> {
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
    console.warn('CefSharp not ready, returning empty result')
    return { Data: [], Total: 0, Page: page, PageSize: pageSize }
  }
  try {
    const result = await window.bestPriceApi.getBestPrices(page, pageSize, searchKey)
    console.log('GetBestPrices raw result:', result)
    const parsed = JSON.parse(result)
    console.log('GetBestPrices parsed result:', parsed)
    return parsed
  } catch (error) {
    console.error('GetBestPrices error:', error)
    throw error
  }
}

export async function addBestPrice(dto: any): Promise<ApiResponse<{ Id: number }>> {
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
