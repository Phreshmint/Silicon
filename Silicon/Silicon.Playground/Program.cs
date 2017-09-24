using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Silicon.Playground {
	public class DelegateHelper {
		private static Dictionary<Type, DelegateHelper> _helpers = new Dictionary<Type, DelegateHelper>();

		public static DelegateHelper Get(Type type) {
			if (!_helpers.TryGetValue(type, out var helper)) {
				_helpers.Add(type, helper = new DelegateHelper(type));
			}
			return helper;
		}

		private DelegateHelper(Type type) {
			InitializeFromDelegateType(type);
			Type = type;
		}
		
		private void InitializeFromDelegateType(Type type) {
			var method = type.GetMethod("Invoke") ?? throw new ArgumentException();

			var inputMap = new List<int>();
			var outputMap = new List<int>();

			var parameters = 0;

			if (method.ReturnType != typeof(void)) {
				outputMap.Add(parameters++);
			}

			foreach (var parameter in method.GetParameters()) {
				if (!parameter.ParameterType.IsByRef) {
					// if the type is not a by ref type it must be input
					inputMap.Add(parameters);
				}
				else {
					// if the type is a by ref type it must at least be output
					outputMap.Add(parameters);
					if (!parameter.Attributes.HasFlag(ParameterAttributes.Out)) {
						// if the parameter is not out then it is ref
						inputMap.Add(parameters);
					}
				}
				parameters++;
			}

			Parameters = parameters;

			_inputs = inputMap.ToArray();
			_outputs = outputMap.ToArray();
		}

		public Type Type { get; }

		public int Parameters { get; private set; }

		private int[] _inputs;
		public int Inputs => _inputs.Length;
		public int GetInputIndex(int input) => _inputs[input];

		private int[] _outputs;
		public int Outputs => _outputs.Length;
		public int GetOutputIndex(int output) => _outputs[output];		
	}

	public class DelegateHelper<TDelegate> {
		private static DelegateHelper _helper;

		static DelegateHelper() {
			_helper = DelegateHelper.Get(typeof(TDelegate));
		}

		public static int Parameters => _helper.Parameters;
		
		public static int Inputs => _helper.Inputs;
		public static int GetInputIndex(int input) => _helper.GetInputIndex(input);
		
		public static int Outputs => _helper.Outputs;
		public static int GetOutputIndex(int output) => _helper.GetOutputIndex(output);
	}
	
	public interface IGenericDelegate<T> {
		int Parameters { get; }

		ref T Input(int input);
		int Inputs { get; }

		ref T Output(int output);
		int Outputs { get; }

		void Invoke();
	}

	public class GenericDelegate<T> : IGenericDelegate<T> {
		private DelegateHelper _helper;

		private T[] _parameters;
		private Action<T[]> _action;

		public GenericDelegate(Type type, LambdaExpression lambda) {
			_helper = DelegateHelper.Get(type);
			var expression = BuildActionExpression(lambda);
			_action = expression.Compile();

			_parameters = new T[_helper.Parameters];
		}

		public int Parameters => _helper.Parameters;

		public int Inputs => _helper.Inputs;
		public ref T Input(int input) => ref _parameters[_helper.GetInputIndex(input)];
		
		public int Outputs => _helper.Outputs;
		public ref T Output(int output) => ref _parameters[_helper.GetOutputIndex(output)];
		
		public void Invoke() => _action(_parameters);

		private Expression<Action<T[]>> BuildActionExpression(LambdaExpression lambda) {
			var expressions = new List<Expression>();

			var parameters = lambda.Parameters;
	
			var parametersParameter = Expression.Parameter(typeof(T).MakeArrayType(), "parameters");
			
			for (var input = 0; input < _helper.Inputs; input++) {
				var index = _helper.GetInputIndex(input);

				// in_i = parameters[i]
				var arrayAccess = Expression.ArrayAccess(parametersParameter, Expression.Constant(index));
				expressions.Add(Expression.Assign(parameters[index], arrayAccess));
			}

			var output = 0;

			if (lambda.ReturnType == typeof(void)) {
				expressions.Add(lambda.Body);
			}
			else {
				var arrayAccess = Expression.ArrayAccess(parametersParameter, Expression.Constant(_helper.GetOutputIndex(output)));
				expressions.Add(Expression.Assign(arrayAccess, lambda.Body));
			}

			for (output = 1; output < _helper.Outputs; output++) {
				var index = _helper.GetOutputIndex(output);

				// parameters[i] = out_i
				var arrayAccess = Expression.ArrayAccess(parametersParameter, Expression.Constant(index));
				expressions.Add(Expression.Assign(arrayAccess, parameters[index]));
			}

			expressions.Add(Expression.Empty());

			var newBody = Expression.Block(
				parameters,
				expressions
			);

			return Expression.Lambda<Action<T[]>>(newBody, parametersParameter);
		}
	}
	
	class Program {
		delegate T Foo<T>(out T out1, out T out2, ref T inout1, T in1, T in2);

		static Expression<Func<T, T, T>> BuildBinaryAddLambdaExpression<T>() {
			var a = Expression.Parameter(typeof(T), "a");
			var b = Expression.Parameter(typeof(T), "b");

			var body = Expression.Block(
				Expression.Add(a, b)
			);


			return Expression.Lambda<Func<T, T, T>>(body, a, b);
		}

		static Expression<Foo<T>> BuildFooLambdaExpression<T>() {
			return Expression.Lambda<Foo<T>>(
				Expression.Default(typeof(T)),
				Expression.Parameter(typeof(T).MakeByRefType()),
				Expression.Parameter(typeof(T).MakeByRefType()),
				Expression.Parameter(typeof(T).MakeByRefType()),
				Expression.Parameter(typeof(T)),
				Expression.Parameter(typeof(T))
			);
		}

		static void Main(string[] args) {
			var lambda = BuildBinaryAddLambdaExpression<int>();
			var f = new GenericDelegate<int>(typeof(Func<int, int, int>), lambda);

			f.Input(0) = 10;
			f.Input(1) = 11;

			f.Invoke();

			var output = f.Output(0);
		}

		delegate void Bar<T>(ref T a);
		// inputs = 1

		delegate void Baz<T>(out T a);

	}
}
