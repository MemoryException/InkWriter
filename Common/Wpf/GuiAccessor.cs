using System;
using System.Windows.Threading;
using Utilities;

namespace Wpf
{
    public static class GuiAccessor
    {
        public static void InvokeOnGui(this DispatcherObject application, DispatcherPriority priority, Action action)
        {
            Safeguard.EnsureNotNull("application", application);
            Safeguard.EnsureNotNull("action", action);

            Dispatcher dispatcher = application.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(priority, action);
            }
        }

        public static void InvokeOnGui(this DispatcherObject application, Action action)
        {
            InvokeOnGui(application, DispatcherPriority.Normal, action);
        }
    }
}
