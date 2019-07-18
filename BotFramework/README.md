# SampleBot Solution Code Strucure
SampleBot solution is written to create an implementation reference on how to integrate Microsoft Bot Framework and UiPath Orchestrator to build a virtual agent that can execute tasks using RPA technology.

This document is a walk through of the code.

# Prerequisites
Refer to [this link](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-sdk-quickstart?view=azure-bot-service-4.0) for prerequisites.
Bot Framework V4 template for C# is not required to run the sample code or create your bot project based on the sample code. It is required when generate a new blank bot project.

# Entry point - SampleBot project
This project is the entry point of the bot. Majority part of this project is generated from the bot framework template.
What this project do:
- Provides the API endpoint to listen to user input message and send bot response
- Read settings
- Inject the dependencies

# Common project
This project implements a common multi-turn bot. This bot is focusing on the conversation strategy and it doesn't couple with real life dialog flows. It includes below functionalities:
- Define common models
    * EntityState: This is the data strucutre to store and retrieve any extracted entity
    * LuisEntity: It provides the ability to post process any entity value that extracted from Luis. For example, normalize names.
    * Intent: There are different types of intents defined. Normal intent is user intent for virtual agent to execute some dialog. DetourIntent (for example help) is the intent the makes the bot need to detour to another task (for example provide help text) and come back to resume the last dialog step. TerminationIntent (for example cancel) is the intent that makes the bot stop what it is doing and start like a new conversation.
- Error handling. For any exception uncaught, reply with internal exception and clean up resources.
- Define conversation strategy. For each conversation turn:
    * If it is conversation update(user join conversation), then send welcome activity
    * If it is message, then do the following:
        1) Recognize intent using Luis
        2) Add extracted entity (if any) to EntityState
        3) If the intent is to terminate or detour, handle the interruption and return
        4) Continue current active dialog with this intent
        5) If no active dialog, then start the dialog mappted to this intent

# Dialog project
This project defines the customized settings for the sample scenario (SAP bot that handles replace item and cancel order). It includes
- Intents. SapIntents defines the intent for the bot and the mapping dialog flow
- SapDialogs. Dialogs in this folder defines the conversation flow for each dialog. To get entities from EntityState, inherit from StatefulDialogBase, otherwise DialogBase.
- SapBotSettings. This file defines the LUIS configuration it wants to use and the postprocessing for LUIS entities
- SapBot. This file adds all the dialogs to the dialog set.

# Actions project
This project is called by Dialog project to execute bot actions. In this sample code, all the actions talk to orchestrator to run a certain process. This project includes:
- Models as interface between Dialog and Actions
- Models as interface between Actions and Orchestrator. Note that the models may not necessarily the same as the models above. GetItemsOutputInternal.cs provides an example of this. It needs to be mapped to the external model for dialog consumption.
- SapRpaClient that is to prepare the input parameter and call the Orchestrator client to run a certain process

# OrchestratorClient project
This project is to run a job in orchestrator and return the result. The public method it provides is ExecuteJobAsync, which first start job, then wait for job to complete, then get job detail and return.
Note that when all robots are busy and have at least one job pending already, start job may fail. So depending on the user scenario and the robot settings, a retry may be need to add in this project to handle that situation.


# Helpers - Utils and Resources project
Utils project provides common functions for other projects to use, such as null check.
Resources project put all the text message in resource file, so it can be easily update externally or extended for multiple lanugage support.
