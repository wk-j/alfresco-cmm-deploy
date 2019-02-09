## Alfresco CMM Deploy

Command line utility provide functionality for deploy content model

## Installation

```bash
dotnet tool install -g wk.CmmDeploy
```

## Start Alfresco

```bash
echo WORKING_PATH=(pwd)/working > .env
docker-compose up
```

## Usage

```bash
wk-cmm-deploy http://localhost:8080/alfresco resources/NBTCcrm.zip
```