using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HQ.Util.General;
using Permutations.EnumWrapperSimple;

namespace Permutations
{
	public class MainWindowModel : NotifyPropertyChangeBase
	{
		private int _count = 0;
		private int _countOfElementsToPermute = 12;
		private EnumAction _enumAction;

		// ************************************************************************
		public MainWindowModel()
		{
			EnumAction = EnumAction.Nothing_BestForPerformanceTest;
			AlgoPermutation =
				EnumAlgoPermutation.Ouellet |
				EnumAlgoPermutation.SaniSinghHuttunen |
				EnumAlgoPermutation.OuelletIndexedv3SaniSinghHuttunenMT;
			EnumWrapperAlgoPermutation = new EnumWrapperSource(this, nameof(AlgoPermutation));
		}

		// ************************************************************************
		public int CountOfElementsToPermute
		{
			get { return _countOfElementsToPermute; }
			set
			{
				if (value == _countOfElementsToPermute) return;
				_countOfElementsToPermute = value;
				RaisePropertyChanged();
			}
		}

		// ************************************************************************
		public EnumAction EnumAction
		{
			get { return _enumAction; }
			set
			{

				if (value == _enumAction) return;
				_enumAction = value;
				RaisePropertyChanged();
			}
		}

		// ************************************************************************
		private int[] GetValues()
		{
			int[] values = new int[CountOfElementsToPermute];
			for (int n = 0; n < values.Length; n++)
			{
				values[n] = n;
			}

			return values;
		}

		// ************************************************************************
		private void DumpValues(IEnumerable<int> enumValues)
		{
			Interlocked.Increment(ref _count);
			StringBuilder sb = new StringBuilder();
			foreach (int val in enumValues)
			{
				sb.Append(val);
				sb.Append(", ");
			}

			int[] values = enumValues.ToArray();
			Array.Sort(values);
			

			PermutationOuelletLexico3<int> p = new PermutationOuelletLexico3<int>(values);
			long index = p.GetIndexOfValues(enumValues.ToArray());
			sb.Append($", Index: {index}");


			WriteLine(sb.ToString());
		}

		// ************************************************************************
		private void DoNothing(IEnumerable<int> enumValues)
		{

		}

		// ************************************************************************
		private void IncreaseCount(IEnumerable<int> enumValues)
		{
			_count++;
		}

		// ************************************************************************
		private void IncreaseCountSafeInterlockedIncrement(IEnumerable<int> enumValues)
		{
			Interlocked.Increment(ref _count);
		}

		// ************************************************************************
		private void IncreaseCountSafeSpinLock(IEnumerable<int> enumValues)
		{
			var spinLock = new SpinLock();   // Enable owner tracking
			bool lockTaken = false;
			try
			{
				spinLock.Enter(ref lockTaken);
				_count++;
			}
			finally
			{
				if (lockTaken) spinLock.Exit();
			}
		}

		// ************************************************************************
		private object _lockObj = new object();
		private bool _isTestRunning = false;

		public bool IsTestRunning
		{
			get { return _isTestRunning; }
			set
			{
				if (value == _isTestRunning) return;
				_isTestRunning = value;
				RaisePropertyChanged();
			}
		}

		private int _testCount = 0;

