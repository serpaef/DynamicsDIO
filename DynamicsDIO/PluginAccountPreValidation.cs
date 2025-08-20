using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace DynamicsDIO
{
    public class PluginAccountPreValidation : IPlugin
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

            if (context.InputParameters.Contains("Target"))
            {
                entity = (Entity)context.InputParameters["Target"]; // atribui contexto da entidade

                trace.Trace("Entidade: " + entity.Attributes.Count);

                if (entity == null)
                {
                    return;
                }

                if (!entity.Contains("telephone1"))
                {
                    throw new InvalidPluginExecutionException(OperationStatus.Failed, "O campo Telefone é obrigatório.");
                }
            }

        }
    }
}
