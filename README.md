


在开放的API接口中为了保证数据的安全性，我们往往采用数字签名的方式对请求进行加密

###  功能

- Sign签名
- 应用授权
- 用户授权
 
###  错误参数
 
 **全局返回参数说明** 
1	   Success	操作成功
100000	NULL	Null值错误
100001	SIGN_ERROR	签名验证失败
100002	APP_AUTH_ERROR	应用授权失败
100003	USER_AUTH_ERROR	用户授权失败
100004	SYSTEM_ERROR	系统错误
100006	Failed	服务器操作失败

### 参数说明

 **1.系统参数** 
[method]：商户要调用的App接口名称
timestamp：时间戳 统一格式yyyyMMddHHmmss
app_key：分配给商户的app_key
version：对应接口的版本号 当前为1.0
sign：签名，对 API 调用参数（除sign外）进行 md5 加密获得。

 **注：** 
1.系统参数在每次请求中都是必须的;
2.应用参数根据调用的接口具体传参;
3.app_secret 仅作加密使用, 为了保证数据安全请不要在请求参数中使用.

 **Sign签名** 
调用API 时需要对请求参数进行签名验证，签名方式如下：
1. 按照请求参数名称将所有请求参数按照字母先后顺序排序得到：
    keyvaluekeyvalue...keyvalue  字符串
    如：将arong=1,mrong=2,crong=3 排序为：arong=1, crong=3,mrong=2  然后将参数名和参数值进行拼接得到参数字符串：arong1crong3mrong2。
2. 将app_secret加在参数字符串的头部后进行MD5加密 ,加密后的字符串需大写。即得到签名Sign

 **用户授权** 
1. 通过账号和密码登陆授权成功后,系统返回授权Token, Token有效期为30天,每次重新登陆都会生成一个新的Token值.
2. 将Token加入请求的参数中且参与Sign签名 
注: 不需要用户登陆的公共模块无需传入Token进行用户授权     
    通常情况下sign签名验证都是必须的。

 **请求案例** 
获取产品列表举例：
方法名称:GetProducts  请求权限:签名验证 参数:pageindex,pagesize 请求方式：get
eg: 
1. 按照规则生成签名
参数排序后将app_secret加在字符串头部得到参数字符串：
212821ec2035d78f524a86da13a9dceeapp_key076ba2bcb4a0cb38ce721cc00d27426bpageindex1pagesize10timestamp20150507162828

如上所示 请求所需的4个参数组成的参数字符串，接下来进行MD5加密
得到签名Sign=bcc7c71cf93f9cdbdb88671b701d8a35 记得要大写
这才是正确的签名：Sign=BCC7C71CF93F9CDBDB88671B701D8A35

(可将上面的参数字符串进行MD5加密检测你的加密结果是否一致)

2. 请求链接：```
http://api.daimali.com/pro/getproducts?app_key=076ba2bcb4a0cb38ce721cc00d27426b&pageindex=1&pagesize=10&sign=BCC7C71CF93F9CDBDB88671B701D8A35&timestamp=20150507162828
```


### 已知BUG
1. URL重放攻击