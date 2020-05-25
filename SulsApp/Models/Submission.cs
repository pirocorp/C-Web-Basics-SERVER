namespace SulsApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Submission
    {
        public Submission()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        [Required]
        [MaxLength(800)]
        public string Code { get; set; }

        public int AchievedResult { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ProblemId { get; set; }

        /// <summary>
        /// virtual if you want to enable Lazy loading
        /// </summary>
        public virtual Problem Problem { get; set; }

        public string UserId { get; set; }
        
        /// <summary>
        /// virtual if you want to enable Lazy loading
        /// Do not use it :)
        /// </summary>
        public virtual User User { get; set; }
    }
}
