import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { TaskItem, TaskConfiguration } from '@/types'
import {
  getTasks as fetchTasksApi,
  getTaskById as fetchTaskByIdApi,
  createTask as createTaskApi,
  cancelTask as cancelTaskApi,
  retryTask as retryTaskApi,
  deleteTask as deleteTaskApi,
  terminateTask as terminateTaskApi,
  getTaskConfiguration as fetchTaskConfigApi,
  updateTaskConfiguration as updateTaskConfigApi,
  getRunningTaskCount as getRunningCountApi,
  getQueueLength as getQueueLenApi
} from '@/utils/bridge'

export const useTaskStore = defineStore('task', () => {
  const tasks = ref<TaskItem[]>([])
  const currentTask = ref<TaskItem | null>(null)
  const configuration = ref<TaskConfiguration | null>(null)
  const loading = ref(false)
  const total = ref(0)
  const page = ref(1)
  const pageSize = ref(10)
  const runningCount = ref(0)
  const queueLength = ref(0)

  const totalPages = computed(() => Math.ceil(total.value / pageSize.value))

  async function fetchTasks(p: number = page.value, ps: number = pageSize.value, status?: number, type?: number) {
    loading.value = true
    try {
      const result = await fetchTasksApi(p, ps, status, type)
      if (result.Success && result.Data) {
        tasks.value = result.Data
        total.value = result.Total || 0
        page.value = result.Page || p
        pageSize.value = result.PageSize || ps
      }
      return result
    } finally {
      loading.value = false
    }
  }

  async function fetchTaskById(id: number) {
    loading.value = true
    try {
      const result = await fetchTaskByIdApi(id)
      if (result.Success && result.Data) {
        currentTask.value = result.Data
      }
      return result
    } finally {
      loading.value = false
    }
  }

  async function createTask(type: number, name: string, parameters: string, description?: string, priority?: number) {
    loading.value = true
    try {
      const result = await createTaskApi(type, name, parameters, description, priority)
      if (result.Success) {
        await fetchTasks()
      }
      return result
    } finally {
      loading.value = false
    }
  }

  async function cancelTask(id: number) {
    loading.value = true
    try {
      const result = await cancelTaskApi(id)
      if (result.Success) {
        await fetchTasks()
      }
      return result
    } finally {
      loading.value = false
    }
  }

  async function retryTask(id: number) {
    loading.value = true
    try {
      const result = await retryTaskApi(id)
      if (result.Success) {
        await fetchTasks()
      }
      return result
    } finally {
      loading.value = false
    }
  }

  async function deleteTask(id: number) {
    loading.value = true
    try {
      const result = await deleteTaskApi(id)
      if (result.Success) {
        await fetchTasks()
      }
      return result
    } finally {
      loading.value = false
    }
  }

  async function terminateTask(id: number) {
    loading.value = true
    try {
      const result = await terminateTaskApi(id)
      if (result.Success) {
        await fetchTasks()
      }
      return result
    } finally {
      loading.value = false
    }
  }

  async function fetchConfiguration() {
    loading.value = true
    try {
      const result = await fetchTaskConfigApi()
      if (result.Success && result.Data) {
        configuration.value = result.Data
      }
      return result
    } finally {
      loading.value = false
    }
  }

  async function updateConfiguration(config: TaskConfiguration) {
    loading.value = true
    try {
      const result = await updateTaskConfigApi(config)
      if (result.Success && result.Data) {
        configuration.value = result.Data
      }
      return result
    } finally {
      loading.value = false
    }
  }

  async function refreshRunningCount() {
    const result = await getRunningCountApi()
    if (result.Success && result.Count !== undefined) {
      runningCount.value = result.Count
    }
  }

  async function refreshQueueLength() {
    const result = await getQueueLenApi()
    if (result.Success && result.Count !== undefined) {
      queueLength.value = result.Count
    }
  }

  function clearCurrentTask() {
    currentTask.value = null
  }

  return {
    tasks,
    currentTask,
    configuration,
    loading,
    total,
    page,
    pageSize,
    runningCount,
    queueLength,
    totalPages,
    fetchTasks,
    fetchTaskById,
    createTask,
    cancelTask,
    retryTask,
    deleteTask,
    terminateTask,
    fetchConfiguration,
    updateConfiguration,
    refreshRunningCount,
    refreshQueueLength,
    clearCurrentTask
  }
})
