﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PWET.Domain
{
    public class ExpenseGroup
    {
        public ExpenseGroup()
        {
            Expenses = new HashSet<Expense>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(250)]
        public string Description { get; set; }

        public int ExpenseGroupStatusId { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; }

        public virtual ExpenseGroupStatus ExpenseGroupStatus { get; set; }
    }
}
