import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import Brand from '@/views/Brand.vue'
import * as bridge from '@/utils/bridge'

vi.mock('@/utils/bridge')

describe('Brand.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('loadData', () => {
    it('should load brand data successfully', async () => {
      const mockData = {
        Data: [
          { Id: 1, Name: 'Brand 1' },
          { Id: 2, Name: 'Brand 2' }
        ],
        Total: 2
      }

      vi.mocked(bridge.getBrands).mockResolvedValue(mockData)

      const wrapper = mount(Brand)
      await wrapper.vm.$nextTick()

      expect(bridge.getBrands).toHaveBeenCalledWith('')
      expect(wrapper.vm.tableData).toHaveLength(2)
    })

    it('should filter brands by search text', async () => {
      const mockData = {
        Data: [{ Id: 1, Name: 'Test Brand' }],
        Total: 1
      }

      vi.mocked(bridge.getBrands).mockResolvedValue(mockData)

      const wrapper = mount(Brand)
      wrapper.vm.searchText = 'Test'
      await wrapper.vm.$nextTick()

      await wrapper.vm.handleSearch()
      expect(bridge.getBrands).toHaveBeenCalledWith('Test')
    })
  })

  describe('handleEdit', () => {
    it('should enable editing mode', async () => {
      const mockData = {
        Data: [{ Id: 1, Name: 'Brand 1' }],
        Total: 1
      }

      vi.mocked(bridge.getBrands).mockResolvedValue(mockData)

      const wrapper = mount(Brand)
      await wrapper.vm.$nextTick()

      const row = wrapper.vm.tableData[0]
      wrapper.vm.handleEdit(row)

      expect(row.editing).toBe(true)
      expect(row.editName).toBe('Brand 1')
    })
  })

  describe('handleSave', () => {
    it('should update brand successfully', async () => {
      const mockData = {
        Data: [{ Id: 1, Name: 'Brand 1' }],
        Total: 1
      }

      vi.mocked(bridge.getBrands).mockResolvedValue(mockData)
      vi.mocked(bridge.updateBrand).mockResolvedValue({ Success: true })

      const wrapper = mount(Brand)
      await wrapper.vm.$nextTick()

      const row = wrapper.vm.tableData[0]
      row.editing = true
      row.editName = 'Updated Brand'

      await wrapper.vm.handleSave(row)

      expect(bridge.updateBrand).toHaveBeenCalledWith(1, 'Updated Brand')
      expect(row.Name).toBe('Updated Brand')
      expect(row.editing).toBe(false)
    })

    it('should add new brand successfully', async () => {
      const mockData = {
        Data: [],
        Total: 0
      }

      vi.mocked(bridge.getBrands).mockResolvedValue(mockData)
      vi.mocked(bridge.addBrand).mockResolvedValue({ Success: true, Id: 1 })

      const wrapper = mount(Brand)
      await wrapper.vm.$nextTick()

      await wrapper.vm.handleAdd()
      const row = wrapper.vm.tableData[0]
      row.editName = 'New Brand'

      await wrapper.vm.handleSave(row)

      expect(bridge.addBrand).toHaveBeenCalledWith('New Brand')
    })
  })

  describe('handleDelete', () => {
    it('should delete brand successfully', async () => {
      const mockData = {
        Data: [{ Id: 1, Name: 'Brand 1' }],
        Total: 1
      }

      vi.mocked(bridge.getBrands).mockResolvedValue(mockData)
      vi.mocked(bridge.deleteBrand).mockResolvedValue({ Success: true })

      const wrapper = mount(Brand)
      await wrapper.vm.$nextTick()

      const row = wrapper.vm.tableData[0]
      await wrapper.vm.handleDelete(row)

      expect(bridge.deleteBrand).toHaveBeenCalledWith(1)
    })
  })
})
