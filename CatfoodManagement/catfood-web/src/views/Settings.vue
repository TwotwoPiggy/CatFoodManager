<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>系统设置</span>
          <el-button type="primary" :icon="Check" @click="handleSave" :loading="saving">保存设置</el-button>
        </div>
      </template>

      <el-tabs v-model="activeTab">
        <el-tab-pane label="AI 配置" name="ai">
          <el-form :model="settingsForm" label-width="140px">
            <el-form-item label="API Key">
              <el-input
                v-model="settingsForm.AI.ApiKey"
                type="password"
                show-password
                placeholder="请输入 Gemini API Key"
              />
            </el-form-item>
            <el-form-item label="模型">
              <div class="model-select-wrapper">
                <el-select v-model="settingsForm.AI.ModelName" placeholder="请选择模型" :loading="loadingModels" class="model-select">
                  <el-option
                    v-for="model in models"
                    :key="model.Name"
                    :label="model.DisplayName"
                    :value="model.Name"
                  />
                </el-select>
                <el-button :icon="Refresh" @click="handleRefreshModels" :loading="loadingModels" title="刷新模型列表" />
              </div>
            </el-form-item>
            <el-form-item label="每分钟请求数">
              <el-input-number v-model="settingsForm.AI.RPM" :min="1" :max="60" />
            </el-form-item>
            <el-form-item label="每分钟令牌数">
              <el-input-number v-model="settingsForm.AI.TPM" :min="1000" :max="1000000" />
            </el-form-item>
            <el-form-item label="每天请求数">
              <el-input-number v-model="settingsForm.AI.RPD" :min="1" :max="1000" />
            </el-form-item>
            <el-form-item label="启用代理">
              <el-switch v-model="settingsForm.AI.Proxy.Enabled" />
            </el-form-item>
            <el-form-item v-if="settingsForm.AI.Proxy.Enabled" label="代理地址">
              <el-input v-model="settingsForm.AI.Proxy.Address" placeholder="http://127.0.0.1:10808" />
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <el-tab-pane label="OCR 设置" name="ocr">
          <el-form label-width="120px">
            <el-form-item label="模型状态">
              <el-tag :type="modelValid ? 'success' : 'danger'">
                {{ modelValid ? '已就绪' : '未就绪' }}
              </el-tag>
              <el-button type="primary" @click="validateModel" :loading="validating" style="margin-left: 10px">
                验证模型
              </el-button>
            </el-form-item>
            
            <el-divider />
            
            <!-- <el-form-item label="图片文件夹">
              <el-input v-model="ocrFolderPath" placeholder="选择图片文件夹" readonly>
                <template #append>
                  <el-button @click="handleSelectOcrFolder">浏览</el-button>
                </template>
              </el-input>
            </el-form-item> -->
            
            <el-form-item label="提示词模板">
              <div class="prompt-template-wrapper">
                <el-select 
                  v-model="selectedPromptId" 
                  placeholder="选择提示词模板" 
                  class="prompt-select"
                  @change="handlePromptSelect"
                >
                  <el-option
                    v-for="prompt in appConfigStore.ocrPrompts"
                    :key="prompt.Id"
                    :label="prompt.Name"
                    :value="prompt.Id"
                  >
                    <span>{{ prompt.Name }}</span>
                    <el-tag v-if="prompt.IsDefault" type="success" size="small" style="margin-left: 8px">默认</el-tag>
                  </el-option>
                </el-select>
                <el-button type="primary" :icon="Plus" @click="handleCreatePrompt">新建</el-button>
                <el-button :icon="Edit" @click="handleEditPrompt" :disabled="!selectedPromptId || isOnlyDefaultPrompt">编辑</el-button>
                <el-button type="danger" :icon="Delete" @click="handleDeletePrompt" :disabled="!selectedPromptId || isOnlyDefaultPrompt">删除</el-button>
              </div>
            </el-form-item>
            
            <el-form-item label="提示词内容">
              <el-input
                v-model="promptText"
                type="textarea"
                :rows="5"
                placeholder="输入 OCR 提示词"
                :disabled="isOnlyDefaultPrompt"
              />
            </el-form-item>
            
            <!-- <el-form-item>
              <el-button type="primary" @click="handleSyncFromPictures" :loading="syncing">
                从图片同步
              </el-button>
            </el-form-item> -->
          </el-form>
        </el-tab-pane>

        <el-tab-pane label="平台文件夹" name="platform">
          <el-form :model="settingsForm" label-width="140px">
            <el-form-item label="平台文件夹配置">
              <div class="platform-folders">
                <div v-for="platform in platforms" :key="platform.value" class="platform-folder-item">
                  <div class="platform-label">{{ platform.label }}</div>
                  <el-input 
                    :model-value="settingsForm.App.PlatformFolders[platform.value] || ''" 
                    placeholder="选择文件夹路径" 
                    readonly
                  >
                    <template #append>
                      <el-button @click="handleSelectPlatformFolder(platform.value)">浏览</el-button>
                    </template>
                  </el-input>
                  <el-button 
                    v-if="settingsForm.App.PlatformFolders[platform.value]" 
                    type="danger" 
                    :icon="Delete" 
                    circle 
                    @click="handleClearPlatformFolder(platform.value)"
                  />
                </div>
              </div>
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <el-tab-pane label="数据库配置" name="database">
          <el-form :model="settingsForm" label-width="140px">
            <el-form-item label="数据库路径">
              <el-input v-model="settingsForm.Database.ConnectionString" placeholder="./data/catfood.db" />
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <el-tab-pane label="关于" name="about">
          <el-descriptions :column="1" border>
            <el-descriptions-item label="应用名称">CatFood Manager</el-descriptions-item>
            <el-descriptions-item label="版本">1.0.0</el-descriptions-item>
            <el-descriptions-item label="描述">猫粮库存管理系统</el-descriptions-item>
          </el-descriptions>
        </el-tab-pane>
      </el-tabs>
    </el-card>

    <el-dialog
      v-model="promptDialogVisible"
      :title="isEditingPrompt ? '编辑提示词' : '新建提示词'"
      width="600px"
    >
      <el-form :model="promptForm" label-width="100px">
        <el-form-item label="名称" required>
          <el-input v-model="promptForm.Name" placeholder="请输入提示词名称" />
        </el-form-item>
        <el-form-item label="内容" required>
          <el-input
            v-model="promptForm.Content"
            type="textarea"
            :rows="6"
            placeholder="请输入提示词内容"
          />
        </el-form-item>
        <el-form-item label="设为默认">
          <el-switch v-model="promptForm.IsDefault" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="promptForm.Description" placeholder="可选描述" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="promptDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSavePrompt">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Check, Delete, Refresh, Plus, Edit } from '@element-plus/icons-vue'
