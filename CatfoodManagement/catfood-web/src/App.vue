<template>
  <el-config-provider :locale="zhCn">
    <router-view />
  </el-config-provider>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import zhCn from 'element-plus/es/locale/lang/zh-cn'
import { useThemeStore } from '@/stores/theme'
import { useTaskNotification, setTaskNotificationRouter } from '@/composables/useTaskNotification'
import { useRouter } from 'vue-router'
import { waitForCefSharp } from '@/utils/bridge'

const themeStore = useThemeStore()
const router = useRouter()
const { initNotification } = useTaskNotification()

setTaskNotificationRouter(router)

onMounted(async () => {
  const savedTheme = localStorage.getItem('theme-mode') as 'light' | 'dark' | null
  if (savedTheme) {
    themeStore.setTheme(savedTheme)
  }
  
  await waitForCefSharp()
  initNotification()
})
</script>

<style>
#app {
  width: 100%;
  height: 100vh;
  overflow: hidden;
}
</style>
