# Daz Package Mananger

## Introduction
This program lets you load Daz figures quicker by limiting the amount of product Daz Studio must process. You can also use this program to browse the thumbnail of products you have installed.

If you got too many characters and morphs installed, Daz Studio figure loading process becomes tediously slow. The only way to speed to process up is by selectively include files you want Daz to load. Fortunately, Daz only "see" files in folders you included as Base Directory. By only including folders containing contents you want to use, the character load process will be fast. However, manually managing which product to include is practical, especially when switching between scenes. This program automates the process by creating a "Virtual Folder" containing links to files you want to include, thus can be used as a Base Directory in Daz Studio. 

The advantages of this program are:
1. Space efficient, the virtual folder contains only links to the actual files, thus take a negligible amount of space.
2. Permanent, multiple virtual folders can be created for different scenes, allowing easy switch between scenes and come back to the scene later.
3. Repeatable, products are selected based on the content of a scene file. The generated virtual folder can be deleted and recreated later.
4. (For DIM installed product) Files from the same product also included, automatically including corrective morphs and other files that items in the scene might need*.
*Note transitive dependencies are not handled, but nearly all Daz products should be standalone and work correctly.


## How To Use
### Install
1. Download program file [here](https://github.com/TheNoobDucky/Daz-Package-Manager/releases/tag/V1.6.0)
2. May also need to install ".NET Desktop Runtime 5" [Download here](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.5-windows-x64-binaries)
3. Either turn developer mode on windows 10 or run the program as administrator. [How to turn developer mode on](https://docs.microsoft.com/en-us/windows/apps/get-started/enable-your-device-for-development)
4. Run Daz Package Manager.exe.

#### Use
Step 0: Set Windows to developer mode or run the program as admin.

Step 1: Press "Select Install Manifest Archive Folder" button to select the folder where DIM save install manifest files. 
You can find them by going to DIM -> advanced setting -> Installation -> Manifest Archive. 

Step 2a: Press "Scan Install Manifest Archive" button. 
A list of installed packages should show up in "Log Output" tab. 
The results are cached and will persist over program sessions.
Only need to rerun this step after installing new content.

Step 2b: Add other folders containing Daz file that is not installed via DIM/Central such as 3rd party stores by using the "Add 3rd Party Folder" button. 

Optional Step 3: Press "Select Scene File" button to select a scene file.

Optional Step 4a: Press "Select Packages Based on Scene" button to select packages based on files referenced by the selected scene. It is also possible to bulk process all scene files in the same folder.

Step 4b: Select additional packages you want to include.
There is a "Clear Package Selection" button to unselect all. 

Step 5: Press "Select Output Folder Location" button to select where to save the virtual folder. 
Virtual folder need to be on a local drive, 
it does not work with network drive.
However, source files can be on network drive.

Step 6: Press "Generate Virtual Install Folder" to generate the virtual folder.
There is an option to create it in a subfolder with scene file name, allowing you to create a virtual folder per scene so you can switch between different scenes.

Step 7: Add the virtual folder as a base folder in Daz Studio. 
Remove base folders that contains contents you dont want daz to see.
Optionally use "Generate Install Script" button to create a Dazscript that will automatically include the virtual folder and open the scene (Dazscript can only handle paths with English letters).


## Background Info
The approach taken is to create a virtual folder containing Symlink references to all the files needed in a scene.
By using the virtual folder as Daz base folder,
only morphs needed are visible to Daz, 
making the character load much faster.

It is also possible to select manually which packages to install.

This program also serves as a product manager that allows you to view all items with a much bigger preview image than shown in Daz.


The algorithm:

1. Scan Daz install manifest archive to find all installed packages.
2. Scan a scene file to find all the files it needs. 
3. Select all the packages containing the required files.
4. Create a virtual folder with Symbolic link to all the files. 


## Limitations
**Warning: Do not save the scene while the virtual folder is not added as a base folder.**
After opening a scene using Virtual folder, texture images might be resolved using the virtual folder link. Saving the scene while the virtual folder is removed will cause daz to save the image path as absolute path. This will cause missing texture issue when you next open the scene without the virtual folder added. Save the scene again with the virtual folder added will fix the issue.

* Only run on Windows 10 and later.

* Require .NET Desktop Runtime 5 at [https://dotnet.microsoft.com/download/dotnet/5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
Download link:[https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.5-windows-x64-binaries](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.5-windows-x64-binaries)

* Need to run with developer mode or admin privilege. 
[Due to the permission required to create symbolic link](https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-createsymboliclinka)

* Works best with packages installed by DIM/Central and have an install manifest.

* Character detection depends on default metadata.


## Version Log
Change Log:

V1.8.1
* Fix 3rd party crash

V1.8
* Better 3rd party tab presentation and internal changes.

V1.7
* Add ability to save/load selection to file.

V1.6.1 
* add 3rd party folder button now work correctly.
* select the whole folder when selecting a 3rd party file in case other files in the folder are transitively referenced.
* Improved GUI text.

V 1.6.0:
* Add ability to handle files not installed using DIM.
* Refactor to use async.
* Better error handling.
*
V 1.6.0:
* Add ability to handle files not installed using DIM.
* Refactor to use async.

V 1.5.0:
* Rearranged how content is organised on display.
* Change the way contents are sorted, to be more user friendly.
* Add filtering by gender for package.

V 1.4.0:
New Features:

* Image gallery for character, clothing, hair, and poses.
* Performance improvement by grouping items.

V 1.3.4:
Bugfix: 
* Disable canceling scan (was not working before).
* Better error handling when parsing metadata file.

V 1.3.0:
New Features:
* Can browse accessories, attachments, characters, clothings, hairs, morpsh, props, poses
* Performance Improvements including more efficient processing and filtering contents.
* More robust error handling.
*  Better error reporting.

V 1.2.0:
New Features:
* Images are not resizable.
* Hide Image. This improve UI speed.
* Detect asset compatibility with Daz figure generation.
* Able to sort by figure generation.
* Show package content types (for content types that are implemented).
* Performance improvement.

WIP:
* Character gender detection not implemented.

V 1.1.0:
Bug fix.
* Able to find poses.
* Improved Performance

V 1.0.0:
Basic Functionality.

Bugfix:
* Sort by selected.

## License
[GPL 2.0](https://www.gnu.org/licenses/old-licenses/gpl-2.0.html)

Duck Icon from https://iconarchive.com/show/free-flat-sample-icons-by-thesquid.ink/rubber-duck-icon.html under CC Attribution 3.0 Unported
