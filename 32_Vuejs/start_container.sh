# **************************************************************************************************
# Environment variables. Adapt these values.
FRONTEND="$(pwd)/Spengernews.Client"             # to build the spa with npm run build
SSL_CERT_FILE="spengernews.pfx"                  # generated with dotnet dev-certs in this script
DOCKER_IMAGE=spengernews_webapp
SQL_IMAGE=spengernews_sqlserver
# Use INTERNAL port for the communication inside the docker network (1433 not 11433)
CONN_STR="Server=10.0.38.3,1433;Initial Catalog=SpengernewsDb;User Id=sa;Password=SqlServer2019;TrustServerCertificate=true"

# **************************************************************************************************
# Program. Adapt with care.
YELLOW='\033[0;33m'   # Yellow
NC='\033[0m'          # No Color

# Generate random secret (the secret in appsettings.json is empty)
SECRET=$(dd if=/dev/random bs=128 count=1 2> /dev/null | base64)
BRANCH=$(git branch --show-current)
CWD=$(pwd)

read -p "$(echo -e "Build app from branch ${YELLOW}$BRANCH${NC}. Press to continue or CTRL+C to cancel building.")"

# Build SPA
cd "$FRONTEND"
rm -rf node_modules
npm install && npm run build
if [ $? -ne 0 ]; then
    echo "Error building the Vue.js application in $FRONTEND with npm run build."
    exit 1
fi

# Create HTTPS Certificates
CERT_PASS=$(dd if=/dev/random bs=128 count=1 2> /dev/null | base64)
rm "$HOME/.aspnet/https/$SSL_CERT_FILE"
dotnet dev-certs https -ep "$HOME/.aspnet/https/$SSL_CERT_FILE" -p "$CERT_PASS"
dotnet dev-certs https --trust

# Cleanup
docker rm -f $DOCKER_IMAGE
docker rm -f $SQL_IMAGE
docker volume prune -f
docker image prune -f
docker network prune -f
docker network rm sqlserver_network

# Create a docker network.
docker network create --subnet=10.0.38.0/24 sqlserver_network
# Run SQL Server container with assigned ip in docker network.
docker run -d -p 11433:1433 --network=sqlserver_network --ip=10.0.38.3 --name $SQL_IMAGE \
    -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2019" \
    mcr.microsoft.com/azure-sql-edge:latest

# Build and run app container.
cd "$CWD"
docker build -t $DOCKER_IMAGE . 
MSYS_NO_PATHCONV=1 docker run -d -p 5000:80 -p 5001:443 --name $DOCKER_IMAGE \
    --network=sqlserver_network --ip=10.0.38.2 \
    -e "ASPNETCORE_URLS=https://+;http://+" \
    -e "ASPNETCORE_HTTPS_PORT=5001" \
    -e ASPNETCORE_Kestrel__Certificates__Default__Password="$CERT_PASS" \
    -e ASPNETCORE_Kestrel__Certificates__Default__Path="/https/$SSL_CERT_FILE" \
    -e "ASPNETCORE_ENVIRONMENT=Production" \
    -e "CONNECTIONSTRINGS__DEFAULT=$CONN_STR" \
    -e "SECRET=$SECRET" \
    -v $HOME/.aspnet/https:/https/ \
    $DOCKER_IMAGE
cd "$CWD"
