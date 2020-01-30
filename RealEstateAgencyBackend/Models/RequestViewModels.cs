﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealEstateAgencyBackend.Models
{
    // 1 class per file
    public class RentalRequestCreateViewModel
    {
        public String Title { get; set; }

        public String Description { get; set; }

        public Int32 Area { get; set; }

        public String PrefferedAddress { get; set; }

        public Int32 MaxPrice { get; set; }

        public String UserId { get; set; }
    }

    public class RentalRequestViewModel
    {
        public int Id { get; set; }

        public String Title { get; set; }

        public String Description { get; set; }

        public Int32 Area { get; set; }

        public String PrefferedAddress { get; set; }

        public Int32 MaxPrice { get; set; }

        public String UserId { get; set; }
    }

	public class RentalRequestPageView
	{
		public List<RentalRequestViewModel> RentalRequests { get; set; }
		public Int32 PageCount { get; set; }
		public Int32 CurrentPage { get; set; }
	}
}