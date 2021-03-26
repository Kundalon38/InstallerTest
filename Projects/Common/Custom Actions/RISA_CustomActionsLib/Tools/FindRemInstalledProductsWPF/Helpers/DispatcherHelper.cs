using System;
using System.Windows;
using System.Windows.Threading;

namespace FindRemInstalledProductsWPF.Helpers
{
    public interface IDispatcherHelper
    {
        void InvokeOnUiThread(Action action);
        void BeginInvokeOnUiThread(Action action);
        void DoEvents();
        void WaitForPriority(DispatcherPriority priority);
    }

    #region DispatcherHelper

    public class DispatcherHelper : IDispatcherHelper
    {
        // invoke via...
        //
        //    DispatcherHelper.InvokeOnUiThread(
        //        delegate()
        //        {
        //            MessageBox.Show("Hello world!");
        //        });
        //
        // Or, the anonymous method format becomes:
        //
        //    DispatcherHelper.InvokeOnUiThread(() => { MessageBox.Show("Hello world!"); });
        //
        // Or, if you prefer:
        //
        //    Action actionToInvokeOnUiThread = () => { MessageBox.Show("Hello world!"); };
        //    DispatcherHelper.InvokeOnUiThread(actionToInvokeOnUiThread);
        //
        public void InvokeOnUiThread(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.Invoke(action);
            }
        }

        public void BeginInvokeOnUiThread(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(action);
            }
        }

        // This causes WPF to wait until operations of the specified priority are reached.
        // It mimics "Application.DoEvents" of VB6 - flushing the message pump.
        // From Bea Costa's blog: http://www.zagstudio.com/blog/493#.VKCQZsDA
        //
        public void DoEvents()
        {
            WaitForPriority(DispatcherPriority.Background);
        }

        public void WaitForPriority(DispatcherPriority priority)
        {
            var frame = new DispatcherFrame();
            var dispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, new DispatcherOperationCallback(exitFrameOperation), frame);
            Dispatcher.PushFrame(frame);
            if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
            {
                dispatcherOperation.Abort();
            }
        }

        private object exitFrameOperation(object obj)
        {
            ((DispatcherFrame)obj).Continue = false;
            return null;
        }
    }

    #endregion // DispatcherHelper

    #region TestDispatcherHelper

    public class TestDispatcherHelper : IDispatcherHelper
    {
        public void InvokeOnUiThread(Action action)
        {
            action();
        }

        public void BeginInvokeOnUiThread(Action action)
        {
            action();
        }

        // This causes WPF to wait until operations of the specified priority are reached.
        // It mimics "Application.DoEvents" of VB6 - flushing the message pump.
        // From Bea Costa's blog: http://www.zagstudio.com/blog/493#.VKCQZsDA
        //
        public void DoEvents()
        {
            return;
        }

        public void WaitForPriority(DispatcherPriority priority)
        {
            return;
        }
    }

    #endregion // TestDispatcherHelper
}
