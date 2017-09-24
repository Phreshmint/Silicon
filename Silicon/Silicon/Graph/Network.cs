
using Silicon.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Silicon {
    // Network is, essentially a graph with
    // (Action<T[], T[]> action, int inputs, int outputs, int delay)
    // (T value, int outport, int inport)
    
    public abstract class Network<T> {
		public abstract void Load(DirectedGraph<NodeDescription, EdgeDescription<T>> graph);
    }

	public struct NodeDescription {
		public int Inputs { get; }
		public int Outputs { get; }
		public int Delay { get; }
		public LambdaExpression Action { get; }
	}

	public struct EdgeDescription<T> {
		public int Outport { get; }
		public int Inport { get; }
		public T InitialValue { get; }
	}
}
