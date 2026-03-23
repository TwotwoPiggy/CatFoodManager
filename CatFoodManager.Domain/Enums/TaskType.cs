namespace CatFoodManager.Domain.Enums;

/// <summary>
/// 任务类型枚举，定义支持的任务类型。
/// Task type enum, defining supported task types.
/// </summary>
public enum TaskType
{
    /// <summary>
    /// 图片同步。
    /// Image sync.
    /// </summary>
    ImageSync = 0,

    /// <summary>
    /// 图片删除。
    /// Image delete.
    /// </summary>
    ImageDelete = 1,

    /// <summary>
    /// 图片移动。
    /// Image move.
    /// </summary>
    ImageMove = 2,

    /// <summary>
    /// 图片处理。
    /// Image process.
    /// </summary>
    ImageProcess = 3
}
