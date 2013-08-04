using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;
using System.Diagnostics;

namespace Arrow.Windows.Mvvm
{
	/// <summary>
	/// Base class for ViewModel instances
	/// </summary>
	public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		/// <summary>
		/// Raised when a property changes
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
	
		private string m_DisplayName;
	
		/// <summary>
		/// Initializes the instance
		/// </summary>
		protected ViewModelBase() : this(null)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="displayName">A user readable name for the view model</param>
		protected ViewModelBase(string displayName)
		{
			this.Dispatcher=Dispatcher.CurrentDispatcher;
			this.DisplayName=displayName;
		}
		
		/// <summary>
		/// A human readable display name for the view model
		/// </summary>
		public string DisplayName
		{
			get{return m_DisplayName;}
			set{if(m_DisplayName!=value) SetAndNotify("DisplayName",ref m_DisplayName,value);}
		}
		
		/// <summary>
		/// Set a variable to a value and raises a PropertyChanged event
		/// </summary>
		/// <typeparam name="T">The type of the variable</typeparam>
		/// <param name="propertyName">The name of the property the target represents</param>
		/// <param name="target">The variable to set</param>
		/// <param name="value">The value to set the target to</param>
		protected void SetAndNotify<T>(string propertyName, ref T target, T value)
		{
			target=value;
			OnPropertyChanged(propertyName);
		}
		
		/// <summary>
		/// Provides a thread dispatcher for the model.
		/// This can be used to call back into the thread the model was created on
		/// </summary>
		protected Dispatcher Dispatcher{get;private set;}
		
		/// <summary>
        /// Warns the developer if this object does not have a public property with the specified name.
        /// This method does not exist in a Release build.
        /// </summary>
		/// <param name="propertyName">The name of the property to check for</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        protected void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if(TypeDescriptor.GetProperties(this)[propertyName]==null)
            {
                string message="Invalid property name: "+propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new InvalidOperationException(message);
                else
                    Debug.Fail(message);
            }
        }
        
        /// <summary>
        /// Indicates wha to do if an invalid property name is pased
		/// to "OnPropertyChanged". If false then "Debug.Fail" is called
        /// </summary>
        protected bool ThrowOnInvalidPropertyName{get;set;}
		
		/// <summary>
		/// Raises the PropertyChanged event
		/// </summary>
		/// <param name="propertyName">The name of the property that has changed</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			VerifyPropertyName(propertyName);
		
			var d=PropertyChanged;
			if(d!=null) d(this,new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Disposes of any resources
		/// </summary>
		public void Dispose()
		{
			OnDispose();
		}

		/// <summary>
		/// Disposes of any resources
		/// </summary>
		protected virtual void OnDispose()
		{
		
		}
	}
}
