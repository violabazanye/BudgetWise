using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BudgetWise
{
    public class Wallets_Table
    {
        public Wallets_Table(string initialInput, string rent, string insurance, string school, string groceries, 
            string fuel, string savings, string others, string balance, string expenditure)
        {
            this.InitialInput = initialInput;

            this.Rent = rent;
            
            this.Insurance = insurance;
            
            this.School = school;
            
            this.Groceries = groceries;
            
            this.Fuel = fuel;
            
            this.Savings = savings;
            
            this.Others = others;
            
            this.Balance = balance;

            this.Expenditure = expenditure;
        }

        public string Id { get; set; }

        [JsonProperty(PropertyName = "initialinput")]
        public string InitialInput { get; set; }

        [JsonProperty(PropertyName = "rent")]
        public string Rent { get; set; }

        [JsonProperty(PropertyName = "insurance")]
        public string Insurance { get; set; }

        [JsonProperty(PropertyName = "school")]
        public string School { get; set; }

        [JsonProperty(PropertyName = "groceries")]
        public string Groceries { get; set; }

        [JsonProperty(PropertyName = "fuel")]
        public string Fuel { get; set; }

        [JsonProperty(PropertyName = "savings")]
        public string Savings { get; set; }

        [JsonProperty(PropertyName = "others")]
        public string Others { get; set; }

        [JsonProperty(PropertyName = "balance")]
        public string Balance { get; set; }

        [JsonProperty(PropertyName = "expenditure")]
        public string Expenditure { get; set; }
    }

}