import { 
  validateModel as validateModelApi, 
  syncFromPictures, 
  selectFolder,
  waitForCefSharp,
  getSettings,
  saveSettings,
  getModels,
  clearModelsCache,
  getOcrPrompts,
  getOcrPromptDefault,
  createOcrPrompt,
  updateOcrPrompt,
  deleteOcrPrompt,
  setOcrPromptDefault,
  type ModelInfo,
  type AppSettings,
  type OcrPrompt
} from '@/utils/bridge'
import { PlatformType, PlatformTypeLabels } from '@/types'
import { useAppConfigStore } from '@/stores/appConfig'

const appConfigStore = useAppConfigStore()

const activeTab = ref('ai')
const modelValid = ref(false)
const validating = ref(false)
const syncing = ref(false)
const saving = ref(false)
const loadingModels = ref(false)
const models = ref<ModelInfo[]>([])
const ocrFolderPath = ref('')
const promptText = ref('')
const selectedPromptId = ref<number | null>(null)
const promptDialogVisible = ref(false)
const promptForm = reactive({
  Id: 0,
  Name: '',
  Content: '',
  IsDefault: false,
  Description: ''
})
const isEditingPrompt = ref(false)

const isOnlyDefaultPrompt = computed(() => {
  return appConfigStore.ocrPrompts.length === 1 && appConfigStore.ocrPrompts[0].IsDefault
})

