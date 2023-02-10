using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.Enum
{
	public enum PaymentStatus
	{
		AwaitingPayment,
		PaymentProcessing,
		Paid,
		Rejected,
		ApprovedForDelayedPayment
	}
}
