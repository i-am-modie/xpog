﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xpog.Entities;

namespace Xpog.Enitities.Expenses
{
    public abstract class BaseExpense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public ExpenseData ExpenseData { get; set; }
    }
}
