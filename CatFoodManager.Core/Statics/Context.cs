using CatFoodManager.Core.Models;

namespace CatFoodManager.Core.Statics
{
    public sealed class Context
    {
        private Context() { }

        private static readonly Lazy<Context> _instanceLazy = new(() => new Context());
        public static Context Instance => _instanceLazy.Value;

        private static IEnumerable<PlatformRegExp>? _platformRegExps;
        public static IEnumerable<PlatformRegExp>? PlatformRegExps => _platformRegExps;

        private static string? _connectionString;
        public static string? ConnectionString => _connectionString;

        public static void FillPlatformRegExps(IEnumerable<PlatformRegExp> platformRegExps)
        {
            ArgumentNullException.ThrowIfNull(platformRegExps);
            _platformRegExps = platformRegExps;
        }

        public static void FillConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            _connectionString = connectionString;
        }
    }
}
