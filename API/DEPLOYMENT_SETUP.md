# Azure DevOps Deployment Setup Guide

This guide will help you set up the Azure DevOps pipeline to deploy your Aro Admin API to Azure App Service.

## Prerequisites
- Azure DevOps project
- Azure subscription (ID: 14436730-4293-4954-ba81-dab002f32613)
- Azure App Service (arobe1) already created
- Repository with `cicd/production-api` branch

## Step 1: Create Azure Service Connection

This is a one-time setup that allows Azure DevOps to deploy to your Azure subscription.

1. **Navigate to Project Settings**
   - Go to your Azure DevOps project
   - Click on **Project Settings** (bottom left corner, gear icon)

2. **Create Service Connection**
   - In the left menu, under "Pipelines", click **Service connections**
   - Click **New service connection** (or **Create service connection**)
   - Select **Azure Resource Manager**
   - Click **Next**

3. **Choose Authentication Method**
   - Select **Service principal (automatic)** (recommended)
   - Click **Next**

4. **Configure Service Connection**
   - **Scope level**: Select **Subscription**
   - **Subscription**: Select your subscription (14436730-4293-4954-ba81-dab002f32613)
   - **Resource group**: Select `AroBE-Adil` (optional, but recommended for security)
   - **Service connection name**: Enter `AzureServiceConnection` (must match the name in azure-pipelines.yml)
   - **Description**: Enter "Connection for Aro API deployment" (optional)
   - Check ✅ **Grant access permission to all pipelines** (for easier setup)
   - Click **Save**

5. **Verify Connection**
   - The service connection should now appear in your list
   - You may need to wait a few seconds for Azure to complete the setup

### Alternative: Manual Service Principal Setup

If automatic creation doesn't work, you can create a service principal manually:

1. In Azure Portal, go to **Azure Active Directory** → **App registrations** → **New registration**
2. Create an app registration for Azure DevOps
3. Create a client secret
4. Go to your subscription → **Access control (IAM)** → **Add role assignment**
5. Assign **Contributor** role to the app registration
6. In Azure DevOps, choose **Service principal (manual)** and enter the credentials

## Step 2: Add the Pipeline to Azure DevOps

1. **Navigate to Pipelines**
   - In your Azure DevOps project, click on **Pipelines** in the left menu
   - Click **New Pipeline** (or **Create Pipeline**)

2. **Connect to Your Repository**
   - Select where your code is stored (e.g., Azure Repos Git, GitHub, etc.)
   - Select your repository

3. **Configure Pipeline**
   - Select **Existing Azure Pipelines YAML file**
   - Select branch: `cicd/production-api`
   - Path: `/azure-pipelines.yml`
   - Click **Continue**

4. **Review and Run**
   - Review the pipeline YAML
   - Click **Save** (or **Save and run** if you want to test immediately)
   - If you click "Save", choose a commit message and branch

## Step 3: Create Azure Environment (Optional but Recommended)

This allows you to track deployments and add approval gates if needed later.

1. **Navigate to Environments**
   - In your Azure DevOps project, click on **Pipelines** → **Environments**
   - Click **New environment** (or **Create environment**)

2. **Create Environment**
   - Name: `Azure` (must match the environment name in azure-pipelines.yml)
   - Description: "Production Azure environment" (optional)
   - Resource: Select **None**
   - Click **Create**

## Step 4: Test the Pipeline

1. **Trigger Deployment**
   - Make a small change to your code
   - Commit and push to the `cicd/production-api` branch
   - The pipeline should automatically trigger

2. **Monitor Pipeline**
   - Go to **Pipelines** → **Pipelines**
   - Click on your pipeline run
   - You can see real-time logs and progress

3. **Verify Deployment**
   - After successful deployment, visit your App Service URL: `https://arobe1.azurewebsites.net`
   - Verify the API is running with the latest changes

## Pipeline Overview

### Trigger
- Automatically runs on commits to `cicd/production-api` branch

### Build Stage
1. Uses .NET 8.0 SDK
2. Restores NuGet packages
3. Builds the entire solution in Release mode
4. Publishes the `Aro.Admin.Presentation.Entry` project
5. Creates deployment artifacts

### Deploy Stage
1. Deploys the published artifacts to Azure App Service `arobe1`
2. Sets `ASPNETCORE_ENVIRONMENT` to `Azure` (uses appsettings.Azure.json)
3. Automatically restarts the app service

## Configuration Files

### appsettings.Azure.json
The pipeline sets `ASPNETCORE_ENVIRONMENT=Azure`, which means your application will:
1. Load `appsettings.json` (base configuration)
2. Override with `appsettings.Azure.json` (Azure-specific configuration)

Your current Azure configuration includes:
- CORS policy for `https://agreeable-water-06e3de21e.3.azurestaticapps.net`
- SQL Server connection string

### App Service Configuration (Recommended for Secrets)
While the connection string is currently in `appsettings.Azure.json`, you can override it in Azure App Service Configuration for better security:

1. Go to Azure Portal → App Service `arobe1`
2. Click **Configuration** → **Application settings**
3. Add/Edit: `ConnectionStrings__sqlConnection` with your connection string value
4. Click **Save** and **Continue**

This way, the connection string in App Service will override the one in the JSON file.

## Troubleshooting

### Pipeline Fails with "Service connection not found"
- Ensure the service connection name is exactly `AzureServiceConnection`
- Grant permissions to the pipeline if prompted
- Check that the service connection has access to the resource group

### Deployment Fails with Permission Error
- Verify the service principal has **Contributor** role on the resource group or subscription
- Check that the App Service name is correct: `arobe1`

### Application Not Starting After Deployment
- Check App Service logs in Azure Portal
- Verify `ASPNETCORE_ENVIRONMENT` is set to `Azure`
- Ensure `appsettings.Azure.json` is included in the published output

### Wrong .NET Version
- The pipeline uses .NET 8.0 (matching your project's target framework)
- If you need a different version, update the `UseDotNet@2` task in the pipeline

## Pipeline Customization

### Add Manual Approval
If you want to add manual approval before deployment:

1. Go to **Pipelines** → **Environments** → **Azure**
2. Click the menu (⋯) → **Approvals and checks**
3. Click **+** → **Approvals**
4. Add approvers and save

### Change Trigger Branch
To deploy from a different branch, edit `azure-pipelines.yml`:

```yaml
trigger:
  branches:
    include:
      - main  # or any other branch
```

### Add Multiple Environments
To deploy to staging and production:

1. Create multiple App Services in Azure
2. Create multiple environments in Azure DevOps
3. Add multiple deployment stages in the pipeline

## Support

For issues or questions:
1. Check Azure DevOps pipeline logs for detailed error messages
2. Check Azure App Service logs in Azure Portal
3. Review this guide's troubleshooting section

