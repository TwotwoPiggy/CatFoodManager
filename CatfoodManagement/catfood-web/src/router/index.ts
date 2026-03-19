import { createRouter, createWebHashHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import MainLayout from '@/layouts/MainLayout.vue'
import CatFood from '@/views/CatFood.vue'
import BestPrice from '@/views/BestPrice.vue'
import Brand from '@/views/Brand.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Layout',
    component: MainLayout,
    redirect: '/catfood',
    children: [
      {
        path: 'catfood',
        name: 'CatFood',
        component: CatFood,
        meta: { title: '猫粮库存管理' }
      },
      {
        path: 'bestprice',
        name: 'BestPrice',
        component: BestPrice,
        meta: { title: '最佳价格管理' }
      },
      {
        path: 'brand',
        name: 'Brand',
        component: Brand,
        meta: { title: '品牌管理' }
      }
    ]
  }
]

const router = createRouter({
  history: createWebHashHistory(),
  routes
})

router.beforeEach((to, _from, next) => {
  const title = to.meta.title as string
  if (title) {
    document.title = `${title} - CatFood Manager`
  }
  next()
})

export default router
