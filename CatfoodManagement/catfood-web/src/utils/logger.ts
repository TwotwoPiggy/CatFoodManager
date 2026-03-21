import { ref } from 'vue'

export interface LogEntry {
  timestamp: string
  level: 'info' | 'warn' | 'error'
  message: string
  data?: any
}

const logs = ref<LogEntry[]>([])
const maxLogs = 100

const formatTimestamp = () => {
  return new Date().toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

const addLog = (level: LogEntry['level'], message: string, data?: any) => {
  const entry: LogEntry = {
    timestamp: formatTimestamp(),
    level,
    message,
    data
  }
  
  logs.value.unshift(entry)
  
  if (logs.value.length > maxLogs) {
    logs.value = logs.value.slice(0, maxLogs)
  }
  
  const consoleMethod = level === 'error' ? console.error : level === 'warn' ? console.warn : console.info
  consoleMethod(`[${entry.timestamp}] [${level.toUpperCase()}] ${message}`, data || '')
}

export const useLogger = () => {
  const info = (message: string, data?: any) => {
    addLog('info', message, data)
  }
  
  const warn = (message: string, data?: any) => {
    addLog('warn', message, data)
  }
  
  const error = (message: string, data?: any) => {
    addLog('error', message, data)
  }
  
  const getLogs = () => logs.value
  
  const clearLogs = () => {
    logs.value = []
  }
  
  return {
    info,
    warn,
    error,
    getLogs,
    clearLogs
  }
}

export const setupGlobalErrorHandler = () => {
  window.addEventListener('error', (event) => {
    const logger = useLogger()
    logger.error('全局错误', {
      message: event.message,
      filename: event.filename,
      lineno: event.lineno,
      colno: event.colno
    })
  })
  
  window.addEventListener('unhandledrejection', (event) => {
    const logger = useLogger()
    logger.error('未处理的 Promise 拒绝', event.reason)
  })
}
