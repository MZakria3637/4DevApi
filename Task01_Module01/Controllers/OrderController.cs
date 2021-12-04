using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Task01_Module01.Models;

namespace Task01_Module01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                    select ShippingArea, DeliveryCharges, DeliveryDays, DeliveryAddress from dbo.OrderDescription";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ModuleAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }


        [HttpPost]
        public JsonResult Post(OrderDesc sts)
        {
            string query = @"
                    insert into dbo.OrderDescription(ShippingArea, DeliveryCharges, DeliveryDays, DeliveryAddress) values 
                    ('" + sts.ShippingArea + @"'
                    ,'" + sts.DeliveryCharges + @"'
                    ,'" + sts.DeliveryDays + @"'
                    ,'" + sts.DeliveryAddress + @"'
                    )
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ModuleAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Added Successfully");
        }

    }

}
