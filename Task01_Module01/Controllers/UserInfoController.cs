using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task01_Module01.Models;

namespace Task01_Module01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _emailSender;
        public UserInfoController(IConfiguration configuration, IEmailSender emailSender, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _emailSender = emailSender;
            _env = env;
        }
       
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                    select FirstName, LastName, Role, Email, Password, Phone, CompanyName, Country from dbo.UserDesc";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ModuleAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {

                    myReader = myCommand.ExecuteReader();
                    
                    table.Load(myReader); 
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }
        [Route("GetName")]
        [HttpGet]
        public JsonResult GetName(UserInfo usr)
        {
            string query = @"
                    select CONCAT(FirstName, LastName) from dbo.UserDesc where Email = " + usr.Email + @" ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ModuleAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {

                    myReader = myCommand.ExecuteReader();

                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }
        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        [HttpPut]
        public JsonResult UpdatePassword(UserInfo usr)
        {
            string pass = EncodePasswordToBase64(usr.Password);
            string queryPass = @"
                    select Password from dbo.UserDesc 
                    where Password = @pass" 
                    ;
            string sqlDataSourcee = _configuration.GetConnectionString("ModuleAppCon");
            //object myPassReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSourcee))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryPass, myCon))
                {
                    myCommand.Parameters.AddWithValue("@pass", pass);
                    SqlDataReader reader = myCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            String data = reader.GetString(0);
                            if (reader.GetString(0) != pass)
                            {
                                return new JsonResult("This is NOT your Password");
                            }
                        }
                    }
                    myCon.Close();
                }
            }
            string newPass = EncodePasswordToBase64(usr.NewPassword);
            string query = @"
                    update dbo.UserDesc set 
                    Password = @newPass
                    where Password = @pass 
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("ModuleAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@pass", pass);
                    myCommand.Parameters.AddWithValue("@newPass", newPass);
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    if(rowsAffected > 0)
                    {
                        myCon.Close();
                        return new JsonResult("Updated Successfully");
                    }
                   
                }
            }

            return new JsonResult("Could not Update");
        }
        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/CategoryPhotos/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {

                return new JsonResult("anonymous.png");
            }
        }
        private async void verifyEmail(String email, String subject, String Content)
        {
            //var rng = new Random();
            //int _min = 1000;
            //int _max = 9999;
            //Random _rdm = new Random();
            //int code = _rdm.Next(_min, _max);
            //String Content = "Hi, \n\n" +
            //    "Please Enter this Code to Verify \n" + "Code : " + code +
            //    "\n\n\nWarm Regards \n4Material.com";
            var message = new Message(new string[] { email }, subject, Content);
            //_emailSender.SendEmail(message);

            await _emailSender.SendEmailAsync(message);
            return;
        }
        private bool validateEmail(String email)
        {
            var rng = new Random();
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            int code = _rdm.Next(_min, _max);
            String subject = "Verify Email Address";
            
            String Content = "Hi, \n\n" +
                "Please Enter this Code to Verify \n" + "Code : " + code +
                "\n\n\nWarm Regards \n4Material.com";
            verifyEmail(email, subject, Content);
            return true;
        }
        [HttpPost]
        public JsonResult Post(UserInfo usr)
        {
            bool match = Regex.IsMatch(usr.Email,
                   @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$",
                   RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
 
            if (!match)
            {
                return new JsonResult("Invalid Email");
            }
            validateEmail(usr.Email);
            string encodedPassword = EncodePasswordToBase64(usr.Password);
            string query = @"
                    insert into dbo.UserDesc(FirstName, LastName, Role, Email, Password, Phone, CompanyName, Country, UserImage, ShopAddress, BankAccount) values 
                    ('" + usr.FirstName + @"'
                    ,'" + usr.LastName + @"'
                    ,'" + usr.Role + @"'
                    ,'" + usr.Email + @"'
                    ,'" + encodedPassword + @"'
                    ,'" + usr.Phone+ @"'
                    ,'" + usr.CompanyName + @"'
                    ,'" + usr.Country + @"'
                    ,'" + usr.PhotoFileName + @"'
                    ,'" + usr.ShopAddress + @"'
                    ,'" + usr.BankAccount + @"'
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
        //[HttpPost]
        //public JsonResult Update(UserInfo usr)
        //{
        //    bool match = Regex.IsMatch(usr.Email,
        //           @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$",
        //           RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

        //    if (!match)
        //    {
        //        return new JsonResult("Invalid Email");
        //    }
        //    string encodedPassword = EncodePasswordToBase64(usr.Password);
        //    string query = @"
        //            insert into dbo.UserInfo(UserId, FirstName, LastName, Role, Email, Password, Phone, CompanyName, Country) values 
        //            ('" + usr.UserId + @"'
        //            ,'" + usr.FirstName + @"'
        //            ,'" + usr.LastName + @"'
        //            ,'" + usr.Role + @"'
        //            ,'" + usr.Email + @"'
        //            ,'" + encodedPassword + @"'
        //            ,'" + usr.Phone + @"'
        //            ,'" + usr.CompanyName + @"'
        //            ,'" + usr.Country + @"'
        //            )
        //            ";
        //    DataTable table = new DataTable();
        //    string sqlDataSource = _configuration.GetConnectionString("ModuleAppCon");
        //    SqlDataReader myReader;
        //    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
        //    {
        //        myCon.Open();
        //        using (SqlCommand myCommand = new SqlCommand(query, myCon))
        //        {
        //            myReader = myCommand.ExecuteReader();
        //            table.Load(myReader); ;

        //            myReader.Close();
        //            myCon.Close();
        //        }
        //    }

        //    return new JsonResult("Added Successfully");
        //}

    }
}
