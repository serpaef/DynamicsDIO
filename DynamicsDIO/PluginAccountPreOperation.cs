using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsDIO
{
    public class PluginAccountPreOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Contexto da execução
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Service Factory da organização
            IOrganizationServiceFactory organizationServiceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Service que estabelece a conexão com o Dataverse
            IOrganizationService service = organizationServiceFactory.CreateOrganizationService(null);

            // Variável que armazena o log de rastreamento
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Variável do tipo Entity vazia
            Entity entity = null;

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                entity = (Entity)context.InputParameters["Target"]; // atribui contexto da entidade

                trace.Trace("Entidade: " + entity.Attributes.Count);

                if (entity.LogicalName == "account")
                {
                    if (entity.Attributes.Contains("telephone1"))
                    {
                        var phone1 = entity["telephone1"].ToString();

                        string fetchContact = @"<?xml version='1.0'?>" +
                            "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>" +
                            "<entity name='contact'>" +
                            "<attribute name='fullname'/>" +
                            "<attribute name='telephone1'/>" +
                            "<attribute name='contactid'/>" +
                            "<order descending='false' attribute='fullname'>" +
                            "<filter type='and'>" +
                            "<condition attribute='telephone1' value='" + phone1 + "' operator='eq'/>" +
                            "</filter>" +
                            "</entity>" +
                            "</fetch>";
                        
                        trace.Trace("Fetch XML: " + fetchContact); // Log para debug
                    
                        var contactCollection = service.RetrieveMultiple(new FetchExpression(fetchContact));

                        if (contactCollection.Entities.Count > 0)
                        {
                            entity["primarycontactid"] = new EntityReference("contact", entity.Id);
                        }
                    }

                }
            }

        }
    }
}
