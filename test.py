import requests

url = "http://localhost:5057"

for i in range(1,4):
    req = requests.get(url + "/Home/ProductDetails/" + str(i))
    with open('test0' + str(i) + '.html', 'w') as f:
        f.write(req.text)
        f.close()