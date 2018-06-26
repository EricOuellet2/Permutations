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
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using HQ.Util.General;
using OxyPlot;
using OxyPlot.Series;
using Permutations.EnumWrapperSimple;

namespace Permutations
{
	public class MainWindowModel : NotifyPropertyChangeBase
	{
		// ************************************************************************
		private int _count = 0;
		private int _countOfElementsToPermute = 12;
		private EnumAction _enumAction;
		private int _testCount = 0;

		private Dictionary<int, List<PermutationResult>> _permutationResults = new Dictionary<int, List<PermutationResult>>();

		// ************************************************************************
		public MainWindowModel()
		{
			EnumAction = EnumAction.Nothing_BestForPerformanceTest;
			AlgoPermutation =
				EnumAlgoPermutation.OuelletHeap |
				EnumAlgoPermutation.SaniSinghHuttunen |
				EnumAlgoPermutation.OuelletHuttunenMT;
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
		private bool DumpValuesFunc(IEnumerable<int> enumValues)
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

			return false;
		}

		// ************************************************************************
		private void DoNothing(IEnumerable<int> enumValues)
		{

		}

		// ************************************************************************
		private bool DoNothingFunc(IEnumerable<int> enumValues)
		{
			return false;
		}

		// ************************************************************************
		private void IncreaseCount(IEnumerable<int> enumValues)
		{
			_count++;
		}

		// ************************************************************************
		private bool IncreaseCountFunc(IEnumerable<int> enumValues)
		{
			_count++;
			return false;
		}

		// ************************************************************************
		private void IncreaseCountSafeInterlockedIncrement(IEnumerable<int> enumValues)
		{
			Interlocked.Increment(ref _count);
		}

