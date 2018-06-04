using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DewCore.Abstract.Graph
{
    /// <summary>
    /// Vertex state
    /// </summary>
    public enum NodeState
    {
        /// <summary>
        /// Unvisited flag
        /// </summary>
        Unvisited,
        /// <summary>
        /// Pending flag
        /// </summary>
        Visited,
        /// <summary>
        /// Close visited flag
        /// </summary>
        Closed
    }
    /// <summary>
    /// Path functions interface
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public interface IPath<V>
    {
        /// <summary>
        /// Get shortest path between two nodes with Dijkstra algorithm
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        INodeList<V> ShortPathDijkstra(INode<V> start, INode<V> end);
        /// <summary>
        /// Get shortest path between two nodes with BFS alghoritm
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        INodeList<V> ShortPathBFS(INode<V> start, INode<V> end);
        /// <summary>
        /// Get best path with AStar algorithm
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="heuristic">Heuristic function</param>
        /// <returns></returns>
        INodeList<V> AStar(INode<V> start, INode<V> end, Func<INode<V>, INode<V>, double> heuristic);
        /// <summary>
        /// Get optimal path with AStar alghoritm
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="heuristic">Heuristic function</param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        INodeList<V> WAStar(INode<V> start, INode<V> end, Func<INode<V>, INode<V>, double> heuristic, double epsilon);
        /// <summary>
        /// Navigate graph with a predicate to flag a list of nodes returned
        /// </summary>
        /// <param name="start"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        INodeList<V> BFS(INode<V> start, Func<INode<V>, bool> predicate);
        /// <summary>
        /// NAvigate graph with DFS, after this you can use IsAcyclic and IsConnected functions
        /// </summary>
        /// <returns></returns>
        IPath<V> DFS();
        /// <summary>
        /// Check if a graph is acyclic
        /// </summary>
        /// <returns></returns>
        bool IsAcyclic();
        /// <summary>
        /// Check if a graph is connected
        /// </summary>
        /// <returns></returns>
        bool IsConnected();
        /// <summary>
        /// Return a topological order
        /// </summary>
        /// <returns></returns>
        ICollection<INode<V>> GetTopologicalSort();
        /// <summary>
        /// Return strong connected components
        /// </summary>
        /// <returns></returns>
        ICollection<IGraph<V>> GetStronglyConnectedComponents();
    }
    /// <summary>
    /// Graph interface
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public interface IGraph<V> : IBaseGraph<V>, IPath<V>
    {

    }
    /// <summary>
    /// Decoration for vertex for path navigation
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public interface IPathDecoration<V>
    {
        /// <summary>
        /// Node's order
        /// </summary>
        ulong Grade { get; }
        /// <summary>
        /// In vertex
        /// </summary>
        ulong In { get; set; }
        /// <summary>
        /// Out vertex
        /// </summary>
        ulong Out { get; set; }
        /// <summary>
        /// Distance between source
        /// </summary>
        ulong Distance { get; set; }
        /// <summary>
        /// Current vertext state
        /// </summary>
        NodeState State { get; set; }
        /// <summary>
        /// Vertex uid parent
        /// </summary>
        IUid AncestorIdentifier { get; set; }
        /// <summary>
        /// Discovered time
        /// </summary>
        ulong Discovered { get; set; }
        /// <summary>
        /// Closed time
        /// </summary>
        ulong Closed { get; set; }
        /// <summary>
        /// Reset node
        /// </summary>
        void Reset();
    }
    /// <summary>
    /// Graph interface
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public interface IBaseGraph<V>
    {
        /// <summary>
        /// Get the trasposted graph
        /// </summary>
        /// <returns></returns>
        IBaseGraph<V> Traspose();
        /// <summary>
        /// Graph node list
        /// </summary>
        INodeList<V> Nodes { get; set; }
        /// <summary>
        /// Add a new vertex to the graph
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        IBaseGraph<V> AddNode(INode<V> v);
        /// <summary>
        /// Remove a vertext from the graph
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        INode<V> RemoveNode(INode<V> v);
        /// <summary>
        /// Remove vertex with predicate for condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        INode<V> RemoveNode(Func<INode<V>, bool> predicate);
        /// <summary>
        /// Return a vertex with an IUid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        INode<V> GetNode(IUid id);
        /// <summary>
        /// Get nodes that satisfy the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        ICollection<INode<V>> GetNodes(Func<INode<V>, bool> predicate);
        /// <summary>
        /// Reset graph
        /// </summary>
        /// <returns></returns>
        IBaseGraph<V> Reset();
    }
    /// <summary>
    /// Unique indentifier for node
    /// </summary>
    public interface IUid : IComparable
    {
        /// <summary>
        /// Unique indentifier for node
        /// </summary>
        long UniqueIdentifier { get; set; }
    }
    /// <summary>
    /// Node interface
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public interface INode<V>
    {
        /// <summary>
        /// Path decoration for node
        /// </summary>
        IPathDecoration<V> PathDecoration { get; set; }
        /// <summary>
        /// Node value
        /// </summary>
        V Value { get; set; }
        /// <summary>
        /// Node identifier
        /// </summary>
        IUid Identifier { get; set; }
        /// <summary>
        /// Node edge list
        /// </summary>
        IEdgeList<V> Edges { get; set; }
        /// <summary>
        /// Add a new edge to the node
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        INode<V> AddEdge(INode<V> edge, double weight = 0);
        /// <summary>
        /// Remove edge from node
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        IEdge<V> RemoveEdge(IEdge<V> edge);
        /// <summary>
        /// Remove edges with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEdge<V> RemoveEdge(Func<IEdge<V>, bool> predicate);
    }
    /// <summary>
    /// Edge interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEdge<T> : IComparable
    {
        /// <summary>
        /// Node weight
        /// </summary>
        double Weight { get; set; }
        /// <summary>
        /// Node
        /// </summary>
        INode<T> Node { get; set; }
    }
    /// <summary>
    /// Node list interface
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public interface INodeList<V> : ICollection<INode<V>>
    {
        /// <summary>
        /// Get a node with uid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        INode<V> GetNode(IUid id);
        /// <summary>
        /// Remove a node
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        INode<V> RemoveNode(INode<V> item);
        /// <summary>
        /// Remove a node with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        INode<V> RemoveNode(Func<INode<V>, bool> predicate);
    }
    /// <summary>
    /// Edge list interface
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public interface IEdgeList<V> : ICollection<IEdge<V>>
    {
        /// <summary>
        /// Return an edge
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEdge<V> GetEdge(IUid id);
        /// <summary>
        /// Remove an edge
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IEdge<V> RemoveEdge(IEdge<V> item);
        /// <summary>
        /// Remove an edge with predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEdge<V> RemoveEdge(Func<IEdge<V>, bool> predicate);
    }
}
