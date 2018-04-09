using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Angular_Coding_Challenge.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }

        [Index(IsUnique = true)]
        [StringLength(400)]
        public string Username { get; set; }

        /// <summary>
        /// password hash stored in base64 representation
        /// </summary>
        public string Password { get; set; }

        public string Salt { get; set; }

        public bool IsAdministrator { get; set; }

        public User() {
            Indices = new List<Index>();
        }

        public virtual ICollection<Index> Indices { get; set; }
    }

    public partial class ChallengeContext : DbContext {

        public DbSet<User> Users { get; set; }

    }
}