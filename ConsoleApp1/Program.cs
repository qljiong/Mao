using ConsoleApp1.shiti;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.IO;

namespace ConsoleApp1
{
    public class Program
    {
        private const string DbFile = "firstProgram.db";

        public static void Main(string[] args)
        {
            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                // populate the database
                using (var transaction = session.BeginTransaction())
                {
                    Employee employee = new Employee()
                    {
                        Name = "ceshi2",
                        Age = 18

                    };
                    Employee ceshi3 = new Employee()
                    {
                        Name = "ceshi3",
                        Age = 21

                    };
                    Employee ceshi4 = new Employee()
                    {
                        Name = "ceshi4",
                        Age = 25

                    };
                    Employee ceshi5 = new Employee()
                    {
                        Name = "ceshi5",
                        Age = 25

                    };

                    session.SaveOrUpdate(employee);
                    session.SaveOrUpdate(ceshi3);
                    session.SaveOrUpdate(ceshi4);
                    session.SaveOrUpdate(ceshi5);

                    transaction.Commit();
                }
            }

            using (var session = sessionFactory.OpenSession())
            {
                // retreive all stores and display them
                using (session.BeginTransaction())
                {
                    var Employees = session.CreateCriteria(typeof(Employee))
                        .List<Employee>();

                    foreach (var emp in Employees)
                    {
                        Console.WriteLine(emp.Name);
                    }
                    
                }
            }

            Console.ReadKey();
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database(SQLiteConfiguration.Standard
                    .UsingFile(DbFile))
                .Mappings(m =>
                    m.FluentMappings.AddFromAssemblyOf<Program>())
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        private static void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (File.Exists(DbFile))
                File.Delete(DbFile);

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
                .Create(false, true);
        }
    }
}
