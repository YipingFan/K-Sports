using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoDelivery.Models;
using GeoCoding.Google;
using LinqToExcel;
using AutoDelivery.Repository;
using System.Data.Entity;

namespace AutoDelivery.Controllers
{
    public class FileController : Controller
    {
        //
        // GET: /File/

        KSportsEntities context = new KSportsEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            IList<Order> orders = new List<Order>();

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);

                var excel = new ExcelQueryFactory();
                excel.FileName = path;
                excel.AddMapping<Order>(o => o.Item, "item");
                excel.AddMapping<Order>(o => o.Telephone, "telephone");
                excel.AddMapping<Order>(o => o.Address, "address");

                var names = excel.GetWorksheetNames();

                if (names.FirstOrDefault() != null)
                {
                    orders = (from o in excel.Worksheet<Order>(names.First())
                              //where !string.IsNullOrWhiteSpace(o.Address)
                              select o).ToList();

                    var validatedOrders = ValidateAddress(orders.Where(o=>!string.IsNullOrWhiteSpace(o.Address)).ToList()).ToList();
                    

                    //validatedOrders = validatedOrders.Select(o =>
                    //    {
                    //        var postCode = o.GoogleAddresses[0].Components.Where(c => c.Types.Contains(GoogleAddressType.PostalCode)).FirstOrDefault();

                    //    });


                    foreach (var o in validatedOrders)
                    {
                        string postCode = o.GoogleAddresses[0].Components.Where(c => c.Types.Contains(GoogleAddressType.PostalCode)).FirstOrDefault().ShortName;

                        int code = Int32.Parse(postCode);

                        var matchedCode = (from p in context.RegionPostcodes
                                      where p.FromPostCode <= code && p.ToPostCode >= code
                                      select p).FirstOrDefault();

                        if (matchedCode != null)
                            o.Region = matchedCode.Region;
                    }

                    Session["orders"] = validatedOrders;

                    return View("AddressValidator", validatedOrders);
                    //return View("Direction", validatedOrders);
                }
            }

            return View("Direction", orders);
        }

        [HttpPost]
        public ActionResult Direction(string regionId)
        {
            if (regionId == null)
                throw new ArgumentNullException("region id is null");

            var orders = Session["orders"] as IList<ValidatedOrder>;

            var regonOrders = (from o in orders
                               where o.Region != null && o.Region.Id == Int32.Parse(regionId)
                               select o).ToList();
            
            return View("Direction", regonOrders);
        }

        private IEnumerable<ValidatedOrder> ValidateAddress(IList<Order> orders)
        {
            var geoCoder = new GoogleGeoCoder();

            var validatedOrders = orders.Select(o =>
                {
                    var oos = new ValidatedOrder { Order = o, 
                        GoogleAddresses = string.IsNullOrWhiteSpace(o.Address) ? null : geoCoder.GeoCode(o.Address).ToList(),
                        Region = null};

                    return oos;
                });

            return validatedOrders;
        }
    }
}
