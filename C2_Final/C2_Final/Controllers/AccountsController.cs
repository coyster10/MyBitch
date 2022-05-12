#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using C2_Final.Data;
using C2_Final.Models;

namespace C2_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly Bailey_FinancialContext _context;


        public AccountsController(Bailey_FinancialContext context)
        {
            _context = context;
        }
        /*-------------------------------------------------------- Get Accounts -------------------------------------------------------------------------------------------*/
        [Authorize]
        [HttpGet("Accounts")]
        public ActionResult<IEnumerable<Account>> GetAccounts()
        {
            var currentUser = HttpContext.User;
            var userid = currentUser.Claims.FirstOrDefault(c => c.Type == "userID").Value;
            var admin = currentUser.Claims.FirstOrDefault(c => c.Type == "admin").Value;

            if(admin == "0")
            {
                IEnumerable<Account> results = from num in _context.Accounts where num.MemberId == userid select num;

                var acct = results.ToList();

                return acct;
            }
            else
            {
                var lookup = from num in _context.Accounts where true select num;
                var accts = lookup.ToList();
                return accts;
            }
        }

        /*--------------------------------------------------------- Withdraw funds -----------------------------------------------------------------------------------------*/
        [Authorize]
        [HttpPost("Withdraw_Funds")]
        public async Task<ActionResult<Account>> Withdraw(string acctNum, string value)
        {
            var account = await _context.Accounts.FindAsync(acctNum);

            int valueInt = int.Parse(value);
            int actvalue = int.Parse(account.Value);
            if(actvalue < 1)
            {
                return BadRequest("Insufficent Funds");
            }
            else
            {
                var updatedValue = actvalue - valueInt;

                if(updatedValue < 0)
                {
                    return BadRequest("Insufficent Funds");
                }
                account.Value = updatedValue.ToString();

                await _context.SaveChangesAsync();

                return Ok("Successfully withdrew funds");
            }

            var currentUser = HttpContext.User;
            var userid = currentUser.Claims.FirstOrDefault(c => c.Type == "userID").Value;

        }

        /* -------------------------------------------------------- Add Funds ------------------------------------------------------------------------------------------------*/
        [Authorize]
        [HttpPost("Add_Funds")]
        public async Task<ActionResult<Account>> AddFunds(string acctNum, string amt)
        {
            var currentUser = HttpContext.User;
            var admin = currentUser.Claims.FirstOrDefault(c => c.Type == "admin").Value;

            if(admin == "0")
            {
                return BadRequest("Not authorized to add funds, please see an associate");
            }
            else
            {
                var account = await _context.Accounts.FindAsync(acctNum);
                int amount = int.Parse(amt);
                int accountValue = int.Parse(account.Value);
                if(amount > 10000)
                {
                    return BadRequest("Cannot deposit more than $10,000 at a time");
                }

                var updatedValue = accountValue + amount;
                account.Value = updatedValue.ToString();
                await _context.SaveChangesAsync();
                return Ok("Successfully deposited funds");
            }






            return BadRequest();
        }

        /*----------------------------------------------------------- Add Account -----------------------------------------------------------------------------*/
        [Authorize]
        [HttpPost("Add_Account")]
        public async Task<ActionResult<Account>> AddAccount(Account account)
        {

            _context.Accounts.Add(account);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AccountExists(account.AccountNumber))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAccount", new { id = account.AccountNumber }, account);
        }

        /*----------------------------------------------------------- Get Institutions ------------------------------------------------------------------------*/
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Institution>>> GetInstitutions()
        {
            return await _context.Institutions.ToListAsync();
        }
        private bool AccountExists(string id)
        {
            return _context.Accounts.Any(e => e.AccountNumber == id);
        }
    }
}
