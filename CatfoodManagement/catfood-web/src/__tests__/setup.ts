import { config } from '@vue/test-utils'

config.global.mocks = {
  $router: {
    push: vi.fn()
  },
  $route: {
    path: '/catfood'
  }
}
