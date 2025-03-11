# geo-debugger-lib-dotnet
A nuget package to communicate with the geodebugger


## About GeoDebugger

The GeoDebugger is a tool to display GeoBased information. It consists of a library (this)
 and a [GeoDebugger Server](https://github.com/dev4Agriculture/geo-debugger-server).

Additionally, a Tool like e.g. [QGis](https://qgis.org/de/site/) can be used to display the data.

For Details, see the [GeoDebugger Server Repository](https://github.com/dev4Agriculture/geo-debugger-server).

## About the Author
![dev4Agriculture](resources/banner_dev4ag.png)

dev4Agriculture focusses on data exchange in agriculture. We build, analyse and visualize ISOXML and support companies in data exchange via agrirouter.

Find out more at https://www.dev4Agriculture.de

## Installation

The nuget packages are currently not available in a global nuget system; use a local storage.

## Commands 

### SetURL()

Sets the target URL of the GeoDebug server; default is localhost:8083




### Debug()

Sends all updated information to the GeoDebugger Server.

### DebugClearFeatures()

Sends all updated information to the GeoDebugger Server and Clears all lists afterwards.

### AddFeature(Feature feature, int layer = 0)

Adds a new feature to the specific layer and type (the type is automatically examined)

### AddFeatureList(List<Feature> features, int layer = 0)

Add multiple features at once 


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
