using Mao.Infrastructure.Excep;

namespace Mao.Infrastructure.Util
{
    public class AssertUtilException : BaseException
    {
        public AssertUtilException(string msg) : base(msg)
        {

        }

        public override int Code
        {
            get { return 1000; }
        }

        public override string Description
        {
            get { return "AssertUtil failed"; }
        }
    }
}
