﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Planiranje.Controllers
{
    public class PlanOs1Controller : Controller
    {
        // GET: PlanOs1
        public ActionResult Index()
        {
			if (PlaniranjeSession.Trenutni.PedagogId > 0)
			{
				ViewBag.Title = "Plan - osnovna skola 1";
				return View();
			}
			return RedirectToAction("Prijava");
		}
    }
}