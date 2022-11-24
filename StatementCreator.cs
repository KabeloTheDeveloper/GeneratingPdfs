using pdfs.Moldels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace pdfs.Repositories
{
    public class StatementCreator
    {
        GetData _data = new GetData();
        public  IDictionary<Company, List<Transaction>> SortEachComanyToItsTransactions(List<Transaction> transactions)
        {
            IDictionary<Company, List<Transaction>> compTrans = new Dictionary<Company, List<Transaction>>();
            if (transactions.Count != 0)
            {
                List<string> accNumbs = new List<string>();
                foreach (Transaction transaction in transactions)
                {
                    if (!(accNumbs.Contains(transaction.AccNumber)))
                    {
                        accNumbs.Add(transaction.AccNumber);
                    }
                    else
                    {
                        continue;
                    }

                }
                IDictionary<string, List<Transaction>> firstDictionary = GetLists(accNumbs);
                foreach (KeyValuePair<string, List<Transaction>> pair in firstDictionary)
                {
                    foreach (Transaction transaction in transactions)
                    {
                        if (transaction.AccNumber == pair.Key)
                        {
                            pair.Value.Add(transaction);
                        }
                    }
                }
                List<Company> companies = _data.GetSpecificCompanies(accNumbs);
                foreach (Company company in companies)
                {
                    foreach (KeyValuePair<string, List<Transaction>> pair in firstDictionary)
                    {
                        if (company.AccountNumber.Contains(pair.Key))
                        {
                            compTrans.Add(company, pair.Value);
                        }
                    }
                }
                return compTrans;
            }
            else
            {
                return new Dictionary<Company, List<Transaction>>();
            }
        }
        public static List<Statement> GenerateStatements(IDictionary<Company, List<Transaction>> info)
        {
            List<Statement> statements = new List<Statement>();
            foreach (var data in info)
            {
                Statement statement = new Statement()
                {
                    Customer = data.Key,
                    Transactions = data.Value,
                };
                
                foreach (Transaction trans in data.Value)
                {
                    statement.Total += decimal.Parse(trans.Amount, CultureInfo.InvariantCulture);
                    int daysDifference = DaysDifferenceCalculator(trans.Date);
                    if (daysDifference <= 30) { statement.Current += decimal.Parse(trans.Amount, CultureInfo.InvariantCulture); }
                    else if (daysDifference <= 60) { statement.ThirtyDays += decimal.Parse(trans.Amount, CultureInfo.InvariantCulture); }
                    else if (daysDifference <= 90) { statement.SixtyDays += decimal.Parse(trans.Amount, CultureInfo.InvariantCulture); }
                    else if (daysDifference <= 120) { statement.NinetyDays += decimal.Parse(trans.Amount, CultureInfo.InvariantCulture); }
                    else { statement.OneTwentyDays += decimal.Parse(trans.Amount, CultureInfo.InvariantCulture); }
                }
                statements.Add(statement);
            }
            return statements;
        }
        public static List<Statement> GenerateStatementWithMultiplePages(Company company, Dictionary<int, List<Transaction>> groups)
        {
            // variable to be returned
            List<Statement> statements = new List<Statement>();
            //page numbering variable
            int pageNumbering = 1;
            //totals variables
            decimal total = 0.0M; decimal month = 0.0M; decimal two = 0.0M; decimal current = 0.0M; decimal three = 0.0M; decimal four = 0.0M;
            //foreach dictionary, I create a statemet and calculate the totals.
            foreach (var data in groups)
            {
                Statement statement = new Statement()
                {
                    Customer = company,
                    Transactions = data.Value,
                    statementPageNumber = pageNumbering
                };
                foreach(Transaction tran in data.Value)
                {
                    total += decimal.Parse(tran.Debit, CultureInfo.InvariantCulture);
                    int daysDifference = DaysDifferenceCalculator(tran.Date);
                    if (daysDifference <= 30) { current += decimal.Parse(tran.Amount, CultureInfo.InvariantCulture); }
                    else if (daysDifference <= 60) { month += decimal.Parse(tran.Amount, CultureInfo.InvariantCulture); }
                    else if (daysDifference <= 90) { two += decimal.Parse(tran.Amount, CultureInfo.InvariantCulture); }
                    else if (daysDifference <= 120) { three += decimal.Parse(tran.Amount, CultureInfo.InvariantCulture); }
                    else { four += decimal.Parse(tran.Amount, CultureInfo.InvariantCulture); }
                }
                pageNumbering += 1;
                statements.Add(statement);
                foreach(Statement st in statements)
                {
                    st.Current = month; st.ThirtyDays = two; st.SixtyDays = three; st.NinetyDays = four; st.Total = total;
                }
            }
            return statements;
        }
        public  IDictionary<string, List<Transaction>> GetLists(List<string> accNums)
        {
            IDictionary<string, List<Transaction>> myLists = new Dictionary<string, List<Transaction>>();
            foreach (string a in accNums)
            {
                myLists.Add(a, new List<Transaction>());
            }
            return myLists;
        }
        public static  int DaysDifferenceCalculator(string transDate)
        {
            return (int)(DateTime.Now - DateTime.Parse(transDate)).TotalDays;
        }
        
    }
}