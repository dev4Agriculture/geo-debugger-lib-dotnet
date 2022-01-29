# geodebug-dotnet
A nuget package to communicate with the geodebugger


## About GeoDebugger

The GeoDebugger is a tool to display GeoBased information. It consists of a library (this)
 and a [GeoDebugger Server](https://github.com/dev4Agriculture/geodebug_server).

Additionally, a Tool like e.g. [QGis](https://qgis.org/de/site/) can be used to display the data.

For Details, see the (GeoDebugger Server Repository](https://github.com/dev4Agriculture/geodebug_server).

## About the Author
![dev4Agriculture](resources/banner_dev4ag.png)

dev4Agriculture focusses on data exchange in agriculture. We build, analyse and visualize ISOXML and support companies in data exchange via agrirouter.

Find out more at https://www.dev4Agriculture.de

## Installation

The nuget packages are currently not available in a global nuget system; use a local storage.


## Build instructions

### SDK Version
The nuget packages are currently limited to one SDK version per geodebugger nuget package.

Currently the following assignments apply:

Geodebugger Version |  DotNet Version|
--------------------|----------------|
X.X.0               |   4.5.2        |
X.X.1               |   4.7.2        |

To prepare the different versions, you can use Visual Studio Code and do a few ReplaceAll Instructions


### Build Version X.X.0 ( DotNet SDK 4.5.2)

- Search For version String X.X.1 (e.g. "2.2.1") and replace it with X.X.0 (e.g. "2.2.0") (Simple "Replace all" in VS Code)
- Search for SDK String 4.7.2 and replace it with SDK String 4.5.2 (simple "Replace all" in VS Code)
- Run ```dotnet clean``` to delete old versions
- Run ```dotnet build``` to build the code
- Run ```nuget pack```once the app is ready to be created. 
- Copy the created Nuget Package to your local nuget storage
- Install it in your app


### Build Version X.X.1 (DotNet SDK 4.7.2)

- Search For version String X.X.0 (e.g. "2.2.0") and replace it with X.X.1 (e.g. "2.2.1") (Simple "Replace all" in VS Code)
- Search for SDK String 4.5.2 and replace it with SDK String 4.7.2 (simple "Replace all" in VS Code)
- Run ```dotnet clean``` to delete old versions
- Run ```dotnet build``` to build the code
- Run ```nuget pack```once the app is ready to be created. 
- Copy the created Nuget Package to your local nuget storage
- Install it in your app

## Commands

### SetURL()

Sets the target URL of the GeoDebug server; default is localhost:8083

### Debug()

Sends all updated information to the GeoDebugger Server.

### DebugClearFeatures()

Sends all updated information to te GeoDebuger Server and Clears all lists afterwards.

### AddFeature(Feature feature, int layer = 0)

Adds a new feature to the specific layer and type (the type is automatically examined)

### WaitDebugger()

Stops the program execution until all data was sent to the GeoDebugger Server

### ClearPoints(int index = -1)

Deletes all points in the given index or in all indizes, if index==-1


### ClearLines(int index = -1)

Deletes all lines in the given index or in all indizes, if index==-1

### ClearPolygons(int index = -1)

Deletes all polygons in the given index or in all indizes, if index==-1

### ClearAll()

Clear all Lines, Points, Polygons