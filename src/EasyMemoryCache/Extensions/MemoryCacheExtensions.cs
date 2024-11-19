using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.Caching.Memory;

namespace EasyMemoryCache.Extensions;

public static class MemoryCacheExtensions
{
    #region Microsoft.Extensions.Caching.Memory_6_OR_OLDER

    private static readonly Lazy<Func<MemoryCache, object>> _getEntries6 =
        new(() => (Func<MemoryCache, object>)Delegate.CreateDelegate(
            typeof(Func<MemoryCache, object>),
            typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true),
            throwOnBindFailure: true));

    #endregion

    #region Microsoft.Extensions.Caching.Memory_7_OR_NEWER

    private static readonly Lazy<Func<MemoryCache, object>> _getCoherentState =
        new(() => CreateGetter<MemoryCache, object>(typeof(MemoryCache)
            .GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance)));

    #endregion

    #region Microsoft.Extensions.Caching.Memory_7_TO_8.0.8

    private static readonly Lazy<Func<object, IDictionary>> _getEntries7 =
        new(() => CreateGetter<object, IDictionary>(typeof(MemoryCache)
            .GetNestedType("CoherentState", BindingFlags.NonPublic)
            .GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance)));

    #endregion

    #region Microsoft.Extensions.Caching.Memory_8.0.10_OR_NEWER

    private static readonly Lazy<Func<object, IDictionary>> _getStringEntries8010 =
        new(() => CreateGetter<object, IDictionary>(typeof(MemoryCache)
            .GetNestedType("CoherentState", BindingFlags.NonPublic)
            .GetField("_stringEntries", BindingFlags.NonPublic | BindingFlags.Instance)));

    private static readonly Lazy<Func<object, IDictionary>> _getNonStringEntries8010 =
        new(() => CreateGetter<object, IDictionary>(typeof(MemoryCache)
            .GetNestedType("CoherentState", BindingFlags.NonPublic)
            .GetField("_nonStringEntries", BindingFlags.NonPublic | BindingFlags.Instance)));

    #endregion

    private static Func<TParam, TReturn> CreateGetter<TParam, TReturn>(FieldInfo field)
    {
        var methodName = $"{field.ReflectedType.FullName}.get_{field.Name}";
        var method = new DynamicMethod(methodName, typeof(TReturn), [typeof(TParam)], typeof(TParam), true);
        var ilGen = method.GetILGenerator();
        ilGen.Emit(OpCodes.Ldarg_0);
        ilGen.Emit(OpCodes.Ldfld, field);
        ilGen.Emit(OpCodes.Ret);
        return (Func<TParam, TReturn>)method.CreateDelegate(typeof(Func<TParam, TReturn>));
    }

    private static readonly Func<MemoryCache, IEnumerable> _getKeys =
        FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(MemoryCache)).Location) switch
        {
            { ProductMajorPart: < 7 } =>
                static cache => ((IDictionary)_getEntries6.Value(cache)).Keys,
            { ProductMajorPart: < 8 } or { ProductMajorPart: 8, ProductMinorPart: 0, ProductBuildPart: < 10 } =>
                static cache => _getEntries7.Value(_getCoherentState.Value(cache)).Keys,
            _ =>
                static cache => ((ICollection<string>)_getStringEntries8010.Value(_getCoherentState.Value(cache)).Keys)
                    .Concat((ICollection<object>)_getNonStringEntries8010.Value(_getCoherentState.Value(cache)).Keys)
        };

    public static IEnumerable GetKeys(this IMemoryCache memoryCache) =>
        _getKeys((MemoryCache)memoryCache);

    public static IEnumerable<T> GetKeys<T>(this IMemoryCache memoryCache) =>
        memoryCache.GetKeys().OfType<T>();
}