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
  [PlatformType.None]: '未知',
  [PlatformType.JD]: '京东',
  [PlatformType.Taobao]: '淘宝',
  [PlatformType.PDD]: '拼多多',
  [PlatformType.Douyin]: '抖音',
  [PlatformType.Kuaishou]: '快手'
}
