using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Angular_Coding_Challenge.Models
{
    public class Index
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IndexId { get; set; }

        public string Name { get; set; }

        [Index(IsUnique = true)]
        [StringLength(400)]
        public string Ticker { get; set; }

        public decimal Price { get; set; }

        public Index()
        {
            Users = new List<User>();
        }

        public virtual ICollection<User> Users{ get; set; }
    }
    public partial class ChallengeContext : DbContext
    {

        public DbSet<Index> Indices { get; set; }

    }
}