using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DewCore.Abstract.Graph;
using DewCore.Algorithms.Sort;

namespace DewCore.Graph
{
    public class Edge<V> : IEdge<V>
    {
        public Edge()
        {
        }
        public Edge(INode<V> node)
        {
            Node = node;
        }
        public Edge(INode<V> node, double weight)
        {
            Node = node;
            Weight = weight;
        }
        public INode<V> Node { get; set; }
        public double Weight { get; set; } = double.MaxValue;
        public int CompareTo(object obj)
        {
            var toCompare = obj as Edge<V>;
            return toCompare.Weight == Weight ? 0 : (Weight > toCompare.Weight ? 1 : -1);
        }
    }
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

    public class EdgeList<V> : IEdgeList<V>
    {
        private Dictionary<long, IEdge<V>> _nodes = new Dictionary<long, IEdge<V>>();
        public int Count => _nodes.Count;
        public bool IsSynchronized => false;
        public bool IsReadOnly => false;
        public void Add(IEdge<V> item)
        {
            _nodes.Add(item.Node.Identifier.UniqueIdentifier, item);
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public bool Contains(IEdge<V> item)
        {
            return _nodes.ContainsKey(item.Node.Identifier.UniqueIdentifier);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IEdge<V>[] array, int arrayIndex)
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

        public IEdge<V> GetEdge(IUid id)
        {
            if (_nodes.ContainsKey(id.UniqueIdentifier))
                return _nodes[id.UniqueIdentifier];
            else
                return null;
        }

        public bool Remove(IEdge<V> item)
        {
            bool result = false;
            if (Contains(item as IEdge<V>))
            {
                result = _nodes.Remove(item.Node.Identifier.UniqueIdentifier);
            }
            return result;
        }

        public IEdge<V> RemoveEdge(IEdge<V> item)
        {
            IEdge<V> result = null;
            if (Contains(item as IEdge<V>))
            {
                result = _nodes[item.Node.Identifier.UniqueIdentifier];
                _nodes.Remove(item.Node.Identifier.UniqueIdentifier);
            }
            return result;
        }

        public IEdge<V> RemoveEdge(Func<IEdge<V>, bool> predicate)
        {
            IEdge<V> result = null;
            foreach (var item in _nodes)
            {
                if (predicate(item.Value))
                {
                    result = _nodes[item.Value.Node.Identifier.UniqueIdentifier];
                    _nodes.Remove(item.Value.Node.Identifier.UniqueIdentifier);
                    break;
                }
            }
            return result;
        }

        IEnumerator<IEdge<V>> IEnumerable<IEdge<V>>.GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }
    }

    public class NodeList<V> : INodeList<V>
    {
        private Dictionary<long, INode<V>> _nodes = new Dictionary<long, INode<V>>();
        public int Count => _nodes.Count;

        public bool IsSynchronized => false;

        public bool IsReadOnly => false;

