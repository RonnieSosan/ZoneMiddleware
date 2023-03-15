using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class Card
    {
        public string issuer_nr { get; set; }

        public string pan { get; set; }
        public string seq_nr { get; set; }
        public string card_program { get; set; }

        public string default_account_type { get; set; }

        public string card_status { get; set; }

        public string expiry_date { get; set; }

        public DateTime date_issued { get; set; }

        public DateTime date_activated { get; set; }

        public DateTime date_deleted { get; set; }

        public string issuer_reference { get; set; }

        public string last_updated_user { get; set; }

        public string customer_id { get; set; }
        public string branch_code { get; set; }

        public string batch_nr { get; set; }

        public Account TheAccount { get; set; }

        public string hold_rsp_code { get; set; }
        public string last_updated_date { get; set; }
    }

    public class Account
    {
        public string AccountNumber { get; set; }
    }
}
