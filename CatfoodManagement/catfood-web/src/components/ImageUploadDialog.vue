<template>
  <el-dialog 
    :model-value="modelValue" 
    title="上传照片" 
    width="600px"
    @update:model-value="handleDialogClose"
  >
    <el-form :model="uploadForm" label-width="100px">
      <el-form-item label="选择图片">
        <el-button type="primary" @click="handleSelectImage">选择图片</el-button>
        <span v-if="uploadForm.selectedFileName" style="margin-left: 10px">
          {{ uploadForm.selectedFileName }}
        </span>
      </el-form-item>
      
      <el-form-item v-if="uploadForm.previewUrl" label="预览">
        <div class="image-preview">
          <img :src="uploadForm.previewUrl" alt="预览图片" />
        </div>
      </el-form-item>
      
      <el-form-item label="保存路径">
        <div class="path-selector">
          <el-radio-group v-model="uploadForm.pathMode" style="margin-bottom: 10px">
            <el-radio value="default">默认路径</el-radio>
            <el-radio value="custom">自定义路径</el-radio>
          </el-radio-group>
          
          <div v-if="uploadForm.pathMode === 'default'" class="default-path">
            <el-input 
              :model-value="defaultPath" 
              readonly
              placeholder="请先在设置中配置图片保存根路径"
            >
              <template #prepend>
                <el-tag type="info" size="small">默认</el-tag>
              </template>
            </el-input>
          </div>
          
          <div v-else class="custom-path">
            <el-input 
              v-model="uploadForm.customPath" 
              placeholder="选择自定义保存路径" 
              readonly
            >
              <template #append>
                <el-button @click="handleSelectCustomPath">浏览</el-button>
              </template>
            </el-input>
          </div>
        </div>
      </el-form-item>
    </el-form>
    
    <template #footer>
      <el-button @click="handleCancel">取消</el-button>
      <el-button 
        type="primary" 
        :loading="uploading"
        :disabled="!canUpload"
        @click="handleConfirmUpload"
      >
        确认上传
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onBeforeUnmount } from 'vue'
import { selectFolder, selectFile } from '@/utils/bridge'
import { useAppConfigStore } from '@/stores/appConfig'

interface Props {
  modelValue: boolean
  recordId: number
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'success': [result: { recordId: number; imagePath: string; targetPath: string }]
}>()

const appConfigStore = useAppConfigStore()
const uploading = ref(false)

const uploadForm = reactive({
  selectedImagePath: '',
  selectedFileName: '',
  previewUrl: '',
  pathMode: 'default' as 'default' | 'custom',
  customPath: ''
})

const defaultPath = computed(() => {
  return appConfigStore.todayDefaultPath || ''
})

const canUpload = computed(() => {
  if (!uploadForm.selectedImagePath) return false
  if (uploadForm.pathMode === 'default') {
    return !!defaultPath.value
  } else {
    return !!uploadForm.customPath
  }
})

const targetPath = computed(() => {
  return uploadForm.pathMode === 'default' 
    ? defaultPath.value 
    : uploadForm.customPath
})

const handleSelectImage = async () => {
  const path = await selectFile()
  if (path) {
    uploadForm.selectedImagePath = path
    uploadForm.selectedFileName = path.split(/[/\\]/).pop() || path
    uploadForm.previewUrl = `file://${path}`
  }
}

const handleSelectCustomPath = async () => {
  const path = await selectFolder()
  if (path) {
    uploadForm.customPath = path
  }
}

const handleCancel = () => {
  emit('update:modelValue', false)
}

const handleConfirmUpload = async () => {
  if (!uploadForm.selectedImagePath || !targetPath.value) {
    return
  }
  
  uploading.value = true
  try {
    emit('success', {
      recordId: props.recordId,
      imagePath: uploadForm.selectedImagePath,
      targetPath: targetPath.value
    })
  } finally {
    uploading.value = false
  }
}

const handleDialogClose = (value: boolean) => {
  if (!value) {
    uploadForm.selectedImagePath = ''
    uploadForm.selectedFileName = ''
    uploadForm.previewUrl = ''
  }
  emit('update:modelValue', value)
}

watch(() => props.modelValue, (newVal) => {
  if (!newVal) {
    uploadForm.selectedImagePath = ''
    uploadForm.selectedFileName = ''
    uploadForm.previewUrl = ''
  }
})
</script>

<style scoped lang="scss">
.image-preview {
  max-width: 400px;
  max-height: 300px;
  overflow: hidden;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  
  img {
    width: 100%;
    height: auto;
    display: block;
  }
}

.path-selector {
  width: 100%;
}

.default-path,
.custom-path {
  width: 100%;
}
</style>
