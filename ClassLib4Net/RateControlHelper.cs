using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLib4Net
{
    public class RateControlHelper
    {
        /// <summary>
        /// 线程休眠毫秒数
        /// </summary>
        private int _SleepMsec = 1000;
        /// <summary>
        /// 线程当前应休眠的毫秒数
        /// </summary>
        /// <returns></returns>
        public int SleepMsec
        {
            get
            {
                return _SleepMsec;
            }
        }
        /// <summary>
        /// 线程最大休眠毫秒数
        /// </summary>
        private int _MaxSleepMsec = 2 * 60 * 1000;
        /// <summary>
        /// 线程最小休眠毫秒数
        /// </summary>
        private int _MinSleepMsec = 1000;
        /// <summary>
        /// 线程休眠时间增加的步长
        /// </summary>
        private int _SleepStep = 10 * 1000;
        #region 构造函数：指定步长、最大值、最小值或都使用默认值
        public RateControlHelper()
        {
        }

        public RateControlHelper(int SleepStep)
        {
            _SleepStep = SleepStep;
        }

        public RateControlHelper(int MaxSleepMsec, int MinSleepMsec)
        {
            _MaxSleepMsec = MaxSleepMsec;
            _MinSleepMsec = MinSleepMsec;
            if (_MaxSleepMsec <= _MinSleepMsec) _MaxSleepMsec = _MinSleepMsec + 5 * 60 * 1000;
        }

        public RateControlHelper(int SleepStep, int MaxSleepMsec, int MinSleepMsec)
        {
            _SleepStep = SleepStep;
            _MaxSleepMsec = MaxSleepMsec;
            _MinSleepMsec = MinSleepMsec;
            if (_MaxSleepMsec <= _MinSleepMsec) _MaxSleepMsec = _MinSleepMsec + 5 * 60 * 1000;
        }
        #endregion
        /// <summary>
        /// 检查当前休眠毫秒数是否超出阈值范围
        /// </summary>
        private void Corrector()
        {
            if (_SleepMsec > _MaxSleepMsec)
            {
                _SleepMsec = _MaxSleepMsec;
            }
            else if (_SleepMsec < _MinSleepMsec)
            {
                _SleepMsec = _MinSleepMsec;
            }
        }
        /// <summary>
        /// 任务处理成功时调用的方法，成功时直接将休眠值改成最小，加快正常任务处理速度
        /// </summary>
        /// <returns></returns>
        public virtual int Successful()
        {
            _SleepMsec = _MinSleepMsec;
            return _SleepMsec;
        }
        /// <summary>
        /// 任务处理失败时调用的方法，把休眠时间按步长值增加，减少请求等待被调用方查检
        /// </summary>
        /// <returns></returns>
        public virtual int Error()
        {
            _SleepMsec += _SleepStep;
            Corrector();
            return _SleepMsec;
        }
        /// <summary>
        /// 无任务时调用的方法，无任务则直接将休眠值改成最大值，减小资源消耗
        /// </summary>
        /// <returns></returns>
        public virtual int None()
        {
            _SleepMsec = _MaxSleepMsec;
            return _SleepMsec;
        }
    }
}
