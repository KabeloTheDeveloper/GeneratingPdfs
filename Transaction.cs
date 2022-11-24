using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pdfs.Moldels
{
    public class Transaction
    {
        public string AccNumber { get; set; } = "";
        public string Date { get; set; } = "";
        public string RaNumber { get; set; } = "";
        public string Voucher { get; set; } = "";
        public string OrderNumebr { get; set; } = "";
        public string DriverName { get; set; } = "";
        public string Details { get; set; } = "";
        public string Debit { get; set; } = "";
        public string Credit { get; set; } = "";
        public string Amount { get; set; } = "";
    }
}