using Godot;
using System;
using System.Collections.Generic;

public partial class UndoService
{
    private Stack<Update> _undoStack = new();

    private Stack<Update> _redoStack = new();

    public void ClearStack()
    {
        
    }

    public void Add(Update update)
    {
        _undoStack.Push(update);
    }

    public void Add(Change change)
    {
        var u = new Update();
        u.Add(change);
        Add(u);
    }

    public void Add(VisualComponentBase component, Change.ChangeType changeType, object beginState,
        object endState)
    {
        var change = new Change
        {
            Action = changeType,
            Component = component,
            Begin = beginState,
            End = endState
        };
        Add(change);
    }

    public void Undo()
    {
        if (!_undoStack.TryPop(out var update)) return;

        foreach (var c in update)
        {
            switch (c.Action)
            {
                case Change.ChangeType.Transform:
                    ExecuteTransform(c);
                    break;
                case Change.ChangeType.Creation:
                    break;
                case Change.ChangeType.Deletion:
                    break;
                case Change.ChangeType.LockStatus:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ExecuteTransform(Change change)
    {
        if (change.Begin is Transform3D t)
        {
            change.Component.Transform = t;
        }
    }

    public void BeginUpdate()
    {
    }

    public void CommitUpdate()
    {
    }
}