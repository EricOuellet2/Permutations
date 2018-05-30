using System;
using System.Runtime.InteropServices;
using System.Threading;

public class PermutationSimpleVarUnsafe
{
	private readonly Mutex _mutex = new Mutex();

	private Action<int[]> _action;
	private Action<IntPtr> _actionUnsafe;
	private unsafe int* _arr;
	private IntPtr _arrIntPtr;
	private unsafe int* _last;
	private unsafe int* _lastPrev;
	private unsafe int* _lastPrevPrev;

	public int Size { get; private set; }

	public bool IsRunning()
	{
		return this._mutex.SafeWaitHandle.IsClosed;
	}

	public bool Permutate(int start, int count, Action<int[]> action, bool async = false)
	{
		return this.Permutate(start, count, action, null, async);
	}

	public bool Permutate(int start, int count, Action<IntPtr> actionUnsafe, bool async = false)
	{
		return this.Permutate(start, count, null, actionUnsafe, async);
	}

	private unsafe bool Permutate(int start, int count, Action<int[]> action, Action<IntPtr> actionUnsafe, bool async = false)
	{
		if (!this._mutex.WaitOne(0))
		{
			return false;
		}

		var x = (Action)(() =>
		{
			this._actionUnsafe = actionUnsafe;
			this._action = action;

			this.Size = count;

			this._arr = (int*)Marshal.AllocHGlobal(count * sizeof(int));
			this._arrIntPtr = new IntPtr(this._arr);

			for (var i = 0; i < count - 3; i++)
			{
				this._arr[i] = start + i;
			}

			this._last = this._arr + count - 1;
			this._lastPrev = this._last - 1;
			this._lastPrevPrev = this._lastPrev - 1;

			*this._last = count - 1;
			*this._lastPrev = count - 2;
			*this._lastPrevPrev = count - 3;

			this.Permutate(count, this._arr);
		});

		if (!async)
		{
			x();
		}
		else
		{
			new Thread(() => x()).Start();
		}

		return true;
	}

	private unsafe void Permutate(int size, int* start)
	{
		if (size == 3)
		{
			this.DoAction();
			Swap(this._last, this._lastPrev);
			this.DoAction();
			Swap(this._last, this._lastPrevPrev);
			this.DoAction();
			Swap(this._last, this._lastPrev);
			this.DoAction();
			Swap(this._last, this._lastPrevPrev);
			this.DoAction();
			Swap(this._last, this._lastPrev);
			this.DoAction();

			return;
		}

		var sizeDec = size - 1;
		var startNext = start + 1;
		var usedStarters = 0;

		for (var i = 0; i < sizeDec; i++)
		{
			this.Permutate(sizeDec, startNext);

			usedStarters |= 1 << *start;

			for (var j = startNext; j <= this._last; j++)
			{
				var mask = 1 << *j;

				if ((usedStarters & mask) != mask)
				{
					Swap(start, j);
					break;
				}
			}
		}

		this.Permutate(sizeDec, startNext);

		if (size == this.Size)
		{
			this._mutex.ReleaseMutex();
		}
	}

	private unsafe void DoAction()
	{
		if (this._action == null)
		{
			if (this._actionUnsafe != null)
			{
				this._actionUnsafe(this._arrIntPtr);
			}

			return;
		}

		var result = new int[this.Size];

		fixed (int* pt = result)
		{
			var limit = pt + this.Size;
			var resultPtr = pt;
			var arrayPtr = this._arr;

			while (resultPtr < limit)
			{
				*resultPtr = *arrayPtr;
				resultPtr++;
				arrayPtr++;
			}
		}

		this._action(result);
	}

	private static unsafe void Swap(int* a, int* b)
	{
		var tmp = *a;
		*a = *b;
		*b = tmp;
	}
}