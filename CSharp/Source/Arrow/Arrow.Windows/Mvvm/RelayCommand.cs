using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Arrow.Windows.Mvvm
{
	/// <summary>
	/// Relays a command to another oject
	/// </summary>
	public class RelayCommand : ICommand
	{
		private readonly Action<object> m_Execute;
        private readonly Predicate<object> m_CanExecute;
        
        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="execute">The command to execute</param>
        public RelayCommand(Action<object> execute) : this(execute,null)
        {
        }
        
        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="execute">The command to execute</param>
        /// <param name="canExecute">Determines if Execute can be called</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
			if(execute==null) throw new ArgumentNullException("execute");
			
			m_Execute=execute;
			m_CanExecute=canExecute;
        }
	
		#region ICommand Members

		/// <summary>
		/// Determines is a command can execute
		/// </summary>
		/// <param name="parameter">Data used by the command</param>
		/// <returns>true if the command can execute, false if not</returns>
		public bool CanExecute(object parameter)
		{
			return m_CanExecute==null ? true : m_CanExecute(parameter);
		}

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged
        {
            add{CommandManager.RequerySuggested+=value;}
            remove{CommandManager.RequerySuggested-=value;}
        }

		/// <summary>
		/// Executes the command
		/// </summary>
		/// <param name="parameter">Data used by the command</param>
		public void Execute(object parameter)
		{
			m_Execute(parameter);
		}

		#endregion
	}
}
