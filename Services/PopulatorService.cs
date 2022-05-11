using WebApi.Entities;
using WebApi.Models.Transactions;
using WebApi.Models.Users;

namespace WebApi.Services
{
    public class PopulatorService
    {
        public BasicInfoResponse PopulateBasicInfoResponse(User dbUser)
        {
            var mdl = new BasicInfoResponse();

            mdl.Username = dbUser.Username;
            mdl.Email = dbUser.Email;
            mdl.LastName = dbUser.LastName;
            mdl.FirstName = dbUser.FirstName;

            return mdl;
        }

        public TransactionEntry PopulateTransactionEntry(Transaction dbTxn)
        {
            var mdl = new TransactionEntry();

            mdl.Id = dbTxn.Id;
            mdl.Amount = dbTxn.Amount;
            mdl.CreatedDate = dbTxn.Created;

            return mdl;
        }
    }
}
