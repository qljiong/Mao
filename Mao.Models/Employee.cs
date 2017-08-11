using System;

namespace Mao.Models
{
    public class Employee
    {
        public virtual Guid Id { get; protected set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Store { get; set; }
    }
}
