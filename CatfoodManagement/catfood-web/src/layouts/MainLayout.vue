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
        </el-menu>
      </div>
    </el-header>
    <el-main class="main">
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
    </el-main>
  </el-container>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { Avatar, Food, PriceTag, OfficeBuilding } from '@element-plus/icons-vue'

const router = useRouter()
const route = useRoute()

const activeMenu = computed(() => route.path)

const handleMenuSelect = (index: string) => {
  router.push(index)
}
</script>

<style scoped lang="scss">
.main-layout {
  height: 100vh;
  background-color: #f5f7fa;
}

.header {
  background-color: #fff;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 0 20px;
  z-index: 100;
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
  color: #409eff;
  font-size: 18px;
  font-weight: bold;

  .title {
    margin-left: 10px;
  }
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
