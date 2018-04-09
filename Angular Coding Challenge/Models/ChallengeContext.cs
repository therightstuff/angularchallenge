using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Angular_Coding_Challenge.Models
{
	public partial class ChallengeContext : DbContext
	{
        public ChallengeContext() : base("Data Source=.\\SQLEXPRESS;Initial Catalog=angular;User Id=username;Password=password;") {
            Database.SetInitializer<ChallengeContext>(new ChallengeDbInitializer());
        }
    }

    public class ChallengeDbInitializer : CreateDatabaseIfNotExists<ChallengeContext>
    {
        protected override void Seed(ChallengeContext context)
        {
            IList<Index> defaultIndices = new List<Index>();

            defaultIndices.Add(new Index { Name = "SPDR S&P 500 ETF", Ticker = "SPY", Price = 272.12M });
            defaultIndices.Add(new Index { Name = "SPDR Dow Jones Industrial Average ETF", Ticker = "DIA", Price = 250.80M });
            defaultIndices.Add(new Index { Name = "S&P 500 High Dividend Index", Ticker = "SPYD", Price = 36.58M });
            defaultIndices.Add(new Index { Name = "Vanguard Total Stock Market ETF", Ticker = "VTI", Price = 139.34M });

            context.Indices.AddRange(defaultIndices);

            IList<User> defaultUsers = new List<User>();

            // passwords are "secure password <username>" eg. "secure password admin"
            defaultUsers.Add(
                new User { IsAdministrator = true, Username = "admin", Password = "D14rXlIwL44dXEQQaSNCXe/osi0pmtn4g3/oXVyFdGs=", Salt = "random salt 1" });
            defaultUsers.Add(new User { Username = "A", Password = "DdXe3w7Ay1QDthKnIXbxH4vvn15yHaEen/v7gw3aTtY=", Salt = "random salt 2" });
            defaultUsers.Add(new User { Username = "B", Password = "jq4tPr8nLRT69XZKrNl69s2QcReYwf3yhkOvDnSrBkM=", Salt = "random salt 3" });

            context.Users.AddRange(defaultUsers);

            context.SaveChanges();

            base.Seed(context);
        }
    }
}
