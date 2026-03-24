<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>任务管理</span>
          <div class="header-actions">
            <el-button type="primary" :icon="Setting" @click="showConfigDialog">配置</el-button>
            <el-button :icon="Refresh" @click="handleRefresh" :loading="loading">刷新</el-button>
          </div>
        </div>
      </template>

      <div class="service-status-card">
        <div class="status-info">
          <div class="status-indicator">
            <el-tag :type="serviceStatusType" size="large">
              <span class="status-dot" :class="serviceStatusClass"></span>
              {{ serviceStatusText }}
            </el-tag>
          </div>
          <div class="status-stats">
            <span class="stat-item">
              <el-icon><List /></el-icon>
              队列: {{ serviceStatus?.QueueLength ?? queueLength }}
            </span>
            <span class="stat-item">
              <el-icon><Loading /></el-icon>
              执行中: {{ serviceStatus?.RunningTaskCount ?? runningCount }}
            </span>
            <span class="stat-item" v-if="serviceStatus?.StartedAt">
              <el-icon><Clock /></el-icon>
              启动: {{ formatDate(serviceStatus.StartedAt) }}
            </span>
          </div>
        </div>
        <div class="status-actions">
          <el-button 
            v-if="!serviceStatus?.IsRunning" 
            type="success" 
            :icon="VideoPlay" 
            @click="handleStartService"
            :loading="serviceLoading"
          >启动服务</el-button>
          <el-button 
            v-if="serviceStatus?.IsRunning && !serviceStatus?.IsPaused" 
            type="warning" 
            :icon="VideoPause" 
            @click="handlePauseService"
            :loading="serviceLoading"
          >暂停</el-button>
          <el-button 
            v-if="serviceStatus?.IsRunning && serviceStatus?.IsPaused" 
            type="success" 
            :icon="VideoPlay" 
            @click="handleResumeService"
            :loading="serviceLoading"
          >恢复</el-button>
          <el-button 
            v-if="serviceStatus?.IsRunning" 
            type="danger" 
            :icon="RefreshRight" 
            @click="handleRestartService"
            :loading="serviceLoading"
          >重启</el-button>
        </div>
      </div>

      <div class="filter-bar">
        <el-select v-model="filterStatus" placeholder="状态筛选" clearable @change="handleFilterChange" style="width: 120px">
          <el-option v-for="(label, value) in TaskStatusLabels" :key="value" :label="label" :value="Number(value)" />
        </el-select>
        <el-select v-model="filterType" placeholder="类型筛选" clearable @change="handleFilterChange" style="width: 120px; margin-left: 12px">
          <el-option v-for="(label, value) in TaskTypeLabels" :key="value" :label="label" :value="Number(value)" />
        </el-select>
      </div>

      <el-table :data="tasks" v-loading="loading" stripe>
        <el-table-column prop="Id" label="ID" width="80" />
        <el-table-column prop="Name" label="名称" min-width="150" />
        <el-table-column prop="Type" label="类型" width="100">
          <template #default="{ row }">
            <el-tag>{{ TaskTypeLabels[row.Type] || row.Type }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Status" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusTagType(row.Status)">{{ TaskStatusLabels[row.Status] || row.Status }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="CreatedAt" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.CreatedAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button size="small" @click="showDetailDialog(row)">详情</el-button>
            <el-button 
              v-if="row.Status === TaskStatus.Running" 
              size="small" 
              type="warning" 
              @click="handleTerminate(row.Id)"
            >终止</el-button>
            <el-button 
              v-if="canRetry(row.Status)" 
              size="small" 
              type="primary" 
              @click="handleRetry(row.Id)"
            >重试</el-button>
            <el-button 
              v-if="canDelete(row.Status)" 
              size="small" 
              type="danger" 
              @click="handleDelete(row.Id)"
            >删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-container">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="currentPageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handlePageChange"
        />
      </div>
    </el-card>

    <el-dialog v-model="detailDialogVisible" title="任务详情" width="600px">
      <el-descriptions :column="1" border v-if="currentTask">
        <el-descriptions-item label="ID">{{ currentTask.Id }}</el-descriptions-item>
        <el-descriptions-item label="名称">{{ currentTask.Name }}</el-descriptions-item>
        <el-descriptions-item label="类型">{{ TaskTypeLabels[currentTask.Type] }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <el-tag :type="getStatusTagType(currentTask.Status)">{{ TaskStatusLabels[currentTask.Status] }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="描述">{{ currentTask.Description || '-' }}</el-descriptions-item>
        <el-descriptions-item label="参数">
          <pre class="json-content">{{ formatJson(currentTask.Parameters) }}</pre>
        </el-descriptions-item>
        <el-descriptions-item label="结果" v-if="currentTask.Result">
          <pre class="json-content">{{ formatJson(currentTask.Result) }}</pre>
        </el-descriptions-item>
        <el-descriptions-item label="错误信息" v-if="currentTask.ErrorMessage">
          <el-text type="danger">{{ currentTask.ErrorMessage }}</el-text>
        </el-descriptions-item>
        <el-descriptions-item label="重试次数">{{ currentTask.RetryCount }} / {{ currentTask.MaxRetries }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">{{ formatDate(currentTask.CreatedAt) }}</el-descriptions-item>
        <el-descriptions-item label="开始时间">{{ currentTask.StartedAt ? formatDate(currentTask.StartedAt) : '-' }}</el-descriptions-item>
        <el-descriptions-item label="完成时间">{{ currentTask.CompletedAt ? formatDate(currentTask.CompletedAt) : '-' }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="detailDialogVisible = false">关闭</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="configDialogVisible" title="任务配置" width="500px">
      <el-form :model="configForm" label-width="140px" v-if="configForm">
        <el-form-item label="最大并发任务数">
          <el-input-number v-model="configForm.MaxConcurrentTasks" :min="1" :max="10" />
        </el-form-item>
        <el-form-item label="轮询间隔(秒)">
          <el-input-number v-model="configForm.PollingIntervalSeconds" :min="5" :max="3600" />
        </el-form-item>
        <el-form-item label="启用定时调度">
          <el-switch v-model="configForm.EnableScheduling" />
        </el-form-item>
        <el-form-item label="默认调度表达式" v-if="configForm.EnableScheduling">
          <ScheduleSelector v-model="configForm.DefaultSchedule" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="configDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSaveConfig" :loading="saving">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted, onActivated } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Setting, Refresh, VideoPlay, VideoPause, RefreshRight, List, Loading, Clock } from '@element-plus/icons-vue'
import { useTaskStore } from '@/stores/task'
import { TaskStatus, TaskType, TaskStatusLabels, TaskTypeLabels, type TaskItem, type TaskConfiguration } from '@/types'
import { storeToRefs } from 'pinia'
import ScheduleSelector from '@/components/ScheduleSelector.vue'
import { useServiceStatus } from '@/composables/useServiceStatus'
import { startBackgroundService, pauseBackgroundService, resumeBackgroundService, restartBackgroundService, waitForCefSharp } from '@/utils/bridge'

const taskStore = useTaskStore()
const { tasks, loading, total, page, pageSize, runningCount, queueLength } = storeToRefs(taskStore)

const {
  status: serviceStatus,
  loading: serviceLoading,
  isRunning,
  isPaused,
  isStopped,
  fetchStatus: fetchServiceStatus
} = useServiceStatus()

const currentPage = ref(1)
const currentPageSize = ref(10)
const filterStatus = ref<number | undefined>()
const filterType = ref<number | undefined>()
const detailDialogVisible = ref(false)
const configDialogVisible = ref(false)
const currentTask = ref<TaskItem | null>(null)
const configForm = reactive<TaskConfiguration>({
  Id: 0,
  MaxConcurrentTasks: 2,
  PollingIntervalSeconds: 60,
  EnableScheduling: false,
  DefaultSchedule: '',
  CreatedAt: ''
})
const saving = ref(false)

let refreshTimer: ReturnType<typeof setInterval> | null = null
let serviceStatusTimer: ReturnType<typeof setInterval> | null = null

const serviceStatusType = computed(() => {
  if (!serviceStatus.value?.IsRunning) return 'danger'
  if (serviceStatus.value?.IsPaused) return 'warning'
  return 'success'
})

const serviceStatusClass = computed(() => {
  if (!serviceStatus.value?.IsRunning) return 'stopped'
  if (serviceStatus.value?.IsPaused) return 'paused'
  return 'running'
})

const serviceStatusText = computed(() => {
  if (!serviceStatus.value?.IsRunning) return '已停止'
  if (serviceStatus.value?.IsPaused) return '已暂停'
  return '运行中'
})

const handleRefresh = async () => {
  await taskStore.fetchTasks(currentPage.value, currentPageSize.value, filterStatus.value, filterType.value)
  await taskStore.refreshRunningCount()
  await taskStore.refreshQueueLength()
  await fetchServiceStatus()
}

const handleFilterChange = () => {
  currentPage.value = 1
  handleRefresh()
}

const handlePageChange = (p: number) => {
  currentPage.value = p
  taskStore.fetchTasks(p, currentPageSize.value, filterStatus.value, filterType.value)
}

const handleSizeChange = (size: number) => {
  currentPageSize.value = size
  currentPage.value = 1
  taskStore.fetchTasks(1, size, filterStatus.value, filterType.value)
}

const showDetailDialog = (task: TaskItem) => {
  currentTask.value = task
  detailDialogVisible.value = true
}

const showConfigDialog = async () => {
  const result = await taskStore.fetchConfiguration()
  if (result.Success && result.Data) {
    Object.assign(configForm, result.Data)
    configDialogVisible.value = true
  }
}

const handleSaveConfig = async () => {
  saving.value = true
  try {
    const result = await taskStore.updateConfiguration(configForm)
    if (result.Success) {
      ElMessage.success('配置保存成功')
      configDialogVisible.value = false
    } else {
      ElMessage.error(result.Message || '保存失败')
    }
  } finally {
    saving.value = false
  }
}

const handleTerminate = async (id: number) => {
  try {
    await ElMessageBox.confirm('确定要终止此任务吗？', '确认终止', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    const result = await taskStore.terminateTask(id)
    if (result.Success) {
      ElMessage.success('任务已终止')
    } else {
      ElMessage.error(result.Message || '终止失败')
    }
  } catch {
  }
}

const handleRetry = async (id: number) => {
  const result = await taskStore.retryTask(id)
  if (result.Success) {
    ElMessage.success('任务已重新加入队列')
  } else {
    ElMessage.error(result.Message || '重试失败')
  }
}

const handleDelete = async (id: number) => {
  try {
    await ElMessageBox.confirm('确定要删除此任务吗？', '确认删除', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    const result = await taskStore.deleteTask(id)
    if (result.Success) {
      ElMessage.success('任务已删除')
    } else {
      ElMessage.error(result.Message || '删除失败')
    }
  } catch {
  }
}

const handleStartService = async () => {
  const result = await startBackgroundService()
  if (result.Success) {
    ElMessage.success('后台服务已启动')
    await fetchServiceStatus()
  } else {
    ElMessage.error(result.Message || '启动失败')
  }
}

const handlePauseService = async () => {
  const result = await pauseBackgroundService()
  if (result.Success) {
    ElMessage.success('后台服务已暂停')
    await fetchServiceStatus()
  } else {
    ElMessage.error(result.Message || '暂停失败')
  }
}

const handleResumeService = async () => {
  const result = await resumeBackgroundService()
  if (result.Success) {
    ElMessage.success('后台服务已恢复')
    await fetchServiceStatus()
  } else {
    ElMessage.error(result.Message || '恢复失败')
  }
}

const handleRestartService = async () => {
  try {
    await ElMessageBox.confirm('确定要重启后台服务吗？正在执行的任务将被中断。', '确认重启', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    const result = await restartBackgroundService()
    if (result.Success) {
      ElMessage.success('后台服务已重启')
      await fetchServiceStatus()
    } else {
      ElMessage.error(result.Message || '重启失败')
    }
  } catch {
  }
}

const checkServiceBeforeCreateTask = async (): Promise<boolean> => {
  const status = await fetchServiceStatus()
  
  if (!status?.IsRunning || status?.IsPaused) {
    try {
      await ElMessageBox.confirm(
        '后台服务未运行，任务将无法自动执行。是否立即启动后台服务？',
        '服务未运行',
        {
          confirmButtonText: '启动服务',
          cancelButtonText: '稍后处理',
          type: 'warning'
        }
      )
      const result = await startBackgroundService()
      if (result.Success) {
        ElMessage.success('后台服务已启动')
        await fetchServiceStatus()
        return true
      } else {
        ElMessage.error(result.Message || '启动失败')
        return false
      }
    } catch {
      return true
    }
  }
  
  return true
}

const getStatusTagType = (status: TaskStatus): 'success' | 'warning' | 'danger' | 'info' | '' => {
  switch (status) {
    case TaskStatus.Completed:
      return 'success'
    case TaskStatus.Running:
      return 'warning'
    case TaskStatus.Failed:
      return 'danger'
    case TaskStatus.Pending:
    case TaskStatus.Queued:
      return 'info'
    default:
      return ''
  }
}

const canRetry = (status: TaskStatus): boolean => {
  return status === TaskStatus.Failed || status === TaskStatus.Cancelled
}

const canDelete = (status: TaskStatus): boolean => {
  return status !== TaskStatus.Running
}

const formatDate = (dateStr: string): string => {
  if (!dateStr) return '-'
  try {
    const date = new Date(dateStr)
    return date.toLocaleString('zh-CN')
  } catch {
    return dateStr
  }
}

const formatJson = (jsonStr: string): string => {
  if (!jsonStr) return '-'
  try {
    return JSON.stringify(JSON.parse(jsonStr), null, 2)
  } catch {
    return jsonStr
  }
}

onMounted(async () => {
  await waitForCefSharp()
  await handleRefresh()
  await fetchServiceStatus()
  
  refreshTimer = setInterval(() => {
    taskStore.refreshRunningCount()
    taskStore.refreshQueueLength()
  }, 5000)
  
  serviceStatusTimer = setInterval(() => {
    fetchServiceStatus()
  }, 10000)
})

onActivated(async () => {
  await handleRefresh()
  await fetchServiceStatus()
})

onUnmounted(() => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
  }
  if (serviceStatusTimer) {
    clearInterval(serviceStatusTimer)
  }
})

defineExpose({
  checkServiceBeforeCreateTask
})
</script>

<style scoped lang="scss">
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-actions {
  display: flex;
  gap: 8px;
}

.service-status-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  background-color: #f5f7fa;
  border-radius: 8px;
  margin-bottom: 16px;
}

.status-info {
  display: flex;
  align-items: center;
  gap: 24px;
}

.status-indicator {
  .el-tag {
    padding: 8px 16px;
    font-size: 14px;
  }
}

.status-dot {
  display: inline-block;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-right: 8px;
  
  &.running {
    background-color: #67c23a;
    animation: pulse 2s infinite;
  }
  
  &.paused {
    background-color: #e6a23c;
  }
  
  &.stopped {
    background-color: #f56c6c;
  }
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.status-stats {
  display: flex;
  gap: 20px;
  
  .stat-item {
    display: flex;
    align-items: center;
    gap: 4px;
    color: #606266;
    font-size: 14px;
  }
}

.status-actions {
  display: flex;
  gap: 8px;
}

.filter-bar {
  display: flex;
  margin-bottom: 16px;
}

.pagination-container {
  margin-top: 16px;
  display: flex;
  justify-content: flex-end;
}

.json-content {
  background-color: #f5f7fa;
  padding: 8px;
  border-radius: 4px;
  font-size: 12px;
  max-height: 200px;
  overflow: auto;
  white-space: pre-wrap;
  word-break: break-all;
}
</style>
