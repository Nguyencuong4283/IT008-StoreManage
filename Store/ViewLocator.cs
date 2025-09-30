using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Store.ViewModels;

namespace Store;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data == null)
        {
            return null;
        }
        
        var viewName = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.InvariantCulture);
        var type = Type.GetType(viewName);

        if (type == null)
        {
            return null;
        }
        
        var control = Activator.CreateInstance(type) as Control;
        if(control != null)
            control.DataContext = data;
        return control;
    }

    public bool Match(object? data) => data is ViewModelBase;
}