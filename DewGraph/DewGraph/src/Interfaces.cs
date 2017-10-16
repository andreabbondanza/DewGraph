using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DewCore.Abstract.Graph
{
    public enum VertexState
    {
        Unvisited,
        Visited,
        Closed
    }
    public interface IPath<V>
    {
        INodeList<V> ShortPathDijkstra(INode<V> start, INode<V> end);
        INodeList<V> ShortPathBFS(INode<V> start, INode<V> end);
        INodeList<V> AStar(INode<V> start, INode<V> end);
        INodeList<V> WAStar(INode<V> start, INode<V> end);
        INodeList<V> BFS(INode<V> start, INode<V> end);
        INodeList<V> Dijkstra(INode<V> start, INode<V> end);
        IPath<V> DFS();
        bool IsAcyclic();
        bool IsConnected();
    }
    public interface IPathDecoration<V>
    {
        ulong Distance { get; set; }
        VertexState State { get; set; }
        IUid AncestorIdentifier { get; set; }
        uint Discovered { get; set; }
        uint Closed { get; set; }
        void Reset();
    }
    public interface IGraph<V>
    {
        IGraph<V> Traspose();
        INodeList<V> Nodes { get; set; }
        IGraph<V> AddVertex(INode<V> v);
        INode<V> RemoveVertex(INode<V> v);
        INode<V> RemoveVertex(Func<INode<V>, bool> predicate);
        INode<V> GetVertex(IUid id);
        INode<V> GetVertex(Func<INode<V>, bool> predicate);
        IGraph<V> Reset();
    }
    public interface IUid : IComparable
    {
        long UniqueIdentifier { get; set; }
    }
    public interface INode<V>
    {
        IPathDecoration<V> PathDecoration { get; set; }
        V Value { get; set; }
        IUid Identifier { get; set; }
        IEdgeList<V> Edges { get; set; }
        INode<V> AddEdge(INode<V> edge, double weight = 0);
        IEdge<V> RemoveEdge(IEdge<V> edge);
        IEdge<V> RemoveEdge(Func<IEdge<V>, bool> predicate);
    }

    public interface IEdge<T> : IComparable
    {
        double Weight { get; set; }
        INode<T> Node { get; set; }
    }

    public interface INodeList<V> : ICollection<INode<V>>
    {
        INode<V> GetNode(IUid id);
        INode<V> RemoveNode(INode<V> item);
        INode<V> RemoveNode(Func<INode<V>, bool> predicate);
    }
    public interface IEdgeList<V> : ICollection<IEdge<V>>
    {
        IEdge<V> GetEdge(IUid id);
        IEdge<V> RemoveEdge(IEdge<V> item);
        IEdge<V> RemoveEdge(Func<IEdge<V>, bool> predicate);
    }
}
