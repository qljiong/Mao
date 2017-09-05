using System;
using System.IO;
using System.Reflection;

namespace Mao.Infrastructure.Util
{
    public static class ResourceUtil
    {
        private static readonly Assembly _Assembly = typeof(ResourceUtil).Assembly;
        public static Assembly DefAssembly
        {
            get { return _Assembly; }
        }

        public static Stream GetResourceStream(string res, Type t)
        {
            return GetResourceStream(res, t.Assembly);
        }

        public static Stream GetResourceStream(string res, Assembly asm = null)
        {
            asm = asm ?? DefAssembly;
            var ns = asm.FullName.Split(new[] { ',' })[0];
            var stream = asm.GetManifestResourceStream(res) ?? asm.GetManifestResourceStream(ns + "." + res);

            if (stream == null)
                throw new FileNotFoundException(res);

            return stream;
        }
    }
}
