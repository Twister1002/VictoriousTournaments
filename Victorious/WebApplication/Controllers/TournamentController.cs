﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class TournamentController : Controller
    {
        // GET: Tournament
        [Route("Tournament")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Tournament/{name}
        [Route("Tournament/{guid}")]
        public ActionResult Tournament(String guid)
        {
            TournamentModel model = new TournamentModel("Hi");

            return View("Tournament", model);
        }

        [Route("Tournament/{org}/{guid}")]
        public ActionResult TournamentOrginization(String org, String guid)
        {
            return View();
        }

        // GET: Tournament/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tournament/Create
        [HttpPost]
        public ActionResult Create(TournamentModel model)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tournament/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Tournament/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tournament/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tournament/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}