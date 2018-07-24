using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restorante.Models.OrderDetailsViewModels
{
    public class OrderListViewModel
    {
        public IList<OrderDetailViewModel> Orders{ get; set; }
        public PagingInfo PagingInfo { get; set; }

    }
}