        public void Add(INode<V> item)
        {
            _nodes.Add(item.Identifier.UniqueIdentifier, item);
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public bool Contains(INode<V> item)
        {
            return _nodes.ContainsKey(item.Identifier.UniqueIdentifier);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(INode<V>[] array, int arrayIndex)
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

        public INode<V> GetNode(IUid id)
        {
            if (_nodes.ContainsKey(id.UniqueIdentifier))
                return _nodes[id.UniqueIdentifier];
            else
                return null;
        }

        public bool Remove(INode<V> item)
        {
            bool result = false;
            if (Contains(item))
            {
                result = _nodes.Remove(item.Identifier.UniqueIdentifier);
            }
            return result;
        }

        public INode<V> RemoveNode(INode<V> item)
        {
            INode<V> result = null;
            if (Contains(item))
            {
                result = _nodes[item.Identifier.UniqueIdentifier];
                result.Edges.Clear();
                _nodes.Remove(item.Identifier.UniqueIdentifier);
            }
            return result;
        }

        public INode<V> RemoveNode(Func<INode<V>, bool> predicate)
        {
            INode<V> result = null;
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

        IEnumerator<INode<V>> IEnumerable<INode<V>>.GetEnumerator()
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
                return toCompare.PWeight == PWeight ? 0 : (PWeight > toCompare.PWeight ? 1 : -1);
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

        public IGraph<V> AddVertex(INode<V> v)
        {
            _nodes.Add(v);
            return this;
        }

        public INode<V> RemoveVertex(INode<V> v)
        {
            return _nodes.RemoveNode(v);
        }

        public INode<V> RemoveVertex(Func<INode<V>, bool> predicate)
        {
            return _nodes.RemoveNode(predicate);
        }

        public INodeList<V> AStar(INode<V> start, INode<V> end)
        {
            throw new NotImplementedException();
        }

        public INodeList<V> WAStar(INode<V> start, INode<V> end)
        {
            throw new NotImplementedException();
        }

        public INodeList<V> BFS(INode<V> start, INode<V> end)
        {
            INodeList<V> result = new NodeList<V>();
            Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            Reset();
            start.PathDecoration.State = VertexState.Visited;
            start.PathDecoration.Distance = 0;
            start.PathDecoration.AncestorIdentifier = null;
            Queue<INode<V>> queue = new Queue<INode<V>>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var u = queue.Dequeue();
                foreach (var v in u.Edges)
                {
                    if (v.Node.PathDecoration.State == VertexState.Unvisited)
                    {
                        v.Node.PathDecoration.State = VertexState.Visited;
                        v.Node.PathDecoration.Distance = u.PathDecoration.Distance;
                        v.Node.PathDecoration.AncestorIdentifier = u.Identifier;
                        anchestors.Add(v.Node.Identifier, u.Identifier);
                        v.Node.PathDecoration.AncestorIdentifier = u.Identifier;
                        queue.Enqueue(v.Node);
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

        private void DFSVisit(INode<V> current, Dictionary<IUid, IUid> anchestors, ref uint time)
        {
            time++;
            current.PathDecoration.Discovered = time;
            current.PathDecoration.State = VertexState.Visited;
            foreach (var v in current.Edges)
            {
                if (v.Node.PathDecoration.State == VertexState.Unvisited)
                {
                    anchestors[v.Node.Identifier] = current.Identifier;
                    v.Node.PathDecoration.AncestorIdentifier = current.Identifier;
                    DFSVisit(v.Node, anchestors, ref time);
                }
                else
                {
                    if (v.Node.PathDecoration.State == VertexState.Visited)
                        _acyclic = false;
                }
            }
            current.PathDecoration.State = VertexState.Closed;
            time++;
            current.PathDecoration.Closed = time;
        }

        public INode<V> GetVertex(IUid id)
        {
            return _nodes.GetNode(id);
        }

        public INode<V> GetVertex(Func<INode<V>, bool> predicate)
        {
            INode<V> result = null;
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

        public INodeList<V> ShortPathBFS(INode<V> start, INode<V> end)
        {
            INodeList<V> result = new NodeList<V>();
            Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            ///BFS CODE
            Reset();
            start.PathDecoration.State = VertexState.Visited;
            start.PathDecoration.Distance = 0;
            start.PathDecoration.AncestorIdentifier = null;
            Queue<INode<V>> queue = new Queue<INode<V>>();
            queue.Enqueue(start);
            bool finded = false;
            while (queue.Count > 0 && finded == false)
            {
                var u = queue.Dequeue();
                foreach (var v in u.Edges)
                {
                    if (v.Node.PathDecoration.State == VertexState.Unvisited)
                    {
                        v.Node.PathDecoration.State = VertexState.Visited;
                        v.Node.PathDecoration.Distance = u.PathDecoration.Distance;
                        v.Node.PathDecoration.AncestorIdentifier = u.Identifier;
                        anchestors.Add(v.Node.Identifier, u.Identifier);
                        queue.Enqueue(v.Node);
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
                trasposed.AddVertex(new Node<V>(id) as INode<V>);
            }
            foreach (var u in _nodes)
            {
                foreach (var v in u.Edges)
                {
                    trasposed.GetVertex(v.Node.Identifier).AddEdge(trasposed.GetVertex(u.Identifier));
                }
            }
            return trasposed;
        }
        public INodeList<V> ShortPathDijkstra(INode<V> start, INode<V> end)
        {
            throw new NotImplementedException();
        }
        private class TempEdge : IComparable
        {
            public INode<V> Node { get; set; }
            public double Weight { get; set; }
            public TempEdge(INode<V> node, double weight)
            {
                Node = node;
                Weight = weight;
            }
            public int CompareTo(object obj)
            {
                var toCompare = (TempEdge)obj;
                return toCompare.Weight == Weight ? 0 : (Weight > toCompare.Weight ? 1 : -1);
            }
        }
        private double Dist(INode<V> start, INode<V> end)
        {
            return start.Edges.First(x => x.Node.Identifier.CompareTo(end.Identifier) == 0).Weight;
        }
        public INodeList<V> Dijkstra(INode<V> start, INode<V> end)
        {
            Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            Reset();
            var set1 = new List<TempEdge>();
            var set2 = new List<TempEdge>();
            foreach (var item in _nodes)
            {
                set2.Add(new TempEdge(item,double.MaxValue));
            }
            var first = set2.First(x => x.Node.Identifier.CompareTo(start.Identifier) == 0);
            first.Weight = 0;
            var sorter = HeapSort.GetSorter();
            while(set2.Count > 0)
            {
                var curr = sorter.PerformHeapSortBaseType<TempEdge, List<TempEdge>>(set2, HeapSort.Order.Desc).First();
                set1.Add(curr);
                set2.Remove(curr);
                if (curr.Weight == double.MaxValue)
                    return null;
                foreach (var item in curr.Node.Edges)
                {
                    var currdist = item.Weight + Dist(curr.Node,item.Node);
                    if (currdist < item.Weight)
                    {
                        item.Weight = currdist;
                        anchestors.Add(item.Node.Identifier, curr.Node.Identifier);
                    }
                }
            }
            

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
    public class Node<V> : INode<V>
    {
        public IPathDecoration<V> PathDecoration { get; set; } = new NodeDecoration<V>();
        private IUid _identifier = null;
        public IUid Identifier { get => _identifier; set => _identifier = value; }
        private IEdgeList<V> _edges = new EdgeList<V>();
        public IEdgeList<V> Edges { get => _edges; set => _edges = value; }
        private V _value = default(V);
        public V Value { get => _value; set => _value = value; }
        public Node(IUid identifier) => _identifier = identifier;
        public INode<V> AddEdge(INode<V> node, double weight = 0)
        {
            var n = node as INode<V>;
            var edge = new Edge<V>(n);
            _edges.Add(edge);
            return this;
        }
        public override bool Equals(object obj)
        {
            var node = obj as Node<V>;
            return node != null &&
                   EqualityComparer<IUid>.Default.Equals(_identifier, node._identifier);
        }
        public override int GetHashCode()
        {
            return 1278429765 + EqualityComparer<IUid>.Default.GetHashCode(_identifier);
        }
        public IEdge<V> RemoveEdge(IEdge<V> edge)
        {
            return _edges.RemoveEdge(edge);
        }
        public IEdge<V> RemoveEdge(Func<IEdge<V>, bool> predicate)
        {
            return _edges.RemoveEdge(predicate);
        }
        public static bool operator ==(Node<V> node1, Node<V> node2)
        {
            return node1.Identifier.CompareTo(node2.Identifier) == 0;
        }
        public static bool operator !=(Node<V> node1, Node<V> node2)
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
