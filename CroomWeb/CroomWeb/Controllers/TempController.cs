
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
        /// <summary>
        /// Get all readings from Database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Readings> GetAllReads()
        {
            //get data from database.
            return new DataConnection(@"Server=(localdb)\MSSQLLocalDB;").Read();
        }

        //Method for later used.
        public IHttpActionResult GetReads(int id)
        {

            return NotFound();
            //  return Ok(READING);
        }
    }
}
