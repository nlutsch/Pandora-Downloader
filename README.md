Pandora Downloader
==================
This is a simple project that uses a google chrome extension to download songs from Pandora while listening to it.

It uses a small .Net Core server to download the songs.

Installation
============

Download *PandoraChromeExtension.crx*.

Navigate to **chrome://extensions** in Chrome

Drag and Drop *PandoraChromeExtension.crx* into the Chrome Browser Window

Download *Web Service/src/PandoraService2* folder

Open cmd or terminal

Change directory to the recently downloaded folder

run "dotnet restore"

then "dotnet run"