﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GingerEditor.Utilities
{
    public interface IUndoRedo
    {
        string Name { get; }
        void Undo();
        void Redo();
    }

    public class UndoRedoAction : IUndoRedo
    {
        private Action _undoAction;
        private Action _redoAction;
        public string Name { get; }

        public void Undo() => _undoAction();

        public void Redo() => _redoAction();

        public UndoRedoAction(string name)
        {
            Name = name;
        }

        public UndoRedoAction(Action undo, Action redo, string name)
            : this(name)
        {
            Debug.Assert(undo != null && redo != null);
            _undoAction = undo;
            _redoAction = redo;
        }
    }

    public class UndoRedo
    {
        private readonly ObservableCollection<IUndoRedo> _undoList = new ObservableCollection<IUndoRedo>();
        public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }

        private readonly ObservableCollection<IUndoRedo> _redoList = new ObservableCollection<IUndoRedo>();
        public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }

        public UndoRedo()
        {
            UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
            RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
        }

        public void Reset()
        {
            _undoList.Clear();
            _redoList.Clear();
        }

        public void Undo()
        {
            if (_undoList.Any())
            {
                var lastAction = _undoList.Last();
                _undoList.RemoveAt(_undoList.Count - 1);
                lastAction.Undo();
                _redoList.Insert(0, lastAction);
            }
        }

        public void Redo()
        {
            if ( _redoList.Any())
            {
                var lastAction = _redoList.First();
                _redoList.RemoveAt(0);
                lastAction.Redo();
                _undoList.Add(lastAction);
            }
        }

        public void Add(IUndoRedo action)
        {
            _undoList.Add(action);
            _redoList.Clear();
        }
    }
}
