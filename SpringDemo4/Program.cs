using Spring.Context;
using Spring.Context.Support;
using System;

namespace SpringDemo4
{
    // 构造函数依赖注入示例

    public interface IUserDAL
    {
        void Show();
    }

    public class UserDAL : IUserDAL
    {
        public string Name { get; set; }

        public UserDAL(string name)
        {
            Name = name;
        }

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
            IUserDAL userDAL = context.GetObject<IUserDAL>("UserDAL");

            //调用方法
            userDAL.Show();

            Console.WriteLine("spring 构造函数依赖注入调用完成");
            Console.ReadLine();
        }
    }
}
