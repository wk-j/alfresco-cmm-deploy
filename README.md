## Alfresco CMM Deploy

Command line utility provide functionality for deploy content model

## Installation

```bash
dotnet tool install -g BCircle.CmmDeploy
```

## Start Alfresco

```bash
echo WORKING_PATH=(pwd)/working > .env
docker-compose up
```

## Usage

```bash
bc-cmm-deploy http://admin:admin@localhost:8080/alfresco resources/NBTCcrm.zip
```