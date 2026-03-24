import { ref, computed } from 'vue'
import type { ServiceStatus } from '@/types'
import { getBackgroundServiceStatus } from '@/utils/bridge'

const globalStatus = ref<ServiceStatus | null>(null)
const globalLoading = ref(false)
const globalError = ref<string | null>(null)

export function useServiceStatus() {
  const status = globalStatus
  const loading = globalLoading
  const error = globalError

  const isRunning = computed(() => status.value?.IsRunning ?? false)
  const isPaused = computed(() => status.value?.IsPaused ?? false)
  const isStopped = computed(() => !status.value?.IsRunning)

  async function fetchStatus(): Promise<ServiceStatus | null> {
    loading.value = true
    error.value = null

    try {
      const result = await getBackgroundServiceStatus()
      console.log('[useServiceStatus] API result:', JSON.stringify(result, null, 2))

      if (result.Success && result.Data) {
        console.log('[useServiceStatus] Data.IsRunning:', result.Data.IsRunning, 'type:', typeof result.Data.IsRunning)
        status.value = result.Data
        return result.Data
      } else {
        error.value = result.Message || 'Failed to get service status'
        console.log('[useServiceStatus] Error:', error.value)
        return null
      }
    } finally {
      loading.value = false
    }
  }

  function clearStatus(): void {
    status.value = null
  }

  return {
    status,
    loading,
    error,
    isRunning,
    isPaused,
    isStopped,
    fetchStatus,
    clearStatus
  }
}
