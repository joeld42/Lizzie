using Godot;
using System;

public partial class QuickTextureEntry : BoxContainer
{
	private Label _fieldCaption;
	private LineEdit _text;
	private ColorPickerButton _colorPicker;
	private OptionButton _optionTypes;

	private bool _initializing;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_initializing = true;
		
		_fieldCaption = GetNode<Label>("%Label");
		_fieldCaption.Text = _fieldName;
		
		_optionTypes = GetNode<OptionButton>("%OptionButton");
		_optionTypes.Selected = 0;
		_optionTypes.ItemSelected += TypeChanged;

		_colorPicker = GetNode<ColorPickerButton>("%TopTextColor");
		_colorPicker.Color = Colors.Black;
		_colorPicker.ColorChanged += _ => RaiseFieldChanged();
		
		_text = GetNode<LineEdit>("%TopCaption");
		_text.TextChanged += _ => RaiseFieldChanged();
		
		_initializing = false;
	}

	private void TypeChanged(long index)
	{
		_text.Visible = (index == 0);
		
		RaiseFieldChanged();
	}

	private string _fieldName;
	
	[Export]
	public string FieldCaption
	{
		get => _fieldName;
		set 
		{
			_fieldName = value;
			if (_fieldCaption == null) return;
			_fieldCaption.Text = value; 
		}
	}

	public string TextValue
	{
		get => _text.Text;
		set => _text.Text = value;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void RaiseFieldChanged()
	{
		if (_initializing) return;
		
		var qt = GetQuickTextureField();
		
		FieldChanged?.Invoke(this, new QuickTextureFieldEventArgs(qt));
	}
	
	public event EventHandler<QuickTextureFieldEventArgs> FieldChanged;

	public QuickTextureField GetQuickTextureField()
	{
		var qt = new QuickTextureField
		{
			Caption = _text.Text,
			ForegroundColor = _colorPicker.Color,
		};

		switch (_optionTypes.Selected)
		{
			case 0:
				qt.FaceType = TextureFactory.TextureObjectType.Text;
				break;
			
			case 1:
				qt.FaceType = TextureFactory.TextureObjectType.RectangleShape;
				break;
			
			case 2:
				qt.FaceType = TextureFactory.TextureObjectType.CircleShape;
				break;
			
			case 3:
				qt.FaceType = TextureFactory.TextureObjectType.HexFlatUpShape;
				break;
			
			case 4:
				qt.FaceType = TextureFactory.TextureObjectType.HexPointUpShape;
				break;
			
			case 5:
				qt.FaceType = TextureFactory.TextureObjectType.TriangleShape;
				break;
			
			case 6:
				qt.FaceType = TextureFactory.TextureObjectType.StarShape;
				break;
			
			case 7:
				qt.FaceType = TextureFactory.TextureObjectType.PentagonShape;
				break;
		}

		return qt;
	}
}

public class QuickTextureFieldEventArgs : EventArgs
{
	public QuickTextureFieldEventArgs(QuickTextureField field)
	{
		QuickTextureField = field;
	}
	public QuickTextureField QuickTextureField { get; set; }
}

public class QuickTextureField
{
	public TextureFactory.TextureObjectType FaceType { get; set; }
	public Color ForegroundColor { get; set; }
	public string Caption { get; set; }
}
