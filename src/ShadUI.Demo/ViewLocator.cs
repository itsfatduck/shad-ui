using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ShadUI.Demo.ViewModels;

namespace ShadUI.Demo;

public class ViewLocator : IDataTemplate
{
    private static readonly ConcurrentDictionary<Type, Type?> Cache = [];
    private static readonly ConditionalWeakTable<object, Control> ViewCache = new();

    private static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();

    public Control? Build(object? param)
    {
        if (param is null)
        {
            return null;
        }

        var viewModelType = param.GetType();

        var viewType = Cache.GetOrAdd(
            viewModelType,
            type =>
            {
                var nameSpace = type.Namespace;
                if (nameSpace is null)
                    return null;

                var name = type.Name;
                var viewNameSpace = nameSpace.Replace(
                    "ViewModel",
                    "View",
                    StringComparison.Ordinal
                );

                var viewName = name.Replace("ViewModel", "Page", StringComparison.Ordinal);
                var fullName = $"{viewNameSpace}.{viewName}";

                var view = CurrentAssembly.GetType(fullName);

                if (view is not null)
                    return view;

                viewName = name.Replace("ViewModel", "Content", StringComparison.Ordinal);
                view = CurrentAssembly.GetType($"{viewNameSpace}.{viewName}");

                return view;
            }
        );

        if (viewType is null)
            return new TextBlock { Text = "View not found: " + viewModelType.FullName };

        if (ViewCache.TryGetValue(param, out var cached))
        {
            cached.DataContext = param;
            return cached;
        }

        var control = (Control)Activator.CreateInstance(viewType)!;
        control.DataContext = param;
        ViewCache.Add(param, control);
        return control;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
