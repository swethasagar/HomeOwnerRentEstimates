using HomeOwnerRentEstimates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace HomeOwnerRentEstimates.Controllers
{
    public class HomeOwnerController : Controller
    {

        private HomeOwnerDatabaseContext dbContext = new HomeOwnerDatabaseContext();
        // GET: HomeOwner
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(HomeOwner owner)
        {

            var ipAddress = GetIPAddress();
            owner.IPAddress = ipAddress;
            owner.StreetAddress = "";
            owner.City = "";
            owner.State = "";
            owner.ZipCode = "";
            if (dbContext.Owners.Where(o => o.Email.Equals(owner.Email)).FirstOrDefault() == null)
            {
                dbContext.Owners.Add(owner);                
                dbContext.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(HomeOwner owner)
        {
            if (owner != null)
            {
                using (HomeOwnerDatabaseContext dbContext = new HomeOwnerDatabaseContext())
                {
                    var validOwner = dbContext.Owners.Where(o => o.Email.Equals(owner.Email) && o.Password.Equals(owner.Password)).FirstOrDefault();
                    if (validOwner != null)
                    {
                        //Session["UserId"] = validOwner.Id.ToString();
                        //Session["UserName"] = validOwner.Email.ToString();
                        FormsAuthentication.SetAuthCookie(owner.Email, false);
                        TempData["firstName"] = validOwner.FirstName;
                        return RedirectToAction("EnterDetails");
                    }
                }

            }
            return View(owner);
        }

        [Authorize]
        [HttpGet]
        public ActionResult logOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");

        }

        [Authorize]
        public ActionResult EnterDetails()
        {
            ViewBag.firstName = TempData["firstName"];   
            return View();
          
        }

        [Authorize]
        public ActionResult AddressDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddressDetails(HomeOwner owner)
        {

            string userName = User.Identity.Name;
            var ownerObj= dbContext.Owners.Where(o => o.Email == userName).FirstOrDefault();
            if (ownerObj != null)
            {
                ownerObj.City = owner.City;
                ownerObj.StreetAddress = owner.StreetAddress;
                ownerObj.ZipCode = owner.ZipCode;
                ownerObj.State = owner.State;
                ownerObj.Country = owner.Country;
            }
            dbContext.SaveChanges();

            using (var client = new HttpClient())
            {
                const string zwsID = "X1-ZWz17iwesnxcln_2xcn2";           
                string apiURI = "http://www.zillow.com/webservice/GetSearchResults.htm?zws-id=" + zwsID + "&address=" +owner.StreetAddress + "&citystatezip=" + owner.ZipCode + "&rentzestimate=true";

                try
                {
                    var response = client.GetAsync(apiURI);
                    response.Wait();
                    var result = response.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var content = result.Content.ReadAsStringAsync();
                        content.Wait();

                        XDocument xdoc = XDocument.Parse(content.Result);
                        var apiResponse = xdoc.Descendants("response").FirstOrDefault();
                        if (apiResponse != null)
                        {
                            var apiRentEstimates = xdoc.Descendants("rentzestimate").FirstOrDefault();
                            if (apiRentEstimates != null)
                            {
                                //RentEstimates rentEstimate = new RentEstimates();
                                ownerObj.rentAmount = apiRentEstimates.Element("amount").Value;
                                ownerObj.maxRentAmount = apiRentEstimates.Element("valuationRange").Element("high").Value;
                                ownerObj.minRentAmount = apiRentEstimates.Element("valuationRange").Element("low").Value;

                               
                            }
                            else
                            {
                                var apiZestimates = xdoc.Descendants("zestimate").FirstOrDefault();
                               var x= apiZestimates.Element("amount").Value;
                                var apiAnnualRent = 0.05 * Convert.ToInt32(x);
                                var monthlyRent =apiAnnualRent / 12;
                                ownerObj.rentAmount = monthlyRent.ToString();
                                ownerObj.maxRentAmount = (monthlyRent + (monthlyRent * 0.1)).ToString();
                                ownerObj.minRentAmount = (monthlyRent - (monthlyRent * 0.1)).ToString();
                            }

                            dbContext.SaveChanges();
                            TempData["ownerObj"] = ownerObj;

                            return RedirectToAction("RentEstimates");
                        }
                        else
                        {
                            var apiError = xdoc.Descendants("message").Single();
                            var errorMessage=apiError.Element("text").Value;

                            ViewBag.apiErrorMsg = errorMessage ?? "no exact match found for input address";
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    return View("Error");
                }
            }

            return View();

        }

        [Authorize]
        public ActionResult RentEstimates()
        {
            HomeOwner ownerObj =(HomeOwner) TempData["ownerObj"];
            return View(ownerObj);
        }

        [HttpPost]
        public ActionResult RentEstimates(HomeOwner owner)
        {
            string userName = User.Identity.Name;
            try
            {
                var ownerObj = dbContext.Owners.Where(o => o.Email == userName).FirstOrDefault();
                if (ownerObj != null)
                {
                    if (!string.IsNullOrEmpty(owner.ownerRentAmount))
                    {
                        ownerObj.rentAmount = owner.ownerRentAmount;
                    }
                }
                dbContext.SaveChanges();
                sendMail(ownerObj);
                ViewBag.successMsg = "Profile created successfully!!!!. Please check your mail for more info.";
            }
            catch(Exception ex)
            {
                return View("error");
            }
            return View();
        }

        private void sendMail(HomeOwner ownerDetails)
        {
            string rentAmount = string.IsNullOrEmpty(ownerDetails.rentAmount) ? "0" : ownerDetails.rentAmount;
            string maxRentAmount = string.IsNullOrEmpty(ownerDetails.maxRentAmount) ? "0" : ownerDetails.maxRentAmount;
            string minRentAmount = string.IsNullOrEmpty(ownerDetails.rentAmount) ? "0" : ownerDetails.rentAmount;
                var fromMail = new MailAddress("noreplytestapp123@gmail.com", "welcome"+ownerDetails.FirstName);
                var toMail = new MailAddress(ownerDetails.Email);
                var frontEmailPassowrd = "mypassword@123";
                string subject = "Your account is successfully created";
                string body = "<br/><br/>We are excited to tell you that your account is" +
            " successfully created. Please find the rent estimate details below :<br/> " +
            " <br/><br/>Address : " + ownerDetails.StreetAddress+","+ownerDetails.City+","+
            ownerDetails.State+","+ownerDetails.Country+","+ownerDetails.ZipCode+"<br/>"+
            "Rent Amount : "+rentAmount+"<br/>"+ "Max Rent Amount :"+maxRentAmount
            +"<br/>"+"Min Rent Amount :"+minRentAmount+"";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,

            Credentials = new System.Net.NetworkCredential(fromMail.Address, frontEmailPassowrd)

                };
                using (var message = new MailMessage(fromMail, toMail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);
            }


        private string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
        
    }
}