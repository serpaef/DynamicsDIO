using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsDIO
{
    public class PluginAccountPostOperation : IPlugin
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

            try
            {
                if(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    entity = (Entity)context.InputParameters["Target"];

                    if(!entity.Contains("websiteurl"))
                    {
                        throw new InvalidPluginExecutionException("Campo websiteurl é obrigatório");
                    }

                    var task = new Entity("task");

                    task.Attributes["ownerid"] = new EntityReference("systemuser", context.UserId);
                    task.Attributes["regardingobjectid"] = new EntityReference("account", context.PrimaryEntityId);
                    task.Attributes["subject"] = "Visite nosso site: " + entity["websiteurl"].ToString();
                    task.Attributes["description"] = "Task criada via post-operation.";

                    service.Create(task);
                }
            }
            catch (Exception ex)
            {
                // Log de erro
                trace.Trace("Erro: " + ex.Message);
                throw new InvalidPluginExecutionException("Ocorreu um erro: " + ex.Message, ex);
            }
        }
    }
}
