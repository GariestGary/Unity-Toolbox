[![openupm](https://img.shields.io/npm/v/com.volumebox.toolbox?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.volumebox.toolbox/) ![GitHub](https://img.shields.io/github/license/GariestGary/Unity-Toolbox)

# Unity Toolbox

<img src="Resources/Icons/toolbox_banner.png" width=100%>

Set of architectural solutions for Unity.

Toolbox is a framework that makes it easier to create games using Unity. Many frequently used things by programmers are taken into account in the toolbox and configured in such a way as to reduce the time and number of lines of code spent on them.

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
        "com.volumebox.toolbox": "1.0.0"
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

# Documentation

You can read documentation on [my website](https://gariestgary.github.io/toolbox/about/)
