import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import CatFood from '@/views/CatFood.vue'
import * as bridge from '@/utils/bridge'

vi.mock('@/utils/bridge')

describe('CatFood.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('loadData', () => {
    it('should load cat food data successfully', async () => {
      const mockData = {
        Data: [
          { Id: 1, Name: 'Test Cat Food', BrandName: 'Test Brand', FoodType: 0, Count: 10, Price: 100, Weights: 500, ProductionDate: '2024-01-01', FeededCount: 5, PicturePath: '', UpdatedAt: '2024-01-01T00:00:00' }
        ],
        Total: 1,
        Page: 1,
        PageSize: 50
      }

      vi.mocked(bridge.getCatFoods).mockResolvedValue(mockData)

      const wrapper = mount(CatFood)
      await wrapper.vm.$nextTick()

      expect(bridge.getCatFoods).toHaveBeenCalledWith(1, 50, '')
      expect(wrapper.vm.tableData).toHaveLength(1)
      expect(wrapper.vm.total).toBe(1)
    })

    it('should handle search correctly', async () => {
      const mockData = {
        Data: [],
        Total: 0,
        Page: 1,
        PageSize: 50
      }

      vi.mocked(bridge.getCatFoods).mockResolvedValue(mockData)

      const wrapper = mount(CatFood)
      await wrapper.vm.$nextTick()

      await wrapper.vm.handleSearch()
      expect(wrapper.vm.currentPage).toBe(1)
    })

    it('should reset search correctly', async () => {
      const mockData = {
        Data: [],
        Total: 0,
        Page: 1,
        PageSize: 50
      }

      vi.mocked(bridge.getCatFoods).mockResolvedValue(mockData)

      const wrapper = mount(CatFood)
      wrapper.vm.searchText = 'test'
      await wrapper.vm.$nextTick()

      await wrapper.vm.handleReset()
      expect(wrapper.vm.searchText).toBe('')
      expect(wrapper.vm.currentPage).toBe(1)
    })
  })
})
