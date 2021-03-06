﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Planiranje.Models;
using Planiranje.Reports;

namespace Planiranje.Controllers
{
    public class MjesecniPlanController : Controller
	{
		private Mjesecni_plan_DBHandle mjesecni_planovi = new Mjesecni_plan_DBHandle();
		private Godisnji_plan_DBHandle godisnji_planovi = new Godisnji_plan_DBHandle();
		private Podrucje_rada_DBHandle podrucja_rada = new Podrucje_rada_DBHandle();
		private Subjekt_DBHandle subjekti = new Subjekt_DBHandle();
		private Aktivnost_DBHandle aktivnosti = new Aktivnost_DBHandle();

		public ActionResult Index(string Plan)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			MjesecniModel mjesecniModel = new MjesecniModel();
			ViewBag.Title = "Pregled mjesečnih planova";
			
			int idPlan = 0;
			mjesecniModel.GodisnjiPlanovi = new List<SelectListItem>(godisnji_planovi.ReadGodisnjePlanove().Select(i => new SelectListItem()
			{
				Text = i.Ak_godina,
				Value = i.Id_god.ToString()
			}));

			if (Plan != null)
			{
				idPlan = mjesecniModel.GodisnjiPlanovi.FindIndex(x => x.Value == Plan);
				mjesecniModel.ID_GODINA = Convert.ToInt32(mjesecniModel.GodisnjiPlanovi.ElementAt(idPlan).Value.ToString());
				ViewBag.HasGodPlan = true;
			}
			else
			{
				if (mjesecniModel.GodisnjiPlanovi.Count < 1)
				{
					ViewBag.HasGodPlan = false;
				}
				else
				{
					mjesecniModel.ID_GODINA = Convert.ToInt32(mjesecniModel.GodisnjiPlanovi.ElementAt(0).Value.ToString());
					idPlan = 0;
					ViewBag.HasGodPlan = true;
				}
			}
			if (mjesecniModel.GodisnjiPlanovi.Count > 0)
			{
				mjesecniModel.GodisnjiPlanovi.ElementAt(idPlan).Selected = true;
			}
			mjesecniModel.MjesecniPlanovi = mjesecni_planovi.ReadMjesecnePlanove(mjesecniModel.ID_GODINA);
			return View("Index", mjesecniModel);
		}

		public ActionResult NoviPlan(int id_godina)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			ViewBag.Title = "Novi mjesečni plan";
			MjesecniModel mjesecniModel = new MjesecniModel();
			mjesecniModel.MjesecniPlan = new Mjesecni_plan();
			mjesecniModel.ID_GODINA = id_godina;
			mjesecniModel.GodisnjiPlanovi = new List<SelectListItem>(godisnji_planovi.ReadGodisnjePlanove().Select(i => new SelectListItem()
			{
				Text = i.Ak_godina,
				Value = i.Id_god.ToString()
			}));
			
