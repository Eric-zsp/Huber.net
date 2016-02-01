
using Huber.ComponentsTemplate.Models;
using Huber.Kernel;
using Huber.Kernel.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Huber.ComponentsTemplate.Controllers
{
    public class DemoController : HuberController
    {
      
        //
        // GET: /Demo/

        public RefRespondEntity Index(RefRequestEntity param)
        {
            RefRespondEntity result = new RefRespondEntity(RespondType._Redirect);
            result.ResultContext = HuberVariable.CurWebUrl+ "/Demo/Index2";
            return result;
        }
        public RefRespondEntity Index2(RefRequestEntity param)
        {
          var o1=  param.Request["qq"];
          var o2 = param.Request["addr"];
            RefRespondEntity result = new RefRespondEntity(RespondType._String);
            var M = new TestM(){A="aaa",B="gffff",C="fff"};
            M.A = HuberVariable.CurWebDir;
            ViewBag.abc = "ccc";
            ViewBag.test = "my test result";
            result.ResultContext = View("Index2", M);
            return result;
        }

        public RefRespondEntity Index3(RefRequestEntity param)
        {
            RefRespondEntity result = new RefRespondEntity(RespondType._String);
            result.ResultContext = "我是INDEX3乱码";
            return result;
        }

        public RefRespondEntity Index4(RefRequestEntity param)
        {

             object AA = param.Request["A"];
             object BB = param.Request["B"];
             object CC = param.Request["C"];

            RefRespondEntity result = new RefRespondEntity(RespondType._String);
            result.ResultContext = View();
            object tt =   ViewBag.test;
            return result;
        }
        public string geta()
        {

            return "geta"+DateTime.Now.ToString();

        }
           
            
    }
}
