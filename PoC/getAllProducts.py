import requests

cookie = {
    'username':'user1'
}

url = 'https://localhost:7057/Home/AddToCart'

for i in range(1,4):
    data = {
        'id' : str(i)
    }
    for _ in range(100):
        res = requests.post(url, data=data, cookies=cookie,verify=False)
        if res.json()['message'] == "Product out of stock.":
            break