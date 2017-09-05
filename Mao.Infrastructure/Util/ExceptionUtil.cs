using Mao.Infrastructure.Excep;
using System;

namespace Mao.Infrastructure.Util
{
    public static class ExceptionUtil
    {
        /// <summary>
        /// 取异常的所有文本
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>所有inner异常的文本</returns>
        public static string InnerMessage(this Exception ex)
        {
            string exMsg = string.Format("【{0}】\r\n{1} {2}", _getTypeFullName(ex), _getMsg(ex), ex.StackString());
            Exception innerEx = ex.InnerException;
            while (innerEx != null)
            {
                if (!exMsg.Contains(innerEx.Message))
                {
                    exMsg += string.Format("\r\n【{0}】\r\n{1} {2}", _getTypeFullName(innerEx), _getMsg(innerEx), innerEx.StackString());
                }
                innerEx = innerEx.InnerException;
            }
            return exMsg;
        }

        private static string _getMsg(Exception ex)
        {
            string msg = _isBaseExceptionSubclassOf(ex) ? ((BaseException)ex).Message : ex.Message;
            return msg;
        }

        private static string _getTypeFullName(Exception ex)
        {
            string typeFullName = _isBaseExceptionSubclassOf(ex) ? "Xal-" + ((BaseException)ex).Code + " " + ex.GetType().FullName : ex.GetType().FullName;
            return typeFullName;
        }

        private static bool _isBaseExceptionSubclassOf(Exception ex)
        {
            return ex.GetType().IsSubclassOf(typeof(BaseException));
        }

        /// <summary>
        /// 异常的代码信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>代码信息</returns>
        public static string StackString(this Exception ex)
        {
            string stackStr = (ex == null ? "" : ex.StackTrace);
            return stackStr;
        }
    }
}
