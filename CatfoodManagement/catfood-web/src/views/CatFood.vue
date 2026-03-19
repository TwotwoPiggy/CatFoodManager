<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>猫粮库存管理</span>
          <el-button type="primary" :icon="Refresh" @click="handleSync">同步数据</el-button>
        </div>
      </template>

      <div class="search-bar">
        <el-input
          v-model="searchText"
          placeholder="搜索名称、品牌或ID"
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
              size="small"
              @keyup.enter="handleSaveEdit(row, 'Name')"
              @blur="handleSaveEdit(row, 'Name')"
            />
            <span v-else class="editable-cell">{{ row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="BrandName" label="品牌" width="120">
          <template #default="{ row }">
            <span class="editable-cell">{{ row.BrandName || '-' }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="FoodType" label="类型" width="140">
          <template #default="{ row }">
            <el-select
              v-if="row.editing === 'FoodType'"
              v-model="row.editValue"
              size="small"
              @change="handleSaveEdit(row, 'FoodType')"
            >
              <el-option
                v-for="(label, value) in ProductTypeLabels"
                :key="value"
                :label="label"
                :value="Number(value)"
              />
            </el-select>
            <el-tag v-else>{{ getProductTypeLabel(row.FoodType) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="Count" label="数量" width="100">
          <template #default="{ row }">
            <el-input-number
              v-if="row.editing === 'Count'"
              v-model="row.editValue"
              size="small"
              :min="0"
              controls-position="right"
              @keyup.enter="handleSaveEdit(row, 'Count')"
              @blur="handleSaveEdit(row, 'Count')"
            />
            <span v-else class="editable-cell">{{ row.Count }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="Price" label="价格" width="100">
          <template #default="{ row }">
            <el-input-number
              v-if="row.editing === 'Price'"
              v-model="row.editValue"
              size="small"
              :min="0"
              :precision="2"
              controls-position="right"
              @keyup.enter="handleSaveEdit(row, 'Price')"
              @blur="handleSaveEdit(row, 'Price')"
            />
            <span v-else class="editable-cell">¥{{ row.Price?.toFixed(2) || '0.00' }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="Weights" label="份量(g)" width="100">
          <template #default="{ row }">
            <el-input-number
              v-if="row.editing === 'Weights'"
              v-model="row.editValue"
              size="small"
              :min="0"
              controls-position="right"
              @keyup.enter="handleSaveEdit(row, 'Weights')"
              @blur="handleSaveEdit(row, 'Weights')"
            />
            <span v-else class="editable-cell">{{ row.Weights }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="ProductionDate" label="生产日期" width="140">
          <template #default="{ row }">
            <el-date-picker
              v-if="row.editing === 'ProductionDate'"
              v-model="row.editValue"
              size="small"
              type="date"
              value-format="YYYY-MM-DD"
              @change="handleSaveEdit(row, 'ProductionDate')"
            />
            <span v-else class="editable-cell">{{ formatDate(row.ProductionDate) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="FeededCount" label="已投喂" width="100">
          <template #default="{ row }">
            <el-input-number
              v-if="row.editing === 'FeededCount'"
              v-model="row.editValue"
              size="small"
              :min="0"
              :max="row.Count"
              controls-position="right"
              @keyup.enter="handleSaveEdit(row, 'FeededCount')"
              @blur="handleSaveEdit(row, 'FeededCount')"
            />
            <span v-else class="editable-cell">{{ row.FeededCount }} / {{ row.Count }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="PicturePath" label="照片" width="100" fixed="right">
          <template #default="{ row }">
            <el-button
              type="primary"
              link
              @click.stop="handleViewImage(row)"
            >
              {{ row.PicturePath ? '查看' : '上传' }}
            </el-button>
          </template>
        </el-table-column>
        <el-table-column prop="UpdatedAt" label="更新时间" width="160" fixed="right">
          <template #default="{ row }">
            {{ formatDateTime(row.UpdatedAt) }}
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

    <el-dialog v-model="imageDialogVisible" title="图片查看" width="600px">
      <img :src="currentImageUrl" style="width: 100%" />
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Search, Refresh, RefreshRight } from '@element-plus/icons-vue'
import { getCatFoods, updateCatFood, viewImage, waitForCefSharp } from '@/utils/bridge'
import { ProductType } from '@/types'

interface CatFoodRow {
  Id: number
  Name: string
  BrandName: string
  FoodType: ProductType
  Count: number
  Price: number
  Weights: number
  ProductionDate: string
  FeededCount: number
  PicturePath: string
  UpdatedAt: string
  editing?: string
  editValue?: any
}

const loading = ref(false)
const searchText = ref('')
const currentPage = ref(1)
const pageSize = ref(50)
const total = ref(0)
const tableData = ref<CatFoodRow[]>([])
const imageDialogVisible = ref(false)
const currentImageUrl = ref('')

const ProductTypeLabels = {
  [ProductType.CatFood]: '猫粮',
  [ProductType.CatSnack]: '零食',
  [ProductType.CannedFood]: '主食罐头',
  [ProductType.FreezeDriedFood]: '主食冻干',
  [ProductType.Others]: '其他'
}

const getProductTypeLabel = (type: ProductType) => {
  return ProductTypeLabels[type] || '未知'
}

const formatDate = (date: string) => {
  if (!date) return '-'
  return date.split('T')[0]
}

const formatDateTime = (date: string) => {
  if (!date) return '-'
  return date.replace('T', ' ').substring(0, 19)
}

const loadData = async () => {
  loading.value = true
  try {
    const result = await getCatFoods(currentPage.value, pageSize.value, searchText.value)
    if (!result || !result.Data) {
      throw new Error('返回数据格式无效')
    }
    tableData.value = result.Data.map((item: any) => ({
      ...item,
      editing: undefined,
      editValue: undefined
    }))
    total.value = result.Total
  } catch (error: any) {
    const errorMessage = error?.message || '加载数据失败'
    ElMessage.error(`加载数据失败: ${errorMessage}`)
    console.error('加载猫粮数据失败:', error)
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  currentPage.value = 1
  loadData()
}

const handleReset = () => {
  searchText.value = ''
  currentPage.value = 1
  loadData()
}

const handleSync = () => {
  ElMessage.info('同步功能开发中...')
}

const handleCellDblClick = (row: CatFoodRow, column: any) => {
  const field = column.property
  if (!field || field === 'Id' || field === 'BrandName' || field === 'PicturePath' || field === 'UpdatedAt') {
    return
  }
  
  row.editing = field
  row.editValue = row[field as keyof CatFoodRow]
}

const handleSaveEdit = async (row: CatFoodRow, field: string) => {
  if (row.editing !== field) return
  
  const oldValue = row[field as keyof CatFoodRow]
  const newValue = row.editValue
  
  if (oldValue === newValue) {
    row.editing = undefined
    row.editValue = undefined
    return
  }
  
  try {
    const result = await updateCatFood(row.Id, field, newValue)
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

const handleViewImage = (row: CatFoodRow) => {
  if (row.PicturePath) {
    if (row.PicturePath.startsWith('http')) {
      currentImageUrl.value = row.PicturePath
      imageDialogVisible.value = true
    } else {
      viewImage(row.PicturePath)
    }
  } else {
    ElMessage.info('上传功能开发中...')
  }
}

onMounted(async () => {
  await waitForCefSharp()
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
</style>
