
namespace Bench
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Threading;

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
