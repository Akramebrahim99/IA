using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using E_Market.Models;

namespace E_Market.Controllers
{
    public class ProductController : Controller
    {
        private Store db = new Store();

        ///////////////////////////// Add Product to Cart //////////////////////////////
        public ActionResult addtocart(int product_id)
        {
            if (Session["cart"] == null)
            {
                var car = new Cart();
                List<Cart> cart = new List<Cart>();
                var product = db.Products.Find(product_id);
                cart.Add(new Cart() { Product = product, product_id = product.id, added_at = DateTime.Now });
                car.added_at = DateTime.Now;
                car.Product = product;
                car.product_id = product_id;

                db.Carts.Add(car);
                db.SaveChanges();
                Session["cart"] = cart;
            }
            else
            {
                List<Cart> cart = (List<Cart>)Session["cart"];
                //  List<Cart> cart2 = new List<Cart>();
                var product = db.Products.Find(product_id);
                cart.Add(new Cart() { Product = product, product_id = product.id, added_at = DateTime.Now });
                Session["cart"] = cart;
                var car = new Cart();
                car.added_at = DateTime.Now;
                car.Product = product;
                car.product_id = product_id;
                db.Carts.Add(car);
                db.SaveChanges();
                //    db.Carts.Add(product.Cart);
            }

            return Redirect("index");
        }
        ///////////////////////////// End /////////////////////////////////////////////

        ///////////////////////////// Show All Product //////////////////////////////
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }
        ///////////////////////////// End /////////////////////////////////////////////

        ///////////////////////////// Filter All Product //////////////////////////////
        [HttpPost]
        public ActionResult Index(string searchtext)
        {
            var pro = db.Products.Include(p => p.Category).Where(p => p.Category.name.Contains(searchtext) || searchtext == null).ToList();
            return View(pro);
        }
        ///////////////////////////// End /////////////////////////////////////////////

        ///////////////////////////// Details of Product //////////////////////////////
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        ///////////////////////////// End //////////////////////////////////////
       
        ///////////////////////////// Add Product ////////////////////////////////////
        public ActionResult Create()
        {
            ViewBag.category_id = new SelectList(db.Categories, "id", "name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (product.ImageFile == null)
            {
                ViewBag.error = "*Required";
            }
            else
            {
                string FileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                string Extention = Path.GetExtension(product.ImageFile.FileName);
                FileName = FileName + DateTime.Now.ToString("yymmssfff") + Extention;
                product.image = "~/Images/" + FileName;
                FileName = Path.Combine(Server.MapPath("~/Images/"), FileName);
                
                if (Extention.ToLower() == ".jpg" || Extention.ToLower() == ".jpeg" || Extention.ToLower() == ".png")
                {
                    
                    if (ModelState.IsValid)
                    {
                        product.ImageFile.SaveAs(FileName);
                        db.Products.Add(product);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.msg = "Invaild File Type";
                }

            }
            ViewBag.category_id = new SelectList(db.Categories, "id", "name", product.category_id);
            return View(product);
        }
        ///////////////////////////// End //////////////////////////////////////
        
        ///////////////////////////// Update Product /////////////////////////////////
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            TempData["imgPath"] = product.image;
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.category_id = new SelectList(db.Categories, "id", "name", product.category_id);
            return View(product);
        }

        
        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                    string Extention = Path.GetExtension(product.ImageFile.FileName);
                    FileName = FileName + DateTime.Now.ToString("yymmssfff") + Extention;
                    product.image = "~/Images/" + FileName;
                    FileName = Path.Combine(Server.MapPath("~/Images/"), FileName);
                    
                    if (Extention.ToLower() == ".jpg" || Extention.ToLower() == ".jpeg" || Extention.ToLower() == ".png")
                    {
                        product.ImageFile.SaveAs(FileName);
                        db.Entry(product).State = EntityState.Modified;
                        db.SaveChanges();

                        string old_image = Request.MapPath(TempData["imgPath"].ToString());
                        if (System.IO.File.Exists(old_image))
                        {
                            System.IO.File.Delete(old_image);
                        }

                        return RedirectToAction("Index");

                    }
                    else
                    {
                        ViewBag.msg = "Invaild File Type";
                    }

                }
                else
                {
                    product.image = TempData["imgPath"].ToString();
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.category_id = new SelectList(db.Categories, "id", "name", product.category_id);
            return View(product);
        }

        ///////////////////////////// End ////////////////////////////////////////////////

        ///////////////////////////// Delete product /////////////////////////////////////
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            String currentImg = Request.MapPath(product.image);
            Cart cart = db.Carts.Find(id);
            if(cart != null)
            {
                db.Carts.Remove(cart);
            }
            else {}
            db.Products.Remove(product);
            db.SaveChanges();
            if (System.IO.File.Exists(currentImg))
            {
                System.IO.File.Delete(currentImg);
            }
            return RedirectToAction("Index");
        }
        ///////////////////////////// End //////////////////////////////////////

    }
}