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
        INodeList<V> ShortPathDijkstra(INode<IEdgeList<V>, V> start, INode<IEdgeList<V>, V> end);
        INodeList<V> ShortPathBFS(INode<IEdgeList<V>, V> start, INode<IEdgeList<V>, V> end);
        INodeList<V> AStar(INode<IEdgeList<V>, V> start, INode<IEdgeList<V>, V> end);
        INodeList<V> WAStar(INode<IEdgeList<V>, V> start, INode<IEdgeList<V>, V> end);
        INodeList<V> BFS(INode<IEdgeList<V>, V> start, INode<IEdgeList<V>, V> end);
        INodeList<V> Dijkstra(INode<IEdgeList<V>, V> start, INode<IEdgeList<V>, V> end);
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
        IGraph<V> AddVertex(INode<IEdgeList<V>, V> v);
        INode<IEdgeList<V>, V> RemoveVertex(INode<IEdgeList<V>, V> v);
        INode<IEdgeList<V>, V> RemoveVertex(Func<INode<IEdgeList<V>, V>, bool> predicate);
        INode<IEdgeList<V>, V> GetVertex(IUid id);
        INode<IEdgeList<V>, V> GetVertex(Func<INode<IEdgeList<V>, V>, bool> predicate);
        IGraph<V> Reset();
    }
    public interface IUid : IComparable
    {
        long UniqueIdentifier { get; set; }
    }
    public interface INode<T, V> where T : IEdgeList<V> 
    {
        IPathDecoration<V> PathDecoration { get; set; }
        V Value { get; set; }
        IUid Identifier { get; set; }
        IEdgeList<V> Edges { get; set; }
        INode<T, V> AddEdge(INode<T, V> edge);
        IEdge<INode<T, V>> RemoveEdge(INode<T, V> edge);
        IEdge<INode<T, V>> RemoveEdge(Func<INode<T, V>, bool> predicate);
    }

    public interface IEdge<T> 
    {
        double Weight { get; set; }
        INode<IEdgeList<T>, T> Node { get; set; }
    }

    public interface INodeList<V> : ICollection<INode<IEdgeList<V>,V>>
    {
        INode<IEdgeList<V>, V> GetNode(IUid id);
        INode<IEdgeList<V>, V> RemoveNode(INode<IEdgeList<V>, V> item);
        INode<IEdgeList<V>, V> RemoveNode(Func<INode<IEdgeList<V>, V>, bool> predicate);
    }
    public interface IEdgeList<V> : ICollection<IEdge<INode<IEdgeList<V>,V>>>
    {
        IEdge<INode<IEdgeList<V>, V>> GetEdge(IUid id);
        IEdge<INode<IEdgeList<V>, V>> RemoveEdge(IEdge<INode<IEdgeList<V>, V>> item);
        IEdge<INode<IEdgeList<V>, V>> RemoveEdge(Func<IEdge<INode<IEdgeList<V>, V>>, bool> predicate);
    }


}
