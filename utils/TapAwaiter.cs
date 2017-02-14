
namespace Bench
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Diagnostics;
	using System.IO;

	public class TapAwaiter<T> : INotifyCompletion
	{
		private bool isCompleted = true;

		public T Result
		{
			get;
			set;
		}

		public TimeSpan TimeSpan
		{
			get;
			set;
		}

		public bool IsCompleted
		{
			get { return this.isCompleted; }
			set { this.isCompleted = value; }
		}

		public void OnCompleted(Action continuation)
		{
			Thread.Sleep(this.TimeSpan);
			continuation();
		}

		public T GetResult()
		{
			return this.Result;
		}
	}
}
