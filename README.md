# SceneSearcher
SceneSearcher Unity3D Plugin

## Overview
The purpose of this plugin is to make process of switching between scenes more convenient. When you come to the new project, which was developing for some time before you joined it, you clone the project to your computer and face the first problem - find the entry point of the project. Maybe you colleagues will tell you that you should take a look at some SceneA. Then you have to locate SceneA in the project and remember the folder where it is. This may not be a problem if you have one scene in project. But if there are several scenes you have to remember more and more folders and locations. It may be a solution to come to convention that you store scenes in some special folder. But to navigate between them you still have to locate the folder in Unity3D and click on the scene. So the main idea is to make switching between scenes as easy as selecting one menu item.
![fig 1](https://github.com/vmchar/SceneSearcher/blob/master/images/SceneMenu.png)

This picture shows the result of plugin’s work. To switch between your scenes you only have to select scene you need from menu and it’s done.

 Absolutely no scripting required. All you have to do is open SceneScearcher window, select scene files in your project, or a folder, containing multiple scene files and generate a menu. All this can be done only once and can be changed if you want to add more scenes.
## Integration
After you import unitypackage to your project, you’ll have a menu item under the ‘Window’ tab called SceneSearcher:
![fig 2](https://github.com/vmchar/SceneSearcher/blob/master/images/SceneScearcherItem.png)

Clicking this item will open a new window:
![fig 3](https://github.com/vmchar/SceneSearcher/blob/master/images/SSWindowBlank.png)

There are 4 sections in this window. Let’s take a closer look at each of them.
## Preferences
This section contains a couple of fields made for customizing the final appearance of editor’s menu items you’ll get as a result of the plugin’s work.

![fig 4](https://github.com/vmchar/SceneSearcher/blob/master/images/SS_prefs.png)

* Menu Item Top Name - string field. How your menu will be called. You can call it as you project name, or enter the name of already existing menu to put this menu inside another menu you want.
* How to order scenes - 2 options - Numeric, Alphabetic. This manages the order scenes will appear inside the menu. if Numeric is selected, scenes will be ordered by Order coefficient, which can be set individually for each scene. If Alphabetic is selected, scenes will be ordered in alphabetical order according to scene name (or alias you set for the scene).
## Add New Scene
This section allows you to add a single scene to you menu by selecting a scene file. Scene are stored in Unity as a .unity files. Let take a look at this section:

![Fig 5](https://github.com/vmchar/SceneSearcher/blob/master/images/SS%20Add%20scene.png)

* Name of the scene - shows the name of a file you have selected.
* Path to scene - physical path to the file you have selected. Path is related to your project path and will start from ‘Assets/’ folder.
* Your alias for the scene - optional field. How you want your scene to be called inside menu you’ll generate.
* Order number among scenes - optional field. If you want to use Numeric sorting, this parameter will be used sort menu items inside menu ascending. 
* Select Scene - this button will open unity’s file browser inside the ‘Assets/’ folder. There you can select only .unity files.
* Add a Scene - this button will add selected scene and all the parameters you set up into Processed Scenes (the list of scenes for which menu item will be generated).
* Reset All Fields - button will reset all selection in Add New Scene section and all parameters you have entered.
## Add scenes in folder
This section allows you to select a folder where SceneSearcher will automatically search for .unity files recursively. Number in brackets is the amount of scenes, SceneSearched has found in the folder. This section has also a sub-section called ‘Scenes in selected folder’, where you’re able to see a list of scenes, found in folder. Scenes are presented just like in “Add new scene” section. Let’s take a look at this Add scenes in folder section:

![fig 6](https://github.com/vmchar/SceneSearcher/blob/master/images/SS%20folder%20scenes.png)

* Select folder - button will open unity’s file browser where you’re able to select only folders.
* Selected folder  - label will show you the absolute path to selected folder.
* Add Selected Scenes - button will add selected scenes and all the parameters you set up into Processed Scenes (the list of scenes for which menu item will be generated). All selected scenes will be listed under the ‘Scenes in selected folder’ section. In this section you can modify all the parameters and remove scene you don’t need from the list.
* Reset Selection - button will clear all your selection, including folder, scenes and their parameters.
## Processed Scenes
This section contains the list of scenes which you have selected using “Add new scene” or “Add scenes in folder” and also scenes you have processed during previous usages of SceneSearcher. Here you can remove scenes from processed list of modify their parameters. This section looks like this:

![Fig 7](https://github.com/vmchar/SceneSearcher/blob/master/images/SS%20Processed%20scenes.png)

This section contains no new options, we have covered them all in  previous sections.

## Function Buttons
All the main functionality is hidden under the bottom 3 buttons:
* Save Changes - saves all parameters in “Preferences” and “Processed Scenes” sections. This information is stored between sessions and is saved in two special .json files in SceneSearcher’s folder. Files are in SceneSearcher/Data folder and called: prefs.json and sceneData.json. Be careful not to remove this files accidentally if you don’t want to lose all your settings. Although if you’ll manually remove this two files after you generated your scene menu, you won’t lose the menu itself. Changes are not saved automatically, so you have to save them manually using this button.
* Remove All Scenes - removes everything from Processed Scenes, everything from saved scene data and generated menu.
* Generate Menu - After you have set up everything (Preferences and Processed Scenes) you should use this button to generate the scene menu.
## How Does It Work
Assuming that you have set up Processed Scenes and Preferences, you have to generate menu. All the information is stored inside 2 json files. When you click Generate Menu button, according to information in this json files a source code file is generated. This file is stored in SceneSearcher/Editor folder. After you press generate button a SceneSearcherGeneratedClass.cs file is created and added into your unity project. This source code is fully harmless and contains only templated editor code. Just take a look at this code to make sure that it works only in editor and completely safe:

![Fig 8](https://github.com/vmchar/SceneSearcher/blob/master/images/SS%20code.png)

This source code was generated using data shown on the previous screenshot. Feel free to modify it manually. But every time you re-generate the menu the old source file is removed and the new one with the same name is created.
