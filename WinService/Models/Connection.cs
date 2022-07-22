using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WinService.Models
{
    public class Connection
    {
        public static IDbConnection conn = new SqlConnection(@"Data Source=HASANPC;Initial Catalog=FirmaYonetim;Integrated Security=true;");
    }
}