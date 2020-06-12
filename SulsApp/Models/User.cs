namespace SulsApp.Models
{
    using System;
    using System.Collections.Generic;
    using SIS.MvcFramework;

    public class User : IdentityUser<string>
    {
        public User()
        {
            this.Id = Guid.NewGuid().ToString();
            // ReSharper disable once VirtualMemberCallInConstructor
            this.Submissions = new HashSet<Submission>();
        }

        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
