using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace TTSS.Scripts.Templating;

public partial class TemplateElement : HBoxContainer, ITemplateElement
{
    private List<TemplateParameter> _parameters;
    
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _parameters = new List<TemplateParameter>();
        
        Parameters.Add(new TemplateParameter(){Type = TemplateParameter.TemplateParameterType.Number, Name = "X", Value = "0"});
        Parameters.Add(new TemplateParameter(){Type = TemplateParameter.TemplateParameterType.Number, Name = "Y", Value = "0"});
        Parameters.Add(new TemplateParameter(){Type = TemplateParameter.TemplateParameterType.Number, Name = "Width", Value = "0"});
        Parameters.Add(new TemplateParameter(){Type = TemplateParameter.TemplateParameterType.Number, Name = "Height", Value = "0"});
        Parameters.Add(new TemplateParameter(){Type = TemplateParameter.TemplateParameterType.Number, Name = "Rotation", Value = "0"});
    }

   
    
    public ITemplateElement.TemplateElementType ElementType { get; protected set; }
    public string ElemeentName { get; set; }
    public IList<TemplateParameter> Parameters => _parameters;
    public virtual IList<TextureFactory.TextureObject> ElementData { get; }
    public TemplateElementPosition Position { get; set; }

    public event EventHandler<TemplateElementUpdateEventArgs> ElementUpdated;

    protected virtual void OnElementUpdated() =>
        ElementUpdated?.Invoke(this, new TemplateElementUpdateEventArgs(ElementData));
    
    
    
}



public interface ITemplateElement
{
    public enum TemplateElementType
    {
        Text,
        Box,
        Image,
        Note,
        Cirlce,
        Polygon,
        Table,
        Line,
        Container
    }

    TemplateElementType ElementType { get; }

    string ElemeentName { get; set; }

    IList<TemplateParameter> Parameters { get; }

    IList<TextureFactory.TextureObject> ElementData { get; }
}

public class TemplateParameter 
{
    public enum TemplateParameterType
    {
        Text,
        Number,
        Color,
        List
    }

    public TemplateParameterType Type { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}

public class TemplateElementUpdateEventArgs(IList<TextureFactory.TextureObject> data) : EventArgs
{
    public IList<TextureFactory.TextureObject> ElementData { get; private set; } = data;
}

public static class Extensions
{
    public static string EvaluateTextParameter(this IList<TemplateParameter> parameters, string key)
    {
        var p = parameters.FirstOrDefault(x => x.Name == key);
        if (p == null) return string.Empty;
        return p.Value;
    }
    
    public static int EvaluateNumberParameter(this IList<TemplateParameter> parameters, string key)
    {
        var p = parameters.FirstOrDefault(x => x.Name == key);
        if (p == null) return 0;
        var i = int.TryParse(p.Value, out var result) ? result : 0;
        return result;
    }
    
    public static Color EvaluateColorParameter(this IList<TemplateParameter> parameters, string key)
    {
        var p = parameters.FirstOrDefault(x => x.Name == key);
        if (p == null) return Colors.Black;
        return new Color(p.Value);
    }
}