using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Arrow.Windows.Mvvm
{
	/// <summary>
	/// A view model for a command with an associated name
	/// </summary>
	public class CommandViewModel : ViewModelBase
	{
		private ICommand m_Command;
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="displayName">The displayable name for the command</param>
		/// <param name="command">The command to execute</param>
		public CommandViewModel(string displayName, ICommand command)
		{
			if(command==null) throw new ArgumentNullException("command");
		
			this.DisplayName=displayName;
			m_Command=command;
		}
				
		/// <summary>
		/// The actual command
		/// </summary>
		public ICommand Command
		{
			get{return m_Command;}
		}
	}
}
