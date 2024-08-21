using Microsoft.AspNetCore.Mvc;
using Owasp2021Top10.Models;
using System.Diagnostics;
using System.Text;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore; // 添加這行

namespace Owasp2021Top10.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShoppingContext _context;

        public HomeController(ShoppingContext context)
        {   
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Products.ToList());
        }

        [HttpPost]
        public IActionResult Search(string query)
        {
            
            var products = _context.Products
                .FromSqlRaw($"SELECT * FROM Products WHERE Name LIKE '%{query}%'")
                .ToList();

            var result = products.Select(p => new { p.Id, p.Name, p.Price }).ToList();

            return Json(result);
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, message = "Username and password cannot be empty." });
            }

            try
            {
                var encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
                var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == encodedPassword);

                if (user != null)
                {
                    var options = new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(1)
                    };
                    Response.Cookies.Append("username", username, options);

                    return Json(new { success = true, message = "Login successful!" });
                }
                return Json(new { success = false, message = "Login failed. Please check your username and password." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            user.Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(user.Password));
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ProductDetails(int id)
        {
            var product = _context.Products.Find(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var username = Request.Cookies["username"];
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { success = false, message = "User not logged in or invalid session." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return Json(new { success = false, message = "User does not exist." });
            }

            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product does not exist." });
            }
            else if (product.Quantity == 0)
            {
                return Json(new { success = false, message = "Product out of stock." });
            }
            else
            {
                product.Quantity -= 1;
                _context.SaveChanges();
            }

            user.Bill += product.Price;
            _context.SaveChanges();

            return Json(new { success = true, message = "Product added to cart successfully." , bill = user.Bill, quantity = product.Quantity});
        }

        [HttpPut]
        public IActionResult Restock(int id, int quantity)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found", success=false });
            }

            if (quantity <= 0)
            {
                return BadRequest(new { message = "Quantity must be greater than zero" , success=false });
            }

            product.Quantity += quantity;
            _context.SaveChanges();

            return Ok(new { message = "Restock successful", product,success = true });
        }
        public IActionResult Error()
        {
            // OWASP A05:2021 - Security Misconfiguration
            // Detailed error information exposed
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}