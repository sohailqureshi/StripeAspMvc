using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StripeAspMvc.Models.Book
{
	public class IndexViewModel
	{
		public string StripePublishableKey { get; set; }
	}

	public class ChargeViewModel
	{
		public string StripeToken { get; set; }
		public string StripeEmail { get; set; }
	}

	public class CustomViewModel
	{
		public string StripePublishableKey { get; set; }
		public string StripeToken { get; set; }
		public string StripeEmail { get; set; }
		public bool PaymentForHidden { get; set; }
		public string PaymentForHiddenCss
		{
			get
			{
				return PaymentForHidden ? "hidden" : "";
			}
		}
	}

	public class SubscriptionProductPlans
	{
		public Stripe.Product Product { get; set; }

		public Stripe.StripeList<Stripe.Plan> Plan { get; set; }
	}

	public class SubscriptionPlanViewModel
	{
		public string ProductId { get; set; }
		public string PlanId { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Amount { get; set; }

		[Required]
		public string Currency { get; set; }

		[Required]
		public string Interval { get; set; }
	}
}