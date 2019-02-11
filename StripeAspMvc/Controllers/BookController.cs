using StripeAspMvc.Models.Book;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Web.Mvc;
using Stripe;

namespace StripeAspMvc.Controllers
{
	public class BookController : Controller
	{
		private readonly string stripePublishableKey;
		public BookController()
		{
			stripePublishableKey = ConfigurationManager.AppSettings["StripeApi.PublishableKey"];
		}

		// embedded form
		public ActionResult Index()
		{
			var model = new IndexViewModel() { StripePublishableKey = stripePublishableKey };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken()]
		public ActionResult Charge(ChargeViewModel chargeViewModel)
		{
			Debug.WriteLine(chargeViewModel.StripeEmail);
			Debug.WriteLine(chargeViewModel.StripeToken);

			return RedirectToAction("Confirmation");
		}

		public ActionResult Confirmation()
		{
			return View();
		}

		public ActionResult Custom()
		{
			var model = new CustomViewModel() { StripePublishableKey = stripePublishableKey, PaymentForHidden = true };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken()]
		public ActionResult Custom(CustomViewModel customViewModel)
		{
			customViewModel.PaymentForHidden = false;
			var chargeOptions = new Stripe.ChargeCreateOptions()
			{
				//required
				Amount = 3900,
				Currency = "usd",
				//Source = new StripeSourceOptions() { TokenId = customViewModel.StripeToken },
			 
				 
				//SourceTokenOrExistingSourceId = customViewModel.StripeToken,
				//optional
				Description = string.Format("JavaScript Framework Guide Ebook for {0}", customViewModel.StripeEmail),
				ReceiptEmail = customViewModel.StripeEmail
			};

			var chargeService = new Stripe.ChargeService();

			try
			{
				var stripeCharge = chargeService.Create(chargeOptions);
			}
			catch (StripeException stripeException)
			{
				Debug.WriteLine(stripeException.Message);
				ModelState.AddModelError(string.Empty, stripeException.Message);
				return View(customViewModel);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				ModelState.AddModelError(string.Empty, ex.Message);
				return View(customViewModel);
			}

			//LOG stripeCharge log Id property in to the DB to refrence the transaction later

			return RedirectToAction("Confirmation");
		}

	}
}