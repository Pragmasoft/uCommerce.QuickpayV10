using System;
using System.Linq;
using Pragmasoft.QuickpayV10.Extensions.Services.Interfaces;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;

namespace Pragmasoft.QuickpayV10.Extensions.Pipelines.Initialize
{
    public class QuickpayV10InitializationPipelineTask : IPipelineTask<InitializeArgs>
    {
        private readonly IQuickPayV10Logger _logger;

        public QuickpayV10InitializationPipelineTask(IQuickPayV10Logger logger)
        {
            _logger = logger;
        }

        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            var paymentMethodDefinitionName = "QuickpayV10"; //Must be equal to id in Payments.config
            var paymentMethodName = paymentMethodDefinitionName;

            var definition =
                Definition.All()
                    .FirstOrDefault(d => d.DefinitionType.DefinitionTypeId == 4 && d.Name == paymentMethodDefinitionName);
            if (definition != null)
            {
                definition.Deleted = false;
            }
            else
            {
                definition = new Definition
                {
                    Name = paymentMethodDefinitionName,
                    Description = "Configuration for QuickpayV10",
                    DefinitionType = DefinitionType.Get(4)
                };
                _logger.Log("Payment definition created.");
            }

            var definitionfields = definition.DefinitionFields;
            if (definitionfields != null)
            {
                CreateOrUpdateDefinitionField(definition, "ApiKey", "ShortText");
                CreateOrUpdateDefinitionField(definition, "PrivateAccountKey", "ShortText");
                CreateOrUpdateDefinitionField(definition, "Merchant", "ShortText", "12345678");
                CreateOrUpdateDefinitionField(definition, "AgreementId", "ShortText", "12345678");
                CreateOrUpdateDefinitionField(definition, "CallbackUrl", "ShortText", "(auto)");
                CreateOrUpdateDefinitionField(definition, "ContinueUrl", "ShortText");
                CreateOrUpdateDefinitionField(definition, "CancelUrl", "ShortText");
                CreateOrUpdateDefinitionField(definition, "AutoCapture", "Boolean");
                CreateOrUpdateDefinitionField(definition, "CancelTestCardOrders", "Boolean", "True");
            
                definition.Save();

                var isPaymentMethodCreated =
                    PaymentMethod.Exists(x => x.PaymentMethodServiceName == paymentMethodDefinitionName);

                if (!isPaymentMethodCreated)
                {
                    var newPaymentMethod = new PaymentMethod
                    {
                        PaymentMethodServiceName = paymentMethodDefinitionName,
                        Name = paymentMethodName
                    };
                    //add display names:
                    foreach(var culture in Country.All().Where(c => !c.Deleted).Select(c => c.Culture)
                        .ToList().Where(w => !String.IsNullOrWhiteSpace(w)).Distinct(StringComparer.InvariantCultureIgnoreCase))
                    {
                        newPaymentMethod.PaymentMethodDescriptions.Add(new PaymentMethodDescription
                        {
                            DisplayName = paymentMethodName,
                            CultureCode = culture,
                            Description = String.Empty,
                            PaymentMethod = newPaymentMethod,
                        });
                    }

                    newPaymentMethod.Save();
                    _logger.Log("Payment method created and saved.");
                }
            }
            return PipelineExecutionResult.Success;
        }

        private void CreateOrUpdateDefinitionField(Definition definition, string name, string dataType, string defaultValue = "")
        {
            if(definition == null) throw new ArgumentNullException("definition");

            var definitionfields = definition.DefinitionFields;
            if (!definitionfields.Any(x => x.Name == name && x.Definition.DefinitionId == definition.DefinitionId))
            {
                CreateAndAddDefinitionField(definition, name, false, true, true, defaultValue, dataType);
            }
            else
            {
                var definitionfield = definitionfields.FirstOrDefault(x => x.Name == name && x.Definition.DefinitionId == definition.DefinitionId);
                UpdateDefinitionField(definitionfield, false, true, true, defaultValue, dataType, false);
            }
        }

        private void CreateAndAddDefinitionField(Definition definition, string name, bool multilingual, bool displayOnSite, bool rederInEditor, string defaultValue, string dataType)
        {
            definition.AddDefinitionField(new DefinitionField
            {
                //If the money should be withdrawn instantly, e.g download
                Name = name,
                Multilingual = multilingual,
                DisplayOnSite = displayOnSite,
                RenderInEditor = rederInEditor,
                DefaultValue = defaultValue,
                DataType = DataType.FirstOrDefault(x => x.TypeName.Equals(dataType))
            });
        }

        public void UpdateDefinitionField(DefinitionField definitionField, bool multilingual, bool displayOnSite, bool rederInEditor, string defaultValue, string dataType, bool deleted)
        {

                definitionField.Multilingual = multilingual;
                definitionField.Deleted = deleted;
                definitionField.DisplayOnSite = displayOnSite;
                definitionField.RenderInEditor = rederInEditor;
                definitionField.DefaultValue = defaultValue;
                definitionField.DataType = DataType.FirstOrDefault(x => x.TypeName.Equals(dataType));
        }
    }
}
