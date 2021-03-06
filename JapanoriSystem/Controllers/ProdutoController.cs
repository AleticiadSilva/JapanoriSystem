﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JapanoriSystem.DAL;
using JapanoriSystem.Models;
using PagedList;

namespace JapanoriSystem.Controllers
{
    public class ProdutoController : Controller
    {
        private bdJapanoriContext db = new bdJapanoriContext();

        //          Tela Inicial Produtos
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            //      Cadeia de objetos para definir a "current" Ordem da listagem das comandas
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CodSortParm = String.IsNullOrEmpty(sortOrder) ? "cod_cre" : ""; // objeto que organiza a lista em ordem do código
            ViewBag.NomeSortParm = String.IsNullOrEmpty(sortOrder) ? "nome_cre" : ""; // objeto que organiza a lista em ordem da situacao
            ViewBag.PriceSortParm = String.IsNullOrEmpty(sortOrder) ? "preco_decre" : ""; // objeto que organiza a lista em ordem de preço

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var produtos = from p in db.tbProduto
                           select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                produtos = produtos.Where(p => p.ProdutoID.ToString().Contains(searchString)
                    || p.Nome.ToString().Contains(searchString)
                    || p.cDesc.ToString().Contains(searchString));

            }
            switch (sortOrder)
            {
                case "cod_cre":
                    produtos = produtos.OrderBy(p => p.ProdutoID);
                    break;
                case "nome_cre":
                    produtos = produtos.OrderBy(p => p.Nome);
                    break;
                case "preco_decre":
                    produtos = produtos.OrderByDescending(p => p.Preco);
                    break;
                default:
                    produtos = produtos.OrderByDescending(p => p.Preco);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(produtos.ToPagedList(pageNumber, pageSize));
        }

        // GET: Produto/Details/id
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.tbProduto.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            return View(produto);
        }

        // GET: Produto/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Produto/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Nome, string cDesc, double Preco)
        {
            if (Nome == null || cDesc == null || Preco == 0)
            {
                ViewBag.ConfErro = "1";
                ViewBag.Erro = "Insira valores válidos nos 3 campos";
            }
            else
            {
                var param = db.tbProduto
                                .Where(i => i.Nome == Nome)
                                .ToList()
                                .Count();
                if (param > 0)
                {
                    ViewBag.ConfErro = "2";
                    ViewBag.Erro = "Já existe um produto com o mesmo nome";
                    
                }
                if (param == 0)
                {
                    string cStatus = "On";
                    ViewBag.addProduto = db.Database
                                        .ExecuteSqlCommand("INSERT INTO tbProduto (Nome, cDesc, Preco, cStatus) " +
                                        "VALUES ( '" + Nome + "','" + cDesc + "'," + Preco + ",'"+cStatus+"')");
                    db.SaveChanges();
                    ViewBag.Success = "Produto criado com sucesso";
                }
            }

            return View();
        }

        // GET: Produto/Edit/id
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.tbProduto.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            return View(produto);
        }

        // POST: Produto/Edit/id

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProdutoID,Nome,cDesc,Preco")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(produto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(produto);
        }

        // GET: Produto/Delete/id
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.tbProduto.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            return View(produto);
        }

        // POST: Produto/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Produto produto = db.tbProduto.Find(id);
            db.tbProduto.Remove(produto);
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
