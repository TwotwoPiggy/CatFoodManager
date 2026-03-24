import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'

function removeModuleType() {
  return {
    name: 'remove-module-type',
    transformIndexHtml(html: string) {
      html = html.replace(/type="module"\s*/g, '').replace(/crossorigin\s*/g, '')
      const scriptMatch = html.match(/<script\s+src="[^"]+"><\/script>/)
      if (scriptMatch) {
        html = html.replace(scriptMatch[0], '')
        html = html.replace('</body>', `  ${scriptMatch[0]}\n  </body>`)
      }
      return html
    }
  }
}

export default defineConfig({
  plugins: [vue(), removeModuleType()],
  resolve: {
    alias: {
      '@': resolve(__dirname, 'src')
    }
  },
  base: './',
  build: {
    outDir: '../wwwroot',
    emptyOutDir: true,
    assetsDir: 'assets',
    modulePreload: false,
    rollupOptions: {
      output: {
        format: 'iife',
        entryFileNames: 'assets/[name].js',
        chunkFileNames: 'assets/[name].js',
        assetFileNames: 'assets/[name].[ext]'
      }
    }
  },
  server: {
    port: 3000,
    open: false
  }
})
