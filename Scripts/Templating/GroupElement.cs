using Godot;
using System;
using System.Collections.Generic;
using TTSS.Scripts.Templating;

public partial class GroupElement : TemplateElement
{
	[Export]TextElement _textElement;

	[Export] private VBoxContainer _groupElements;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		_add.Pressed += AddPressed;
	}

	private void AddPressed()
	{
		var e = new TextElement();
		AddElement(e);
	}

	private List<TemplateElement> _elements = [];

	private void AddElement(TemplateElement element)
	{
		element.ElementUpdated += ElementOnElementUpdated;
		_elements.Add(element);
		_groupElements.AddChild(element);
	}

	private void RemoveElement(TemplateElement element)
	{
		element.ElementUpdated -= ElementOnElementUpdated;
		_elements.Remove(element);
		_groupElements.RemoveChild(element);
		element.QueueFree();
	}
	
	private void ElementOnElementUpdated(object sender, TemplateElementUpdateEventArgs e)
	{
		OnElementUpdated();
	}

	public override List<TextureFactory.TextureObject> ElementData
	{
		get
		{
			var l = new List<TextureFactory.TextureObject>();

			foreach (var e in _elements)
			{
				l.AddRange(e.ElementData);
			}

			return l;
		}
	}

}
