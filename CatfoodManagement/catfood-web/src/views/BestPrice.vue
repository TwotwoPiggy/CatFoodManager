<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>最佳价格管理</span>
          <div class="header-actions">
            <el-button type="primary" :icon="Refresh" @click="handleSync">AI同步数据</el-button>
            <el-button type="success" :icon="Plus" @click="handleAdd">新增记录</el-button>
          </div>
        </div>
      </template>

      <div class="search-bar">
        <el-input
          v-model="searchText"
          placeholder="搜索名称"
          clearable
          style="width: 300px"
          @keyup.enter="handleSearch"
        >
          <template #prefix>
            <el-icon><Search /></el-icon>
          </template>
        </el-input>
        <el-button type="primary" :icon="Search" @click="handleSearch">搜索</el-button>
        <el-button :icon="RefreshRight" @click="handleReset">重置</el-button>
      </div>

      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        height="calc(100vh - 320px)"
        @cell-dblclick="handleCellDblClick"
      >
        <el-table-column prop="Id" label="ID" width="80" fixed />
        <el-table-column prop="Name" label="名称" min-width="150">
          <template #default="{ row }">
            <el-input
              v-if="row.editing === 'Name'"
              v-model="row.editValue"
              type="textarea"
              :rows="2"
              :autosize="{ minRows: 2, maxRows: 4 }"
              size="small"
              @keyup.enter.ctrl="handleSaveEdit(row, 'Name')"
              @blur="handleSaveEdit(row, 'Name')"
            />
            <span v-else class="editable-cell">{{ row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="Type" label="类型" width="140">
          <template #default="{ row }">
            <el-select
              v-if="row.editing === 'Type'"
              v-model="row.editValue"
              size="small"
              @change="handleSaveEdit(row, 'Type')"
            >
              <el-option
                v-for="(label, value) in ProductTypeLabels"
                :key="value"
                :label="label"
                :value="Number(value)"
              />
            </el-select>
            <el-tag v-else>{{ getProductTypeLabel(row.Type) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Platform" label="平台" width="120">
          <template #default="{ row }">
            <el-select
              v-if="row.editing === 'Platform'"
              v-model="row.editValue"
              size="small"
              @change="handleSaveEdit(row, 'Platform')"
            >
              <el-option
                v-for="(label, value) in PlatformTypeLabels"
                :key="value"
                :label="label"
                :value="Number(value)"
              />
            </el-select>
            <span v-else class="editable-cell">{{ getPlatformLabel(row.Platform) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="LowestPrice" label="史低" width="120">
          <template #default="{ row }">
            <el-input-number
              v-if="row.editing === 'LowestPrice'"
              v-model="row.editValue"
              size="small"
              :min="0"
              :precision="2"
              controls-position="right"
              @keyup.enter="handleSaveEdit(row, 'LowestPrice')"
              @blur="handleSaveEdit(row, 'LowestPrice')"
            />
            <span v-else class="editable-cell">¥{{ row.LowestPrice?.toFixed(2) || '0.00' }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="FinalPrice" label="购买价格" width="120">
          <template #default="{ row }">
            <el-input-number
              v-if="row.editing === 'FinalPrice'"
              v-model="row.editValue"
              size="small"
              :min="0"
              :precision="2"
              controls-position="right"
              @keyup.enter="handleSaveEdit(row, 'FinalPrice')"
              @blur="handleSaveEdit(row, 'FinalPrice')"
            />
            <span v-else class="editable-cell">{{ row.FinalPrice ? '¥' + row.FinalPrice.toFixed(2) : '-' }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="HasPurchased" label="已购买" width="90">
          <template #default="{ row }">
            <el-switch
              v-if="row.editing === 'HasPurchased'"
              v-model="row.editValue"
              @change="handleSaveEdit(row, 'HasPurchased')"
            />
            <el-tag v-else :type="row.HasPurchased ? 'success' : 'info'">
              {{ row.HasPurchased ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="PicturePath" label="照片" width="100">
          <template #default="{ row }">
            <el-button
              v-if="row.PicturePath"
              type="primary"
              link
              @click="handleViewImage(row.PicturePath)"
            >
              查看
            </el-button>
            <el-button v-else type="primary" link @click="handleUploadImage(row)">上传</el-button>
          </template>
        </el-table-column>
        <el-table-column prop="FactoryName" label="代工厂" width="120">
          <template #default="{ row }">
            <el-input
              v-if="row.editing === 'FactoryName'"
              v-model="row.editValue"
              size="small"
              @keyup.enter="handleSaveEdit(row, 'FactoryName')"
              @blur="handleSaveEdit(row, 'FactoryName')"
            />
            <span v-else class="editable-cell">{{ row.FactoryName || '-' }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="HasTestReport" label="检测报告" width="100">
          <template #default="{ row }">
            <el-switch
              v-if="row.editing === 'HasTestReport'"
              v-model="row.editValue"
              @change="handleSaveEdit(row, 'HasTestReport')"
            />
            <el-tag v-else :type="row.HasTestReport ? 'success' : 'info'">
              {{ row.HasTestReport ? '有' : '无' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="IsWorthRepurchasing" label="值得回购" width="100">
          <template #default="{ row }">
            <el-switch
              v-if="row.editing === 'IsWorthRepurchasing'"
              v-model="row.editValue"
              @change="handleSaveEdit(row, 'IsWorthRepurchasing')"
            />
            <el-tag v-else :type="row.IsWorthRepurchasing ? 'success' : 'info'">
              {{ row.IsWorthRepurchasing ? '是' : '否' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="PurchasedAt" label="购买时间" width="140">
          <template #default="{ row }">
            <el-date-picker
              v-if="row.editing === 'PurchasedAt'"
              v-model="row.editValue"
              size="small"
              type="date"
              value-format="YYYY-MM-DD"
              @change="handleSaveEdit(row, 'PurchasedAt')"
            />
            <span v-else class="editable-cell">{{ formatDate(row.PurchasedAt) }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ row }">
            <el-popconfirm
              title="确定要删除这条记录吗?"
              @confirm="handleDelete(row)"
            >
              <template #reference>
                <el-button type="danger" link>删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-container">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="loadData"
          @current-change="loadData"
        />
      </div>
    </el-card>

    <el-dialog v-model="addDialogVisible" title="新增最佳价格记录" width="500px">
      <el-form :model="addForm" label-width="100px">
        <el-form-item label="名称" required>
          <el-input v-model="addForm.Name" placeholder="请输入名称" />
        </el-form-item>
        <el-form-item label="类型">
          <el-select v-model="addForm.Type" placeholder="请选择类型">
            <el-option
              v-for="(label, value) in ProductTypeLabels"
              :key="value"
              :label="label"
              :value="Number(value)"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="平台">
          <el-select v-model="addForm.Platform" placeholder="请选择平台">
            <el-option
              v-for="(label, value) in PlatformTypeLabels"
              :key="value"
              :label="label"
              :value="Number(value)"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="史低价格">
          <el-input-number v-model="addForm.LowestPrice" :min="0" :precision="2" />
        </el-form-item>
        <el-form-item label="购买价格">
          <el-input-number v-model="addForm.FinalPrice" :min="0" :precision="2" />
        </el-form-item>
        <el-form-item label="已购买">
          <el-switch v-model="addForm.HasPurchased" />
        </el-form-item>
        <el-form-item label="代工厂">
          <el-input v-model="addForm.FactoryName" placeholder="请输入代工厂名称" />
        </el-form-item>
        <el-form-item label="有检测报告">
          <el-switch v-model="addForm.HasTestReport" />
        </el-form-item>
        <el-form-item label="值得回购">
          <el-switch v-model="addForm.IsWorthRepurchasing" />
        </el-form-item>
        <el-form-item label="购买时间">
          <el-date-picker
            v-model="addForm.PurchasedAt"
            type="date"
            value-format="YYYY-MM-DD"
            placeholder="请选择购买时间"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="addDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSaveAdd">确定</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="imageDialogVisible" title="图片查看" width="600px">
      <img :src="currentImageUrl" style="width: 100%" />
    </el-dialog>

    <el-dialog v-model="syncDialogVisible" title="OCR 同步" width="700px" align-center>
      <div class="sync-content">
        <el-form :model="syncForm" label-width="120px">
          <el-form-item label="图片文件夹">
            <el-select v-model="syncForm.folderPath" placeholder="请选择图片文件夹" style="width: 100%">
              <el-option
                v-for="option in appConfigStore.folderOptions"
                :key="option.value"
                :label="`${option.label} (${option.value})`"
                :value="option.value"
              />
            </el-select>
          </el-form-item>
          <el-form-item label="平台">
            <el-input :model-value="getPlatformLabel(syncForm.platform)" readonly placeholder="根据文件夹自动关联" style="width: 100%" />
          </el-form-item>
          <el-form-item label="提示词模板">
            <el-select 
              v-model="selectedPromptId" 
              placeholder="选择提示词模板" 
              style="width: 100%"
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
          </el-form-item>
          <el-form-item label="提示词内容">
            <el-input
              v-model="syncForm.promptText"
              type="textarea"
              :rows="10"
              readonly
              placeholder="请先选择提示词模板"
            />
          </el-form-item>
        </el-form>
      </div>
      <template #footer>
        <el-button @click="syncDialogVisible = false">取消</el-button>
        <el-button type="primary" :loading="syncing" @click="handleStartSync">
          开始同步
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onActivated, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { Search, RefreshRight, Plus, Refresh } from '@element-plus/icons-vue'
import { getBestPrices, addBestPrice, updateBestPrice, deleteBestPrice, viewImage, waitForCefSharp, createTask } from '@/utils/bridge'
import { ProductType, PlatformType, TaskType } from '@/types'
import { useAppConfigStore } from '@/stores/appConfig'

// 最佳价格记录的数据结构
interface BestPriceRow {
  Id: number
  Name: string
  Type: ProductType
  Platform: PlatformType
  LowestPrice: number
  HasPurchased: boolean
  FinalPrice: number | null
  PicturePath: string | null
  FactoryName: string | null
  HasTestReport: boolean
  IsWorthRepurchasing: boolean
  PurchasedAt: string | null
  editing?: string
  editValue?: any
}

// 表格相关状态
const loading = ref(false)
const searchText = ref('')
const currentPage = ref(1)
const pageSize = ref(50)
const total = ref(0)
const tableData = ref<BestPriceRow[]>([])

// 对话框相关状态
const addDialogVisible = ref(false)
const imageDialogVisible = ref(false)
const currentImageUrl = ref('')
const syncDialogVisible = ref(false)
const syncing = ref(false)

// 使用 appConfig store
const appConfigStore = useAppConfigStore()

// OCR 同步表单
const syncForm = reactive({
  folderPath: '',
  promptText: '',
  platform: PlatformType.Taobao
})

// 选中的 prompt ID
const selectedPromptId = ref<number | null>(null)

// 新增记录表单
const addForm = reactive({
  Name: '',
  Type: ProductType.CatFood,
  Platform: PlatformType.Taobao,
  LowestPrice: 0,
  FinalPrice: null as number | null,
  HasPurchased: false,
  FactoryName: '',
  HasTestReport: false,
  IsWorthRepurchasing: false,
  PurchasedAt: null as string | null
})

// 产品类型映射
const ProductTypeLabels = {
  [ProductType.CatFood]: '猫粮',
  [ProductType.CatSnack]: '零食',
  [ProductType.CannedFood]: '主食罐头',
  [ProductType.FreezeDriedFood]: '主食冻干',
  [ProductType.Others]: '其他'
}

// 平台类型映射
const PlatformTypeLabels = {
  [PlatformType.None]: '未知',
  [PlatformType.JD]: '京东',
  [PlatformType.Taobao]: '淘宝',
  [PlatformType.PDD]: '拼多多',
  [PlatformType.Douyin]: '抖音',
  [PlatformType.Kuaishou]: '快手'
}

const PlatformNameToType: Record<string, PlatformType> = {
  'JD': PlatformType.JD,
  'Taobao': PlatformType.Taobao,
  'PDD': PlatformType.PDD,
  'Douyin': PlatformType.Douyin,
  'Kuaishou': PlatformType.Kuaishou
}

// 获取产品类型显示名称
const getProductTypeLabel = (type: ProductType) => {
  return ProductTypeLabels[type] || '未知'
}

// 获取平台显示名称
const getPlatformLabel = (platform: PlatformType) => {
  return PlatformTypeLabels[platform] || '未知'
}

// 格式化日期
const formatDate = (date: string) => {
  if (!date) return '-'
  return date.split('T')[0]
}

// 加载表格数据
const loadData = async () => {
  loading.value = true
  try {
    const result = await getBestPrices(currentPage.value, pageSize.value, searchText.value)
    console.log('BestPrice loadData result:', result)
    if (result && result.Data) {
      tableData.value = result.Data.map((item: any) => ({
        ...item,
        editing: undefined,
        editValue: undefined
      }))
      total.value = result.Total
    } else {
      tableData.value = []
      total.value = 0
    }
  } catch (error) {
    ElMessage.error('加载数据失败')
    console.error(error)
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  currentPage.value = 1
  loadData()
}

// 重置搜索
const handleReset = () => {
  searchText.value = ''
  currentPage.value = 1
  loadData()
}

const updatePlatformFromFolder = (folderPath: string) => {
  if (!folderPath) return
  // console.log('updatePlatformFromFolder - folderPath:', folderPath)
  // console.log('updatePlatformFromFolder - folderOptions:', appConfigStore.folderOptions)
  const selectedOption = appConfigStore.folderOptions.find(opt => opt.value === folderPath)
  // console.log('updatePlatformFromFolder - selectedOption:', selectedOption)
  if (selectedOption) {
    const platformType = PlatformNameToType[selectedOption.label]
    // console.log('updatePlatformFromFolder - label:', selectedOption.label, 'platformType:', platformType)
    if (platformType !== undefined) {
      syncForm.platform = platformType
    }
  }
}

watch(() => syncForm.folderPath, (newPath) => {
  updatePlatformFromFolder(newPath)
})

// 打开同步对话框，每次打开时重新加载设置以实现热加载
const handleSync = async () => {
  await appConfigStore.fetchAll(true)
  if (appConfigStore.folderOptions.length > 0) {
    if (!syncForm.folderPath) {
      syncForm.folderPath = appConfigStore.folderOptions[0].value
    }
    updatePlatformFromFolder(syncForm.folderPath)
  }
  if (!selectedPromptId.value && appConfigStore.defaultPrompt) {
    selectedPromptId.value = appConfigStore.defaultPrompt.Id
    syncForm.promptText = appConfigStore.defaultPromptContent
  }
  syncDialogVisible.value = true
}

// 选择 prompt 时更新 promptText
const handlePromptSelect = (id: number) => {
  const prompt = appConfigStore.ocrPrompts.find(p => p.Id === id)
  if (prompt) {
    syncForm.promptText = prompt.Content
  }
}

// 开始 OCR 同步 - 创建任务
const handleStartSync = async () => {
  if (!syncForm.folderPath) {
    ElMessage.warning('请先选择图片文件夹')
    return
  }
  
  syncing.value = true
  try {
    const parameters = JSON.stringify({
      FolderPath: syncForm.folderPath,
      PromptText: syncForm.promptText,
      Platform: syncForm.platform
    })
    
    const result = await createTask(
      TaskType.ImageSync,
      `同步 ${syncForm.folderPath.split(/[/\\]/).pop() || syncForm.folderPath}`,
      parameters,
      `从文件夹同步图片数据`
    )
    
    if (result.Success) {
      ElMessage.success('任务已创建，请前往任务管理查看进度')
      syncDialogVisible.value = false
    } else {
      ElMessage.error(result.Message || '创建任务失败')
    }
  } catch (error) {
    ElMessage.error('创建任务时发生错误')
    console.error(error)
  } finally {
    syncing.value = false
  }
}

// 打开新增对话框，重置表单
const handleAdd = () => {
  addForm.Name = ''
  addForm.Type = ProductType.CatFood
  addForm.Platform = PlatformType.Taobao
  addForm.LowestPrice = 0
  addForm.FinalPrice = null
  addForm.HasPurchased = false
  addForm.FactoryName = ''
  addForm.HasTestReport = false
  addForm.IsWorthRepurchasing = false
  addForm.PurchasedAt = null
  addDialogVisible.value = true
}

// 保存新增记录
const handleSaveAdd = async () => {
  if (!addForm.Name.trim()) {
    ElMessage.warning('请输入名称')
    return
  }

  try {
    const result = await addBestPrice(addForm)
    if (result.Success) {
      ElMessage.success('添加成功')
      addDialogVisible.value = false
      loadData()
    } else {
      ElMessage.error(result.Message || '添加失败')
    }
  } catch (error) {
    ElMessage.error('添加失败')
    console.error(error)
  }
}

// 双击单元格进入编辑模式
const handleCellDblClick = (row: BestPriceRow, column: any) => {
  const field = column.property
  if (!field || field === 'Id' || field === 'PicturePath') {
    return
  }
  
  row.editing = field
  row.editValue = row[field as keyof BestPriceRow]
}

// 保存单元格编辑
const handleSaveEdit = async (row: BestPriceRow, field: string) => {
  if (row.editing !== field) return
  
  const oldValue = row[field as keyof BestPriceRow]
  const newValue = row.editValue
  
  // 值未变化，直接退出编辑模式
  if (oldValue === newValue) {
    row.editing = undefined
    row.editValue = undefined
    return
  }
  
  try {
    const result = await updateBestPrice(row.Id, field, newValue)
    if (result.Success) {
      ElMessage.success('更新成功')
      ;(row as any)[field] = newValue
      row.editing = undefined
      row.editValue = undefined
    } else {
      ElMessage.error(result.Message || '更新失败')
      row.editing = undefined
      row.editValue = undefined
    }
  } catch (error) {
    ElMessage.error('更新失败')
    console.error(error)
    row.editing = undefined
    row.editValue = undefined
  }
}

// 删除记录
const handleDelete = async (row: BestPriceRow) => {
  try {
    const result = await deleteBestPrice(row.Id)
    if (result.Success) {
      ElMessage.success('删除成功')
      loadData()
    } else {
      ElMessage.error(result.Message || '删除失败')
    }
  } catch (error) {
    ElMessage.error('删除失败')
    console.error(error)
  }
}

// 查看图片
const handleViewImage = (picturePath: string) => {
  if (picturePath.startsWith('http')) {
    currentImageUrl.value = picturePath
    imageDialogVisible.value = true
  } else {
    viewImage(picturePath)
  }
}

// 上传图片（待开发）
const handleUploadImage = (row: BestPriceRow) => {
  ElMessage.info('上传功能开发中...')
}

// 组件挂载时初始化
onMounted(async () => {
  await waitForCefSharp()
  await Promise.all([loadData(), appConfigStore.fetchAll()])
})

// 组件激活时刷新数据（keep-alive）
onActivated(() => {
  loadData()
})
</script>

<style scoped lang="scss">
.search-bar {
  margin-bottom: 16px;
  display: flex;
  gap: 10px;
}

.editable-cell {
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: background-color 0.2s;
  
  &:hover {
    background-color: #f5f7fa;
  }
}

.pagination-container {
  margin-top: 16px;
  display: flex;
  justify-content: flex-end;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.sync-content {
  padding: 10px 0;
}
</style>
