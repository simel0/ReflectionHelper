using System.Collections.Concurrent;
using System.Reflection;

namespace ReflectionHelper
{
    public static class ReflectionHelper
    {
        private static readonly ConcurrentDictionary<string, MethodInfo> _dictMethodInfos = new ConcurrentDictionary<string, MethodInfo>();
        private static readonly ConcurrentDictionary<string, TypeInfo> _dictTypes = new ConcurrentDictionary<string, TypeInfo>();

        private static readonly Lazy<IEnumerable<Assembly>> _assemblies = new Lazy<IEnumerable<Assembly>>(() => AppDomain
            .CurrentDomain.GetAssemblies()
            .Reverse());

        public static object GetInstance<T>(this IServiceProvider serviceProvider, params object[] parameters)
        {
            return ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
        }

        public static object GetInstance(this IServiceProvider serviceProvider, Type instanceType,
            params object[] parameters)
        {
            return ActivatorUtilities.CreateInstance(serviceProvider, instanceType, parameters);
        }

        public static TypeInfo GetTypeByName(string typeName)
        {
            return _dictTypes.GetOrAdd(typeName, (name) =>
            {
                return
                    _assemblies.Value
                        .Select(assembly => assembly.GetType(name))
                        .FirstOrDefault(t => t != null)?.GetTypeInfo()
                    // Safely delete the following part
                    // if you do not want fall back to first partial result
                    ??
                    _assemblies.Value
                        .SelectMany(assembly => assembly.GetTypes())
                        .FirstOrDefault(t => t.Name.Contains(name)).GetTypeInfo();
            });
        }

        public static MethodInfo GetMethodInfo(TypeInfo typeinfo, string methodName)
        {
            var methodKey = GetMethodInfoKey(nameof(typeinfo), methodName);
            return _dictMethodInfos.GetOrAdd(methodKey, (method) =>
            {
                return typeinfo.GetDeclaredMethod(methodName);
            });
        }

        public static MethodInfo? GetMethodInfo(string typeName, string methodName)
        {
            var type = GetTypeByName(typeName);
            if (type != null)
            {
                var methodKey = GetMethodInfoKey(nameof(type), methodName);
                return _dictMethodInfos.GetOrAdd(methodKey, (method) =>
                {
                    return type.GetMethod(methodName);
                });
            }

            return null;

        }

        private static string GetMethodInfoKey(string typeName, string methodName)
        {
            return $"{typeName}:{methodName}";
        }
    }
}
