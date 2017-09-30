using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLib4Net
{
    /// <summary>
    /// 异步处理帮助器
    /// </summary>
    /// <typeparam name="T">异步处理传入的类型</typeparam>
    public class AsyncHelper<T>
    {
        /// <summary>
        /// 异步处理委托
        /// </summary>
        /// <param name="actionContent"></param>
        public delegate void AsynActionHandler(T actionContent);

        /// <summary>
        /// 异步处理执行此逻辑
        /// </summary>
        public event AsynActionHandler OnActing;

        /// <summary>
        /// 异步处理执行完成后执行此逻辑
        /// </summary>
        public event AsynActionHandler OnActed;

        /// <summary>
        /// 开始执行异步处理
        /// </summary>
        /// <param name="actionContent"></param>
        public void Act(T actionContent)
        {
            if (this.OnActing != null)
            {
                AsyncCallback callback = new AsyncCallback(this.AsynAction);
                this.OnActing.BeginInvoke(actionContent, callback, actionContent);
            }
        }

        private void AsynAction(IAsyncResult ar)
        {
            if (this.OnActed != null)
            {
                T content = (T)ar.AsyncState;
                this.OnActed.Invoke(content);
            }
        }
    }
}