using SQLite;

namespace CatFoodManager.Infrastructure.Persistence;

/// <summary>
/// 数据库上下文实现类，提供SQLite数据库操作。
/// Database context implementation class, providing SQLite database operations.
/// </summary>
public class DbContext : IDbContext
{
    private readonly SQLiteConnection _connection;
    private bool _disposed;

    /// <summary>
    /// 构造函数。
    /// Constructor.
    /// </summary>
    /// <param name="databasePath">数据库文件路径 / Database file path</param>
    public DbContext(string databasePath)
    {
        _connection = new SQLiteConnection(databasePath);
    }

    /// <summary>
    /// 获取SQLite数据库连接。
    /// Gets the SQLite database connection.
    /// </summary>
    public SQLiteConnection Connection => _connection;

    /// <summary>
    /// 创建数据表。
    /// Creates a database table.
    /// </summary>
    /// <typeparam name="T">实体类型 / Entity type</typeparam>
    /// <returns>创建的表数量 / Number of tables created</returns>
    public Task<int> CreateTableAsync<T>() where T : new()
    {
        return Task.Run(() =>
        {
            var result = _connection.CreateTable<T>();
            return (int)result;
        });
    }

    /// <summary>
    /// 执行SQL命令。
    /// Executes a SQL command.
    /// </summary>
    /// <param name="sql">SQL语句 / SQL statement</param>
    /// <param name="args">参数 / Arguments</param>
    /// <returns>受影响的行数 / Number of affected rows</returns>
    public Task<int> ExecuteAsync(string sql, params object[] args)
    {
        return Task.Run(() => _connection.Execute(sql, args));
    }

    /// <summary>
    /// 查询数据。
    /// Queries data.
    /// </summary>
    /// <typeparam name="T">实体类型 / Entity type</typeparam>
    /// <param name="sql">SQL语句 / SQL statement</param>
    /// <param name="args">参数 / Arguments</param>
    /// <returns>查询结果列表 / List of query results</returns>
    public Task<List<T>> QueryAsync<T>(string sql, params object[] args) where T : new()
    {
        return Task.Run(() => _connection.Query<T>(sql, args));
    }

    /// <summary>
    /// 释放资源。
    /// Disposes resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _connection.Close();
            _connection.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
