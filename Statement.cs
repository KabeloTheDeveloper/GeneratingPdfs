using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pdfs.Moldels
{
    public class Statement
    {
        public Company Customer { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public DateTime StatementDate { get; set; } = DateTime.Now;
        public int statementPageNumber { get; set; } = 1;
        public decimal Current { get; set; } = 0.0M;
        public decimal ThirtyDays { get; set; } = 0.0M;
        public decimal SixtyDays { get; set; } = 0.0M;
        public decimal NinetyDays { get; set; } = 0.0M;
        public decimal OneTwentyDays { get; set; } = 0.0M;
        public decimal Total { get; set; } = 0.0M;
    }
}