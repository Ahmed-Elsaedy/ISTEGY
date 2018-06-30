using ISTEGY.Sales.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISTEGY.Sales.Controllers
{
    public class TransactionController : Controller
    {
        DatabaseContext db = new DatabaseContext();

        [HttpGet]
        public ActionResult In(int? storeId)
        {
            List<Transaction> transactions = null;
            SelectList stores = null;
            if (storeId.HasValue)
            {
                Store store = db.Stores.Find(storeId);
                if (store != null)
                {
                    transactions = store.Transactions;
                    stores = new SelectList(db.Stores, "Id", "Title", store.Id);
                }
            }
            ViewBag.Stores = stores ?? new SelectList(db.Stores, "Id", "Title");
            var result = (transactions ?? db.Transactions.ToList()).Where(x => x.Type == TransactionType.Purchase);
            return View(result);
        }

        public ActionResult Out(int? storeId)
        {
            List<Transaction> transactions = null;
            SelectList stores = null;
            if (storeId.HasValue)
            {
                Store store = db.Stores.Find(storeId);
                if (store != null)
                {
                    transactions = store.Transactions;
                    stores = new SelectList(db.Stores, "Id", "Title", store.Id);
                }
            }
            ViewBag.Stores = stores ?? new SelectList(db.Stores, "Id", "Title");
            var result = (transactions ?? db.Transactions.ToList()).Where(x => x.Type == TransactionType.Sales);
            return View(result);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Stores = new SelectList(db.Stores, "Id", "Title");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Transaction transaction, List<int> prods, List<int> qnts)
        {
            ViewBag.Stores = new SelectList(db.Stores, "Id", "Title");
            if (prods == null || qnts == null || prods.Count <= 0 || prods.Count != qnts.Count)
            {
                ModelState.AddModelError("", "Bad Data From Client, try again by selecting the store first and then selecting a products");
                return View(transaction);
            }
            if (qnts.Any(x => x <= 0))
            {
                ModelState.AddModelError("", "Quantites cannot go beyond zero, enter positive value");
                return View(transaction);
            }
            Store store = db.Stores.Find(transaction.StoreId);
            if (store != null)
            {
                for (int i = 0; i < prods.Count; i++)
                {
                    Inventory inv = store.Inventories.SingleOrDefault(x => x.ProductId == prods[i]);
                    if (inv != null)
                        switch (transaction.Type)
                        {
                            case TransactionType.Purchase:
                                inv.Quantity += qnts[i];
                                break;
                            case TransactionType.Sales:
                                var result = (inv.Quantity -= qnts[i]);
                                if (result >= 0)
                                    inv.Quantity = result;
                                else
                                {
                                    ModelState.AddModelError("", $"Quantites of ({inv.Product.Title}) in Store  cannot go beyond zero, please revise your entered value");
                                    return View(transaction);
                                }
                                break;
                        }
                    else
                    {
                        ModelState.AddModelError("", $"Product cannot be found in the selected store, try again");
                        return View(transaction);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", $"Store cannot be found, try again");
                return View(transaction);
            }

            db.Transactions.Add(transaction);
            db.SaveChanges();
            transaction.Details = new List<TransactionDetail>();
            for (int i = 0; i < prods.Count; i++)
            {
                TransactionDetail dt = new TransactionDetail()
                {
                    TranId = transaction.VoucherSerial,
                    ProductId = prods[i],
                    Quantity = qnts[i],
                };
                transaction.Details.Add(dt);
            }
            db.SaveChanges();

            if (transaction.Type == TransactionType.Purchase)
                return RedirectToAction("In");
            else
                return RedirectToAction("Out");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Stores = new SelectList(db.Stores, "Id", "Title");
            return View(db.Transactions.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                Transaction local = db.Transactions.Find(transaction.VoucherSerial);
                local.TransactionDate = transaction.TransactionDate;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = transaction.VoucherSerial });
            }
            else
                return View(transaction);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {
                Transaction tr = db.Transactions.Find(id);

                var det = tr.Details.Select(x => x.ProductId);
                var prods = tr.Store.Inventories.Where(x => !det.Contains(x.ProductId)).Select(x => x.Product);
                ViewBag.Products = new SelectList(prods, "Id", "Title");
                return View(tr);
            }
            else
                return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Details(int? id, int? pid, int? qnt)
        {
            if (id.HasValue && pid.HasValue && qnt.HasValue && qnt > 0)
            {
                Transaction tr = db.Transactions.Find(id);
                Inventory inv = db.Inventories.SingleOrDefault
                    (x => x.StoreId == tr.StoreId && x.ProductId == pid);
                switch (tr.Type)
                {
                    case TransactionType.Purchase:
                        inv.Quantity += qnt.Value;
                        break;
                    case TransactionType.Sales:
                        int result = inv.Quantity -= qnt.Value;
                        if (result >= 0)
                            inv.Quantity = result;
                        else
                        {
                            TempData.Add("msg", "When attemping to add an item, quantity in inventory go beyound zero, please revise your data");
                        }
                        break;
                }
                if (!TempData.ContainsKey("msg"))
                {
                    TransactionDetail dt = new TransactionDetail()
                    {
                        TranId = id.Value,
                        ProductId = pid.Value,
                        Quantity = qnt.Value
                    };
                    tr.Details.Add(dt);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Details", new { id });
        }

        public ActionResult RemoveDetail(int? pid, int? tid)
        {
            TransactionDetail td = db.TransactionDetails.SingleOrDefault(x => x.ProductId == pid && x.TranId == tid);
            if (td != null)
            {
                Inventory inv = db.Inventories.SingleOrDefault(x => x.StoreId == td.Transaction.StoreId && x.ProductId == pid);
                switch (td.Transaction.Type)
                {
                    case TransactionType.Purchase:
                        int result = inv.Quantity -= td.Quantity;
                        if (result >= 0)
                            inv.Quantity = result;
                        else
                        {
                            TempData.Add("msg", "When attemping to delete, quantity in inventory go beyound zero, please revise your data");
                        }
                        break;
                    case TransactionType.Sales:
                        inv.Quantity += td.Quantity;
                        break;
                }
                if (!TempData.ContainsKey("msg"))
                {
                    db.TransactionDetails.Remove(td);
                    db.SaveChanges();
                }
                return RedirectToAction("Details", new { id = tid });
            }
            else
                return HttpNotFound();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            bool canSave = true;
            List<string> errorInv = new List<string>();
            foreach (TransactionDetail dt in transaction.Details)
            {
                Inventory inv = transaction.Store.Inventories.SingleOrDefault(x => x.ProductId == dt.ProductId);
                switch (transaction.Type)
                {
                    case TransactionType.Purchase:
                        var result = inv.Quantity -= dt.Quantity;
                        if (result < 0)
                        {
                            canSave = false;
                            errorInv.Add(inv.Product.Title);
                        }
                        break;
                }
            }
            ViewBag.CanDelete = canSave;
            ViewBag.Errors = errorInv;
            return View(transaction);
        }

        [HttpPost]
        public ActionResult Delete(int id, object obj)
        {
            Transaction transaction = db.Transactions.Find(id);
            bool canSave = true;
            List<string> errorInv = new List<string>();
            foreach (TransactionDetail dt in transaction.Details)
            {
                Inventory inv = transaction.Store.Inventories.SingleOrDefault(x => x.ProductId == dt.ProductId);
                switch (transaction.Type)
                {
                    case TransactionType.Purchase:
                        var result = inv.Quantity -= dt.Quantity;
                        if (result >= 0)
                            inv.Quantity = result;
                        else
                        {
                            canSave = false;
                            errorInv.Add(inv.Product.Title);
                        }
                        break;
                    case TransactionType.Sales:
                        inv.Quantity += dt.Quantity;
                        break;
                }
            }
            if (canSave)
            {
                var list = transaction.Details.ToList();
                foreach (TransactionDetail dt in list)
                    db.TransactionDetails.Remove(dt);
                db.Transactions.Remove(transaction);
                db.SaveChanges();
                return RedirectToAction("Index", "Inventory");
            }
            else
                return RedirectToAction("Delete", new { id = transaction.VoucherSerial });
        }

        [HttpGet]
        public ActionResult ProductsByStore(int? storeId)
        {
            var prods = storeId.HasValue ? db.Inventories.Where(x => x.StoreId == storeId).Select(x => x.Product).ToList() : new List<Product>();
            ViewBag.Products = new SelectList(prods, "Id", "Title");
            return PartialView();
        }

        [HttpGet]
        public ActionResult EditItem(int tid, int pid)
        {
            TransactionDetail dt = db.TransactionDetails.SingleOrDefault(x => x.TranId == tid && x.ProductId == pid);
            return View(dt);
        }

        [HttpPost]
        public ActionResult EditItem(TransactionDetail model)
        {
            var local = db.TransactionDetails.SingleOrDefault(x => x.ProductId == model.ProductId && x.TranId == model.TranId);
            if (local.Quantity != model.Quantity && model.Quantity > 0)
            {
                Transaction transaction = local.Transaction;
                Inventory inv = db.Inventories.SingleOrDefault(x => x.StoreId == transaction.StoreId && x.ProductId == model.ProductId);
                bool canSave = true;
                switch (transaction.Type)
                {
                    case TransactionType.Purchase:
                        var result = (inv.Quantity - local.Quantity) + model.Quantity;
                        if (result >= 0)
                        {
                            inv.Quantity = result;
                            local.Quantity = model.Quantity;
                        }
                        else
                            canSave = false;
                        break;
                    case TransactionType.Sales:
                        result = (inv.Quantity + local.Quantity) - model.Quantity;
                        if (result >= 0)
                        {
                            inv.Quantity = result;
                            local.Quantity = model.Quantity;
                        }
                        else
                            canSave = false;
                        break;
                }
                if (canSave)
                {
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = transaction.VoucherSerial });
                }
                else
                {
                    ModelState.AddModelError("", "When attempting to modify quantity, it results inventory to go beyond zero, try again with another value");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Quantity");
                return View(model);
            }
        }
    }
}