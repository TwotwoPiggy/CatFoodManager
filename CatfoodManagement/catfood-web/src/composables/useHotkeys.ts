import { onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { useThemeStore } from '@/stores/theme'

export function useHotkeys() {
  const router = useRouter()
  const themeStore = useThemeStore()

  const handleKeydown = (event: KeyboardEvent) => {
    if (event.ctrlKey || event.metaKey) {
      switch (event.key.toLowerCase()) {
        case '1':
          event.preventDefault()
          router.push('/catfood')
          ElMessage.success('切换到猫粮库存')
          break
        case '2':
          event.preventDefault()
          router.push('/bestprice')
          ElMessage.success('切换到最佳价格')
          break
        case '3':
          event.preventDefault()
          router.push('/brand')
          ElMessage.success('切换到品牌管理')
          break
        case '4':
          event.preventDefault()
          router.push('/settings')
          ElMessage.success('切换到系统设置')
          break
        case 'd':
          event.preventDefault()
          themeStore.toggleTheme()
          ElMessage.success(`已切换到${themeStore.mode === 'dark' ? '暗色' : '亮色'}模式`)
          break
        case 'f':
          event.preventDefault()
          const searchInput = document.querySelector('.search-bar input') as HTMLInputElement
          if (searchInput) {
            searchInput.focus()
            ElMessage.info('已聚焦到搜索框')
          }
          break
        case 'r':
          event.preventDefault()
          const refreshBtn = document.querySelector('.card-header .el-button--primary') as HTMLButtonElement
          if (refreshBtn) {
            refreshBtn.click()
            ElMessage.info('正在刷新数据...')
          }
          break
      }
    }
  }

  const registerHotkeys = () => {
    document.addEventListener('keydown', handleKeydown)
  }

  const unregisterHotkeys = () => {
    document.removeEventListener('keydown', handleKeydown)
  }

  return {
    registerHotkeys,
    unregisterHotkeys
  }
}

export const hotkeyList = [
  { key: 'Ctrl + 1', description: '切换到猫粮库存' },
  { key: 'Ctrl + 2', description: '切换到最佳价格' },
  { key: 'Ctrl + 3', description: '切换到品牌管理' },
  { key: 'Ctrl + 4', description: '切换到系统设置' },
  { key: 'Ctrl + D', description: '切换明暗主题' },
  { key: 'Ctrl + F', description: '聚焦搜索框' },
  { key: 'Ctrl + R', description: '刷新当前页面数据' }
]
