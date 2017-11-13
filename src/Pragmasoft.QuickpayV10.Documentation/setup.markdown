# How to Setup the Quickpay App

After installing the app then you need to setup the Quickpay payment method in uCommerce.

![image](images/quickpay-setup-ucommerce.jpg)

Api Key, Private Account Key, Merchant and Agreement Id is mandatory for the payment method to function.
In the CallBackUrl type (auto) unless you need do a custom callback.

You can find the Api Key, Private Account Key, Merchant and Agreement Id in Quickpay's system.
Go to http://quickpay.net and login.
Navigate to Settings -> Integration in the left panel

![image](images/quickpay-keys.jpg)

# How to automatically capture, refund and cancel payments
uCommerce can automatically capture, refund and cancel payments.

## uCommerce v7.0.5 and higher
In uCommerce v7.0.5 the two pipeline tasks AcquirePaymentTask and CancelPaymentTask are now partials components.
You can find the setup documentation here: <a href="http://docs.ucommerce.net/ucommerce/v7.0/payment-providers/general-setup-of-payment-methods.html">General setup of payment methods</a>

## For uCommerce lower than v7.0.5:
To enable this functionality you must do two things:

#### Enable AcquirePaymentTask(Capture)
You can enable AcquirePaymentTask in the Orders.ToCompletedOrder.config in the uCommerce folder

{CODE-START:xml /}
<configuration>
	<components>
		<!-- Pipeline Instance -->
		<component id="ToCompletedOrder"
				   service="UCommerce.Pipelines.IPipeline`1[[UCommerce.EntitiesV2.PurchaseOrder, UCommerce]], UCommerce"
				   type="UCommerce.Pipelines.Transactions.Orders.OrderProcessingPipeline, UCommerce.Pipelines">
			<parameters>
				<tasks>
					<array>
            <value>${ToCompletedOrder.AcquirePaymentTask}</value>
					</array>
				</tasks>
			</parameters>
		</component>
    <!-- Pipeline Tasks-->
    <component id="ToCompletedOrder.AcquirePaymentTask"
				   service="UCommerce.Pipelines.IPipelineTask`1[[UCommerce.EntitiesV2.PurchaseOrder, UCommerce]], UCommerce"
				   type="UCommerce.Pipelines.Transactions.Orders.ToCompleted.AcquirePaymentTask, UCommerce.Pipelines" />
		
	</components>
</configuration>
{CODE-END /}

#### Enable CancelPaymentTask(Cancel/Refund)
You can enable CancelPaymentTask in the Orders.ToCancelled.config in the uCommerce folder

{CODE-START:xml /}
<configuration>
	<components>
		<!-- Pipeline Instance -->
		<component id="ToCancelled"
				   service="UCommerce.Pipelines.IPipeline`1[[UCommerce.EntitiesV2.PurchaseOrder, UCommerce]], UCommerce"
				   type="UCommerce.Pipelines.Transactions.Orders.OrderProcessingPipeline, UCommerce.Pipelines">
			<parameters>
				<tasks>
					<array>
            <value>${ToCancelled.CancelPaymentTask}</value>
					</array>
				</tasks>
			</parameters>
		</component>
    <!-- Pipeline Tasks-->
    <component id="ToCancelled.CancelPaymentTask"
				   service="UCommerce.Pipelines.IPipelineTask`1[[UCommerce.EntitiesV2.PurchaseOrder, UCommerce]], UCommerce"
				   type="UCommerce.Pipelines.Transactions.Orders.ToCancelled.CancelPaymentTask, UCommerce.Pipelines" />

	</components>
</configuration>

{CODE-END /}