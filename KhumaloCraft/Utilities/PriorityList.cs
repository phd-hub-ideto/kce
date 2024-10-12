namespace KhumaloCraft.Utilities;

//this class is threadsafe
[Serializable]
public class PriorityList<Key, Value>
{
    public PriorityList(int maxSize, TimeSpan maxAge)
    {
        MaxSize = maxSize;
        MaxAge = maxAge;
    }

    [Serializable]
    private class Node
    {
        public Node()
        {
            Instantiated = DateTime.Now;
        }
        public DateTime Instantiated { get; private set; }
        public int Size { get; set; }
        public Key Key { get; set; }
        public Value Value { get; set; }

        private LinkedListNode<Node> _node;
        public LinkedListNode<Node> LinkedListNode
        {
            get
            {
                return _node;
            }
            set
            {
                if ((value != null) && (value.Value != this))
                {
                    throw new Exception("Foreign node encountered.");
                }
                _node = value;
            }
        }
    }

    private object _listLock = new object();
    private LinkedList<Node> _priorityList = new LinkedList<Node>();
    private Dictionary<Key, Node> _dictionary = new Dictionary<Key, Node>();

    private int _totalSize = 0;

    public bool TryGetValue(Key key, out Value value, TryGetValueGetter<Key, Value> tryGetValueGetter)
    {
        Node node;

        bool found
            ;
        lock (_listLock)
        {
            found = _dictionary.TryGetValue(key, out node);
            if (found)
            {
                if (DateTime.Now - node.Instantiated > MaxAge)
                {
                    RemoveNode(node);
                    found = false;
                }
                else
                {
                    RaisePriority(node);
                }
            }
        }

        if (!found)
        {
            var sizeValuePair = new SizeValuePair<Value>();

            if (!tryGetValueGetter(key, ref sizeValuePair))
            {
                value = default;

                return false;
            }
            lock (_listLock)
            {
                node = AddNode(key, sizeValuePair);
            }
        }
        value = node.Value;

        return true;
    }

    public void RemoveValue(Key key)
    {
        bool found;

        lock (_listLock)
        {
            found = _dictionary.TryGetValue(key, out Node node);
            if (found)
            {
                RemoveNode(node);
            }
        }
    }

    //must be called withing _listLock
    private void RaisePriority(Node node)
    {
        _priorityList.Remove(node.LinkedListNode);
        node.LinkedListNode = _priorityList.AddFirst(node);
    }

    //must be called withing _listLock
    private Node AddNode(Key key, SizeValuePair<Value> sizeValuePair)
    {

        if (_dictionary.TryGetValue(key, out Node node))
        {
            RemoveNode(node);
            node = null;
        }

        node = new Node();
        node.Size = sizeValuePair.Size;
        node.Key = key;
        node.Value = sizeValuePair.Value;
        node.LinkedListNode = _priorityList.AddFirst(node);
        _dictionary.Add(key, node);
        _totalSize += node.Size;

        PruneList();

        return node;
    }

    //must be called withing _listLock
    private void PruneList()
    {
        LinkedListNode<Node> linkedListNode;

        while ((_totalSize > _maxSize) && ((linkedListNode = _priorityList.Last) != null))
        {
            RemoveNode(linkedListNode.Value);
        }
    }

    //must be called withing _listLock
    private void RemoveNode(Node node)
    {
        _dictionary.Remove(node.Key);
        _priorityList.Remove(node.LinkedListNode);
        _totalSize -= node.Size;
    }

    private int _maxSize;

    public int MaxSize
    {
        get
        {
            return _maxSize;
        }
        set
        {
            if (_maxSize != value)
            {
                lock (_listLock)
                {
                    _maxSize = value;
                    PruneList();
                }
            }
        }
    }

    private TimeSpan _maxAge;

    public TimeSpan MaxAge
    {
        get
        {
            return _maxAge;
        }
        set
        {
            if (_maxAge != value)
            {
                lock (_listLock)
                {
                    _maxAge = value;
                    PruneList();
                }
            }
        }
    }

}

public delegate bool TryGetValueGetter<Key, Value>(Key key, ref SizeValuePair<Value> sizeValuePair);

[Serializable]
public struct SizeValuePair<T>
{
    public T Value { get; set; }
    public int Size { get; set; }
}