using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ISTEGY.Sales.Models;

namespace ISTEGY.Sales.Controllers
{
    public class StoreController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            return View(db.Stores.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }

            List<int> productsId = store.Inventories.Select(x => x.ProductId).ToList();
            var Products = db.Products.Where(x => !productsId.Contains(x.Id));
            ViewBag.Products = new SelectList(Products, "Id", "Title");
            return View(store);
        }

        public ActionResult RemoveProduct(int id, int pid)
        {
            var target = db.Inventories.SingleOrDefault(x => x.StoreId == id && x.ProductId == pid);
            if (target != null)
            {
                db.Inventories.Remove(target);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        public ActionResult AddProduct(int id, int? pid)
        {
            if (pid == null)
                return RedirectToAction("Details", new { id });
            else
            {
                var target = db.Inventories.SingleOrDefault(x => x.StoreId == id && x.ProductId == pid);
                if (target == null)
                {
                    var inv = new Inventory() { ProductId = pid.Value, StoreId = id };
                    db.Inventories.Add(inv);
                    db.SaveChanges();
                }
                return RedirectToAction("Details", new { id });
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,Title")] Store store)
        {
            if (ModelState.IsValid)
            {
                db.Stores.Add(store);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = store.Id });
            }

            return View(store);
        }

        // GET: Stores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,Title")] Store store)
        {
            if (ModelState.IsValid)
            {
                db.Entry(store).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(store);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Store store = db.Stores.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            var trans = db.Transactions.Where(x => x.StoreId == store.Id).ToList();
            if (trans.Count > 0)
            {
                ViewBag.CanDelete = false;
                ViewBag.Count = trans.Count;
            }
            else
                ViewBag.CanDelete = true;
            return View(store);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Store store = db.Stores.Find(id);
            db.Stores.Remove(store);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
