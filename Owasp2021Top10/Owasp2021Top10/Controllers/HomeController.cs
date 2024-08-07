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
            // OWASP A03:2021 - Injection
            // 注意：這仍然是不安全的，因為它直接將用戶輸入拼接到查詢中
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
                    // Set the cookie with the username without security settings
                    var options = new CookieOptions
                    {
                        Expires = DateTime.UtcNow.AddDays(1),
                        Secure = true,
                        SameSite = SameSiteMode.None
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
            // OWASP A02:2021 - Cryptographic Failures
            user.Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(user.Password));
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ProductDetails(int id)
        {
            // OWASP A01:2021 - Broken Access Control
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

            user.Bill += product.Price;
            _context.SaveChanges();

            return Json(new { success = true, message = "Product added to cart successfully." , bill = user.Bill});
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