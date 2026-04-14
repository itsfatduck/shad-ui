using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace ShadUI.Demo;

public sealed class PageManager(ServiceProvider serviceProvider)
{
    private readonly ConcurrentDictionary<Type, INavigable> _pageCache = new();

    public void Navigate<T>() where T : INavigable
    {
        var attr = typeof(T).GetCustomAttribute<PageAttribute>();
        if (attr is null) throw new InvalidOperationException("Not a valid page type, missing PageAttribute");

        var page = (T)_pageCache.GetOrAdd(typeof(T), _ =>
        {
            var resolved = serviceProvider.GetService<T>();
            return resolved ?? throw new InvalidOperationException("Page not found");
        });
        if (page is null) throw new InvalidOperationException("Page not found");

        OnNavigate?.Invoke(page, attr.Route);
    }

    private Action<INavigable, string>? _onNavigate;

    public Action<INavigable, string>? OnNavigate
    {
        private get => _onNavigate;
        set
        {
            if (_onNavigate is not null)
            {
                throw new InvalidOperationException("OnNavigate is already set");
            }

            _onNavigate = value;
        }
    }

    public void DisposeCachedPages()
    {
        foreach (var page in _pageCache.Values)
        {
            if (page is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        _pageCache.Clear();
    }
}

public interface INavigable
{
    void Initialize()
    {
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PageAttribute(string route) : Attribute
{
    public string Route { get; } = route;
}