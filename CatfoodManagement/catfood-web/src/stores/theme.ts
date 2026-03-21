import { defineStore } from 'pinia'
import { ref, watch } from 'vue'

export type ThemeMode = 'light' | 'dark'

export const useThemeStore = defineStore('theme', () => {
  const mode = ref<ThemeMode>((localStorage.getItem('theme-mode') as ThemeMode) || 'light')

  const toggleTheme = () => {
    mode.value = mode.value === 'light' ? 'dark' : 'light'
  }

  const setTheme = (theme: ThemeMode) => {
    mode.value = theme
  }

  watch(mode, (newMode) => {
    localStorage.setItem('theme-mode', newMode)
    document.documentElement.setAttribute('data-theme', newMode)
    if (newMode === 'dark') {
      document.documentElement.classList.add('dark')
    } else {
      document.documentElement.classList.remove('dark')
    }
  }, { immediate: true })

  return {
    mode,
    toggleTheme,
    setTheme
  }
})
