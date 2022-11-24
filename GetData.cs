using Microsoft.AspNetCore.Http;
using pdfs.Moldels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace pdfs.Repositories
{
    public class GetData
    {
        private string compPath = @"C:\Users\kabelos\Desktop\CompaniesData\Companies.csv";
        private string uploadedPath = @"C:\Users\kabelos\Desktop\pdfs\pdfs\UploadedFiles";
        public string CompsPath { get { return compPath; } }
        public string UploadedPath { get { return uploadedPath; } }
        public List<Company> GetAllCompanies()
        {
            
            string[] csvLines = File.ReadAllLines(CompsPath);
            List<Company> companies = new List<Company>();
            foreach (string line in csvLines)
            {
                Company company = new Company();
                string[] rowData = line.Split(';');
                company.Name = rowData[0];
                company.Address = rowData[1];
                company.AccountNumber = rowData[2];
                company.Attn = rowData[3];
                companies.Add(company);
            }
            companies.RemoveAt(0);
            return companies;
        }
        public List<Company> GetSpecificCompanies(List<string> accNums)
        {
           
            string[] cvsLines = File.ReadAllLines(CompsPath);
            List<Company> companies = new List<Company>();
            foreach (string line in cvsLines)
            {
                foreach (string acc in accNums)
                {
                    if (line.Contains(acc))
                    {
                        Company company = new Company();
                        string[] rowData = line.Split(';');
                        company.Name = rowData[0];
                        company.Address = rowData[1];
                        company.AccountNumber = rowData[2];
                        company.Attn = rowData[3];
                        companies.Add(company);
                    }
                }
            }
            return companies;
        }
        public string SaveFile(IFormFile file)
        {
            var filePath = UploadedPath+file.FileName;
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                file.CopyTo(stream);
            return filePath;
        }
        public List<Transaction> GetTransactions(string path)
        {
            string[] cvsLines = File.ReadAllLines(path);
            List<Transaction> transactions = new List<Transaction>();
            foreach (string line in cvsLines)
            {
                Transaction transaction = new Transaction();
                string[] rowData = line.Split(',');
                transaction.AccNumber = rowData[0];
                transaction.Date = rowData[1];
                transaction.RaNumber = rowData[2];
                transaction.Voucher = rowData[3];
                transaction.OrderNumebr = rowData[4];
                transaction.DriverName = rowData[5];
                transaction.Details = rowData[6];
                transaction.Debit = rowData[7];
                transaction.Credit = rowData[8];
                transaction.Amount = rowData[9];
                transactions.Add(transaction);
            }
            transactions.RemoveAt(0);
            return transactions;
        }

    }
}