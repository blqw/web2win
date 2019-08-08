using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace web2win
{
    static class DispatcherExtensions
    {
        public static void Invoke(this DispatcherObject dispatcher, Action action)
        {
            try
            {
                if (dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    dispatcher.Invoke(action);
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("执行代码中,控件可能已释放...");
            }
            catch
            {
                throw;
            }
        }

        public static T Invoke<T>(this DispatcherObject dispatcher, Func<T> func)
        {
            try
            {
                if (dispatcher.CheckAccess())
                {
                    return func();
                }
                else
                {
                    return dispatcher.Dispatcher.Invoke(func);
                }
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("执行代码中,控件可能已释放...");
                return default;
            }
            catch
            {
                throw;
            }
        }
    }
}
