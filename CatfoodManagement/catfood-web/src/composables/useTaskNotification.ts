import { ElNotification } from 'element-plus'
import type { TaskStatus, TaskType } from '@/types'

export interface TaskStatusEvent {
  taskId: number
  taskName: string
  taskType: TaskType
  oldStatus: number
  newStatus: TaskStatus
  result?: string
  errorMessage?: string
  timestamp: string
}

const TaskStatusLabels: Record<number, string> = {
  0: '待处理',
  1: '已入队',
  2: '运行中',
  3: '已完成',
  4: '已失败',
  5: '已取消',
  6: '重试中'
}

const TaskTypeLabels: Record<number, string> = {
  0: '图片同步',
  1: '图片删除',
  2: '图片移动',
  3: '图片处理'
}

let router: ReturnType<typeof import('vue-router').useRouter> | null = null

export function setTaskNotificationRouter(r: typeof router) {
  router = r
}

export function useTaskNotification() {
  const handleTaskStatusChanged = (event: TaskStatusEvent) => {
    console.log('[TaskNotification] Received event:', event)
    
    const statusText = TaskStatusLabels[event.newStatus] || `状态 ${event.newStatus}`
    const typeText = TaskTypeLabels[event.taskType] || `类型 ${event.taskType}`

    switch (event.newStatus) {
      case 0:
        handleTaskCreated(event, typeText)
        break
      case 2:
        handleTaskRunning(event, statusText)
        break
      case 3:
        handleTaskCompleted(event, typeText)
        break
      case 4:
        handleTaskFailed(event, typeText)
        break
      case 5:
        handleTaskCancelled(event, typeText)
        break
    }
  }

  const handleTaskCreated = (event: TaskStatusEvent, typeText: string) => {
    ElNotification({
      title: '任务已创建',
      message: `任务 "${event.taskName}" (${typeText}) 已加入队列`,
      type: 'info',
      duration: 3000,
      position: 'top-right',
      onClick: () => navigateToTask(event.taskId)
    })
  }

  const handleTaskRunning = (event: TaskStatusEvent, statusText: string) => {
    ElNotification({
      title: '任务开始执行',
      message: `任务 "${event.taskName}" 开始执行`,
      type: 'info',
      duration: 2000,
      position: 'top-right'
    })
  }

  const handleTaskCompleted = (event: TaskStatusEvent, typeText: string) => {
    let message = `任务 "${event.taskName}" (${typeText}) 已成功完成`
    
    if (event.result) {
      try {
        const result = JSON.parse(event.result)
        if (result.ProcessedCount !== undefined) {
          message += `\n处理了 ${result.ProcessedCount} 条数据`
        }
      } catch {
        // Ignore parse errors
      }
    }

    ElNotification({
      title: '任务完成',
      message,
      type: 'success',
      duration: 5000,
      position: 'top-right',
      onClick: () => navigateToTask(event.taskId)
    })
  }

  const handleTaskFailed = (event: TaskStatusEvent, typeText: string) => {
    const message = `任务 "${event.taskName}" (${typeText}) 执行失败\n错误: ${event.errorMessage || '未知错误'}`

    ElNotification({
      title: '任务失败',
      message,
      type: 'error',
      duration: 0,
      position: 'top-right',
      onClick: () => navigateToTask(event.taskId)
    })
  }

  const handleTaskCancelled = (event: TaskStatusEvent, typeText: string) => {
    ElNotification({
      title: '任务已取消',
      message: `任务 "${event.taskName}" (${typeText}) 已被取消`,
      type: 'warning',
      duration: 3000,
      position: 'top-right'
    })
  }

  const navigateToTask = (taskId: number) => {
    if (router) {
      router.push({ path: '/tasks', query: { id: taskId.toString() } })
    }
  }

  const initNotification = () => {
    (window as any).onTaskStatusChanged = handleTaskStatusChanged
    console.log('[TaskNotification] Notification listener initialized')
  }

  return {
    initNotification,
    handleTaskStatusChanged
  }
}
