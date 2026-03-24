<template>
  <div class="schedule-selector">
    <el-radio-group v-model="scheduleMode" @change="handleModeChange">
      <el-radio-button value="preset">预设</el-radio-button>
      <el-radio-button value="simple">简单</el-radio-button>
      <el-radio-button value="advanced">高级</el-radio-button>
    </el-radio-group>

    <div class="schedule-content">
      <div v-if="scheduleMode === 'preset'" class="preset-options">
        <el-select v-model="selectedPreset" placeholder="选择预设调度" @change="handlePresetChange">
          <el-option label="每分钟" value="everyMinute" />
          <el-option label="每小时" value="everyHour" />
          <el-option label="每天凌晨 0 点" value="dailyMidnight" />
          <el-option label="每天早上 8 点" value="dailyMorning" />
          <el-option label="每天中午 12 点" value="dailyNoon" />
          <el-option label="每天晚上 6 点" value="dailyEvening" />
          <el-option label="每周一早上 8 点" value="weeklyMonday" />
          <el-option label="每周一三五早上 8 点" value="weeklyMonWedFri" />
          <el-option label="每月 1 号凌晨 0 点" value="monthlyFirst" />
        </el-select>
        <div class="schedule-preview" v-if="cronExpression">
          <el-tag type="info">表达式: {{ cronExpression }}</el-tag>
          <span class="preview-text">{{ humanReadable }}</span>
        </div>
      </div>

      <div v-else-if="scheduleMode === 'simple'" class="simple-options">
        <div class="simple-row">
          <span class="label">每</span>
          <el-select v-model="simpleInterval" style="width: 100px">
            <el-option label="分钟" value="minute" />
            <el-option label="小时" value="hour" />
            <el-option label="天" value="day" />
            <el-option label="周" value="week" />
            <el-option label="月" value="month" />
          </el-select>
          <el-input-number 
            v-if="simpleInterval !== 'minute'" 
            v-model="simpleValue" 
            :min="1" 
            :max="getMaxValue()" 
            style="width: 100px; margin-left: 8px" 
          />
        </div>
        
        <div class="simple-row" v-if="simpleInterval === 'hour' || simpleInterval === 'day' || simpleInterval === 'week' || simpleInterval === 'month'">
          <span class="label">在</span>
          <el-time-select 
            v-model="simpleTime" 
            start="00:00" 
            step="00:30" 
            end="23:30" 
            placeholder="选择时间"
            style="width: 120px"
          />
        </div>

        <div class="simple-row" v-if="simpleInterval === 'week'">
          <span class="label">在</span>
          <el-select v-model="simpleWeekday" multiple placeholder="选择星期" style="width: 200px">
            <el-option label="周日" :value="0" />
            <el-option label="周一" :value="1" />
            <el-option label="周二" :value="2" />
            <el-option label="周三" :value="3" />
            <el-option label="周四" :value="4" />
            <el-option label="周五" :value="5" />
            <el-option label="周六" :value="6" />
          </el-select>
        </div>

        <div class="simple-row" v-if="simpleInterval === 'month'">
          <span class="label">在每月</span>
          <el-input-number v-model="simpleDayOfMonth" :min="1" :max="31" style="width: 100px" />
          <span class="label">号</span>
        </div>

        <div class="schedule-preview" v-if="cronExpression">
          <el-tag type="info">表达式: {{ cronExpression }}</el-tag>
          <span class="preview-text">{{ humanReadable }}</span>
        </div>
      </div>

      <div v-else class="advanced-options">
        <el-input 
          v-model="cronExpression" 
          placeholder="输入 Cron 表达式 (如: 0 0 8 * * ?)" 
          @input="handleCronInput"
        />
        <div class="cron-help">
          <el-collapse>
            <el-collapse-item title="Cron 表达式说明" name="help">
              <div class="help-content">
                <p><strong>格式:</strong> 秒 分 时 日 月 周</p>
                <table class="cron-table">
                  <thead>
                    <tr>
                      <th>位置</th>
                      <th>说明</th>
                      <th>允许值</th>
                      <th>允许的特殊字符</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr><td>秒</td><td>第几秒</td><td>0-59</td><td>, - * /</td></tr>
                    <tr><td>分</td><td>第几分</td><td>0-59</td><td>, - * /</td></tr>
                    <tr><td>时</td><td>第几时</td><td>0-23</td><td>, - * /</td></tr>
                    <tr><td>日</td><td>第几天</td><td>1-31</td><td>, - * ? / L W</td></tr>
                    <tr><td>月</td><td>第几月</td><td>1-12</td><td>, - * /</td></tr>
                    <tr><td>周</td><td>星期几</td><td>1-7</td><td>, - * ? / L #</td></tr>
                  </tbody>
                </table>
                <p><strong>特殊字符:</strong></p>
                <ul>
                  <li><code>*</code> - 所有值</li>
                  <li><code>?</code> - 不指定值（日和周互斥）</li>
                  <li><code>-</code> - 范围，如 1-5</li>
                  <li><code>,</code> - 列举，如 1,3,5</li>
                  <li><code>/</code> - 增量，如 0/15 表示从0开始每15单位</li>
                </ul>
                <p><strong>示例:</strong></p>
                <ul>
                  <li><code>0 0 12 * * ?</code> - 每天中午12点</li>
                  <li><code>0 30 10 * * ?</code> - 每天上午10:30</li>
                  <li><code>0 0 8 ? * MON-FRI</code> - 周一到周五早上8点</li>
                  <li><code>0 0 0 1 * ?</code> - 每月1号凌晨</li>
                </ul>
              </div>
            </el-collapse-item>
          </el-collapse>
        </div>
        <div class="schedule-preview" v-if="cronExpression">
          <span class="preview-text">{{ humanReadable }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'

const props = defineProps<{
  modelValue?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const scheduleMode = ref<'preset' | 'simple' | 'advanced'>('preset')
const selectedPreset = ref('')
const cronExpression = ref(props.modelValue || '')

const simpleInterval = ref<'minute' | 'hour' | 'day' | 'week' | 'month'>('day')
const simpleValue = ref(1)
const simpleTime = ref('08:00')
const simpleWeekday = ref<number[]>([1])
const simpleDayOfMonth = ref(1)

const presetMap: Record<string, string> = {
  everyMinute: '0 * * * * ?',
  everyHour: '0 0 * * * ?',
  dailyMidnight: '0 0 0 * * ?',
  dailyMorning: '0 0 8 * * ?',
  dailyNoon: '0 0 12 * * ?',
  dailyEvening: '0 0 18 * * ?',
  weeklyMonday: '0 0 8 ? * MON',
  weeklyMonWedFri: '0 0 8 ? * MON,WED,FRI',
  monthlyFirst: '0 0 0 1 * ?'
}

const presetHumanMap: Record<string, string> = {
  everyMinute: '每分钟执行一次',
  everyHour: '每小时执行一次',
  dailyMidnight: '每天凌晨 0:00 执行',
  dailyMorning: '每天早上 8:00 执行',
  dailyNoon: '每天中午 12:00 执行',
  dailyEvening: '每天晚上 18:00 执行',
  weeklyMonday: '每周一早上 8:00 执行',
  weeklyMonWedFri: '每周一、三、五早上 8:00 执行',
  monthlyFirst: '每月 1 号凌晨 0:00 执行'
}

const humanReadable = computed(() => {
  if (!cronExpression.value) return ''
  
  if (scheduleMode.value === 'preset' && selectedPreset.value) {
    return presetHumanMap[selectedPreset.value] || ''
  }
  
  if (scheduleMode.value === 'simple') {
    return generateSimpleHumanReadable()
  }
  
  return parseCronToHuman(cronExpression.value)
})

const getMaxValue = () => {
  switch (simpleInterval.value) {
    case 'hour': return 23
    case 'day': return 31
    case 'week': return 52
    case 'month': return 12
    default: return 1
  }
}

const handleModeChange = () => {
  cronExpression.value = ''
  selectedPreset.value = ''
  
  if (scheduleMode.value === 'simple') {
    generateSimpleCron()
  }
}

const handlePresetChange = () => {
  if (selectedPreset.value && presetMap[selectedPreset.value]) {
    cronExpression.value = presetMap[selectedPreset.value]
    emit('update:modelValue', cronExpression.value)
  }
}

const handleCronInput = () => {
  emit('update:modelValue', cronExpression.value)
}

const generateSimpleCron = () => {
  const [hour, minute] = simpleTime.value.split(':').map(Number)
  
  let cron = ''
  switch (simpleInterval.value) {
    case 'minute':
      cron = `0 */${simpleValue.value} * * * ?`
      break
    case 'hour':
      cron = `0 ${minute} */${simpleValue.value} * * ?`
      break
    case 'day':
      cron = `0 ${minute} ${hour} */${simpleValue.value} * ?`
      break
    case 'week':
      const days = simpleWeekday.value.length > 0 
        ? simpleWeekday.value.map(d => ['SUN','MON','TUE','WED','THU','FRI','SAT'][d]).join(',')
        : '*'
      cron = `0 ${minute} ${hour} ? * ${days}`
      break
    case 'month':
      cron = `0 ${minute} ${hour} ${simpleDayOfMonth.value} * ?`
      break
  }
  
  cronExpression.value = cron
  emit('update:modelValue', cron)
}

const generateSimpleHumanReadable = (): string => {
  const [hour, minute] = simpleTime.value.split(':')
  const timeStr = `${hour}:${minute}`
  
  switch (simpleInterval.value) {
    case 'minute':
      return simpleValue.value === 1 ? '每分钟执行一次' : `每 ${simpleValue.value} 分钟执行一次`
    case 'hour':
      return simpleValue.value === 1 
        ? `每小时 ${minute} 分执行` 
        : `每 ${simpleValue.value} 小时 ${minute} 分执行`
    case 'day':
      return simpleValue.value === 1 
        ? `每天 ${timeStr} 执行` 
        : `每 ${simpleValue.value} 天 ${timeStr} 执行`
    case 'week':
      const dayNames = ['周日', '周一', '周二', '周三', '周四', '周五', '周六']
      const days = simpleWeekday.value.map(d => dayNames[d]).join('、')
      return `每${days} ${timeStr} 执行`
    case 'month':
      return `每月 ${simpleDayOfMonth.value} 号 ${timeStr} 执行`
    default:
      return ''
  }
}

const parseCronToHuman = (cron: string): string => {
  const parts = cron.trim().split(/\s+/)
  if (parts.length < 6) return '无效的 Cron 表达式'
  
  const [second, minute, hour, day, month, weekday] = parts
  
  if (minute === '*' && hour === '*') return '每分钟执行'
  if (minute === '0' && hour === '*') return '每小时整点执行'
  if (hour !== '*' && day === '*' && month === '*' && weekday === '?') {
    return `每天 ${hour}:${minute} 执行`
  }
  if (weekday !== '*' && weekday !== '?') {
    const dayMap: Record<string, string> = {
      'SUN': '周日', 'MON': '周一', 'TUE': '周二', 'WED': '周三',
      'THU': '周四', 'FRI': '周五', 'SAT': '周六',
      '1': '周日', '2': '周一', '3': '周二', '4': '周三',
      '5': '周四', '6': '周五', '7': '周六'
    }
    const days = weekday.split(',').map(d => dayMap[d] || d).join('、')
    return `每${days} ${hour}:${minute} 执行`
  }
  if (day !== '*' && day !== '?') {
    return `每月 ${day} 号 ${hour}:${minute} 执行`
  }
  
  return cron
}

watch([simpleInterval, simpleValue, simpleTime, simpleWeekday, simpleDayOfMonth], () => {
  if (scheduleMode.value === 'simple') {
    generateSimpleCron()
  }
})

watch(() => props.modelValue, (newVal) => {
  if (newVal !== cronExpression.value) {
    cronExpression.value = newVal || ''
  }
})
</script>

<style scoped lang="scss">
.schedule-selector {
  .el-radio-group {
    margin-bottom: 16px;
  }
  
  .schedule-content {
    padding: 12px;
    background-color: var(--el-fill-color-light);
    border-radius: 4px;
  }
  
  .preset-options {
    .el-select {
      width: 100%;
    }
  }
  
  .simple-options {
    .simple-row {
      display: flex;
      align-items: center;
      margin-bottom: 12px;
      
      .label {
        margin: 0 8px;
        white-space: nowrap;
      }
    }
  }
  
  .schedule-preview {
    margin-top: 12px;
    display: flex;
    align-items: center;
    gap: 12px;
    
    .preview-text {
      color: var(--el-text-color-secondary);
      font-size: 13px;
    }
  }
  
  .cron-help {
    margin-top: 12px;
    
    .help-content {
      font-size: 13px;
      line-height: 1.6;
      
      p {
        margin: 8px 0;
      }
      
      ul {
        padding-left: 20px;
        margin: 8px 0;
      }
      
      code {
        background-color: var(--el-fill-color);
        padding: 2px 6px;
        border-radius: 3px;
        font-family: monospace;
      }
    }
    
    .cron-table {
      width: 100%;
      border-collapse: collapse;
      margin: 8px 0;
      font-size: 12px;
      
      th, td {
        border: 1px solid var(--el-border-color);
        padding: 6px 8px;
        text-align: left;
      }
      
      th {
        background-color: var(--el-fill-color);
      }
    }
  }
}
</style>
