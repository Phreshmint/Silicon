using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Silicon.Graph;

namespace Silicon.NET {
	public partial class Network<T> : Silicon.Network<T> {
		public override void Load(DirectedGraph<NodeDescription, EdgeDescription<T>> graph) {
			throw new NotImplementedException();
		}
	}
}
