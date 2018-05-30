using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MoreLinq;

namespace Permutations
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// ************************************************************************
		private MainWindowModel Model;

		// ************************************************************************
		public MainWindow()
		{
			InitializeComponent();
			Model = DataContext as MainWindowModel;
			if (Model != null)
			{
				Model.Results.CollectionChanged += ResultsOnCollectionChanged;
			}
		}

		private void ResultsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				ListBoxResults.ScrollIntoView(ListBoxResults.Items[ListBoxResults.Items.Count - 1]);
			}
		}

		// ************************************************************************
		private async void ButtonBaseOnClick(object sender, RoutedEventArgs e)
		{
			await Model.TestAsync();
		}

		// ************************************************************************
		private void ButtonClearResultClick(object sender, RoutedEventArgs e)
		{
			Model.Results.Clear();
		}

		// ************************************************************************
		private void HyperlinkOnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}

		// ************************************************************************

	}
}
