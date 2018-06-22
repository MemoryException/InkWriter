using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Utilities;

namespace Wpf
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            Safeguard.EnsureNotNull("propertyExpression", propertyExpression);

            MemberExpression memberExpression = null;
            if (propertyExpression.Body is UnaryExpression unary)
            {
                memberExpression = (MemberExpression)unary.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)propertyExpression.Body;
            }

            string propertyName = memberExpression.Member.Name;
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new InvalidOperationException("Failed to get property name.");
            }

            return propertyName;
        }

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
                throw new InvalidOperationException("Property not found.");
            }
        }

        protected void InvokePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = GetPropertyName(propertyExpression);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
