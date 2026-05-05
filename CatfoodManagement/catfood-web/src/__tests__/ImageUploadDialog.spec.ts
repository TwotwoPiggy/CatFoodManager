import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import ImageUploadDialog from '@/components/ImageUploadDialog.vue'
import * as bridge from '@/utils/bridge'

vi.mock('@/utils/bridge')
vi.mock('@/stores/appConfig', () => ({
  useAppConfigStore: vi.fn(() => ({
    todayDefaultPath: '/root/20240101/',
    fetchAll: vi.fn()
  }))
}))

describe('ImageUploadDialog.vue', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('组件渲染', () => {
    it('当 modelValue 为 false 时不显示对话框', () => {
      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: false,
          recordId: 1
        }
      })

      expect(wrapper.find('.el-dialog').exists()).toBe(false)
    })

    it('当 modelValue 为 true 时显示对话框', async () => {
      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      await wrapper.vm.$nextTick()
      expect(wrapper.find('.el-dialog').exists()).toBe(true)
    })
  })

  describe('选择图片', () => {
    it('点击选择图片按钮应调用 selectFile', async () => {
      vi.mocked(bridge.selectFile).mockResolvedValue('/path/to/image.jpg')

      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      await wrapper.vm.handleSelectImage()

      expect(bridge.selectFile).toHaveBeenCalled()
      expect(wrapper.vm.uploadForm.selectedImagePath).toBe('/path/to/image.jpg')
      expect(wrapper.vm.uploadForm.selectedFileName).toBe('image.jpg')
    })

    it('未选择文件时不应更新状态', async () => {
      vi.mocked(bridge.selectFile).mockResolvedValue('')

      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      await wrapper.vm.handleSelectImage()

      expect(wrapper.vm.uploadForm.selectedImagePath).toBe('')
    })
  })

  describe('路径选择', () => {
    it('默认路径应正确显示', async () => {
      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      expect(wrapper.vm.defaultPath).toBe('/root/20240101/')
    })

    it('选择自定义路径应调用 selectFolder', async () => {
      vi.mocked(bridge.selectFolder).mockResolvedValue('/custom/path')

      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      wrapper.vm.uploadForm.pathMode = 'custom'
      await wrapper.vm.handleSelectCustomPath()

      expect(bridge.selectFolder).toHaveBeenCalled()
      expect(wrapper.vm.uploadForm.customPath).toBe('/custom/path')
    })
  })

  describe('上传验证', () => {
    it('未选择图片时禁用上传按钮', async () => {
      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      wrapper.vm.uploadForm.selectedImagePath = ''
      expect(wrapper.vm.canUpload).toBe(false)
    })

    it('选择图片但未配置默认路径时禁用上传按钮', async () => {
      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      wrapper.vm.uploadForm.selectedImagePath = '/path/to/image.jpg'
      wrapper.vm.uploadForm.pathMode = 'default'
      wrapper.vm.uploadForm.customPath = ''

      expect(wrapper.vm.canUpload).toBe(true)
    })

    it('选择图片和自定义路径时启用上传按钮', async () => {
      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      wrapper.vm.uploadForm.selectedImagePath = '/path/to/image.jpg'
      wrapper.vm.uploadForm.pathMode = 'custom'
      wrapper.vm.uploadForm.customPath = '/custom/path'

      expect(wrapper.vm.canUpload).toBe(true)
    })
  })

  describe('上传确认', () => {
    it('确认上传应触发 success 事件', async () => {
      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      wrapper.vm.uploadForm.selectedImagePath = '/path/to/image.jpg'
      wrapper.vm.uploadForm.pathMode = 'default'

      await wrapper.vm.handleConfirmUpload()

      expect(wrapper.emitted('success')).toBeTruthy()
      expect(wrapper.emitted('success')[0][0]).toEqual({
        recordId: 1,
        imagePath: '/path/to/image.jpg',
        targetPath: '/root/20240101/'
      })
    })

    it('未选择图片时不应触发 success 事件', async () => {
      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      await wrapper.vm.handleConfirmUpload()

      expect(wrapper.emitted('success')).toBeFalsy()
    })
  })

  describe('对话框关闭', () => {
    it('关闭对话框应清空表单', async () => {
      const wrapper = mount(ImageUploadDialog, {
        props: {
          modelValue: true,
          recordId: 1
        }
      })

      wrapper.vm.uploadForm.selectedImagePath = '/path/to/image.jpg'
      wrapper.vm.uploadForm.selectedFileName = 'image.jpg'
      wrapper.vm.uploadForm.previewUrl = 'file:///path/to/image.jpg'

      await wrapper.vm.handleDialogClose(false)

      expect(wrapper.vm.uploadForm.selectedImagePath).toBe('')
      expect(wrapper.vm.uploadForm.selectedFileName).toBe('')
      expect(wrapper.vm.uploadForm.previewUrl).toBe('')
    })
  })
})
