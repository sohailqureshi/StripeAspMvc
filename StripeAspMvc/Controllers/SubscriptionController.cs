using StripeAspMvc.Models.Book;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Stripe;
using System;
using System.Net;
using System.Threading.Tasks;

namespace StripeAspMvc.Controllers
{
	public class SubscriptionController : Controller
	{
		private readonly string stripePublishableKey;
		public SubscriptionController()
		{
			stripePublishableKey = ConfigurationManager.AppSettings["StripeApi.PublishableKey"];
			//StripeConfiguration.SetApiKey(stripePublishableKey);
		}

		// GET: Subscription
		public ActionResult Index()
		{
			List<SubscriptionProductPlans> prodPlans = new List<SubscriptionProductPlans>();

			var productService = new ProductService();
			var productOptions = new ProductListOptions
			{
				Limit = 5,
			};

			foreach (var prod in productService.List(productOptions))
			{
				var planService = new PlanService();
				var planOptions = new PlanListOptions
				{
					Limit = 5,
					ProductId = prod.Id
				};

				prodPlans.Add(new SubscriptionProductPlans
				{
					Product = prod,
					Plan = planService.List(planOptions)
				});
			}									

			return View(prodPlans);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// 
		[HttpGet]
		public ActionResult Create()
		{
			var model = new SubscriptionPlanViewModel()
			{
				Name = "",
				Amount = "0",
				Currency = "gbp",
				Interval = "month"
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken()]
		public ActionResult Create(SubscriptionPlanViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			try
			{
				var productOptions = new ProductCreateOptions
				{
					Name = viewModel.Name,
					Type = "service",
				};
				var productService = new ProductService();
				Product product = productService.Create(productOptions);

				if (product != null)
				{
					var planOptions = new PlanCreateOptions();
					planOptions.Currency = viewModel.Currency;
					planOptions.Interval = viewModel.Interval;
					planOptions.Nickname = product.Name;
					planOptions.Amount = Convert.ToInt64(Convert.ToDecimal(viewModel.Amount) * 100);
					planOptions.ProductId = product.Id;

					var planService = new PlanService();
					Plan plan = planService.Create(planOptions);
				}
			}
			catch(Exception ex)
			{
				var msg = ex.Message;
			}

			return RedirectToAction("Index");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<ActionResult> Edit(string id)
		{
			var productService = new ProductService();
			var prod = await productService.GetAsync(id);
			if(prod == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

			var planService = new PlanService();
			var planOptions = new PlanListOptions
			{
				Limit = 3,
				ProductId = prod.Id
			};

			var prodPlans = await planService.ListAsync(planOptions);
			var plan = prodPlans.Data.Find(x => x.ProductId.Equals(prod.Id));

			var model = new SubscriptionPlanViewModel()
			{
				ProductId = prod.Id,
				PlanId = plan.Id,
				Name = prod.Name,
				Amount = Convert.ToString(Convert.ToDecimal(plan.Amount)/100).ToString(),
				Currency = plan.Currency,
				Interval = plan.Interval
			};

			return View(model);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(SubscriptionPlanViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				var productService = new ProductService();
				var prod = await productService.GetAsync(viewModel.ProductId);
				if (prod == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

				var planService = new PlanService();
				var plan = await planService.GetAsync(viewModel.PlanId);
				if (plan == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

				var options = new ProductUpdateOptions
				{
					Name = viewModel.Name
				};
				Product product = productService.Update(prod.Id, options);

				return RedirectToAction("Index");
			}

			return View(viewModel);
		}
	}
}
