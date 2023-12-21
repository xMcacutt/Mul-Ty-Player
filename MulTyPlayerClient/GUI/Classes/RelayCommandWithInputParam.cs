using System;
using System.Windows.Input;

namespace MulTyPlayerClient;

/// <summary>
///     A basic command that runs an Action
/// </summary>
public class RelayCommandWithInputParam : ICommand
{
    #region Private Members

    /// <summary>
    ///     The action to run
    /// </summary>
    private readonly Action<object> mAction;

    #endregion

    #region Constructor

    /// <summary>
    ///     Default constructor
    /// </summary>
    public RelayCommandWithInputParam(Action<object> action)
    {
        mAction = action;
    }

    #endregion

    #region Public Events

    /// <summary>
    ///     The event thats fired when the <see cref="CanExecute(object)" /> value has changed
    /// </summary>
    public event EventHandler CanExecuteChanged = (sender, e) => { };

    #endregion

    #region Command Methods

    /// <summary>
    ///     A relay command can always execute
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object parameter)
    {
        return true;
    }

    /// <summary>
    ///     Executes the commands Action
    /// </summary>
    /// <param name="parameter"></param>
    public void Execute(object parameter)
    {
        mAction(parameter);
    }

    #endregion
}