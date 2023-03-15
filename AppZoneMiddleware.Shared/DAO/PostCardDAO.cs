using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility.EntitySystem;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.DAO
{
    public class PostCardDAO : DAOSystem
    {
        public PostCardDAO() : base("PostCard")
        {
        }

        public IList<Card> RetrieveCard(string pan, string expiryDate, string tableName)
        {
            string NowDate = DateTime.Now.Date.ToString("yyMM");
            string queryString = string.Format("SELECT * from [{0}] where pan like '{1}' AND expiry_date like '{2}' AND expiry_date >= '{3}'", tableName, pan, expiryDate, NowDate);
            OleDbCommand command = new System.Data.OleDb.OleDbCommand(queryString);
            return RetrieveList<Card>(command);
        }

        public IList<Card> RetrieveCard(string pan)
        {
            string NowDate = DateTime.Now.Date.ToString("yyMM");
            string queryString = string.Format("SELECT * from pc_cards where pan like '{0}' AND expiry_date >= '{1}' ORDER BY seq_nr", pan, NowDate);
            OleDbCommand command = new System.Data.OleDb.OleDbCommand(queryString);
            return RetrieveList<Card>(command);
        }

        public IList<Card> RetrieveCardByAccountNumber(string accountNumber)
        {
            string queryString = string.Format("SELECT * from pc_card_accounts WHERE account_id LIKE '{0}'", accountNumber);
            OleDbCommand command = new System.Data.OleDb.OleDbCommand(queryString);
            return RetrieveList<Card>(command);
        }

        public void HotListCard(HotlistCardRequest postCrad, string PostCardTableName)
        {
            string postCardQuery = String.Format("Update {0} set hold_rsp_code='{1}', last_updated_user='{4}', last_updated_date='{2}' where pan='{3}' ;", PostCardTableName, postCrad.ReasonForHotList, DateTime.Now, postCrad.CardPAN, postCrad.last_updated_user);
            StartSQL(postCardQuery);
            CommitSQL();
        }
    }
}