		// ************************************************************************
		public async Task<int> TestAsync()
		{
			return await Task.Run(() =>
				{
					try
					{
						bool canRun = false;
						lock (_lockObj)
						{
							if (!IsTestRunning)
							{
								canRun = true;
								IsTestRunning = true;
							}
						}

						if (!canRun)
						{
							MessageBox.Show("Already running test. Please try later.");
							return 0;
						}

						Action<IEnumerable<int>> actionToDoOnPermutation = null;

						switch (EnumAction)
						{
							case EnumAction.Nothing_BestForPerformanceTest:
								actionToDoOnPermutation = DoNothing;
								break;
							case EnumAction.CountItemWithBugWithMultithreadedCode:
								actionToDoOnPermutation = IncreaseCount;
								break;
							case EnumAction.CountItemSafeInterlockedIncrement:
								actionToDoOnPermutation = IncreaseCountSafeInterlockedIncrement;
								break;
							case EnumAction.CountItemSafeSpinLock:
								actionToDoOnPermutation = IncreaseCountSafeSpinLock;
								break;
							case EnumAction.DumpPermutatedValuesAndCount:
								actionToDoOnPermutation = DumpValues;

								if (CountOfElementsToPermute > 5)
								{
									var result = MessageBox.Show(
										"The count of item to permute is greater than 5, it could take long to dump every items, are you sure you want to continue?",
										"Warning",
										MessageBoxButton.OKCancel);
									if (result == MessageBoxResult.Cancel)
									{
										return 0;
									}
								}

								break;
						}

						EnumAlgoPermutation algoPermutation = AlgoPermutation;
						EnumAlgoPermutation algoPermutationActive;

						_testCount++;
						WriteLine($"Test {_testCount} started. Count {CountOfElementsToPermute}!. Action: {EnumUtil.GetEnumDescription(EnumAction)}.");

						int[] values;
						Stopwatch stopwatch = new Stopwatch();

						// Eric Ouellet Algorithm
						algoPermutationActive = EnumAlgoPermutation.Ouellet;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();
							PermutationOuellet.ForAllPermutation(values, (vals) =>
							{
								actionToDoOnPermutation?.Invoke(vals);
								return false;
							});
							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						// Eric Ouellet indexed Algorithm
						algoPermutationActive = EnumAlgoPermutation.OuelletIndexed;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();
							var permutationGenerator = new PermutationOuelletLexico<int>(values);

							// int[] result = new int[values.Length];
							for (int i = 0; i < permutationGenerator.MaxIndex; i++)
							{
								permutationGenerator.GetSortedValuesFor(i);
								{
									actionToDoOnPermutation?.Invoke(permutationGenerator.Result);
								}
							}
							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}


						// Eric Ouellet indexed Algorithm v2
						algoPermutationActive = EnumAlgoPermutation.OuelletIndexedv2;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();
							var permutationGenerator = new PermutationOuelletLexico2<int>(values);

							// int[] result = new int[values.Length];
							for (int i = 0; i < permutationGenerator.MaxIndex; i++)
							{
								permutationGenerator.GetSortedValuesFor(i);
								{
									actionToDoOnPermutation?.Invoke(permutationGenerator.Result);
								}
							}
							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);

						}

						// Eric Ouellet indexed Algorithm v3
						algoPermutationActive = EnumAlgoPermutation.OuelletIndexedv3;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();
							var permutationGenerator = new PermutationOuelletLexico3<int>(values);

							// int[] result = new int[values.Length];
							for (int i = 0; i < permutationGenerator.MaxIndex; i++)
							{
								permutationGenerator.GetValuesForIndex(i);
								{
									actionToDoOnPermutation?.Invoke(permutationGenerator.Result);
								}
							}
							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						//// Eric Ouellet indexed Algorithm v4
						//algoPermutationActive = EnumAlgoPermutation.OuelletIndexedv4;
						//if (algoPermutation.HasFlag(algoPermutationActive))
						//{
						//	_count = 0;
						//	values = GetValues();
						//	stopwatch.Reset();
						//	stopwatch.Start();
						//	var permutationGenerator = new PermutationOuelletLexico4<int>(values);

						//	// int[] result = new int[values.Length];
						//	for (int i = 0; i < permutationGenerator.MaxIndex; i++)
						//	{
						//		permutationGenerator.GetValuesForIndex(i);
						//		{
						//			actionDumpValues?.Invoke(permutationGenerator.Result);
						//		}
						//	}
						//	stopwatch.Stop();
						//	WriteResult(algoPermutationActive, stopwatch);
						//}

						algoPermutationActive = EnumAlgoPermutation.SaniSinghHuttunen;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();
							do
							{
								actionToDoOnPermutation?.Invoke(values);
							}
							while (PermutationSaniSinghHuttunen.NextPermutation(values));

							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						algoPermutationActive = EnumAlgoPermutation.OuelletIndexedv3SaniSinghHuttunenST;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();

							PermutationMixOuelletSaniSinghHuttunen mix = new PermutationMixOuelletSaniSinghHuttunen(values);
							mix.ExecuteForEachPermutation(actionToDoOnPermutation);

							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						algoPermutationActive = EnumAlgoPermutation.SimpleVar;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();

							do
							{
								actionToDoOnPermutation?.Invoke(values);
							}
							while (!PermutationSimpleVar.NextPermutation(values));
							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						algoPermutationActive = EnumAlgoPermutation.SimpleVarUnsafe;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();
							PermutationSimpleVarUnsafe permutations = new PermutationSimpleVarUnsafe();
							permutations.Permutate(0, values.Length, (int[] vals) =>
							{
								actionToDoOnPermutation?.Invoke(vals);
							});
							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						// ErezRobinson Algorithm
						algoPermutationActive = EnumAlgoPermutation.ErezRobinson;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();
							foreach (var vals in PermutationErezRobinson.QuickPerm(values))
							{
								actionToDoOnPermutation?.Invoke(vals);
							}

							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						// Pengyang linq algorithm
						algoPermutationActive = EnumAlgoPermutation.Pengyang;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();
							foreach (var vals in PermutationPengyang.GetPermutations(values, values.Length))
							{
								actionToDoOnPermutation?.Invoke(vals);
							}

							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						algoPermutationActive = EnumAlgoPermutation.OuelletIndexedMT;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();

							int[] result = new int[values.Length];
							Parallel.For<PermutationOuelletLexico<int>>
							(
								0,
								Factorial.GetFactorial(values.Length),
								() => new PermutationOuelletLexico<int>(GetValues()),
								(index, state, permutationOuellet) =>
								{
									permutationOuellet.GetSortedValuesFor(index);
									actionToDoOnPermutation?.Invoke(permutationOuellet.Result);
									return permutationOuellet;
								},
								(permutationOuellet) => { }
							);

							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						algoPermutationActive = EnumAlgoPermutation.OuelletIndexedv3MT;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();

							int[] result = new int[values.Length];
							Parallel.For<PermutationOuelletLexico3<int>>
							(
								0,
								Factorial.GetFactorial(values.Length),
								() => new PermutationOuelletLexico3<int>(GetValues()),
								(index, state, permutationOuellet) =>
								{
									permutationOuellet.GetValuesForIndex(index);
									actionToDoOnPermutation?.Invoke(permutationOuellet.Result);
									return permutationOuellet;
								},
								(permutationOuellet) => { }
							);

							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						algoPermutationActive = EnumAlgoPermutation.OuelletIndexedv3SaniSinghHuttunenMT;
						if (algoPermutation.HasFlag(algoPermutationActive))
						{
							WriteIntro(algoPermutationActive);
							_count = 0;
							values = GetValues();
							stopwatch.Reset();
							stopwatch.Start();

							PermutationMixOuelletSaniSinghHuttunen.ExecuteForEachPermutationMT(values, actionToDoOnPermutation);

							stopwatch.Stop();
							WriteResult(algoPermutationActive, stopwatch);
						}

						WriteLine("Test " + _testCount + " ended.");
						WriteLine("---------------------------------------------------------------");
						return _count;

					}
					finally
					{
						IsTestRunning = false;
					}
				}
			);
		}

		// ************************************************************************
		private void WriteIntro(EnumAlgoPermutation algoPermutationActive)
		{
			string algo = EnumUtil.GetEnumDescription(algoPermutationActive);

			if (EnumAction == EnumAction.DumpPermutatedValuesAndCount)
			{
				WriteLine("------------------------------------------------------------------");
				WriteLine($"Starting algo: {algo}");
			}
		}

		// ************************************************************************
		private void WriteResult(EnumAlgoPermutation algoPermutationActive, Stopwatch stopWatch)
		{
			string algo = EnumUtil.GetEnumDescription(algoPermutationActive);

			string warning = null;
			if (_enumAction != EnumAction.Nothing_BestForPerformanceTest && (int)algoPermutationActive >= 1024) // >=1024 = Multithread
			{
				warning = $", Count: {_count}. Take care, counting element occurs globaly and has big affect on performance because it does 'false sharing' among threads. Do use 'Nothing' to evaluate strictly performance.";
			}

			WriteLine($"Millisecs: {stopWatch.ElapsedMilliseconds} for: {algo}" + warning);
		}

		// ************************************************************************
		private void WriteLine(string line)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				Results.Add(new ResultRow(line));
			}));
		}

		// ************************************************************************
		public ObservableCollection<ResultRow> Results { get; private set; } = new ObservableCollection<ResultRow>();

		// ************************************************************************
		public EnumAlgoPermutation AlgoPermutation { get; set; }

		// ************************************************************************
		public EnumWrapperSource EnumWrapperAlgoPermutation { get; private set; } = null;

		// ************************************************************************

	}
}

