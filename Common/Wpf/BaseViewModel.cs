using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Wpf
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Gets the name of the property.</summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="propertyExpression">The lambda expression of the property.</param>
        /// <returns>The property name</returns>
        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression", "The property expression is null");
            }

            // Note: UnaryExpression is used to work with value types.
            MemberExpression memberExpression = (MemberExpression)((propertyExpression.Body is UnaryExpression)
                  ? ((UnaryExpression)propertyExpression.Body).Operand : propertyExpression.Body);
            string name = memberExpression.Member.Name;
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Failed to get the property name ", "propertyExpression");
            }

            return name;
        }

        /// <summary>Invokes the property change event.</summary>
        /// <param name="propertyName">Name of the property to change.</param>
        protected void InvokePropertyChanged(string propertyName)
        {
            int propertyCount = (from properties in
                                    this.GetType().GetProperties().AsParallel()
                                 where properties.Name == propertyName
                                 select properties).Count();
            if (propertyCount == 1)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                   "propertyName",
                   "BaseViewModel.InvokeNotifyPropertyChange: Property " + propertyName + " not exist");
            }
        }

        /// <summary>Invokes the notify property change.</summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        protected void InvokePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = GetPropertyName(propertyExpression);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