		// ************************************************************************
		private bool IncreaseCountSafeInterlockedIncrementFunc(IEnumerable<int> enumValues)
		{
			Interlocked.Increment(ref _count);
			return false;
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
		private bool IncreaseCountSafeSpinLockFunc(IEnumerable<int> enumValues)
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

			return false;
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

		// ************************************************************************
		private PlotModel _plotModel;

		public PlotModel PlotModel
		{
			get { return _plotModel; }
			set
			{
				if (Equals(value, _plotModel)) return;
				_plotModel = value;
				RaisePropertyChanged();
			}
		}

		// ************************************************************************
		private bool _showResultInExcel;

		public bool ShowResultInExcel
		{
			get { return _showResultInExcel; }
			set
			{
				if (value == _showResultInExcel) return;
				_showResultInExcel = value;
				RaisePropertyChanged();
			}
		}
		
		// ************************************************************************
		public void FillTheGraph()
		{
			Mouse.OverrideCursor = Cursors.Wait;

			Task.Run(() =>
			{
				List<EnumAlgoPermutation> sortedEnumAlgoPermutation = null;

				try
				{
					_permutationResults = new Dictionary<int, List<PermutationResult>>();

					int maxCountOfElementToPermute = CountOfElementsToPermute;
					for (CountOfElementsToPermute = 8; CountOfElementsToPermute <= maxCountOfElementToPermute; CountOfElementsToPermute++)
					{
						RunTest();
					}

					if (_permutationResults == null || _permutationResults.Count == 0)
					{
						MessageBox.Show("The count of items should be equals or greater to 8");
						return;
					}

					sortedEnumAlgoPermutation = _permutationResults[maxCountOfElementToPermute].OrderBy(pr => pr.Millisecs).Select(pr => pr.AlgoPermutation).ToList();


					CountOfElementsToPermute = maxCountOfElementToPermute;

					PlotModel plotModel = new PlotModel();
					plotModel.Title = "Millisecs (y) / Item count (x)";
					plotModel.LegendPosition = LegendPosition.LeftTop;

					foreach (EnumAlgoPermutation algoPermutationVal in sortedEnumAlgoPermutation)
					{
						var series = new LineSeries
						{
							Title = EnumUtil.GetEnumDescription(algoPermutationVal),
							MarkerType = MarkerType.Circle,
							MarkerSize = 3,
							StrokeThickness = 1
						};

						foreach (var valueCount in _permutationResults.Keys)
						{
							PermutationResult result = _permutationResults[valueCount].FirstOrDefault(res => res.AlgoPermutation == algoPermutationVal);
							if (result != null)
							{
								series.Points.Add(new DataPoint(valueCount, result.Millisecs));
							}
						}

						plotModel.Series.Add(series);
					}

					PlotModel = plotModel;

					if (ShowResultInExcel)
					{
						CreateExcelFile(_permutationResults, sortedEnumAlgoPermutation);
					}
				}
				finally
				{
					Dispatcher.BeginInvoke(
						DispatcherPriority.ContextIdle,
						new Action(() => Mouse.OverrideCursor = Cursors.Arrow));
				}
			});
		}

		// ************************************************************************
		private void CreateExcelFile(Dictionary<int, List<PermutationResult>> permutationResults, List<EnumAlgoPermutation>  sortedEnumAlgoPermutation)
		{
			var workbook = new XLWorkbook();
			var ws = workbook.Worksheets.Add("Results");

			int row = 1;
			int col = 1;

			int minItemCount = permutationResults.Keys.Min();
			int maxItemCount = permutationResults.Keys.Max();

			Dictionary<int, long> itemCountToSmallestTimeTaken = new Dictionary<int, long>();
			for (int itemCount = minItemCount; itemCount <= maxItemCount; itemCount++)
			{
				itemCountToSmallestTimeTaken.Add(itemCount, permutationResults[itemCount].Min(pr => pr.Millisecs));
			}
			
			// Header
			ws.Cell(row, col).Value = "Algo";
			
			for (int itemCount = minItemCount; itemCount <= maxItemCount; itemCount++)
			{
				col++;
				ws.Cell(row, col).Value = itemCount.ToString() + "!";

				col++;
				ws.Cell(row, col).Value = itemCount.ToString() + "! Ratio";
			}

			//Values
			foreach (EnumAlgoPermutation enumPerm in sortedEnumAlgoPermutation)
			{
				row++;
				col = 1;

				string title = EnumUtil.GetEnumDescription(enumPerm);
				ws.Cell(row, col).Value = title;

				for (int itemCount = minItemCount; itemCount <= maxItemCount; itemCount++)
				{
					foreach (PermutationResult pr in permutationResults[itemCount])
					{
						if (pr.AlgoPermutation == enumPerm)
						{
							col++;

							ws.Cell(row, col).Value = pr.Millisecs;

							//Ratio
							col++;
							double ratio = 0;
							if (itemCountToSmallestTimeTaken[itemCount] > 0)
							{
								 ratio = (double)pr.Millisecs / (double)itemCountToSmallestTimeTaken[itemCount];
							}

							ws.Cell(row, col).Value = ratio;
						}
					}
				}
			}

			string path = System.IO.Path.GetTempFileName() + ".xlsx";
			workbook.SaveAs(path);
			System.Diagnostics.Process.Start(path);
		}

		// ************************************************************************

		//series = new LineSeries { Title = title, MarkerType = markerType, MarkerSize = markerSize, StrokeThickness = strokeTickness };
		//}

		//for (int ptIndex = 0; ptIndex<points.Count; ptIndex++)
		//{
		//series.Points.Add(new DataPoint(points[ptIndex].X, points[ptIndex].Y));

		//	// ******************************************************************
		//	private void AddSeriesLines(IReadOnlyList<Point> points, PlotModel plotModel, string title, MarkerType markerType = MarkerType.Circle, int markerSize = 2, int strokeTickness = 2, OxyColor color = default(OxyColor))
		//	{
		//		if (points != null && points.Count > 0)
		//		{
		//			LineSeries series;
		//			if (title == null)
		//			{
		//				series = new LineSeries { MarkerType = markerType, MarkerSize = markerSize, StrokeThickness = strokeTickness };
		//			}
		//			else
		//			{
		//				series = new LineSeries { Title = title, MarkerType = markerType, MarkerSize = markerSize, StrokeThickness = strokeTickness };
		//			}

		//			for (int ptIndex = 0; ptIndex < points.Count; ptIndex++)
		//			{
		//				series.Points.Add(new DataPoint(points[ptIndex].X, points[ptIndex].Y));
		//			}

		//			if (color != default(OxyColor))
		//			{
		//				series.Color = color;
		//			}

		//			plotModel.Series.Add(series);
		//		}

		// ************************************************************************
		public async Task<int> TestAsync()
		{
			return await Task.Run(() => RunTest());
		}
		// ************************************************************************
		private int RunTest()
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
				Predicate<int[]> funcToDoOnPermutation = null; // Direct function for Sam to be fair (I do not want to add an additional indirection)

				switch (EnumAction)
				{
					case EnumAction.Nothing_BestForPerformanceTest:
						actionToDoOnPermutation = DoNothing;
						funcToDoOnPermutation = DoNothingFunc;
						break;
					case EnumAction.CountItemWithBugWithMultithreadedCode:
						actionToDoOnPermutation = IncreaseCount;
						funcToDoOnPermutation = IncreaseCountFunc;
						break;
					case EnumAction.CountItemSafeInterlockedIncrement:
						actionToDoOnPermutation = IncreaseCountSafeInterlockedIncrement;
						funcToDoOnPermutation = IncreaseCountSafeInterlockedIncrementFunc;
						break;
					case EnumAction.CountItemSafeSpinLock:
						actionToDoOnPermutation = IncreaseCountSafeSpinLock;
						funcToDoOnPermutation = IncreaseCountSafeSpinLockFunc;
						break;
					case EnumAction.DumpPermutatedValuesAndCount:
						actionToDoOnPermutation = DumpValues;
						funcToDoOnPermutation = DumpValuesFunc;

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

				// Eric OuelletHeap Algorithm
				algoPermutationActive = EnumAlgoPermutation.OuelletHeap;
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

				// Eric OuelletHeap indexed Algorithm
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
					for (long i = 0; i < permutationGenerator.MaxIndex; i++)
					{
						permutationGenerator.GetSortedValuesFor(i);
						{
							actionToDoOnPermutation?.Invoke(permutationGenerator.Result);
						}
					}
					stopwatch.Stop();
					WriteResult(algoPermutationActive, stopwatch);
				}


				// Eric OuelletHeap indexed Algorithm v2
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
					for (long i = 0; i < permutationGenerator.MaxIndex; i++)
					{
						permutationGenerator.GetSortedValuesFor(i);
						{
							actionToDoOnPermutation?.Invoke(permutationGenerator.Result);
						}
					}
					stopwatch.Stop();
					WriteResult(algoPermutationActive, stopwatch);

				}

