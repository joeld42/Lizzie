using Godot;
using System;
using TTSS.Scripts.Templating;

public interface IParamControl
{
    void SetParameter(TemplateParameter parameter);

    void UpdateParameter(string newValue);
    TemplateParameter GetParameter();
    
    event EventHandler<TemplateParamUpdateEventArgs> ParameterUpdated;
}
