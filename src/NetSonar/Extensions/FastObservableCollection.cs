// © XIV-Tools.
// Licensed under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using MintPlayer.ObservableCollection;
using MintPlayer.ObservableCollection.Extensions;

namespace NetSonar.Avalonia.Extensions;

public class FastObservableCollection<T> : ObservableCollection<T>
{
    public int MaxItemCount
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(MaxItemCount)));
        }
    }

    private void ConstrainItemCount()
    {
        if (MaxItemCount <= 0) return;
        var difference = Count - MaxItemCount;
        if (difference <= 0) return;
        this.RemoveRange(0, difference);
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
        if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Reset or NotifyCollectionChangedAction.Replace)
            ConstrainItemCount();
    }
    /*private bool _suppressChangedEvent;

    public void Replace(IEnumerable<T> other)
    {
        _suppressChangedEvent = true;

        Clear();
        AddRange(other);
    }

    public void Replace(IEnumerable other)
    {
        _suppressChangedEvent = true;

        var items = Items.ToArray();
        Clear();

        foreach (T item in other)
        {
            Add(item);
        }

        _suppressChangedEvent = false;

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, other, items));
        OnPropertyChanged(new(nameof(Count)));
    }

    public void AddRange(IEnumerable other)
    {
        _suppressChangedEvent = true;

        foreach (object item in other)
        {
            if (item is T tItem)
            {
                Add(tItem);
            }
        }

        _suppressChangedEvent = false;

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(new(nameof(Count)));
    }

    public void RemoveRange(IEnumerable other)
    {
        _suppressChangedEvent = true;

        foreach (object item in other)
        {
            if (item is T tItem)
            {
                Remove(tItem);
            }
        }

        _suppressChangedEvent = false;

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(new(nameof(Count)));
    }

    public void RemoveRange(int index, int count)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), index, "Index can not be negative.");
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), count, "Count can not be negative.");
        if (count == 0) return;
        if (index >= Count) throw new ArgumentOutOfRangeException(nameof(index), index, "The index is outside the range of this list.");

        _suppressChangedEvent = true;

        var currentIndex = index;
        var lastIndex = Math.Min(index + count, Count - 1);
        while (currentIndex <= lastIndex)
        {
            RemoveAt(index);
            currentIndex++;
        }

        _suppressChangedEvent = false;

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(new(nameof(Count)));
    }

    public void SortAndReplace(IEnumerable<T> other, IComparer<T> comparer)
    {
        List<T> values = new(other);
        values.Sort(comparer);
        Replace(values);
    }

    public void Sort(IComparer<T> comparer)
    {
        List<T> values = new(this);
        values.Sort(comparer);
        Replace(values);
    }

    public void Synchronize(NotifyCollectionChangedEventArgs args)
    {
        if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
        {
            AddRange(args.NewItems);
        }
        else if (args.Action == NotifyCollectionChangedAction.Remove && args.OldItems != null)
        {
            RemoveRange(args.OldItems);
        }
        else if (args.Action == NotifyCollectionChangedAction.Reset)
        {
            Clear();
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (_suppressChangedEvent)
            return;

        base.OnPropertyChanged(e);
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (_suppressChangedEvent)
            return;

        base.OnCollectionChanged(e);
    }*/
}
/*
public interface IFastObservableCollection
{
    public void Replace(IEnumerable other);
}*/