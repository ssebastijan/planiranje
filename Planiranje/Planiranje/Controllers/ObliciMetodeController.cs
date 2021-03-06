﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Planiranje.Models;


namespace Planiranje.Controllers
{
    public class ObliciMetodeController : Controller
    {
        private Oblici_DBHandle oblici = new Oblici_DBHandle();

        public ActionResult Index()
        {
            if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
            {
                return RedirectToAction("Index", "Planiranje");
            }
            ViewBag.Title = "Pregled oblika i metoda";

			ObliciMetodeModel model = new ObliciMetodeModel();
			model.oblici = oblici.ReadOblici();
			return View("Index", model);
        }

        public ActionResult NoviPlan()
        {
            if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
            {
                return RedirectToAction("Index", "Planiranje");
            }
            if (Request.IsAjaxRequest())
            {
				ObliciMetodeModel model = new ObliciMetodeModel();
                return View("NoviPlan", model);
            }
			return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult NoviPlan(ObliciMetodeModel model)
        {
            if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
            {
                return RedirectToAction("Index", "Planiranje");
            }
            if (model.oblik.Naziv != null && oblici.CreateOblici(model.oblik))
            {
				return RedirectToAction("Index");
			}
            else
            {
				return View("NoviPlan", model);
			}
        }

        public ActionResult Edit(int id)
        {
            if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
            {
                return RedirectToAction("Index", "Planiranje");
            }
            if (Request.IsAjaxRequest())
            {
				ObliciMetodeModel model = new ObliciMetodeModel();
				model.oblik = oblici.ReadOblici(id);
                return View("Uredi", model);
            }
			return RedirectToAction("Index");
		}

        [HttpPost]
        public ActionResult Edit(ObliciMetodeModel model)
        {
            if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
            {
                return RedirectToAction("Index", "Planiranje");
            }
            if (model.oblik.Naziv != null && oblici.UpdateOblici(model.oblik))
            {
				return RedirectToAction("Index");
			}
            else
            {
				return View("Uredi", model);
			}
        }

        public ActionResult Delete(int id)
        {
            if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
            {
                return RedirectToAction("Index", "Planiranje");
            }

            if (Request.IsAjaxRequest())
			{
				ViewBag.ErrorMessage = null;
				ObliciMetodeModel model = new ObliciMetodeModel();
				model.oblik = oblici.ReadOblici(id);
				return View("Obrisi", model);
            }
			return RedirectToAction("Index");
		}

        [HttpPost]
        public ActionResult Delete(ObliciMetodeModel model)
        {
            if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
            {
                return RedirectToAction("Index", "Planiranje");
            }
            if (!oblici.DeleteOblici(model.oblik.Id_oblici))
            {
				ViewBag.ErrorMessage = "Dogodila se greška, nije moguće obrisati oblik!";
				return View("Obrisi", model);
			}
            else
            {
				return RedirectToAction("Index");
			}
        }
    }
}