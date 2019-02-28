## Alfresco CMM Deploy

[![Build Status](https://dev.azure.com/wk-j/alfresco-cmm-deploy/_apis/build/status/wk-j.alfresco-cmm-deploy?branchName=master)](https://dev.azure.com/wk-j/alfresco-cmm-deploy/_build/latest?definitionId=15&branchName=master)
[![NuGet](https://img.shields.io/nuget/v/wk.CmmDeploy.svg)](https://www.nuget.org/packages/wk.CmmDeploy)

Command line utility provide functionality for deploy content model

- [x] Alfresco 5.1
- [x] Alfresco 5.2
- [ ] Alfresco 6.x

## Installation

```bash
dotnet tool install -g wk.CmmDeploy
```

## Usage

```bash
wk-cmm-deploy http://localhost:8080/alfresco resource/ABC.zip
```