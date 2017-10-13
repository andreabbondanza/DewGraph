using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DewCore.Abstract.Graph;
using DewCore.Algorithms.Sort;

namespace DewCore.Graph
{

    public class NodeDecoration<V> : IPathDecoration<V>
    {
        public ulong Distance { get; set; }
        public VertexState State { get; set; } = VertexState.Unvisited;
        public IUid AncestorIdentifier { get; set; }
        public uint Discovered { get; set; }
        public uint Closed { get; set; }
        public void Reset()
        {
            State = VertexState.Unvisited;
            Distance = ulong.MaxValue;
            AncestorIdentifier = null;
            Closed = 0;
            Discovered = 0;
        }
    }
    public class NodeList<V> : INodeList<V>
    {
        private Dictionary<long, INode<INodeList<V>, V>> _nodes = new Dictionary<long, INode<INodeList<V>, V>>();
        public int Count => _nodes.Count;

        public bool IsSynchronized => false;

        public bool IsReadOnly => false;

        public void Add(INode<INodeList<V>, V> item)
        {
            _nodes.Add(item.Identifier.UniqueIdentifier, item);
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public bool Contains(INode<INodeList<V>, V> item)
        {
            return _nodes.ContainsKey(item.Identifier.UniqueIdentifier);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(INode<INodeList<V>, V>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            var i = arrayIndex;
            foreach (var item in _nodes)
            {
                array[i++] = item.Value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        public INode<INodeList<V>, V> GetNode(IUid id)
        {
            if (_nodes.ContainsKey(id.UniqueIdentifier))
                return _nodes[id.UniqueIdentifier];
            else
                return null;
        }

        public bool Remove(INode<INodeList<V>, V> item)
        {
            bool result = false;
            if (Contains(item))
            {
                result = _nodes.Remove(item.Identifier.UniqueIdentifier);
            }
            return result;
        }

        public INode<INodeList<V>, V> RemoveNode(INode<INodeList<V>, V> item)
        {
            INode<INodeList<V>, V> result = null;
            if (Contains(item))
            {
                result = _nodes[item.Identifier.UniqueIdentifier];
                result.Edges.Clear();
                _nodes.Remove(item.Identifier.UniqueIdentifier);
            }
            return result;
        }

        public INode<INodeList<V>, V> RemoveNode(Func<INode<INodeList<V>, V>, bool> predicate)
        {
            INode<INodeList<V>, V> result = null;
            foreach (var item in _nodes)
            {
                if (predicate(item.Value))
                {
                    result = _nodes[item.Value.Identifier.UniqueIdentifier];
                    result.Edges.Clear();
                    _nodes.Remove(item.Value.Identifier.UniqueIdentifier);
                    break;
                }
            }
            return result;
        }

        IEnumerator<INode<INodeList<V>, V>> IEnumerable<INode<INodeList<V>, V>>.GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }
    }
    public class Graph<V> : IGraph<V>, IPath<V>
    {
        private class OrderHelpType : IComparable
        {
            public IUid NodeId { get; set; }
            public double PWeight { get; set; } = double.MaxValue;
            public int CompareTo(object obj)
            {
                var toCompare = obj as OrderHelpType;
                return toCompare.PWeight == PWeight ? 0 :(PWeight > toCompare.PWeight ? 1 : -1);
            }
            public OrderHelpType(IUid id)
            {
                NodeId = id;
            }
        }
        private bool _acyclic = true;
        private INodeList<V> _nodes = null;
        public INodeList<V> Nodes { get => _nodes; set => _nodes = value; }
        public Graph(INodeList<V> list)
        {
            _nodes = list;
        }

        public IGraph<V> AddVertex(INode<INodeList<V>, V> v)
        {
            _nodes.Add(v);
            return this;
        }

        public INode<INodeList<V>, V> RemoveVertex(INode<INodeList<V>, V> v)
        {
            return _nodes.RemoveNode(v);
        }

        public INode<INodeList<V>, V> RemoveVertex(Func<INode<INodeList<V>, V>, bool> predicate)
        {
            return _nodes.RemoveNode(predicate);
        }

        public INodeList<V> AStar(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end)
        {
            throw new NotImplementedException();
        }

        public INodeList<V> WAStar(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end)
        {
            throw new NotImplementedException();
        }

        public INodeList<V> BFS(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end)
        {
            INodeList<V> result = new NodeList<V>();
            Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            Reset();
            start.PathDecoration.State = VertexState.Visited;
            start.PathDecoration.Distance = 0;
            start.PathDecoration.AncestorIdentifier = null;
            Queue<INode<INodeList<V>, V>> queue = new Queue<INode<INodeList<V>, V>>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var u = queue.Dequeue();
                foreach (var v in u.Edges)
                {
                    if (v.PathDecoration.State == VertexState.Unvisited)
                    {
                        v.PathDecoration.State = VertexState.Visited;
                        v.PathDecoration.Distance = u.PathDecoration.Distance;
                        v.PathDecoration.AncestorIdentifier = u.Identifier;
                        anchestors.Add(v.Identifier, u.Identifier);
                        v.PathDecoration.AncestorIdentifier = u.Identifier;
                        queue.Enqueue(v);
                    }
                }
                u.PathDecoration.State = VertexState.Closed;
            }
            if (!anchestors.ContainsKey(end.Identifier))
                return null;
            else
            {
                var current = end;
                while (current != start)
                {
                    result.Add(current);
                    var currId = anchestors[current.Identifier];
                    current = _nodes.GetNode(currId);
                }
                result.Add(start);
            }
            return result;
        }

        public IPath<V> DFS()
        {
            INodeList<V> result = new NodeList<V>();
            Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            uint time = 0;
            Reset();
            foreach (var item in _nodes)
            {
                if (item.PathDecoration.State == VertexState.Unvisited)
                    DFSVisit(item, anchestors, ref time);
            }
            return this;
        }

        private void DFSVisit(INode<INodeList<V>, V> current, Dictionary<IUid, IUid> anchestors, ref uint time)
        {
            time++;
            current.PathDecoration.Discovered = time;
            current.PathDecoration.State = VertexState.Visited;
            foreach (var v in current.Edges)
            {
                if (v.PathDecoration.State == VertexState.Unvisited)
                {
                    anchestors[v.Identifier] = current.Identifier;
                    v.PathDecoration.AncestorIdentifier = current.Identifier;
                    DFSVisit(v, anchestors, ref time);
                }
                else
                {
                    if (v.PathDecoration.State == VertexState.Visited)
                        _acyclic = false;
                }
            }
            current.PathDecoration.State = VertexState.Closed;
            time++;
            current.PathDecoration.Closed = time;
        }

        public INode<INodeList<V>, V> GetVertex(IUid id)
        {
            return _nodes.GetNode(id);
        }

        public INode<INodeList<V>, V> GetVertex(Func<INode<INodeList<V>, V>, bool> predicate)
        {
            INode<INodeList<V>, V> result = null;
            foreach (var item in _nodes)
            {
                if (predicate(item))
                {
                    result = item;
                }
                break;
            }
            return result;
        }

        public INodeList<V> ShortPathBFS(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end)
        {
            INodeList<V> result = new NodeList<V>();
            Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            ///BFS CODE
            Reset();
            start.PathDecoration.State = VertexState.Visited;
            start.PathDecoration.Distance = 0;
            start.PathDecoration.AncestorIdentifier = null;
            Queue<INode<INodeList<V>, V>> queue = new Queue<INode<INodeList<V>, V>>();
            queue.Enqueue(start);
            bool finded = false;
            while (queue.Count > 0 && finded == false)
            {
                var u = queue.Dequeue();
                foreach (var v in u.Edges)
                {
                    if (v.PathDecoration.State == VertexState.Unvisited)
                    {
                        v.PathDecoration.State = VertexState.Visited;
                        v.PathDecoration.Distance = u.PathDecoration.Distance;
                        v.PathDecoration.AncestorIdentifier = u.Identifier;
                        anchestors.Add(v.Identifier, u.Identifier);
                        queue.Enqueue(v);
                        finded = v == end;
                    }
                }
                u.PathDecoration.State = VertexState.Closed;
            }
            if (!anchestors.ContainsKey(end.Identifier))
                return null;
            else
            {
                var current = end;
                while (current != start)
                {
                    result.Add(current);
                    var currId = anchestors[current.Identifier];
                    current = _nodes.GetNode(currId);
                }
                result.Add(start);
            }
            return result;
        }

        public bool IsAcyclic()
        {
            DFS();
            return _acyclic;
        }
        public bool IsConnected()
        {
            if (_nodes.Count > 0)
            {
                DFS();
                int nullCount = 0;
                foreach (var item in _nodes)
                {
                    if (item.PathDecoration.AncestorIdentifier == null)
                    {
                        nullCount++;
                    }
                    if (nullCount > 1)
                        return false;
                }
            }
            return true;
        }

        public IGraph<V> Reset()
        {
            foreach (var item in _nodes)
            {
                item.PathDecoration.Reset();
            }
            return this;
        }

        public IGraph<V> Traspose()
        {
            var trasposed = new Graph<V>(new NodeList<V>());
            foreach (var item in _nodes)
            {
                var id = item.Identifier;
                trasposed.AddVertex(new Node<NodeList<V>, V>(id));
            }
            foreach (var u in _nodes)
            {
                foreach (var v in u.Edges)
                {
                    trasposed.GetVertex(v.Identifier).AddEdge(trasposed.GetVertex(u.Identifier));
                }
            }
            return trasposed;
        }

        public INodeList<V> ShortPathDijkstra(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end)
        {
            throw new NotImplementedException();
        }

        public INodeList<V> Dijkstra(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end)
        {
            //INodeList<V> result = new NodeList<V>();
            //Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            //List<OrderHelpType> source = new List<OrderHelpType>();
            //List<OrderHelpType> set = new List<OrderHelpType>();
            //Reset();
            //start.PathDecoration.State = VertexState.Visited;
            //start.PathDecoration.Distance = 0;
            //start.PathDecoration.AncestorIdentifier = null;
            //foreach (var item in _nodes)
            //{
            //    set.Add(new OrderHelpType(item.Identifier));
            //}
            //var sorter = HeapSort.GetSorter();

            //while (set.Count > 0)
            //{
            //    set = sorter.PerformHeapSortBaseType<OrderHelpType, List<OrderHelpType>>(set);
            //    var uid = set.First();
            //    set.RemoveAt(0);
            //    source.Add(uid);
            //    if (uid.PWeight == double.MaxValue)
            //        break;
            //    var u = GetVertex(uid.NodeId);
            //    foreach (var item in u.Edges)
            //    {
            //        var alt = u.Weigth
            //    }
            //}


            //if (!anchestors.ContainsKey(end.Identifier))
            //    return null;
            //else
            //{
            //    var current = end;
            //    while (current != start)
            //    {
            //        result.Add(current);
            //        var currId = anchestors[current.Identifier];
            //        current = _nodes.GetNode(currId);
            //    }
            //    result.Add(start);
            //}
            //return result;
            return null;
        }
    }
    public class Node<T, V> : INode<INodeList<V>, V> where T : class, INodeList<V>, new()
    {
        private double _weight = 0;        
        public IPathDecoration<V> PathDecoration { get; set; } = new NodeDecoration<V>();
        private IUid _identifier = null;
        public IUid Identifier { get => _identifier; set => _identifier = value; }
        private INodeList<V> _edges = new T();
        public INodeList<V> Edges { get => _edges; set => _edges = value; }
        private V _value = default(V);
        public V Value { get => _value; set => _value = value; }
        public Node(IUid identifier) => _identifier = identifier;
        public Node(IUid identifier, double weight) { _identifier = identifier; _weight = weight; }
        public INode<INodeList<V>, V> AddEdge(INode<INodeList<V>, V> edge)
        {
            _edges.Add(edge);
            return this;
        }
        public INode<INodeList<V>, V> RemoveEdge(INode<INodeList<V>, V> edge)
        {
            return _edges.RemoveNode(edge);
        }
        public INode<INodeList<V>, V> RemoveEdge(Func<INode<INodeList<V>, V>, bool> predicate)
        {
            return _edges.RemoveNode(predicate);
        }
        public override bool Equals(object obj)
        {
            var node = obj as Node<T, V>;
            return node != null &&
                   EqualityComparer<IUid>.Default.Equals(_identifier, node._identifier);
        }
        public override int GetHashCode()
        {
            return 1278429765 + EqualityComparer<IUid>.Default.GetHashCode(_identifier);
        }
        public static bool operator ==(Node<T, V> node1, Node<T, V> node2)
        {
            return node1.Identifier.CompareTo(node2.Identifier) == 0;
        }
        public static bool operator !=(Node<T, V> node1, Node<T, V> node2)
        {
            return node1.Identifier.CompareTo(node2.Identifier) != 0;
        }
    }
    public class NodeIdentifier : IUid
    {
        public long UniqueIdentifier { get; set; }
        public int CompareTo(object obj)
        {
            var id = obj as IUid;
            return id.UniqueIdentifier == UniqueIdentifier ? 0 : (id.UniqueIdentifier < 0 ? -1 : 1);
        }
        public NodeIdentifier(long id) { UniqueIdentifier = id; }
    }
}
