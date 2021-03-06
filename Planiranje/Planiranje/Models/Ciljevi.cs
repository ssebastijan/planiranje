﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Planiranje.Models
{
    public class Ciljevi
	{
		public int Red_br { get; set; }
		[Required(ErrorMessage = "Obavezno polje.")]
		[DisplayName("Id")]
		public int ID_cilj { get; set; }
		[Required(ErrorMessage = "Obavezno polje.")]
		[DisplayName("Naziv")]
		public string Naziv { get; set; }
    }
}