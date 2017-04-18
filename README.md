# XamarinLovesAzure

## Todos to run the application: 
Some platforms need an Maps API key to display the map within the application.

# iOS
You are lucky, you don't need to do anything. Have fun!

# Android
Open the file [/DropItCode/src/DropIt.Mobile.Droid/Properties/AndroidManifest.xml](https://github.com/rherlt/XamarinLovesAzure/DropItCode/src/DropIt.Mobile.Droid/Properties/AndroidManifest.xml). Insert your Google Maps API Key: 
```csharp
<meta-data android:name="com.google.android.geo.API_KEY" android:value="INSERT_MAPS_API_KEY_HERE" />
```
# Windows UWP
Open the file [DropItCode/src/DropIt.Mobile.Uwp/App.xaml.cs](https://github.com/rherlt/XamarinLovesAzure/DropItCode/src/DropIt.Mobile.Uwp/App.xaml.cs). Insert your Bing Maps API Key: 
```csharp
 FormsMaps.Init("INSERT_MAPS_API_KEY_HERE");
```
