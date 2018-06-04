using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DewCore.Abstract.Graph;
using DewCore.Algorithms.Sort;
using DewCore.Types.Base;

namespace DewCore.Graph
{
    /// <summary>
    /// Edge class
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class Edge<V> : IEdge<V>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Edge()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node"></param>
        public Edge(INode<V> node)
        {
            Node = node;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="node"></param>
        /// <param name="weight"></param>
        public Edge(INode<V> node, double weight)
        {
            Node = node;
            Weight = weight;
        }
        /// <summary>
        /// Node
        /// </summary>
        public INode<V> Node { get; set; }
        /// <summary>
        /// Weight
        /// </summary>
        public double Weight { get; set; } = double.MaxValue;
        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            var toCompare = obj as Edge<V>;
            return toCompare.Weight == Weight ? 0 : (Weight > toCompare.Weight ? 1 : -1);
        }
    }
    /// <summary>
    /// Node decoration
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class NodeDecoration<V> : IPathDecoration<V>
    {
        /// <summary>
        /// Distance from source
        /// </summary>
        public ulong Distance { get; set; }
        /// <summary>
        /// Current vertex state
        /// </summary>
        public NodeState State { get; set; } = NodeState.Unvisited;
        /// <summary>
        /// Anchestor IUid
        /// </summary>
        public IUid AncestorIdentifier { get; set; }
        /// <summary>
        /// Discovered time
        /// </summary>
        public ulong Discovered { get; set; }
        /// <summary>
        /// Closed time
        /// </summary>
        public ulong Closed { get; set; }
        /// <summary>
        /// Node's grade
        /// </summary>
        public ulong Grade { get => In + Out; }
        /// <summary>
        /// In vertex
        /// </summary>
        public ulong In { get; set; }
        /// <summary>
        /// Out vertex
        /// </summary>
        public ulong Out { get; set; }
        /// <summary>
        /// Reset
        /// </summary>
        public void Reset()
        {
            State = NodeState.Unvisited;
            Distance = ulong.MaxValue;
            AncestorIdentifier = null;
            Closed = 0;
            Discovered = 0;
        }
    }
    /// <summary>
    /// Edge list
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class EdgeList<V> : IEdgeList<V>
    {
        private Dictionary<long, IEdge<V>> _nodes = new Dictionary<long, IEdge<V>>();
        /// <summary>
        /// Edge count
        /// </summary>
        public int Count => _nodes.Count;
        /// <summary>
        /// IsSyncronized
        /// </summary>
        public bool IsSynchronized => false;
        /// <summary>
        /// IsReadOnly
        /// </summary>
        public bool IsReadOnly => false;
        /// <summary>
        /// Add a new edge
        /// </summary>
        /// <param name="item"></param>
        public void Add(IEdge<V> item)
        {
            item.Node.PathDecoration.In++;
            _nodes.Add(item.Node.Identifier.UniqueIdentifier, item);
        }
        /// <summary>
        /// Clear collection
        /// </summary>
        public void Clear()
        {
            _nodes.Clear();
        }
        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(IEdge<V> item)
        {
            return _nodes.ContainsKey(item.Node.Identifier.UniqueIdentifier);
        }
        /// <summary>
        /// CopyTo
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// CopyTo
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
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
        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
        /// <summary>
        /// Get an Edge
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEdge<V> GetEdge(IUid id)
        {
            if (_nodes.ContainsKey(id.UniqueIdentifier))
                return _nodes[id.UniqueIdentifier];
            else
                return null;
        }
        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(IEdge<V> item)
        {
            bool result = false;
            if (Contains(item as IEdge<V>))
            {
                item.Node.PathDecoration.In--;
                result = _nodes.Remove(item.Node.Identifier.UniqueIdentifier);
            }
            return result;
        }
        /// <summary>
        /// Remove edge
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEdge<V> RemoveEdge(IEdge<V> item)
        {
            IEdge<V> result = null;
            if (Contains(item as IEdge<V>))
            {
                item.Node.PathDecoration.In--;
                result = _nodes[item.Node.Identifier.UniqueIdentifier];
                _nodes.Remove(item.Node.Identifier.UniqueIdentifier);
            }
            return result;
        }
        /// <summary>
        /// Remove edge with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEdge<V> RemoveEdge(Func<IEdge<V>, bool> predicate)
        {
            IEdge<V> result = null;
            foreach (var item in _nodes)
            {
                if (predicate(item.Value))
                {
                    result = _nodes[item.Value.Node.Identifier.UniqueIdentifier];
                    result.Node.PathDecoration.In--;
                    _nodes.Remove(item.Value.Node.Identifier.UniqueIdentifier);
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator<IEdge<V>> IEnumerable<IEdge<V>>.GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }
    }
    /// <summary>
    /// Node list
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class NodeList<V> : INodeList<V>
    {
        private Dictionary<long, INode<V>> _nodes = new Dictionary<long, INode<V>>();
        /// <summary>
        /// Node count
        /// </summary>
        public int Count => _nodes.Count;
        /// <summary>
        /// Is Syncronized
        /// </summary>
        public bool IsSynchronized => false;
        /// <summary>
        /// Is readonly
        /// </summary>
        public bool IsReadOnly => false;
        /// <summary>
        /// Add new vertex
        /// </summary>
        /// <param name="item"></param>
        public void Add(INode<V> item)
        {
            _nodes.Add(item.Identifier.UniqueIdentifier, item);
        }
        /// <summary>
        /// Clear nodelist
        /// </summary>
        public void Clear()
        {
            _nodes.Clear();
        }
        /// <summary>
        /// Check contains
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(INode<V> item)
        {
            return _nodes.ContainsKey(item.Identifier.UniqueIdentifier);
        }
        /// <summary>
        /// Copy to
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Copyto
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
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
        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }
        /// <summary>
        /// Get node by IUid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public INode<V> GetNode(IUid id)
        {
            if (_nodes.ContainsKey(id.UniqueIdentifier))
                return _nodes[id.UniqueIdentifier];
            else
                return null;
        }
        /// <summary>
        /// Remove node with bool return
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(INode<V> item)
        {
            bool result = false;
            if (Contains(item))
            {
                result = _nodes.Remove(item.Identifier.UniqueIdentifier);
            }
            return result;
        }
        /// <summary>
        /// Remove node with node return
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Remove node with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Return enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator<INode<V>> IEnumerable<INode<V>>.GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }
    }
    /// <summary>
    /// Graph object
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class Graph<V> : IGraph<V>
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
        /// <summary>
        /// Graph node list
        /// </summary>
        public INodeList<V> Nodes { get => _nodes; set => _nodes = value; }
        /// <summary>
        /// Graph constructor
        /// </summary>
        /// <param name="list"></param>
        public Graph(INodeList<V> list)
        {
            _nodes = list;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public Graph() { }
        /// <summary>
        /// Add a new vertex to a graph
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public IBaseGraph<V> AddNode(INode<V> v)
        {
            if (_nodes == null)
                _nodes = new NodeList<V>();
            _nodes.Add(v);
            return this;
        }
        /// <summary>
        /// Remove vertex from graph
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public INode<V> RemoveNode(INode<V> v)
        {
            return _nodes.RemoveNode(v);
        }
        /// <summary>
        /// Remove vertex with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public INode<V> RemoveNode(Func<INode<V>, bool> predicate)
        {
            return _nodes.RemoveNode(predicate);
        }
        /// <summary>
        /// Shortest path with A*
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="heuristic"></param>
        /// <returns></returns>
        public INodeList<V> AStar(INode<V> start, INode<V> end, Func<INode<V>, INode<V>, double> heuristic)
        {
            heuristic = heuristic ?? ((x, y) => 0);
            var result = new NodeList<V>();
            Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            Dictionary<IUid, double> dist = new Dictionary<IUid, double>();
            Reset();
            var queue = new List<TempEdge>();
            foreach (var item in _nodes)
            {
                queue.Add(new TempEdge(item, double.MaxValue));
                dist.Add(item.Identifier, double.MaxValue);
            }
            var first = queue.First(x => x.Node.Identifier.CompareTo(start.Identifier) == 0);
            first.Weight = 0;
            dist[first.Node.Identifier] = 0;
            var sorter = HeapSort.GetSorter();
            while (queue.Count > 0)
            {
                var sorted = sorter.PerformHeapSortBaseType<TempEdge, List<TempEdge>>(queue, HeapSort.Order.Asc);
                var curr = sorted.First();
                queue.Remove(curr);
                if (curr.Weight == double.MaxValue)
                    return null;
                foreach (var item in curr.Node.Edges)
                {
                    var currW = curr.Weight + EWeight(curr.Node, item.Node) + heuristic(item.Node, end);
                    if (currW < dist[item.Node.Identifier])
                    {
                        item.Weight = currW;
                        if (anchestors.ContainsKey(item.Node.Identifier))
                            anchestors[item.Node.Identifier] = curr.Node.Identifier;
                        else
                            anchestors.Add(item.Node.Identifier, curr.Node.Identifier);
                        dist[item.Node.Identifier] = currW;
                        queue.First(x => x.Node.Identifier == item.Node.Identifier).Weight = currW;
                    }
                }
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
        /// <summary>
        /// Shortest path with weighted A*
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="heuristic"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public INodeList<V> WAStar(INode<V> start, INode<V> end, Func<INode<V>, INode<V>, double> heuristic, double epsilon = 1)
        {
            heuristic = heuristic ?? ((x, y) => 0);
            var result = new NodeList<V>();
            Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            Dictionary<IUid, double> dist = new Dictionary<IUid, double>();
            Reset();
            var queue = new List<TempEdge>();
            foreach (var item in _nodes)
            {
                queue.Add(new TempEdge(item, double.MaxValue));
                dist.Add(item.Identifier, double.MaxValue);
            }
            var first = queue.First(x => x.Node.Identifier.CompareTo(start.Identifier) == 0);
            first.Weight = 0;
            dist[first.Node.Identifier] = 0;
            var sorter = HeapSort.GetSorter();
            while (queue.Count > 0)
            {
                var sorted = sorter.PerformHeapSortBaseType<TempEdge, List<TempEdge>>(queue, HeapSort.Order.Asc);
                var curr = sorted.First();
                queue.Remove(curr);
                if (curr.Weight == double.MaxValue)
                    return null;
                foreach (var item in curr.Node.Edges)
                {
                    var currW = curr.Weight + EWeight(curr.Node, item.Node) + (heuristic(item.Node, end) * epsilon);
                    if (currW < dist[item.Node.Identifier])
                    {
                        item.Weight = currW;
                        if (anchestors.ContainsKey(item.Node.Identifier))
                            anchestors[item.Node.Identifier] = curr.Node.Identifier;
                        else
                            anchestors.Add(item.Node.Identifier, curr.Node.Identifier);
                        dist[item.Node.Identifier] = currW;
                        queue.First(x => x.Node.Identifier == item.Node.Identifier).Weight = currW;
                    }
                }
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
        /// <summary>
        /// Execute bfs and return all nodes that satisfy the predicate
        /// </summary>
        /// <param name="start"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public INodeList<V> BFS(INode<V> start, Func<INode<V>, bool> predicate = null)
        {
            INodeList<V> result = new NodeList<V>();
            predicate = predicate ?? new Func<INode<V>, bool>((node) => true);
            Reset();
            start.PathDecoration.State = NodeState.Visited;
            start.PathDecoration.Distance = 0;
            start.PathDecoration.AncestorIdentifier = null;
            Queue<INode<V>> queue = new Queue<INode<V>>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var u = queue.Dequeue();
                if (predicate(u))
                    result.Add(u);
                foreach (var v in u.Edges)
                {
                    if (v.Node.PathDecoration.State == NodeState.Unvisited)
                    {
                        v.Node.PathDecoration.State = NodeState.Visited;
                        v.Node.PathDecoration.Distance = u.PathDecoration.Distance + 1;
                        v.Node.PathDecoration.AncestorIdentifier = u.Identifier;
                        queue.Enqueue(v.Node);
                    }
                }
                u.PathDecoration.State = NodeState.Closed;
            }
            return result;
        }
        /// <summary>
        /// Dfs
        /// </summary>
        /// <returns></returns>
        public IPath<V> DFS()
        {
            uint time = 0;
            Reset();
            foreach (var item in _nodes)
            {
                if (item.PathDecoration.State == NodeState.Unvisited)
                    DFSVisit(item, ref time);
            }
            return this;
        }
        /// <summary>
        /// DFS Visit
        /// </summary>
        /// <param name="current"></param>
        /// <param name="anchestors"></param>
        /// <param name="time"></param>
        private List<IUid> DFSVisit(INode<V> current, ref uint time)
        {
            var result = new List<IUid>();
            time++;
            current.PathDecoration.Discovered = time;
            current.PathDecoration.State = NodeState.Visited;
            foreach (var v in current.Edges)
            {
                if (v.Node.PathDecoration.State == NodeState.Unvisited)
                {
                    result.Add(v.Node.Identifier);
                    v.Node.PathDecoration.AncestorIdentifier = current.Identifier;
                    result.AddRange(DFSVisit(v.Node, ref time));
                }
                else
                {
                    if (v.Node.PathDecoration.State == NodeState.Visited)
                        _acyclic = false;
                }
            }
            current.PathDecoration.State = NodeState.Closed;
            time++;
            current.PathDecoration.Closed = time;
            return result;
        }
        /// <summary>
        /// Return a node
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public INode<V> GetNode(IUid id)
        {
            return _nodes.GetNode(id);
        }
        /// <summary>
        /// Return first node with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ICollection<INode<V>> GetNodes(Func<INode<V>, bool> predicate)
        {
            return _nodes?.Where(predicate).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public INodeList<V> ShortPathBFS(INode<V> start, INode<V> end)
        {
            INodeList<V> result = new NodeList<V>();
            Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            Reset();
            start.PathDecoration.State = NodeState.Visited;
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
                    if (v.Node.PathDecoration.State == NodeState.Unvisited)
                    {
                        v.Node.PathDecoration.State = NodeState.Visited;
                        v.Node.PathDecoration.Distance = u.PathDecoration.Distance + 1;
                        v.Node.PathDecoration.AncestorIdentifier = u.Identifier;
                        anchestors.Add(v.Node.Identifier, u.Identifier);
                        queue.Enqueue(v.Node);
                        finded = v == end;
                    }
                }
                u.PathDecoration.State = NodeState.Closed;
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
        /// <summary>
        /// Check if graph is acyclic
        /// </summary>
        /// <returns></returns>
        public bool IsAcyclic()
        {
            DFS();
            return _acyclic;
        }
        /// <summary>
        /// Check if graph is connected
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Reset graph
        /// </summary>
        /// <returns></returns>
        public IBaseGraph<V> Reset()
        {
            foreach (var item in _nodes)
            {
                item.PathDecoration.Reset();
            }
            return this;
        }
        /// <summary>
        /// Traspose graph
        /// </summary>
        /// <returns></returns>
        public IBaseGraph<V> Traspose()
        {
            var trasposed = new Graph<V>();
            foreach (var item in _nodes)
            {
                var id = item.Identifier;
                trasposed.AddNode(new Node<V>(id) as INode<V>);
            }
            foreach (var u in _nodes)
            {
                foreach (var v in u.Edges)
                {
                    trasposed.GetNode(v.Node.Identifier).AddEdge(trasposed.GetNode(u.Identifier));
                }
            }
            return trasposed;
        }
        /// <summary>
        /// Get short path between two nodes with Dijkstra
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public INodeList<V> ShortPathDijkstra(INode<V> start, INode<V> end)
        {
            var result = new NodeList<V>();
            Dictionary<IUid, IUid> anchestors = new Dictionary<IUid, IUid>();
            Dictionary<IUid, double> dist = new Dictionary<IUid, double>();
            Reset();
            var queue = new List<TempEdge>();
            foreach (var item in _nodes)
            {
                queue.Add(new TempEdge(item, double.MaxValue));
                dist.Add(item.Identifier, double.MaxValue);
            }
            var first = queue.First(x => x.Node.Identifier.CompareTo(start.Identifier) == 0);
            first.Weight = 0;
            dist[first.Node.Identifier] = 0;
            var sorter = HeapSort.GetSorter();
            while (queue.Count > 0)
            {
                var sorted = sorter.PerformHeapSortBaseType<TempEdge, List<TempEdge>>(queue, HeapSort.Order.Asc);
                var curr = sorted.First();
                queue.Remove(curr);
                if (curr.Weight == double.MaxValue)
                    return null;
                foreach (var item in curr.Node.Edges)
                {
                    var currW = curr.Weight + EWeight(curr.Node, item.Node);
                    if (currW < dist[item.Node.Identifier])
                    {
                        item.Weight = currW;
                        if (anchestors.ContainsKey(item.Node.Identifier))
                            anchestors[item.Node.Identifier] = curr.Node.Identifier;
                        else
                            anchestors.Add(item.Node.Identifier, curr.Node.Identifier);
                        dist[item.Node.Identifier] = currW;
                        queue.First(x => x.Node.Identifier == item.Node.Identifier).Weight = currW;
                    }
                }
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
        private double EWeight(INode<V> start, INode<V> end)
        {
            return start.Edges.First(x => x.Node.Identifier.CompareTo(end.Identifier) == 0).Weight;
        }
        /// <summary>
        /// Return a topological sort 
        /// </summary>
        /// <returns>Node list, else null if can't be sorted</returns>
        public ICollection<INode<V>> GetTopologicalSort()
        {
            Dictionary<IUid, ulong> nodes = new Dictionary<IUid, ulong>();
            foreach (var item in _nodes)
            {
                nodes.Add(item.Identifier, item.PathDecoration.In);
            }
            var result = new List<INode<V>>();
            while (result.Count < _nodes.Count)
            {
                var temp = nodes.Where(x => x.Value == 0).ToList();
                if (temp.Count > 0)
                {
                    foreach (var item in temp)
                    {
                        nodes.Remove(item.Key);
                        var node = _nodes.GetNode(item.Key);
                        foreach (var item1 in node.Edges)
                        {
                            nodes[item1.Node.Identifier]--;
                        }
                        result.Add(node);
                    }
                }
                else
                {
                    return null;
                }
            }
            return result;

        }
        /// <summary>
        /// Return strong connected componentrs nodes roots
        /// </summary>
        /// <returns></returns>
        public ICollection<IGraph<V>> GetStronglyConnectedComponents()
        {
            var roots = new Dictionary<IUid, List<IUid>>();
            var result = new List<IGraph<V>>();
            DFS();
            var trasposed = (Graph<V>)Traspose();
            uint time = 0;
            var temp = _nodes.OrderByDescending(x => x.PathDecoration.Closed);
            foreach (var item in temp)
            {
                if (trasposed.GetNode(item.Identifier).PathDecoration.State == NodeState.Unvisited)
                {
                    roots.Add(item.Identifier, new List<IUid>());
                    roots[item.Identifier].Add(item.Identifier);
                    roots[item.Identifier].AddRange(trasposed.DFSVisit(trasposed.GetNode(item.Identifier), ref time));
                }
            }
            foreach (var item in roots)
            {
                var nodelist = new NodeList<V>();
                foreach (var item1 in item.Value)
                {
                    nodelist.Add(trasposed.GetNode(item1));
                }
                result.Add(new Graph<V>(nodelist));
            }
            return result;
        }
    }
    /// <summary>
    /// Node class
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class Node<V> : INode<V>
    {
        /// <summary>
        /// Path decoration
        /// </summary>
        public IPathDecoration<V> PathDecoration { get; set; } = new NodeDecoration<V>();
        /// <summary>
        /// Node identifier
        /// </summary>
        private IUid _identifier = null;
        /// <summary>
        /// Node identifier
        /// </summary>
        public IUid Identifier { get => _identifier; set => _identifier = value; }
        private IEdgeList<V> _edges = new EdgeList<V>();
        /// <summary>
        /// The edges list
        /// </summary>
        public IEdgeList<V> Edges { get => _edges; set => _edges = value; }
        private V _value = default(V);
        /// <summary>
        /// Node value
        /// </summary>
        public V Value { get => _value; set => _value = value; }
        /// <summary>
        /// Node constructor
        /// </summary>
        /// <param name="identifier"></param>
        public Node(IUid identifier) => _identifier = identifier;
        /// <summary>
        /// Add edge to node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public INode<V> AddEdge(INode<V> node, double weight = 0)
        {
            var n = node as INode<V>;
            this.PathDecoration.Out++;
            var edge = new Edge<V>(n)
            {
                Weight = weight
            };
            _edges.Add(edge);
            return this;
        }
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var node = obj as Node<V>;
            return node != null &&
                   EqualityComparer<IUid>.Default.Equals(_identifier, node._identifier);
        }
        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 1278429765 + EqualityComparer<IUid>.Default.GetHashCode(_identifier);
        }
        /// <summary>
        /// Remove edge
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public IEdge<V> RemoveEdge(IEdge<V> edge)
        {
            this.PathDecoration.Out--;
            return _edges.RemoveEdge(edge);
        }
        /// <summary>
        /// Remove edge with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEdge<V> RemoveEdge(Func<IEdge<V>, bool> predicate)
        {
            return _edges.RemoveEdge(predicate);
        }
        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        public static bool operator ==(Node<V> node1, Node<V> node2)
        {
            return node1.Identifier.CompareTo(node2.Identifier) == 0;
        }
        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        public static bool operator !=(Node<V> node1, Node<V> node2)
        {
            return node1.Identifier.CompareTo(node2.Identifier) != 0;
        }
    }
    /// <summary>
    /// A node identifier
    /// </summary>
    public class NodeIdentifier : IUid
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public long UniqueIdentifier { get; set; }
        /// <summary>
        /// Identifier comparator
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            var id = obj as IUid;
            return id.UniqueIdentifier == UniqueIdentifier ? 0 : (id.UniqueIdentifier < 0 ? -1 : 1);
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        public NodeIdentifier(long id) { UniqueIdentifier = id; }
    }
}
