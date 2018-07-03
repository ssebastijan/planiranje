﻿using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Planiranje.Models;
using System.Web.Security;
using System.Runtime.Remoting.Messaging;
using System.Net.Mail;
using System.Data.SqlClient;

namespace Planiranje.Controllers
{
	public class PlaniranjeController : Controller
    {
		private BazaPodataka baza = new BazaPodataka();
        

		[HttpGet]
		public ActionResult Prijava()
		{
			PlaniranjeSession.Trenutni.PedagogId = 0;
			ViewBag.Title = "Prijava";
			return View();
		}

		[HttpPost]
		public ActionResult Prijava(Pedagog p)
		{
			Pedagog pedagog = baza.Pedagog.SingleOrDefault(ped => ped.Email == p.Email && ped.Lozinka == p.Lozinka);
			if (pedagog != null)
			{
				PlaniranjeSession.Trenutni.PedagogId = pedagog.Id_Pedagog;
				return RedirectToAction("Index");
			}
			else
			{
				return View("Prijava");
			}
		}
		public ActionResult Index()
		{
			if (PlaniranjeSession.Trenutni.PedagogId > 0)
			{
				ViewBag.Title = "Pocetna";
				return View();
			}
			return RedirectToAction("Prijava", "Planiranje");
		}
		public ActionResult ZaboravljenaLozinka()
		{
			if (PlaniranjeSession.Trenutni.PedagogId == 0)
			{
				ViewBag.Title = "Zaboravljena lozinka";
				return View();
			}
			return RedirectToAction("Prijava", "Planiranje");
		}

        [HttpPost]
        public ActionResult ZaboravljenaLozinka(Pedagog p)
        {
            Pedagog pedagog = baza.Pedagog.SingleOrDefault(ped => ped.Email == p.Email);
            if (pedagog == null)
            {
                ViewBag.Title = "Zaboravljena lozinka";
                return View();
            }
            string[] abeceda = { "a","b", "c", "d", "e", "f", "g" };
            Random r = new Random();
            string lozinka="";
            for(int i = 0; i < 6; i++)
            {
                lozinka += abeceda[r.Next(0, abeceda.Length-1)];
            }
            pedagog.Lozinka = lozinka;

            baza.Pedagog.SqlQuery("UPDATE pedagog SET lozinka = @lozinka WHERE email = @email", new SqlParameter("@email",pedagog.Email)
                ,new SqlParameter("@lozinka",pedagog.Lozinka));
            

                MailMessage mail = new MailMessage("noreply@planiranje.com", pedagog.Email,
                    "Podaci o promjeni lozinke", "Vaša nova lozinka je " + lozinka + ". Molimo promijenite je u što kraćem roku.");
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 25;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = true;
                //smtp.Credentials = new System.Net.NetworkCredential()
                try
                {
                    smtp.Send(mail);
                }
                catch
                {

                }            

            baza.SaveChanges();
            return View("Prijava");
        }
		public ActionResult Registracija()
		{
			if (PlaniranjeSession.Trenutni.PedagogId == 0)
			{
				ViewBag.Title = "Registracija";
				return View();
			}
			return RedirectToAction("Prijava", "Planiranje");
		}

        [HttpPost]
        public ActionResult Registracija(Pedagog p)
        {
            Pedagog ped = baza.Pedagog.SingleOrDefault(pedagog => pedagog.Email == p.Email);
            if (ped != null)
            {
                return RedirectToAction("Registracija");
            }
            p.Titula = "student";
            p.Id_skola = 1;
            p.Licenca = new DateTime(2020, 6, 14, 14, 55, 10);
            p.Aktivan = '1';

            baza.Pedagog.Add(p);
            baza.SaveChanges();
            
            return RedirectToAction("Prijava");
        }
    }
}