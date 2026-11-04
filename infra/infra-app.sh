#
# VERIFIQUE SE ESTA COM A ASSINATURA CORRETA
#
# az account show
# az account list -o table
# az account set --subscription "nome-da-assinatura-correta"

set -euo pipefail

# Variáveis
RESOURCE_GROUP_NAME="rg-petbuddies"
WEBAPP_NAME="petbuddies-net-rm563925"
APP_SERVICE_PLAN="plan-petbuddies"
LOCATION="brazilsouth"
RUNTIME="DOTNETCORE:8.0"
APP_INSIGHTS_NAME="ai-petbuddies-net"
COSMOS_ACCOUNT_NAME="cosmos-petbuddies-rm563925"
MONGO_DATABASE_NAME="petbuddies"

# Registrar os Serviços utilizados
az provider register --namespace Microsoft.Web
az provider register --namespace Microsoft.Insights
az provider register --namespace Microsoft.OperationalInsights
az provider register --namespace Microsoft.DocumentDB

# Registrar a extensão utilizada
az extension add --name application-insights --upgrade --only-show-errors

# Criação do Grupo de Recursos
az group create \
  --name $RESOURCE_GROUP_NAME \
  --location "$LOCATION"

# Criar Application Insights
az monitor app-insights component create \
  --app $APP_INSIGHTS_NAME \
  --location "$LOCATION" \
  --resource-group $RESOURCE_GROUP_NAME \
  --application-type web

# Criar o Plano de Serviço (compartilhado com o Web App do Java)
az appservice plan create \
  --name $APP_SERVICE_PLAN \
  --resource-group $RESOURCE_GROUP_NAME \
  --location "$LOCATION" \
  --sku B1 \
  --is-linux

# Criar o Serviço de Aplicativo
az webapp create \
  --name $WEBAPP_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --plan $APP_SERVICE_PLAN \
  --runtime "$RUNTIME"

az webapp config set \
  --name $WEBAPP_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --always-on true

# Recuperar a String de Conexão do Application Insights
CONNECTION_STRING=$(az monitor app-insights component show \
  --app $APP_INSIGHTS_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --query connectionString \
  --output tsv)

# Configurar as Variáveis de Ambiente do Application Insights
# (banco, JWT e URL do Java são gravados pela pipeline, a partir das variáveis secretas)
az webapp config appsettings set \
  --name "$WEBAPP_NAME" \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --settings \
    APPLICATIONINSIGHTS_CONNECTION_STRING="$CONNECTION_STRING" \
    ApplicationInsightsAgent_EXTENSION_VERSION="~3" \
    XDT_MicrosoftApplicationInsights_Mode="Recommended" \
    XDT_MicrosoftApplicationInsights_PreemptSdk="1"

# Reiniciar o Web App
az webapp restart --name $WEBAPP_NAME --resource-group $RESOURCE_GROUP_NAME

# Criar a conexão do nosso Web App com o Application Insights
az monitor app-insights component connect-webapp \
    --app $APP_INSIGHTS_NAME \
    --web-app $WEBAPP_NAME \
    --resource-group $RESOURCE_GROUP_NAME

# Criar o MongoDB da memória de cálculo dos pontos (Azure Cosmos DB for MongoDB).
# --enable-free-tier: só uma conta gratuita por assinatura; sem ela, tire o parâmetro.
az cosmosdb create \
  --name $COSMOS_ACCOUNT_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --kind MongoDB \
  --server-version 4.2 \
  --default-consistency-level Session \
  --locations regionName="$LOCATION" failoverPriority=0 isZoneRedundant=False \
  --enable-free-tier true

az cosmosdb mongodb database create \
  --account-name $COSMOS_ACCOUNT_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --name $MONGO_DATABASE_NAME

# A connection string é segredo: vai para a variável MONGODB_CONNECTION_STRING da
# pipeline (marcada como secreta), que a grava no Web App como MongoDb__ConnectionString.
echo "Connection string do MongoDB (copie para MONGODB_CONNECTION_STRING na pipeline):"
az cosmosdb keys list \
  --name $COSMOS_ACCOUNT_NAME \
  --resource-group $RESOURCE_GROUP_NAME \
  --type connection-strings \
  --query "connectionStrings[0].connectionString" \
  --output tsv