				// Eric OuelletHeap indexed Algorithm v3
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
					for (long i = 0; i < permutationGenerator.MaxIndex; i++)
					{
						permutationGenerator.GetValuesForIndex(i);
						{
							actionToDoOnPermutation?.Invoke(permutationGenerator.Result);
						}
					}
					stopwatch.Stop();
					WriteResult(algoPermutationActive, stopwatch);
				}

				//// Eric OuelletHeap indexed Algorithm v4
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

				algoPermutationActive = EnumAlgoPermutation.OuelletIndexedv3HuttunenST;
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


				algoPermutationActive = EnumAlgoPermutation.Sam;
				if (algoPermutation.HasFlag(algoPermutationActive))
				{
					WriteIntro(algoPermutationActive);
					_count = 0;
					values = GetValues();
					stopwatch.Reset();
					stopwatch.Start();

					var pf = new PermutationFinder<int>();
					pf.Evaluate(values, funcToDoOnPermutation);

					stopwatch.Stop();
					WriteResult(algoPermutationActive, stopwatch);
				}


				algoPermutationActive = EnumAlgoPermutation.Ziezi;
				if (algoPermutation.HasFlag(algoPermutationActive))
				{
					WriteIntro(algoPermutationActive);
					_count = 0;
					values = GetValues();
					stopwatch.Reset();
					stopwatch.Start();

					PermutationZiezi.EnumeratePermutation(values, actionToDoOnPermutation);

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

				algoPermutationActive = EnumAlgoPermutation.OuelletHuttunenMT;
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

		// ************************************************************************
		private void WriteIntro(EnumAlgoPermutation algoPermutationActive)
		{
			string algo = EnumUtil.GetEnumDescription(algoPermutationActive);

			if (EnumAction == EnumAction.DumpPermutatedValuesAndCount)
			{
				WriteLine("------------------------------------------------------------------");
				WriteLine($"Starting algo: {algo}");
			}

			Console.WriteLine($"Starting algo: {algo}"); // require to find some bug in algorithms...
		}

		// ************************************************************************
		private void WriteResult(EnumAlgoPermutation algoPermutationActive, Stopwatch stopWatch)
		{
			List<PermutationResult> permutationResultsPerItemCount = null;
			if (!_permutationResults.TryGetValue(CountOfElementsToPermute, out permutationResultsPerItemCount))
			{
				permutationResultsPerItemCount = new List<PermutationResult>();
				_permutationResults.Add(CountOfElementsToPermute, permutationResultsPerItemCount);
			}

			permutationResultsPerItemCount.Add(new PermutationResult(algoPermutationActive, stopWatch.ElapsedMilliseconds));

			string algo = EnumUtil.GetEnumDescription(algoPermutationActive);

			string warning = null;
			if (_enumAction != EnumAction.Nothing_BestForPerformanceTest && (int)algoPermutationActive >= 65536) // >=65536 = Multithread
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

