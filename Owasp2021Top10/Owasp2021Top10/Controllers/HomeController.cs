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
                .Where(p => EF.Functions.Like(p.Name, $"%{query}%"))
                .ToList();
            return View("Index", products);
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // OWASP A07:2021 - Identification and Authentication Failures
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                return RedirectToAction("Index");
            }
            return View("Login");
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

        public IActionResult ProductDetails(int id)
        {
            // OWASP A01:2021 - Broken Access Control
            var product = _context.Products.Find(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            // OWASP A04:2021 - Insecure Design
            // No stock check, allowing potential overselling
            // No CSRF protection
            return RedirectToAction("Index");
        }

        public IActionResult Error()
        {
            // OWASP A05:2021 - Security Misconfiguration
            // Detailed error information exposed
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}