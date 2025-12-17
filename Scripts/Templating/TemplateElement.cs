using System;
using System.Collections.Generic;
using Godot;

namespace TTSS.Scripts.Templating;

public partial class TemplateElement : FoldableContainer, ITemplateElement
{
	public enum TemplateElementType {Text, Box, Image, Note, Cirlce, Polygon, Table}
	
	protected Button _add = new Button();
	protected Button _visibility = new Button();
	protected Button _bounds = new Button();
	protected Button _menu = new Button();
	protected Button _moveUp = new Button();
	protected Button _moveDown = new Button();

	[Export] protected PosSizeBlock _posBlock;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_add.Icon = GD.Load<Texture2D>("res://Textures/UI/add16.png");	
		_visibility.Icon = GD.Load<Texture2D>("res://Textures/UI/visibility16.png");	
		_bounds.Icon = GD.Load<Texture2D>("res://Textures/UI/bounds16.png");	
		_menu.Icon = GD.Load<Texture2D>("res://Textures/UI/menu16.png");
		_moveUp.Icon = GD.Load<Texture2D>("res://Textures/UI/arrowup16.png");
		_moveDown.Icon = GD.Load<Texture2D>("res://Textures/UI/arrowdown16.png");
		
		AddTitleBarControl(_add);
		AddTitleBarControl(_visibility);
		AddTitleBarControl(_bounds);
		AddTitleBarControl(_menu);
		AddTitleBarControl(_moveUp);
		AddTitleBarControl(_moveDown);
		
		_posBlock.PosUpdated += (sender, args) => OnElementUpdated();
		
	}


	public TemplateElementType ElementType { get; protected set; }
	public virtual List<TextureFactory.TextureObject> ElementData { get; }
	public TemplateElementPosition Position { get; set; }

	public event EventHandler<TemplateElementUpdateEventArgs> ElementUpdated;
	protected virtual void OnElementUpdated() => ElementUpdated?.Invoke(this, new TemplateElementUpdateEventArgs(ElementData));
}

public interface ITemplateElement
{
	TemplateElement.TemplateElementType ElementType {get;}
	
	TemplateElementPosition Position { get; set; }
}

public class TemplateElementUpdateEventArgs(List<TextureFactory.TextureObject> data) : EventArgs
{
	public List<TextureFactory.TextureObject> ElementData { get; private set; } = data;
}