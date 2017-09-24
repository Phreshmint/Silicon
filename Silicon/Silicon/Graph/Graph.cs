using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silicon.Graph {
	public interface IGraph<TNode, TEdge> {
		ICollection<TNode> Nodes { get; }
		ICollection<(TNode source, TNode target)> Edges { get; }
		TEdge this[TNode source, TNode target] { get; set; }
		IEnumerable<TNode> Neighbors(TNode node);

		bool IsReadOnly { get; }
	}
}
