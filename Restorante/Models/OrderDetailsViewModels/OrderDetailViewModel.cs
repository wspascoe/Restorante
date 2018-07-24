using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restorante.Models.OrderDetailsViewModels
{
    public class OrderDetailViewModel
    {
        public OrderHeader OrderHeader { get; set; }

        public List<OrderDetails> OrderDetail { get; set; }
    }
}

