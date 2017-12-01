# uCommerce.QuickpayV10
Quickpay for uCommerce aids developers in implementing a QuickPayV10 payment gateway in a uCommerce shop.  Quickpay for uCommerce hides away any tedious and repetitive programing task and simplifies the setup. 

Quickpay for uCommerce supports the following features:
      
#### Payment operations:
* Authorize
* Capture
* AutoCapture
* Refund
* Cancel
      
#### Payment methods:
* Cards
* MobilePay
* PayPal
* ViaBill
* Klarna

#### Features:
* Callbackvalues




## Changelog
v.1.3.0.0
* Bugfix: Removed dependency to ucommerce, moved methods into class so it now works in latest version of ucommerce (7.10.0.17325). Thanks @danieland

v.1.2.0.0
* Bugfix: Ensuring that checkout pipeline is called
      
v.1.1.0.6:
* New feature: You can now disable test cards. It will place all orders bought with a test card in the cancelled folder.
* New feature: Added orderId as query parametre to continue and cancel url
* Bugfix: http://eureka.ucommerce.net/#!/question/1979/QucikPay-v10-and-callbacks
