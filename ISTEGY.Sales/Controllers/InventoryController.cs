using ISTEGY.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISTEGY.Sales.Controllers
{
    public class InventoryController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        public ActionResult Index(string by, int? id)
        {
            List<Inventory> inventories = null;
            SelectList products = null;
            SelectList stores = null;
            ViewBag.Filter = null;
            if (!string.IsNullOrEmpty(by) && id.HasValue)
            {
                ViewBag.Filter = by.ToLower();
                switch (by.ToLower())
                {
                    case "store":
                        Store store = db.Stores.Find(id);
                        if (store != null)
                        {
                            inventories = store.Inventories;
                            stores = new SelectList(db.Stores, "Id", "Title", store.Id);
                        }
                        break;
                    case "product":
                        Product product = db.Products.Find(id);
                        if (product != null)
                        {
                            inventories = product.Inventories;
                            products = new SelectList(db.Products, "Id", "Title", product.Id);
                        }
                        break;
                }
            }
            ViewBag.Stores = stores ?? new SelectList(db.Stores, "Id", "Title");
            ViewBag.Products = products ?? new SelectList(db.Products, "Id", "Title");
            return View(inventories ?? db.Inventories.ToList());
        }
    }
}
