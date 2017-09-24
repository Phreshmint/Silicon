using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Silicon.Graph {
	public abstract class GraphBase<TNode, TEdge> : IGraph<TNode, TEdge> {
		public GraphBase() : this(false) {

		}
		public GraphBase(bool isReadOnly) {
			IsReadOnly = isReadOnly;

			Nodes = new NodeCollection(this);
			Edges = new EdgeCollection(this);
		}

		public bool IsReadOnly { get; }

		protected abstract int NodeCount { get; }

		protected abstract bool ContainsNode(TNode node);
		protected abstract IEnumerator<TNode> GetNodes();

		protected virtual void AddNode(TNode node) => throw new NotImplementedException();
		protected virtual bool RemoveNode(TNode node) => throw new NotImplementedException();
		protected virtual void ClearNodes() => throw new NotImplementedException();

		public ICollection<TNode> Nodes { get; }
		private class NodeCollection : ICollection<TNode> {
			private GraphBase<TNode, TEdge> _graph;
			public NodeCollection(GraphBase<TNode, TEdge> graph) {
				_graph = graph;
				IsReadOnly = _graph.IsReadOnly;
			}

			public bool IsReadOnly { get; }

			public int Count => _graph.NodeCount;

			public bool Contains(TNode item) => _graph.ContainsNode(item);
			public IEnumerator<TNode> GetEnumerator() => _graph.GetNodes();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public void Add(TNode item) {
				if (IsReadOnly) throw new NotSupportedException();
				_graph.AddNode(item);
			}
			public bool Remove(TNode item) {
				if (IsReadOnly) throw new NotSupportedException();
				return _graph.RemoveNode(item);
			}
			public void Clear() {
				if (IsReadOnly) throw new NotSupportedException();
				_graph.ClearNodes();
			}

			public void CopyTo(TNode[] array, int arrayIndex) {
				if (array == null)
					throw new ArgumentNullException();
				if (arrayIndex < 0)
					throw new ArgumentOutOfRangeException();
				if (Count > array.Length + arrayIndex)
					throw new ArgumentException();
				foreach (var vertex in this) {
					array[arrayIndex++] = vertex;
				}
			}
		}

		protected abstract int EdgeCount { get; }

		protected abstract bool ContainsEdge(TNode source, TNode target);
		protected abstract IEnumerator<(TNode source, TNode target)> GetEdges();

		protected virtual void AddEdge(TNode source, TNode target) => throw new NotImplementedException();
		protected virtual bool RemoveEdge(TNode source, TNode target) => throw new NotImplementedException();
		protected virtual void ClearEdges() => throw new NotImplementedException();

		public ICollection<(TNode source, TNode target)> Edges { get; }
		private class EdgeCollection : ICollection<(TNode source, TNode target)> {
			private GraphBase<TNode, TEdge> _graph;
			public EdgeCollection(GraphBase<TNode, TEdge> graph) {
				_graph = graph;
				IsReadOnly = _graph.IsReadOnly;
			}

			public bool IsReadOnly { get; }

			public int Count => _graph.EdgeCount;

			public bool Contains((TNode source, TNode target) item) => _graph.ContainsEdge(item.source, item.target);
			public IEnumerator<(TNode source, TNode target)> GetEnumerator() => _graph.GetEdges();
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public void Add((TNode source, TNode target) item) {
				if (IsReadOnly) throw new NotSupportedException();
				_graph.AddEdge(item.source, item.target);
			}
			public bool Remove((TNode source, TNode target) item) {
				if (IsReadOnly) throw new NotSupportedException();
				return _graph.RemoveEdge(item.source, item.target);
			}
			public void Clear() {
				if (IsReadOnly) throw new NotSupportedException();
				_graph.ClearEdges();
			}

			public void CopyTo((TNode source, TNode target)[] array, int arrayIndex) {
				if (array == null)
					throw new ArgumentNullException();
				if (arrayIndex < 0)
					throw new ArgumentOutOfRangeException();
				if (Count > array.Length + arrayIndex)
					throw new ArgumentException();
				foreach (var edge in this) {
					array[arrayIndex++] = edge;
				}
			}
		}

		public abstract TEdge this[TNode tail, TNode head] { get; set; }
		public abstract IEnumerable<TNode> Neighbors(TNode node);
	}

}
