using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using ProductWebAPI.Models;

namespace ProductWebAPI.Controllers
{
    [Authorize]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        // GET api/values
        //[HttpGet("api/test")]
        ProductEntities dbContext = new ProductEntities();

        [HttpGet]
        [Route("api/Product")]
        public IHttpActionResult Get()
        {
            var result = dbContext.Products.ToList();
            return Ok(result);
        }

        [HttpGet]
        [Route("api/userclaim")]
        public IHttpActionResult GetClaim()
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
            var claims = claimsIdentity.Claims.Select(x => new { type = x.Type, value = x.Value });
            return Ok(claims);
        }

        [HttpGet]
        [Route("api/Product/{id}")]
        public IHttpActionResult Get(int id)
        {
            var result = dbContext.Products.Where(x => x.ProductId == id);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/Product")]
        public IHttpActionResult Post([FromBody]Product reuest)
        {
            string result = string.Empty;
            try
            {
                dbContext.Products.Add(reuest);
                dbContext.SaveChanges();
                result = "Record inserted successfully.";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("api/Product/{id}")]
        public IHttpActionResult Put(int id,[FromBody]Product reuest)
        {
            string result = string.Empty;
            try
            {
                Product p =  dbContext.Products.Where(x => x.ProductId == id).SingleOrDefault();
                dbContext.Entry(p).CurrentValues.SetValues(result);
                dbContext.SaveChanges();
            }
            catch (Exception)
            {
                
                throw;
            }
            return Ok(result);
        }

        [HttpDelete]
        [Route("api/Product/{id}")]
        public IHttpActionResult Delete(int id)
        {
            string result = string.Empty;
            try
            {
                Product p = dbContext.Products.Where(x => x.ProductId == id).SingleOrDefault();
                dbContext.Products.Remove(p);
                dbContext.SaveChanges();
                result = "record deleted successfully";
            }
            catch (Exception ex)
            {
                result = "error while deleting record " + ex.Message;
                
            }
            return Ok(result);
        }
      
    }
}