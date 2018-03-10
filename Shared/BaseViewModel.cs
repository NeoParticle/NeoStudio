using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NeoStudio.Shared
{
    public class BaseViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Fields

        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        #endregion

        #region Properties

        public Dictionary<string, List<string>> Errors { get; private set; }

        #endregion

        #region Constructors

        public BaseViewModel()
        {
            Errors = new Dictionary<string, List<string>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <returns>True if the PropertyChanged event has been raised,
        /// false otherwise. The event is not raised if the old
        /// value is equal to the new value.</returns>
        protected virtual bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;
            this.RaisePropertyChanged<T>(propertyExpression);
            return true;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        /// <returns>True if the PropertyChanged event has been raised,
        /// false otherwise. The event is not raised if the old
        /// value is equal to the new value.</returns>
        protected virtual bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Extracts the name of a property from an expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">An expression returning the property's name.</param>
        /// <returns>The name of the property returned by the expression.</returns>
        /// <exception cref="T:System.ArgumentNullException">If the expression is null.</exception>
        /// <exception cref="T:System.ArgumentException">If the expression does not represent a property.</exception>
        protected string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            MemberExpression body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Invalid argument", "propertyExpression");
            }
            PropertyInfo member = body.Member as PropertyInfo;
            if (member == null)
            {
                throw new ArgumentException("Argument is not a property", "propertyExpression");
            }
            return member.Name;
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs after a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                string propertyName = GetPropertyName(propertyExpression);
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyDataErrorInfo Members

        /// <summary>
        /// Occurs after a property validation.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Raises the ErrorsChanged event if needed.
        /// </summary>
        /// <param name="propertyName">The name of the property that errors changed.</param>
        protected virtual void RaiseErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the ErrorsChanged event if needed.
        /// </summary>
        /// <typeparam name="T">The type of the property that error changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property that error changed.</param>
        protected virtual void RaiseErrorsChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var eventHandler = ErrorsChanged;
            if (eventHandler != null)
            {
                string propertyName = GetPropertyName(propertyExpression);
                eventHandler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                return _errors[propertyName];
            }
            return null;
        }

        public bool HasErrors
        {
            get { return _errors.Count > 0; }
        }

        #endregion
    }
}
