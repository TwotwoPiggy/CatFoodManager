using SQLite;

namespace CatFoodManager.Infrastructure.Persistence;

/// <summary>
/// 数据库上下文接口，提供数据库操作的基础功能。
/// Database context interface, providing basic database operations.
/// </summary>
public interface IDbContext : IDisposable
{
    /// <summary>
    /// 获取SQLite数据库连接。
    /// Gets the SQLite database connection.
    /// </summary>
    SQLiteConnection Connection { get; }

    /// <summary>
    /// 创建数据表。
    /// Creates a database table.
    /// </summary>
    /// <typeparam name="T">实体类型 / Entity type</typeparam>
    /// <returns>创建的表数量 / Number of tables created</returns>
    Task<CreateTableResult> CreateTableAsync<T>() where T : new();

    /// <summary>
    /// 执行SQL命令。
    /// Executes a SQL command.
    /// </summary>
    /// <param name="sql">SQL语句 / SQL statement</param>
    /// <param name="args">参数 / Arguments</param>
    /// <returns>受影响的行数 / Number of affected rows</returns>
    Task<int> ExecuteAsync(string sql, params object[] args);

    /// <summary>
    /// 查询数据。
    /// Queries data.
    /// </summary>
    /// <typeparam name="T">实体类型 / Entity type</typeparam>
    /// <param name="sql">SQL语句 / SQL statement</param>
    /// <param name="args">参数 / Arguments</param>
    /// <returns>查询结果列表 / List of query results</returns>
    Task<List<T>> QueryAsync<T>(string sql, params object[] args) where T : new();
}
