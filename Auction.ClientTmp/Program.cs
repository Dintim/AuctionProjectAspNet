﻿using EAuction.BLL.Services;
using EAuction.BLL.Sevices;
using EAuction.BLL.ViewModels;
using EAuction.Core.DataModels;
using EAuction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EAuction.ClientTmp
{
    class Program
    {
        public enum TransType { Deposit, Withdraw }
        static void Main(string[] args)
        {
            Console.WriteLine(TransType.Deposit.ToString());
        }
    }
}
