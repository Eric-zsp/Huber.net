using Huber.Framework.Bll;
using Huber.Kernel.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Huber.FrameworkView.App_Code
{
    public class CheckSysAdmin : ActionFilterAttribute
    {



        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (new UserBll().getCurUser().Uid == UserBll.SuperAdminID)
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {

                ContentResult myresult = new ContentResult();
                myresult.Content = ResponseCodeEntity.NoLogin;
                filterContext.Result = myresult;

            }


        }

    }
}
