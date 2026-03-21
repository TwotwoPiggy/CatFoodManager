<template>
  <div class="page-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>品牌管理</span>
        </div>
      </template>

      <div class="search-bar">
        <el-input
          v-model="searchText"
          placeholder="搜索品牌名称或ID"
          clearable
          style="width: 300px"
          @keyup.enter="handleSearch"
          @input="handleSearch"
        >
          <template #prefix>
            <el-icon><Search /></el-icon>
          </template>
        </el-input>
      </div>

      <el-table
        :data="tableData"
        v-loading="loading"
        border
        stripe
        height="calc(100vh - 280px)"
      >
        <el-table-column prop="Id" label="ID" width="100" />
        <el-table-column prop="Name" label="品牌名称" min-width="200">
          <template #default="{ row }">
            <el-input
              v-if="row.editing"
              v-model="row.editName"
              size="small"
              @keyup.enter="handleSave(row)"
              @blur="handleSave(row)"
            />
            <span v-else>{{ row.Name }}</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button
              v-if="!row.editing"
              type="primary"
              link
              @click="handleEdit(row)"
            >
              编辑
            </el-button>
            <el-button
              v-if="row.editing"
              type="success"
              link
              @click="handleSave(row)"
            >
              保存
            </el-button>
            <el-button
              v-if="row.editing"
              link
              @click="handleCancel(row)"
            >
              取消
            </el-button>
            <el-popconfirm
              title="确定要删除这个品牌吗?"
              @confirm="handleDelete(row)"
            >
              <template #reference>
                <el-button type="danger" link>删除</el-button>
              </template>
            </el-popconfirm>
          </template>
        </el-table-column>
      </el-table>

      <div class="add-row">
        <el-button type="primary" :icon="Plus" @click="handleAdd">新增品牌</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onActivated } from 'vue'
import { ElMessage } from 'element-plus'
import { Search, Plus } from '@element-plus/icons-vue'
import { getBrands, addBrand, updateBrand, deleteBrand, waitForCefSharp } from '@/utils/bridge'

interface BrandRow {
  Id: number
  Name: string
  editing?: boolean
  editName?: string
}

const loading = ref(false)
const searchText = ref('')
const tableData = ref<BrandRow[]>([])

const loadData = async () => {
  loading.value = true
  try {
    const result = await getBrands(searchText.value)
    tableData.value = result.Data.map((item: any) => ({
      ...item,
      editing: false,
      editName: item.Name
    }))
  } catch (error) {
    ElMessage.error('加载数据失败')
    console.error(error)
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  loadData()
}

const handleEdit = (row: BrandRow) => {
  row.editing = true
  row.editName = row.Name
}

const handleSave = async (row: BrandRow) => {
  if (!row.editName?.trim()) {
    ElMessage.warning('品牌名称不能为空')
    return
  }

  try {
    if (row.Id === 0) {
      const result = await addBrand(row.editName)
      if (result.Success) {
        ElMessage.success('添加成功')
        loadData()
      } else {
        ElMessage.error(result.Message || '添加失败')
      }
    } else {
      const result = await updateBrand(row.Id, row.editName!)
      if (result.Success) {
        ElMessage.success('更新成功')
        row.Name = row.editName!
        row.editing = false
      } else {
        ElMessage.error(result.Message || '更新失败')
      }
    }
  } catch (error) {
    ElMessage.error('操作失败')
    console.error(error)
  }
}

const handleCancel = (row: BrandRow) => {
  row.editing = false
  row.editName = row.Name
}

const handleDelete = async (row: BrandRow) => {
  try {
    const result = await deleteBrand(row.Id)
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

const handleAdd = () => {
  tableData.value.unshift({
    Id: 0,
    Name: '',
    editing: true,
    editName: ''
  })
}

onMounted(async () => {
  await waitForCefSharp()
  loadData()
})

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

.add-row {
  margin-top: 16px;
  padding: 10px 0;
  border-top: 1px solid #ebeef5;
}
</style>
