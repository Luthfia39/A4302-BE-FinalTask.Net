﻿using DAL.DTO.Req;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Services.Interfaces
{
    public interface ILoanServices
    {
        Task<string> CreateLoan(ReqLoanDto loan);
        Task<string> UpdateLoan(string id, ReqUpdateLoanDto updateLoan);
        Task<List<ResListLoanDto>> LoanList(string status);

        Task<List<ResListLoanDto>> GetLoansByBorrowerId(string borrowerId);

        Task<ResLoanDto> GetLoansById(string id);

        //payment process
        Task<string> ProcessPayment(string loanId, decimal amountOfPayment);
    }
}
