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
        INodeList<V> ShortPathDijkstra(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end);
        INodeList<V> ShortPathBFS(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end);
        INodeList<V> AStar(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end);
        INodeList<V> WAStar(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end);
        INodeList<V> BFS(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end);
        INodeList<V> Dijkstra(INode<INodeList<V>, V> start, INode<INodeList<V>, V> end);
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
        IGraph<V> AddVertex(INode<INodeList<V>, V> v);
        INode<INodeList<V>, V> RemoveVertex(INode<INodeList<V>, V> v);
        INode<INodeList<V>, V> RemoveVertex(Func<INode<INodeList<V>, V>, bool> predicate);
        INode<INodeList<V>, V> GetVertex(IUid id);
        INode<INodeList<V>, V> GetVertex(Func<INode<INodeList<V>, V>, bool> predicate);
        IGraph<V> Reset();
    }
    public interface IUid : IComparable
    {
        long UniqueIdentifier { get; set; }
    }
    public interface INode<T, V> where T : INodeList<V> 
    {
        IPathDecoration<V> PathDecoration { get; set; }
        V Value { get; set; }
        IUid Identifier { get; set; }
        INodeList<V> Edges { get; set; }
        INode<T, V> AddEdge(INode<T, V> edge);
        INode<T, V> RemoveEdge(INode<T, V> edge);
        INode<T, V> RemoveEdge(Func<INode<T, V>, bool> predicate);
    }

    public interface IEdge<T> where T: INode<INodeList<T>, T>
    {
        double Weight { get; set; }
        T Node { get; set; }
    }

    public interface INodeList<V> : ICollection<INode<INodeList<V>,V>>
    {
        INode<INodeList<V>, V> GetNode(IUid id);
        INode<INodeList<V>, V> RemoveNode(INode<INodeList<V>, V> item);
        INode<INodeList<V>, V> RemoveNode(Func<INode<INodeList<V>, V>, bool> predicate);
    }



}
