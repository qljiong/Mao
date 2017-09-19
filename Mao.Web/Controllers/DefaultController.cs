using Mao.Core;
using Mao.Core.AppService.Public;
using Mao.Core.Util;
using Mao.Models;
using System;
using System.Web.Mvc;

namespace Mao.Web.Controllers
{
    public class DefaultController : Controller
    {
        private EmployeeData _employeeData;

        public DefaultController()
        {
            _employeeData = new EmployeeData();
        }

        // GET: Default
        public ActionResult Index()
        {
            var Emp = new Employee()
            {
                FirstName = "123",
                LastName = "ddd",
                Store = "ceshi"
            };
            var result = _employeeData.AddEmployee(Emp);
            ViewBag.result = result;
            return View();
        }

        public ActionResult GetList()
        {
            var dmeo1 = TransactionUtil.Get<IEmployeeQueryService>();
            //var demo2 = DependencyInjectUtil.Resolve<IEmployeeQueryService>();
            return View();
        }
    }
}