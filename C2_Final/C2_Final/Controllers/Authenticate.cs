using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using C2_Final.Data;
using C2_Final.Models;
using Microsoft.AspNetCore.Authorization;

namespace C2_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtAuthenticationManager _jwtAuthenticationManager;
        private readonly Bailey_FinancialContext _context;

        public AuthenticationController(JwtAuthenticationManager jwt, Bailey_FinancialContext context)
        {
            _jwtAuthenticationManager = jwt;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> Member_Login([FromBody] User user)
        {

            var memPull = from num in _context.Members where num.Email == user.email && num.Password == user.password select num;

            var memPullList = memPull.ToList();

            if (memPullList.Count() > 0)
            {
                var token = _jwtAuthenticationManager.Authenticate(memPullList[0].Email, memPullList[0].MemberId, "0");
                return token;
            } 
            else
            {
                return BadRequest("Member not found");
            }
        }

        [AllowAnonymous]
        [HttpPost("Employees")]
        public async Task<ActionResult<string>> Employee_Login([FromBody] User user)
        {

            var empPull = from var in _context.Employees where var.Email == user.email && var.Password == user.password select var;

            var empPullList = empPull.ToList();

            if (empPullList.Count() > 0)
            {
                var token = _jwtAuthenticationManager.Authenticate(empPullList[0].Email, empPullList[0].EmployeeId, "1");
                return token;
            }
            else
            {
                return BadRequest("Employee not found");
            }
        }
    }

    public class User
    {
        public string? email { get; set; }
        public string? password { get; set; }
    }
}