using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EasyMemoryCache.Extensions
{
    public static class MemoryCacheExtensions
    {
        #region Microsoft.Extensions.Caching.Memory_6_OR_OLDER

        private static readonly Lazy<Func<MemoryCache, object>> GetEntries6 =
            new Lazy<Func<MemoryCache, object>>(() => (Func<MemoryCache, object>)Delegate.CreateDelegate(
                typeof(Func<MemoryCache, object>),
                typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true),
                throwOnBindFailure: true));

        #endregion Microsoft.Extensions.Caching.Memory_6_OR_OLDER

        #region Microsoft.Extensions.Caching.Memory_7_OR_NEWER

        private static readonly Lazy<Func<MemoryCache, object>> GetCoherentState =
            new Lazy<Func<MemoryCache, object>>(() =>
                CreateGetter<MemoryCache, object>(typeof(MemoryCache)
                    .GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance)));

        private static readonly Lazy<Func<object, IDictionary>> GetEntries7 =
            new Lazy<Func<object, IDictionary>>(() =>
                CreateGetter<object, IDictionary>(typeof(MemoryCache)
                    .GetNestedType("CoherentState", BindingFlags.NonPublic)
                    .GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance)));

        private static Func<TParam, TReturn> CreateGetter<TParam, TReturn>(FieldInfo field)
        {
            var methodName = $"{field.ReflectedType.FullName}.get_{field.Name}";
            var method = new DynamicMethod(methodName, typeof(TReturn), new[] { typeof(TParam) }, typeof(TParam), true);
            var ilGen = method.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, field);
            ilGen.Emit(OpCodes.Ret);
            return (Func<TParam, TReturn>)method.CreateDelegate(typeof(Func<TParam, TReturn>));
        }

        #endregion Microsoft.Extensions.Caching.Memory_7_OR_NEWER

        private static readonly Func<MemoryCache, IDictionary> GetEntries;

        static MemoryCacheExtensions() =>
            GetEntries = Assembly.GetAssembly(typeof(MemoryCache)).GetName().Version.Major < 7
                ? (Func<MemoryCache, IDictionary>)(cache => (IDictionary)GetEntries6.Value(cache))
                : cache => GetEntries7.Value(GetCoherentState.Value(cache));

        public static ICollection GetKeys(this IMemoryCache memoryCache) =>
            GetEntries((MemoryCache)memoryCache).Keys;

        public static IEnumerable<T> GetKeys<T>(this IMemoryCache memoryCache) =>
            memoryCache.GetKeys().OfType<T>();
    }
}