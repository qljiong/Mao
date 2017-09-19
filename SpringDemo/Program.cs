using Spring.Context;
using Spring.Context.Support;
using System;

namespace SpringDemo
{
    // spring 简单调用示例 

    public interface IUserDAL
    {
        void Show();
    }

    public class UserDAL : IUserDAL
    {
        public void Show()
        {
            Console.WriteLine("This is UserDAl");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //初始化
            IApplicationContext context = ContextRegistry.GetContext();

            //创建UserDAL实例
            IUserDAL userDAL = context.GetObject("UserDAL") as IUserDAL;

            //调用方法
            userDAL.Show();

            Console.WriteLine("spring 简单调用完成");
            Console.ReadLine();

        }
    }
}
