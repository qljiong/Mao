using Spring.Context;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpringDemo3
{
    // 复杂属性注入示例

    public class UserDAL
    {
        public string Name { get; set; }
        public void Show()
        {
            Console.WriteLine($"This name is：{Name}");
        }
    }

    public class UserServer
    {
        public UserDAL userDAL { get; set; }
        public void Show()
        {
            Console.WriteLine($"This is UserServer invoke:{userDAL.Name}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //初始化
            IApplicationContext context = ContextRegistry.GetContext();

            //创建UserDAL实例
            UserServer userDAL = context.GetObject("UserServer") as UserServer;

            //调用方法
            userDAL.Show();

            Console.WriteLine("spring 复杂属性调用完成");
            Console.ReadLine();
        }
    }
}
