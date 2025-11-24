Source:
https://luizmelo.itch.io/wizard-packt
https://kenney.nl/assets/tiny-town

## About Project
This game takes place in the province of Hynn. At the age of 18, boys and girls must build a house. House construction is regulated by a framework. It determines how fast you can go and how fast you can cut down trees.

## Stack
C# monogame

## Demo video
https://youtu.be/ynO9CGg2GFE

## Build command

Linux:
```dotnet publish -c Release -r linux-x64 -p:PublishReadyToRun=false -p:TieredCompilation=false --self-contained```

MacOs:
```dotnet publish -c Release -r osx-x64 -p:PublishReadyToRun=false -p:TieredCompilation=false --self-contained```

Windows:
```dotnet publish -c Release -r win-x64 -p:PublishReadyToRun=false -p:TieredCompilation=false --self-contained```
