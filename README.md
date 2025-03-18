# Third eye
Find vulnerabilities in your third-party components in your repos and software.

# Prerequisites
You need an access token from either your AzureDevOps server or your personal github account. If you never have started a Power Commands application on your machine you need to do that so the the encryption key is setup properly.

# Setup

## Setup hosts
Run setup command.

```setup```

This will guide you through the process, it is basically this.

1. Setup your ADS token if you intend to use ADS for Third Eye.
2. Setup your personal Github access token if you intend to use Github for Third eye. (you can setup both and Github)
3. Setup team (not applicable for github, you just chose a default one)
4. Setup NVD API key if you want to fetch CVE:s from NVD yourself, you can request a API key from here: https://nvd.nist.gov/developers/request-an-api-key
5. Setup your project. It is recommended to work on one project at a time, but you can work with them all, just replace your project name with * in the `PowerCommandsConfiguration.yaml` file.
6. Setup your host name. (servername, github url to your personal github page)
7. Setup your Organization name, for github it is your username, ADS does not use this, but will still display it.

Two tokens are stored individually one for ADS and another for Github. Data from the hosts is stored individually, so they will not be overwritten if you are changing hosts, for example from a test ADS to a production one. But you need to now that the findings you do for one environment is not affected or shown for another.

## NVD Setup
For the possibility to find vulnerabilities in your components you need a file with the CVE:s. You can either create one your self with the NVD command.

```nvd --fetch```

It is recommended to store this file on a central spot that all the Third Eye users in your organization use, setup the path for this file in the `PowerCommandsConfiguration.yaml` file.
```
nvd:
  url: https://services.nvd.nist.gov/rest/json/cves/2.0
  timeoutSeconds: 120
  pageSize: 2000
  delayIntervalSeconds: 2
  pathToUpdates: $ROAMING$\nvd\updates
```
You can use the same command to fetch updates, but only one person in the organization should do this, the other one will be updated when they are starting Third Eye.

## Test your setup
To se if your setup is ok and you have a valid connection to your ADS or Github server, run the command connect.

```connect```

# Usage

How you will work is different depending on your role, but when all setup is done, here is your main commands.

## Workspace 

### Initialize your workspace
The first step is to collect information about your "Workspace" as the term is in Third Eye, in ADS a workspace is an project, in Github it is placeholder for all your repositories. Run workspace --init to initialize your current workspace, this will overwrite any already stored workspace.

```workspace --init```
<img src="images/workspace_overview.png?raw=true" alt="drawing"/>

You could also use the option flag --update to update the workspace and --analyze to analyze your components and find CVE:s.

## Analyze for vulnerable third party components.

As soon as you have initialized your workspace you have your component in place, you can start analyze them, you could to it in more then one way.

1. Start from your workspace with ```workspace --analyze``` command.
2. Analyze your components directly with ```command --analyze``` command.
3. Start from a repository with ```repository --analyze``` command.
4. Choose a project to analyze with ```project --analyze``` command.
5. Analyze software (more about software later) with ```software --analyze``` command.

## Workflow

The application uses the same kind of workflow and there is always a kind of filter you can use before you actually select a component, a repository or a vulnerability. That is because the number of items will be to much to handle without this filter process. It can feel a bit weird in the beginning maybe, but I hope you get use to it.

In this example I have typed serilog which is displayed in the console title.

<img src="images/filter_example.png?raw=true" alt="drawing"/>

### The workflow could be described as this.
1. Do your filtering and selections, depending on where you start (workspace, Repository, Project, components) the selection steps has one or more selections you have to do.
2. Choose an CVE threshold, I recommend you to start looking for Critical CVE:s and work from there.
3. Filter and select a certain CVE, work with one CVE at a time.
4. Confirm if the vulnerability is affecting something that you really need to work with and fix.
5. Enter details about the findings to save it as an finding that you can view and work with later on.

## Findings
After you have analyzed and found vulnerabilities that you will act upon, the are stored as findings. Just use the ```finding``` command to view them.

<img src="images/findings_example.png?raw=true" alt="drawing"/>

When you do something update that information to the finding and you will have a nice log.

<img src="images/mitigation_log.png?raw=true" alt="drawing"/>

# CVE
There is a CVE search to find out details about a certain CVE, you need the CVE ID.

```cve <CVE-ID>```

<img src="images/cve_detail.png?raw=true" alt="drawing"/>

# Software

Third Eye also have the possibility to import software and analyze them to the same database with CVE:s.
You need to import a file with your software, there is a sample file in this repo (```data``` directory) that you could look at to se the structure of it, very simple.

```
AVG Antivirus 24.3.6105
Bitdefender 27.0.20.106
Kaspersky 21.15.8.493
Norton Security 22.24.1.37
McAfee Total Protection 16.0 R53
Malwarebytes 4.6.12
```
## Import Software file

PowerCommand can navigate in the file system just as an ordinary console with ```cd``` and ```dir``` command. So you could navigate to your update file and use tab to let the code completion do the rest. You can of course just type in or paste the path to your file.

```
software my_software_file.txt
```
<img src="images/software_import.png?raw=true" alt="drawing"/>

# DB
With the DB command you can view details about the database files that are used by the Third Eye Agent.
Good to know is that every host has it own files with the content of things that are stored there, including the findings.

```db```

<img src="images/db_command.png?raw=true" alt="drawing"/>

# Change stuff in the configuration

If you need to change the host or the filters, you could do this easily by just edit the `PowerCommandsConfiguration.yaml` file, all configuration for Third Eye Agent is there.
But you also need to set your Token, and that could be done with the `token` command, use the option flag --github if you setting up a token for github.
This will of course delete the old tokens.

```
//Setup your ADS token
token <my token>
//Setup your github token
token <my token> --github
```
```
thirdEyeAgent:
  host: https://github.com/PowerCommands
  organizationName: PowerCommands    
  workspaces: 
  - "*"
  teams: 
  - "*"     
  ignores:
    repositories:
    - "GlitchFinder"
    projects:
    - "*"
  nvd:
    url: https://services.nvd.nist.gov/rest/json/cves/2.0
    timeoutSeconds: 120
    pageSize: 2000
    delayIntervalSeconds: 2
    pathToUpdates: $ROAMING$\nvd\updates
```

As you maybe see in the configuration, there is a possibility to ignore projects and repositories, just put the names in there.


# Power Commands general info
The Third Eye is built upon a CLI Framework called PowerCommands, and this frameworks comes with some functionality that could be nice to know.
You could always use tab for suggestions, if you type - and press tab you can traverse through any commands list of option flags.

## Show help about a command
Every commando has the option flag --help, just add that and an you get som description of how the command works.

## Standard Commands (if not removed)
```
//Change directory (has many useful options, just type a - and use tab to find them out)
cd
//Clear console
cls
//Show all commands
commands
//Describe a command (almost the same thing as --help)
describe <command name or anything>
//Show the content of the current working directory
dir
//Exit the program (with y it will be without any confirmation)
exit
//Handle a file (have many options)
file
//View log
log
```
## Info panel

Info panel is an experimental feature that is using the two first lines on the top in the console window, if you have trouble with it, turn it of in the configuration.
```
infoPanel:
  use: true
  autoAdjustOnResize: true
  color: DarkBlue
  height: 2
  updateIntervalSeconds: 10
```





