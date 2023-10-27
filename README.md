[![openupm](https://img.shields.io/npm/v/com.volumebox.toolbox?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.volumebox.toolbox/) ![GitHub](https://img.shields.io/github/license/GariestGary/Unity-Toolbox)

![toolbox_banner](https://github.com/GariestGary/Unity-Toolbox/assets/38670681/b4842e79-d75d-49b2-adf9-b60ef5754fc7)

# Unity Toolbox
Set of architectural solutions for Unity
## Table of contents
* [Installation](#installation)
* [Basics](#basics)
* [Update System](#update-system)
* [Dependency Injection](#dependency-injection)
* [Message System](#message-system)
* [Object Pooling](#object-pooling)
* [Save System](#save-system)
* [Travel System](#travel-system)

## Installation

### Install with OpenUPM

Once you have the [OpenUPM cli](https://github.com/openupm/openupm-cli#installation), run the following command:

* Go to your Unity project directory

```cd YOUR_UNITY_PROJECT_DIR```

* Install package: com.volumebox.toolbox

```openupm add com.volumebox.toolbox```

Alternatively, merge the snippet to Packages/manifest.json 

```json
{
    "scopedRegistries": [
        {
            "name": "package.openupm.com",
            "url": "https://package.openupm.com",
            "scopes": [
              "com.volumebox"
              "com.openupm"
            ]
        }
    ],
    "dependencies": {
        "com.volumebox.toolbox": "0.2.4"
    }
}
```

### Install via Package Manager

* open Edit/Project Settings/Package Manager
* add a new Scoped Registry (or edit the existing OpenUPM entry)

    Name: `package.openupm.com`

    URL: `https://package.openupm.com`

* add following scopes:

    `com.volumebox.toolbox`
  
    `com.dbrizov`
  
    `com.solidalloy`
    
    `org.nuget`
	
	`com.cysharp`
  
* click `Save` (or `Apply`)
* open `My Registries` packages in package manager window
* install `VolumeBox Toolbox`

# DOCUMENTATION IN PROGRESS
