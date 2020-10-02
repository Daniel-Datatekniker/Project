
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Data.Database;
using Data;
using System.Web.Http.Cors;

namespace CroomWeb.Controllers
{

    public class TempController : ApiController
    {

        public IEnumerable<Readings> GetAllReads()
        {
            DataConnection db = new DataConnection(@"Server=(localdb)\MSSQLLocalDB;");
            //get data from database.

            var test = db.Read();


            return db.Read();
        }

        public IHttpActionResult GetReads(int id)
        {

            return NotFound();
            //  return Ok(READING);
        }
    }
}
