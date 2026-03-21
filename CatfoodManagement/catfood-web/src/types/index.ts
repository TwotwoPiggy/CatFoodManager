export interface CatFood {
  Id: number
  OrderId: string
  Name: string
  FoodType: ProductType
  Count: number
  Price: number
  Weights: number
  PicturePath: string
  ProductionDate: string
  FeededCount: number
  BrandId: number
  BrandName: string
  FactoryId: number
  CreatedAt: string
  UpdatedAt: string
}

export interface BestPrice {
  Id: number
  Name: string
  Type: ProductType
  Platform: PlatformType
  LowestPrice: number
  HasPurchased: boolean
  FinalPrice: number | null
  PicturePath: string | null
  FactoryName: string | null
  HasTestReport: boolean
  IsWorthRepurchasing: boolean
  PurchasedAt: string | null
  CreatedAt: string
  UpdatedAt: string
}

export interface Brand {
  Id: number
  Name: string
}

export enum ProductType {
  CatFood = 0,
  CatSnack = 1,
  CannedFood = 2,
  FreezeDriedFood = 3,
  Others = 4
}

export enum PlatformType {
  None = 0,
  JD = 1,
  Taobao = 2,
  PDD = 3,
  Douyin = 4,
  Kuaishou = 5
}

export enum TaskStatus {
  Pending = 0,
  Queued = 1,
  Running = 2,
  Completed = 3,
  Failed = 4,
  Cancelled = 5,
  Retrying = 6
}

export enum TaskType {
  ImageSync = 0,
  ImageDelete = 1,
  ImageMove = 2,
  ImageProcess = 3
}

export interface TaskItem {
  Id: number
  Name: string
  Type: TaskType
  Status: TaskStatus
  Description?: string
  Parameters: string
  Result?: string
  ErrorMessage?: string
  RetryCount: number
  MaxRetries: number
  StartedAt?: string
  CompletedAt?: string
  ScheduledAt?: string
  Priority: number
  ParentTaskId?: number
  CreatedAt: string
  UpdatedAt?: string
}

export interface TaskConfiguration {
  Id: number
  Name?: string
  MaxConcurrentTasks: number
  PollingIntervalSeconds: number
  EnableScheduling: boolean
  DefaultSchedule?: string
  CreatedAt: string
  UpdatedAt?: string
}

export interface PagedResult<T> {
  Data: T[]
  Total: number
  Page: number
  PageSize: number
}

export interface ApiResponse<T = any> {
  Success: boolean
  Message?: string
  Data?: T
}

export const ProductTypeLabels: Record<number, string> = {
  [ProductType.CatFood]: '猫粮',
  [ProductType.CatSnack]: '零食',
  [ProductType.CannedFood]: '主食罐头',
  [ProductType.FreezeDriedFood]: '主食冻干',
  [ProductType.Others]: '其他'
}

export const PlatformTypeLabels: Record<number, string> = {
  [PlatformType.None]: 'None',
  [PlatformType.JD]: 'JD',
  [PlatformType.Taobao]: 'Taobao',
  [PlatformType.PDD]: 'PDD',
  [PlatformType.Douyin]: 'Douyin',
  [PlatformType.Kuaishou]: 'Kuaishou'
}

export const TaskStatusLabels: Record<number, string> = {
  [TaskStatus.Pending]: '待处理',
  [TaskStatus.Queued]: '已入队',
  [TaskStatus.Running]: '执行中',
  [TaskStatus.Completed]: '已完成',
  [TaskStatus.Failed]: '失败',
  [TaskStatus.Cancelled]: '已取消',
  [TaskStatus.Retrying]: '重试中'
}

export const TaskTypeLabels: Record<number, string> = {
  [TaskType.ImageSync]: '图片同步',
  [TaskType.ImageDelete]: '图片删除',
  [TaskType.ImageMove]: '图片移动',
  [TaskType.ImageProcess]: '图片处理'
}
