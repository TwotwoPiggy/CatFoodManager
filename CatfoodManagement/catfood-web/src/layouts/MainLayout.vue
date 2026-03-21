<template>
  <el-container class="main-layout">
    <el-header class="header">
      <div class="header-content">
        <div class="logo">
          <el-icon :size="24"><Avatar /></el-icon>
          <span class="title">CatFood Manager</span>
        </div>
        <el-menu
          :default-active="activeMenu"
          mode="horizontal"
          :ellipsis="false"
          @select="handleMenuSelect"
        >
          <el-menu-item index="/catfood">
            <el-icon><Food /></el-icon>
            <span>猫粮库存</span>
          </el-menu-item>
          <el-menu-item index="/bestprice">
            <el-icon><PriceTag /></el-icon>
            <span>最佳价格</span>
          </el-menu-item>
          <el-menu-item index="/brand">
            <el-icon><OfficeBuilding /></el-icon>
            <span>品牌管理</span>
          </el-menu-item>
          <el-menu-item index="/tasks">
            <el-icon><List /></el-icon>
            <span>任务管理</span>
          </el-menu-item>
          <el-menu-item index="/settings">
            <el-icon><Setting /></el-icon>
            <span>系统设置</span>
          </el-menu-item>
        </el-menu>
        <div class="header-actions">
          <el-tooltip :content="themeStore.mode === 'light' ? '切换到暗色模式' : '切换到亮色模式'" placement="bottom">
            <el-button circle @click="themeStore.toggleTheme()">
              <el-icon :size="18">
                <Sunny v-if="themeStore.mode === 'dark'" />
                <Moon v-else />
              </el-icon>
            </el-button>
          </el-tooltip>
        </div>
      </div>
    </el-header>
    <el-main class="main">
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <keep-alive>
            <component :is="Component" />
          </keep-alive>
        </transition>
      </router-view>
    </el-main>
  </el-container>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { Avatar, Food, PriceTag, OfficeBuilding, Setting, Sunny, Moon, List } from '@element-plus/icons-vue'
import { useThemeStore } from '@/stores/theme'
import { useHotkeys } from '@/composables/useHotkeys'

const router = useRouter()
const route = useRoute()
const themeStore = useThemeStore()
const { registerHotkeys, unregisterHotkeys } = useHotkeys()

const activeMenu = computed(() => route.path)

const handleMenuSelect = (index: string) => {
  router.push(index)
}

onMounted(() => {
  registerHotkeys()
})

onUnmounted(() => {
  unregisterHotkeys()
})
</script>

<style scoped lang="scss">
.main-layout {
  height: 100vh;
  background-color: var(--bg-color);
}

.header {
  background-color: var(--header-bg);
  box-shadow: var(--box-shadow);
  padding: 0 20px;
  z-index: 100;
  border-bottom: 1px solid var(--border-color);
}

.header-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.logo {
  display: flex;
  align-items: center;
  margin-right: 40px;
  color: var(--el-color-primary);
  font-size: 18px;
  font-weight: bold;

  .title {
    margin-left: 10px;
  }
}

.header-actions {
  margin-left: auto;
  display: flex;
  align-items: center;
  gap: 10px;
}

.el-menu {
  border-bottom: none;
  background-color: transparent;
}

.el-menu-item {
  font-size: 15px;
}

.main {
  padding: 0;
  overflow: hidden;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