			return View("NoviPlan", mjesecniModel);
		}

		[HttpPost]
		public ActionResult NoviPlan(MjesecniModel _mjesecni_model)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			Mjesecni_plan mjesecni_plan = new Mjesecni_plan();
			mjesecni_plan.ID_pedagog = PlaniranjeSession.Trenutni.PedagogId;
			mjesecni_plan.Id_godina = _mjesecni_model.ID_GODINA;
			mjesecni_plan.Naziv = _mjesecni_model.MjesecniPlan.Naziv;
			mjesecni_plan.Opis = _mjesecni_model.MjesecniPlan.Opis;

			if (mjesecni_plan.Naziv != null && mjesecni_plan.Opis != null && mjesecni_planovi.CreateMjesecniPlan(mjesecni_plan))
			{
				return RedirectToAction("Index", new { Plan = _mjesecni_model.ID_GODINA });
			}
			_mjesecni_model.MjesecniPlan = mjesecni_plan;
			return View("NoviPlan", _mjesecni_model);
		}
		public ActionResult PrikaziDetalje (int id, int id_god)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			MjesecniModel model = new MjesecniModel();
			model.MjesecniPlan = mjesecni_planovi.ReadMjesecniPlan(id);
			model.MjesecniDetalji = mjesecni_planovi.ReadMjesecneDetalje(id);
			model.ID_PLAN = id;
			model.ID_GODINA = id_god;
			return View("Detalji", model);
		}
		[HttpPost]
		public ActionResult Detalji(Mjesecni_plan mjesecni_plan)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			if (!mjesecni_planovi.UpdateMjesecniPlan(mjesecni_plan))
			{
				TempData["alert"] = "<script>alert('Mjesecni plan nije promjenjen!');</script>";
			}
			else
			{
				TempData["alert"] = "<script>alert('Mjesecni plan je uspjesno promjenjen!');</script>";
			}
			return RedirectToAction("Detalji");
		}
		
		public ActionResult Delete(int id, int id_god)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			MjesecniModel mjesecni_model = new MjesecniModel();
			mjesecni_model.MjesecniPlan = mjesecni_planovi.ReadMjesecniPlanForDel(id);
			mjesecni_model.ID_GODINA = id_god;
			if (Request.IsAjaxRequest())
			{
				return PartialView("Obrisi", mjesecni_model);
			}
			return RedirectToAction("Index", new { Plan = id_god });
		}

		[HttpPost]
		public ActionResult Delete(MjesecniModel mjesecni_model)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			if (!mjesecni_planovi.DeleteMjesecniPlan(mjesecni_model.MjesecniPlan.ID_plan))
			{
				mjesecni_model.MjesecniPlan = mjesecni_planovi.ReadMjesecniPlanForDel(mjesecni_model.MjesecniPlan.ID_plan);
				mjesecni_model.ID_GODINA = mjesecni_model.ID_GODINA;
				if (Request.IsAjaxRequest())
				{
					return PartialView("Obrisi", mjesecni_model);
				}
				return RedirectToAction("Index", new { Plan = mjesecni_model.ID_GODINA });
			}
			return RedirectToAction("Index", new { Plan = mjesecni_model.ID_GODINA });
		}

        public FileStreamResult Ispis(int id)
        {
            Mjesecni_plan mjesecni_plan = mjesecni_planovi.ReadMjesecniPlan(id);
			List<Mjesecni_detalji> mjesecni_detalji = mjesecni_planovi.ReadMjesecneDetalje(id);
			MjesecniModel mjesecni_model = new MjesecniModel();
			mjesecni_model.MjesecniPlan = mjesecni_plan;
			mjesecni_model.MjesecniDetalji = mjesecni_detalji;

            MjesecniPlanReport report = new MjesecniPlanReport(mjesecni_model);

            return new FileStreamResult(new MemoryStream(report.Podaci), "application/pdf");
		}

		public ActionResult NoviDetalji(int id, int id_god)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			ViewBag.Title = "Novi detalji mjesečnog plana";
			MjesecniModel mjesecniModel = new MjesecniModel();
			mjesecniModel.MjesecniPlan = new Mjesecni_plan();
			mjesecniModel.PodrucjaRada = new List<SelectListItem>(podrucja_rada.ReadPodrucjeRada().Select(i => new SelectListItem()
			{
				Text = i.Naziv.ToString(),
				Value = i.Id_podrucje.ToString()
			}));
			mjesecniModel.Aktivnosti = new List<SelectListItem>(aktivnosti.ReadAktivnost().Select(i => new SelectListItem()
			{
				Text = i.Naziv.ToString(),
				Value = i.Id_aktivnost.ToString()
			}));
			mjesecniModel.Subjekti = new List<SelectListItem>(subjekti.ReadSubjekti().Select(i => new SelectListItem()
			{
				Text = i.Naziv.ToString(),
				Value = i.ID_subjekt.ToString()
			}));
			mjesecniModel.GodisnjiPlanovi = new List<SelectListItem>(godisnji_planovi.ReadGodisnjePlanove().Select(i => new SelectListItem()
			{
				Text = i.Ak_godina,
				Value = i.Id_god.ToString()
			}));
			mjesecniModel.MjesecniPlan = mjesecni_planovi.ReadMjesecniPlan(id);
			mjesecniModel.MjesecniDetalji = mjesecni_planovi.ReadMjesecneDetalje(id);
			mjesecniModel.ID_PLAN = id;
			mjesecniModel.ID_GODINA = id_god;

			return PartialView("NoviDetalji", mjesecniModel);
		}

		[HttpPost]
		public ActionResult NoviDetalji(MjesecniModel _mjesecni_model)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			_mjesecni_model.mjesecniDetalj.ID_plan = _mjesecni_model.ID_PLAN;
			if (_mjesecni_model.mjesecniDetalj.Suradnici != null &&
				_mjesecni_model.mjesecniDetalj.Aktivnost != null &&
				_mjesecni_model.mjesecniDetalj.Podrucje != null &&
				_mjesecni_model.mjesecniDetalj.Biljeska != null &&
				_mjesecni_model.mjesecniDetalj.Br_sati > 0 &&
				DateTime.Compare(_mjesecni_model.mjesecniDetalj.Vrijeme, DateTime.Now.Date) > 0 && 
				mjesecni_planovi.CreateMjesecniDetalj(_mjesecni_model.mjesecniDetalj))
			{
				_mjesecni_model.MjesecniDetalji = mjesecni_planovi.ReadMjesecneDetalje(_mjesecni_model.ID_PLAN);
				return RedirectToAction("PrikaziDetalje", new { id = _mjesecni_model.ID_PLAN, id_god = _mjesecni_model.ID_GODINA });
			}
			else
			{
				if (DateTime.Compare(_mjesecni_model.mjesecniDetalj.Vrijeme, DateTime.Now.Date) <= 0 || (_mjesecni_model.mjesecniDetalj.Vrijeme.Year < 2))
				{
					ModelState.AddModelError("error_msg", "Datum mora biti noviji od trenutnog.");
				}
				else
				{
					ModelState.AddModelError("error_msg", " ");
				}
				_mjesecni_model.PodrucjaRada = new List<SelectListItem>(podrucja_rada.ReadPodrucjeRada().Select(i => new SelectListItem()
				{
					Text = i.Naziv.ToString(),
					Value = i.Id_podrucje.ToString()
				}));
				_mjesecni_model.Aktivnosti = new List<SelectListItem>(aktivnosti.ReadAktivnost().Select(i => new SelectListItem()
				{
					Text = i.Naziv.ToString(),
					Value = i.Id_aktivnost.ToString()
				}));
				_mjesecni_model.Subjekti = new List<SelectListItem>(subjekti.ReadSubjekti().Select(i => new SelectListItem()
				{
					Text = i.Naziv.ToString(),
					Value = i.ID_subjekt.ToString()
				}));
				_mjesecni_model.GodisnjiPlanovi = new List<SelectListItem>(godisnji_planovi.ReadGodisnjePlanove().Select(i => new SelectListItem()
				{
					Text = i.Ak_godina,
					Value = i.Id_god.ToString()
				}));
				return PartialView("NoviDetalji", _mjesecni_model);
			}
		}

		public ActionResult DeleteDetails(int id, int id_god)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			MjesecniModel model = new MjesecniModel();
			model.mjesecniDetalj = mjesecni_planovi.ReadMjesecniDetalj(id);
			model.ID_GODINA = id_god;
			if (Request.IsAjaxRequest())
			{
				return PartialView("ObrisiDetalj", model);
			}
			return RedirectToAction("Index", new { Plan = model.ID_GODINA });
		}

		[HttpPost]
		public ActionResult DeleteDetails(MjesecniModel model)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			if (!mjesecni_planovi.DeleteMjesecniDetalj(model.mjesecniDetalj.ID))
			{
				model.mjesecniDetalj = mjesecni_planovi.ReadMjesecniDetalj(model.MjesecniPlan.ID_plan);
				model.ID_GODINA = model.ID_GODINA;
				if (Request.IsAjaxRequest())
				{
					return PartialView("ObrisiDetalj", model);
				}
				return RedirectToAction("Index", new { Plan = model.ID_GODINA });
			}
			model.MjesecniPlan = mjesecni_planovi.ReadMjesecniPlan(model.mjesecniDetalj.ID_plan);
			model.MjesecniDetalji = mjesecni_planovi.ReadMjesecneDetalje(model.mjesecniDetalj.ID_plan);
			model.ID_PLAN = model.mjesecniDetalj.ID_plan;
			return View("Detalji", model);
		}
		public ActionResult UrediNoviPlan(int id_godina, int id_plan)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			ViewBag.Title = "Novi mjesečni plan";
			MjesecniModel mjesecniModel = new MjesecniModel();
			mjesecniModel.MjesecniPlan = mjesecni_planovi.ReadMjesecniPlan(id_plan);
			mjesecniModel.ID_GODINA = id_godina;
			mjesecniModel.GodisnjiPlanovi = new List<SelectListItem>(godisnji_planovi.ReadGodisnjePlanove().Select(i => new SelectListItem()
			{
				Text = i.Ak_godina,
				Value = i.Id_god.ToString()
			}));

			return PartialView("UrediNoviPlan", mjesecniModel);
		}

		[HttpPost]
		public ActionResult UrediNoviPlan(MjesecniModel _mjesecni_model)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			Mjesecni_plan mjesecni_plan = new Mjesecni_plan();
			mjesecni_plan.ID_pedagog = PlaniranjeSession.Trenutni.PedagogId;
			mjesecni_plan.Id_godina = _mjesecni_model.ID_GODINA;
			mjesecni_plan.Naziv = _mjesecni_model.MjesecniPlan.Naziv;
			mjesecni_plan.Opis = _mjesecni_model.MjesecniPlan.Opis;
			mjesecni_plan.ID_plan = _mjesecni_model.ID_PLAN;
			mjesecni_plan.Ak_godina = godisnji_planovi.ReadGodisnjiPlan(_mjesecni_model.ID_GODINA).Ak_godina;

			if (mjesecni_planovi.UpdateMjesecniPlan(mjesecni_plan))
			{
				return RedirectToAction("Index", new { Plan = _mjesecni_model.ID_GODINA });
			}
			return PartialView("UrediNoviPlan", _mjesecni_model);
		}

		public ActionResult UpdateNoviDetalji(int id, int id_plan, int id_god)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			ViewBag.Title = "Uređivanje mjesečnog plana";
			MjesecniModel mjesecniModel = new MjesecniModel();
			mjesecniModel.MjesecniPlan = new Mjesecni_plan();
			mjesecniModel.PodrucjaRada = new List<SelectListItem>(podrucja_rada.ReadPodrucjeRada().Select(i => new SelectListItem()
			{
				Text = i.Naziv.ToString(),
				Value = i.Id_podrucje.ToString()
			}));
			mjesecniModel.Aktivnosti = new List<SelectListItem>(aktivnosti.ReadAktivnost().Select(i => new SelectListItem()
			{
				Text = i.Naziv.ToString(),
				Value = i.Id_aktivnost.ToString()
			}));
			mjesecniModel.Subjekti = new List<SelectListItem>(subjekti.ReadSubjekti().Select(i => new SelectListItem()
			{
				Text = i.Naziv.ToString(),
				Value = i.ID_subjekt.ToString()
			}));
			mjesecniModel.GodisnjiPlanovi = new List<SelectListItem>(godisnji_planovi.ReadGodisnjePlanove().Select(i => new SelectListItem()
			{
				Text = i.Ak_godina,
				Value = i.Id_god.ToString()
			}));
			mjesecniModel.MjesecniPlan = mjesecni_planovi.ReadMjesecniPlan(id);
			mjesecniModel.MjesecniDetalji = mjesecni_planovi.ReadMjesecneDetalje(id);
			mjesecniModel.ID_PLAN = id;
			mjesecniModel.ID_GODINA = id_god;
			mjesecniModel.mjesecniDetalj = mjesecni_planovi.ReadMjesecniDetalj(id);

			return PartialView("UrediDetalje", mjesecniModel);
		}

		[HttpPost]
		public ActionResult UpdateNoviDetalji(MjesecniModel _mjesecni_model)
		{
			if (PlaniranjeSession.Trenutni.PedagogId <= 0 || !Request.IsAjaxRequest())
			{
				return RedirectToAction("Index", "Planiranje");
			}
			if (_mjesecni_model.mjesecniDetalj.Suradnici != null &&
				_mjesecni_model.mjesecniDetalj.Aktivnost != null &&
				_mjesecni_model.mjesecniDetalj.Podrucje != null &&
				_mjesecni_model.mjesecniDetalj.Biljeska != null &&
				_mjesecni_model.mjesecniDetalj.Br_sati > 0 &&
				DateTime.Compare(_mjesecni_model.mjesecniDetalj.Vrijeme, DateTime.Now.Date) > 0 &&
				mjesecni_planovi.UpdateMjesecniDetalj(_mjesecni_model.mjesecniDetalj))
			{
				_mjesecni_model.MjesecniDetalji = mjesecni_planovi.ReadMjesecneDetalje(_mjesecni_model.ID_PLAN);
				return RedirectToAction("PrikaziDetalje", new { id = _mjesecni_model.ID_PLAN, id_god = _mjesecni_model.ID_GODINA });
			}
			else
			{
				if (DateTime.Compare(_mjesecni_model.mjesecniDetalj.Vrijeme, DateTime.Now.Date) <= 0 || (_mjesecni_model.mjesecniDetalj.Vrijeme.Year < 2))
				{
					ModelState.AddModelError("error_msg", "Datum mora biti noviji od trenutnog.");
				}
				else
				{
					ModelState.AddModelError("error_msg", " ");
				}
				_mjesecni_model.PodrucjaRada = new List<SelectListItem>(podrucja_rada.ReadPodrucjeRada().Select(i => new SelectListItem()
				{
					Text = i.Naziv.ToString(),
					Value = i.Id_podrucje.ToString()
				}));
				_mjesecni_model.Aktivnosti = new List<SelectListItem>(aktivnosti.ReadAktivnost().Select(i => new SelectListItem()
				{
					Text = i.Naziv.ToString(),
					Value = i.Id_aktivnost.ToString()
				}));
				_mjesecni_model.Subjekti = new List<SelectListItem>(subjekti.ReadSubjekti().Select(i => new SelectListItem()
				{
					Text = i.Naziv.ToString(),
					Value = i.ID_subjekt.ToString()
				}));
				_mjesecni_model.GodisnjiPlanovi = new List<SelectListItem>(godisnji_planovi.ReadGodisnjePlanove().Select(i => new SelectListItem()
				{
					Text = i.Ak_godina,
					Value = i.Id_god.ToString()
				}));
				return PartialView("UrediDetalje", _mjesecni_model);
			}
		}
	}
}