const platforms = Object.entries(PlatformTypeLabels)
  .filter(([key]) => parseInt(key) !== PlatformType.None)
  .map(([key, label]) => ({ value: label, label }))

const settingsForm = reactive<AppSettings>({
  AI: {
    ApiKey: '',
    ModelName: 'gemini-2.5-flash',
    RPM: 5,
    TPM: 250000,
    RPD: 20,
    Proxy: {
      Enabled: false,
      Address: 'http://127.0.0.1:7890'
    }
  },
  Database: {
    ConnectionString: './data/catfood.db'
  },
  App: {
    PlatformFolders: {}
  }
})

const loadSettings = async () => {
  try {
    const result = await getSettings()
    if (result.Success && result.Data) {
      settingsForm.AI = {
        ApiKey: result.Data.AI?.ApiKey || '',
        ModelName: result.Data.AI?.ModelName || 'gemini-2.5-flash',
        RPM: result.Data.AI?.RPM || 5,
        TPM: result.Data.AI?.TPM || 250000,
        RPD: result.Data.AI?.RPD || 20,
        Proxy: {
          Enabled: result.Data.AI?.Proxy?.Enabled || false,
          Address: result.Data.AI?.Proxy?.Address || 'http://127.0.0.1:7890'
        }
      }
      settingsForm.Database = {
        ConnectionString: result.Data.Database?.ConnectionString || './data/catfood.db'
      }
      settingsForm.App = {
        PlatformFolders: result.Data.App?.PlatformFolders || {}
      }
    }
  } catch (error) {
    console.error('Failed to load settings:', error)
  }
}

const loadModels = async (forceRefresh: boolean = false) => {
  if (!settingsForm.AI.ApiKey) {
    return
  }
  
  loadingModels.value = true
  try {
    const result = await getModels(settingsForm.AI.ApiKey, forceRefresh)
    if (result.Success && result.Data) {
      models.value = result.Data
    }
  } catch (error) {
    console.error('Failed to load models:', error)
  } finally {
    loadingModels.value = false
  }
}

const handleRefreshModels = async () => {
  if (!settingsForm.AI.ApiKey) {
    ElMessage.warning('请先输入 API Key')
    return
  }
  
  clearModelsCache(settingsForm.AI.ApiKey)
  await loadModels(true)
  ElMessage.success('模型列表已刷新')
}

const handleSave = async () => {
  saving.value = true
  try {
    const result = await saveSettings(settingsForm)
    if (result.Success) {
      ElMessage.success(result.Message || '设置保存成功')
    } else {
      ElMessage.error(result.Message || '保存失败')
    }
  } catch (error) {
    ElMessage.error('保存设置时发生错误')
    console.error(error)
  } finally {
    saving.value = false
  }
}

const validateModel = async () => {
  validating.value = true
  try {
    const result = await validateModelApi()
    modelValid.value = result.Success
    if (result.Success) {
      ElMessage.success('模型验证成功')
    } else {
      ElMessage.error(result.Message || '模型验证失败')
    }
  } catch (error) {
    ElMessage.error('验证模型时发生错误')
    console.error(error)
  } finally {
    validating.value = false
  }
}

const handleSelectOcrFolder = async () => {
  const path = await selectFolder()
  if (path) {
    ocrFolderPath.value = path
  }
}

const handleSelectPlatformFolder = async (platformName: string) => {
  const path = await selectFolder()
  if (path) {
    settingsForm.App.PlatformFolders[platformName] = path
  }
}

const handleClearPlatformFolder = (platformName: string) => {
  delete settingsForm.App.PlatformFolders[platformName]
}

const handleSyncFromPictures = async () => {
  if (!ocrFolderPath.value) {
    ElMessage.warning('请先选择图片文件夹')
    return
  }
  
  syncing.value = true
  try {
    const result = await syncFromPictures(ocrFolderPath.value, promptText.value)
    if (result.Success) {
      ElMessage.success(`同步成功，共处理 ${result.Count} 条记录`)
    } else {
      ElMessage.error(result.Message || '同步失败')
    }
  } catch (error) {
    ElMessage.error('同步时发生错误')
    console.error(error)
  } finally {
    syncing.value = false
  }
}

