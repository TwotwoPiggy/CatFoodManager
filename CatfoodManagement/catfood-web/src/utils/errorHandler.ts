import { ElMessage, ElMessageBox } from 'element-plus'
import { useLogger } from './logger'

export interface AppError {
  code?: string
  message: string
  details?: any
}

export const handleError = (error: AppError | Error | string, showMessage = true) => {
  const logger = useLogger()
  
  let errorMessage = ''
  let errorDetails = {}
  
  if (typeof error === 'string') {
    errorMessage = error
  } else if (error instanceof Error) {
    errorMessage = error.message
    errorDetails = { stack: error.stack }
  } else {
    errorMessage = error.message
    errorDetails = error.details || {}
  }
  
  logger.error(errorMessage, errorDetails)
  
  if (showMessage) {
    ElMessage.error(errorMessage)
  }
  
  return {
    message: errorMessage,
    details: errorDetails
  }
}

export const handleApiError = (error: any, defaultMessage = '操作失败') => {
  const logger = useLogger()
  
  let message = defaultMessage
  
  if (error?.response?.data?.message) {
    message = error.response.data.message
  } else if (error?.message) {
    message = error.message
  }
  
  logger.error('API 错误', error)
  ElMessage.error(message)
  
  return message
}

export const confirmAction = async (message: string, title = '确认操作'): Promise<boolean> => {
  try {
    await ElMessageBox.confirm(message, title, {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    return true
  } catch {
    return false
  }
}

export const showSuccess = (message: string) => {
  ElMessage.success(message)
}

export const showWarning = (message: string) => {
  ElMessage.warning(message)
}

export const showInfo = (message: string) => {
  ElMessage.info(message)
}
