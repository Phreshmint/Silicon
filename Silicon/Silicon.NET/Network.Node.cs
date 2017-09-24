using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silicon.NET {
	public partial class Network<T> {
		private class Node {
			public T[] Inputs { get; }
		}
	}
}
