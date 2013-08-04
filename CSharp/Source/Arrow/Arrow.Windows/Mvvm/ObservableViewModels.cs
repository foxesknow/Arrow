using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Arrow.Windows.Mvvm
{
	/// <summary>
	/// Groups a number of models together as a view model.
	/// This is useful if you have existing view models that
	/// you wish to group together in a dialog, for example
	/// as a series of tabs
	/// </summary>
	public class ObservableViewModels : ViewModelBase
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public ObservableViewModels() : this(null)
		{
		}
	
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="displayName">A user readable name for the view model</param>
		public ObservableViewModels(string displayName) : base(displayName)
		{
			this.Models=new ObservableCollection<ViewModelBase>();
		}
		
		/// <summary>
		/// The models held by the instance
		/// </summary>
		public ObservableCollection<ViewModelBase> Models{get;private set;}
	}
}
