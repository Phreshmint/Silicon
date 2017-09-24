using System;
using System.Collections.Generic;
using System.Text;

namespace Silicon.Graph {
	public class DirectedGraph<TNode, TEdge> : GraphBase<TNode, TEdge> {
		private IDictionary<TNode, Dictionary<TNode, TEdge>> _nodes;

		public DirectedGraph() : this(EqualityComparer<TNode>.Default) { }
		public DirectedGraph(IEqualityComparer<TNode> comparer) : base(false) {
			NodeComparer = comparer;
			_nodes = new Dictionary<TNode, Dictionary<TNode, TEdge>>(comparer);

		}

		public IEqualityComparer<TNode> NodeComparer { get; }

		protected override int NodeCount => _nodes.Count;

		protected override bool ContainsNode(TNode node) {
			return _nodes.ContainsKey(node);
		}

		protected override IEnumerator<TNode> GetNodes() => _nodes.Keys.GetEnumerator();

		protected override void AddNode(TNode node) {
			_nodes.Add(node, new Dictionary<TNode, TEdge>(NodeComparer));
		}

		protected override bool RemoveNode(TNode node) {
			if (!_nodes.ContainsKey(node))
				return false;
			foreach (var otherNodes in _nodes.Keys) {
				_nodes[otherNodes].Remove(node);
			}
			return _nodes.Remove(node);
		}

		protected override void ClearNodes() {
			_nodes.Clear();
			_edgeCount = 0;
		}

		private int _edgeCount;
		protected override int EdgeCount { get; }

		protected override bool ContainsEdge(TNode tail, TNode head) {
			if (!_nodes.TryGetValue(tail, out var list))
				return false;
			return list.ContainsKey(head);
		}

		protected override IEnumerator<(TNode source, TNode target)> GetEdges() {
			foreach (var source in _nodes.Keys) {
				foreach (var target in _nodes[source].Keys) {
					yield return (source, target);
				}
			}
		}

		protected override void AddEdge(TNode tail, TNode head) {
			_nodes[tail].Add(head, default(TEdge));
			_edgeCount++;
		}

		protected override bool RemoveEdge(TNode tail, TNode head) {
			var removed = _nodes[tail].Remove(head);
			if (removed) _edgeCount--;
			return removed;
		}

		protected override void ClearEdges() {
			foreach (var vertex in _nodes.Keys) {
				_nodes[vertex].Clear();
			}
			_edgeCount = 0;
		}

		public override TEdge this[TNode tail, TNode head] {
			get => _nodes[tail][head];
			set {
				if (!_nodes.TryGetValue(tail, out var list))
					_nodes.Add(tail, list = new Dictionary<TNode, TEdge>(NodeComparer));
				if (!list.ContainsKey(head))
					_edgeCount++;
				list[head] = value;
			}
		}

		public override IEnumerable<TNode> Neighbors(TNode v) {
			if (!_nodes.TryGetValue(v, out var list))
				throw new ArgumentException();
			return list.Keys;
		}
	}
}
