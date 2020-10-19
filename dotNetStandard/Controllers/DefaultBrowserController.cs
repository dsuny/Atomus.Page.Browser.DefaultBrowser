using Atomus.Database;
using Atomus.Page.Browser.Models;
using Atomus.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Atomus.Page.Browser.Controllers
{
    internal static class DefaultBrowserController
    {
        internal static IResponse SearchOpenControl(this ICore core, DefaultBrowserSearchModel search)
        {
            IServiceDataSet serviceDataSet;

            serviceDataSet = new ServiceDataSet
            {
                ServiceName = core.GetAttribute("ServiceName"),
                TransactionScope = false
            };
            serviceDataSet["OpenControl"].ConnectionName = core.GetAttribute("DatabaseName");
            serviceDataSet["OpenControl"].CommandText = core.GetAttribute("ProcedureMenuSelect");
            serviceDataSet["OpenControl"].AddParameter("@MENU_ID", DbType.Decimal, 18);
            serviceDataSet["OpenControl"].AddParameter("@ASSEMBLY_ID", DbType.Decimal, 18);
            serviceDataSet["OpenControl"].AddParameter("@USER_ID", DbType.Decimal, 18);

            serviceDataSet["OpenControl"].NewRow();
            serviceDataSet["OpenControl"].SetValue("@MENU_ID", search.MENU_ID);
            serviceDataSet["OpenControl"].SetValue("@ASSEMBLY_ID", search.ASSEMBLY_ID);
            serviceDataSet["OpenControl"].SetValue("@USER_ID", Config.Client.GetAttribute("Account.USER_ID"));

            return core.ServiceRequest(serviceDataSet);
        }
    }
}