const loadOcrPrompts = async () => {
  await appConfigStore.fetchOcrPrompts()
  if (appConfigStore.ocrPrompts.length > 0 && !selectedPromptId.value) {
    selectedPromptId.value = appConfigStore.defaultPrompt?.Id || null
    promptText.value = appConfigStore.defaultPromptContent
  }
}

const handlePromptSelect = (id: number) => {
  const prompt = appConfigStore.ocrPrompts.find(p => p.Id === id)
  if (prompt) {
    promptText.value = prompt.Content
  }
}

const handleCreatePrompt = () => {
  isEditingPrompt.value = false
  promptForm.Id = 0
  promptForm.Name = ''
  promptForm.Content = promptText.value
  promptForm.IsDefault = false
  promptForm.Description = ''
  promptDialogVisible.value = true
}

const handleEditPrompt = () => {
  if (!selectedPromptId.value) return
  
  const prompt = appConfigStore.ocrPrompts.find(p => p.Id === selectedPromptId.value)
  if (prompt) {
    isEditingPrompt.value = true
    promptForm.Id = prompt.Id
    promptForm.Name = prompt.Name
    promptForm.Content = prompt.Content
    promptForm.IsDefault = prompt.IsDefault
    promptForm.Description = prompt.Description || ''
    promptDialogVisible.value = true
  }
}

const handleDeletePrompt = async () => {
  if (!selectedPromptId.value) return
  
  try {
    await ElMessageBox.confirm('确定要删除此提示词吗？', '确认删除', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    
    const result = await deleteOcrPrompt(selectedPromptId.value)
    if (result.Success) {
      ElMessage.success('删除成功')
      await appConfigStore.fetchOcrPrompts(true)
      selectedPromptId.value = null
      promptText.value = ''
    } else {
      ElMessage.error(result.Message || '删除失败')
    }
  } catch {
    // 用户取消
  }
}

const handleSavePrompt = async () => {
  if (!promptForm.Name.trim()) {
    ElMessage.warning('请输入提示词名称')
    return
  }
  if (!promptForm.Content.trim()) {
    ElMessage.warning('请输入提示词内容')
    return
  }
  
  try {
    let result
    if (isEditingPrompt.value) {
      result = await updateOcrPrompt(
        promptForm.Id,
        promptForm.Name,
        promptForm.Content,
        promptForm.IsDefault,
        promptForm.Description
      )
    } else {
      result = await createOcrPrompt(
        promptForm.Name,
        promptForm.Content,
        promptForm.IsDefault,
        promptForm.Description
      )
    }
    
    if (result.Success) {
      ElMessage.success(isEditingPrompt.value ? '更新成功' : '创建成功')
      promptDialogVisible.value = false
      await appConfigStore.fetchOcrPrompts(true)
      if (result.Data) {
        selectedPromptId.value = result.Data.Id
        promptText.value = result.Data.Content
      }
    } else {
      ElMessage.error(result.Message || '保存失败')
    }
  } catch (error) {
    ElMessage.error('保存提示词时发生错误')
    console.error(error)
  }
}

onMounted(async () => {
  await waitForCefSharp()
  await loadSettings()
  await loadModels()
  await loadOcrPrompts()
  appConfigStore.clearCache()
})
</script>

<style scoped lang="scss">
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.model-select-wrapper {
  display: flex;
  gap: 8px;
  width: 100%;

  .model-select {
    flex: 1;
  }
}

.prompt-template-wrapper {
  display: flex;
  gap: 8px;
  width: 100%;

  .prompt-select {
    flex: 1;
  }
}

.platform-folders {
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.platform-folder-item {
  display: flex;
  align-items: center;
  gap: 12px;

  .platform-label {
    width: 80px;
    flex-shrink: 0;
    font-weight: 500;
  }

  .el-input {
    flex: 1;
  }
}
</style>
