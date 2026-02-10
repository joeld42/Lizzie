using Godot;
using System;
using System.Collections.Generic;

public partial class MeeplePanel : ComponentPanelDialogResult
{
	private const int GridSize = 16;
	private const float CellSize = 30f;
	
	private bool[,] _gridState;
	private Panel[,] _gridCells;
	private GridContainer _gridContainer;
	
	private bool _isMouseDown = false;
	private bool _isRightMouseDown = false;
	
	private Color _onColor = new Color(0.2f, 0.6f, 1.0f); // Blue
	private Color _offColor = new Color(0.15f, 0.15f, 0.15f); // Dark gray
	private Color _hoverColor = new Color(0.3f, 0.3f, 0.3f); // Light gray
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitializeGrid();
		CreateGridUI();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				_isMouseDown = mouseButton.Pressed;
			}
			else if (mouseButton.ButtonIndex == MouseButton.Right)
			{
				_isRightMouseDown = mouseButton.Pressed;
			}
		}
	}
	
	private void InitializeGrid()
	{
		_gridState = new bool[GridSize, GridSize];
		_gridCells = new Panel[GridSize, GridSize];
		
		// Initialize all cells to off
		for (int row = 0; row < GridSize; row++)
		{
			for (int col = 0; col < GridSize; col++)
			{
				_gridState[row, col] = false;
			}
		}
	}
	
	private void CreateGridUI()
	{
		// Create a centered container
		var centerContainer = new CenterContainer();
		centerContainer.SetAnchorsPreset(LayoutPreset.FullRect);
		AddChild(centerContainer);
		
		// Create the grid container
		_gridContainer = new GridContainer();
		_gridContainer.Columns = GridSize;
		_gridContainer.AddThemeConstantOverride("h_separation", 2);
		_gridContainer.AddThemeConstantOverride("v_separation", 2);
		centerContainer.AddChild(_gridContainer);
		
		// Create grid cells
		for (int row = 0; row < GridSize; row++)
		{
			for (int col = 0; col < GridSize; col++)
			{
				var cell = CreateGridCell(row, col);
				_gridCells[row, col] = cell;
				_gridContainer.AddChild(cell);
			}
		}
	}
	
	private Panel CreateGridCell(int row, int col)
	{
		var panel = new Panel();
		panel.CustomMinimumSize = new Vector2(CellSize, CellSize);
		panel.MouseFilter = MouseFilterEnum.Pass;
		
		// Create StyleBox for the panel
		var styleBox = new StyleBoxFlat();
		styleBox.BgColor = _offColor;
		styleBox.BorderColor = new Color(0.4f, 0.4f, 0.4f);
		styleBox.SetBorderWidthAll(1);
		panel.AddThemeStyleboxOverride("panel", styleBox);
		
		// Add mouse click handling
		panel.GuiInput += (inputEvent) => OnCellInput(inputEvent, row, col);
		
		// Add hover effects
		panel.MouseEntered += () => OnCellMouseEntered(panel, row, col);
		panel.MouseExited += () => OnCellMouseExited(panel, row, col);
		
		return panel;
	}
	
	private void OnCellInput(InputEvent @event, int row, int col)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				SetCellOn(row, col);
			}
			else if (mouseButton.ButtonIndex == MouseButton.Right)
			{
				SetCellOff(row, col);
			}
		}
	}
	
	private void SetCellOn(int row, int col)
	{
		if (!_gridState[row, col])
		{
			_gridState[row, col] = true;
			UpdateCellVisual(row, col);
		}
	}
	
	private void SetCellOff(int row, int col)
	{
		if (_gridState[row, col])
		{
			_gridState[row, col] = false;
			UpdateCellVisual(row, col);
		}
	}
	
	private void UpdateCellVisual(int row, int col)
	{
		var panel = _gridCells[row, col];
		var styleBox = panel.GetThemeStylebox("panel") as StyleBoxFlat;
		
		if (styleBox != null)
		{
			styleBox.BgColor = _gridState[row, col] ? _onColor : _offColor;
		}
	}
	
	private void OnCellMouseEntered(Panel panel, int row, int col)
	{
		// Handle drag painting
		if (_isMouseDown)
		{
			SetCellOn(row, col);
		}
		else if (_isRightMouseDown)
		{
			SetCellOff(row, col);
		}
		
		// Hover effect for off cells
		var styleBox = panel.GetThemeStylebox("panel") as StyleBoxFlat;
		if (styleBox != null && !_gridState[row, col])
		{
			styleBox.BorderColor = new Color(0.8f, 0.8f, 0.8f);
			styleBox.SetBorderWidthAll(2);
		}
	}
	
	private void OnCellMouseExited(Panel panel, int row, int col)
	{
		var styleBox = panel.GetThemeStylebox("panel") as StyleBoxFlat;
		if (styleBox != null && !_gridState[row, col])
		{
			styleBox.BorderColor = new Color(0.4f, 0.4f, 0.4f);
			styleBox.SetBorderWidthAll(1);
		}
	}
	
	#region Public API
	
	/// <summary>
	/// Get the current state of the grid
	/// </summary>
	public bool[,] GetGridState()
	{
		return (bool[,])_gridState.Clone();
	}
	
	/// <summary>
	/// Set the state of a specific cell
	/// </summary>
	public void SetCell(int row, int col, bool state)
	{
		if (row >= 0 && row < GridSize && col >= 0 && col < GridSize)
		{
			_gridState[row, col] = state;
			UpdateCellVisual(row, col);
		}
	}
	
	/// <summary>
	/// Set the entire grid state
	/// </summary>
	public void SetGridState(bool[,] state)
	{
		if (state.GetLength(0) != GridSize || state.GetLength(1) != GridSize)
		{
			GD.PrintErr($"Invalid grid state size. Expected {GridSize}x{GridSize}");
			return;
		}
		
		for (int row = 0; row < GridSize; row++)
		{
			for (int col = 0; col < GridSize; col++)
			{
				_gridState[row, col] = state[row, col];
				UpdateCellVisual(row, col);
			}
		}
	}
	
	/// <summary>
	/// Clear all cells (set to off)
	/// </summary>
	public void ClearGrid()
	{
		for (int row = 0; row < GridSize; row++)
		{
			for (int col = 0; col < GridSize; col++)
			{
				_gridState[row, col] = false;
				UpdateCellVisual(row, col);
			}
		}
	}
	
	/// <summary>
	/// Fill all cells (set to on)
	/// </summary>
	public void FillGrid()
	{
		for (int row = 0; row < GridSize; row++)
		{
			for (int col = 0; col < GridSize; col++)
			{
				_gridState[row, col] = true;
				UpdateCellVisual(row, col);
			}
		}
	}
	
	/// <summary>
	/// Invert all cells
	/// </summary>
	public void InvertGrid()
	{
		for (int row = 0; row < GridSize; row++)
		{
			for (int col = 0; col < GridSize; col++)
			{
				_gridState[row, col] = !_gridState[row, col];
				UpdateCellVisual(row, col);
			}
		}
	}
	
	#endregion

    public override List<string> Validity()
    {
        return new List<string>();
    }

    public override Dictionary<string, object> GetParams()
    {
        return new();
    }
}
