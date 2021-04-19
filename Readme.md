# Daz Package Mananger

## Warning: Do not save the scene while the virtual folder is not added as a base folder.
After opening a scene using Virtual folder, texture images might be resolved using the virtual folder link. Saving the scene while the virtual folder is removed will cause daz to save the image path as absolute path. This will cause missing texture issue when you next open the scene without the virtual folder added. Save the scene again with the virtual folder added will fix the issue.


## Introduction
This program let you create a virtual folder with only selected products, thereby improving Daz character load speed. You can select products both manually or get the program to automatically extract referenced products from a scene or folder of scenes.

## How To Use
### Install
1. Download zip file [here](https://github.com/TheNoobDucky/Daz-Package-Manager/releases/tag/V1.5.0)
2. May also need to install ".NET Desktop Runtime 5" at [https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.5-windows-x64-binaries](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.5-windows-x64-binaries)
3. Either turn developer mode on windows 10 or run the program as administrator. [How to turn developer mode on](https://docs.microsoft.com/en-us/windows/apps/get-started/enable-your-device-for-development)
4. Unzip the file and run Daz Package Manager.exe.

#### Use
Step 0: Set Windows to developer mode or run as admin.

Step 1: Press "Select Install Manifest Archive Folder" button to select the folder where DIM save install manifest files. 
You can find them by going to DIM -> advanced setting -> Installation -> Manifest Archive. 

Step 2: Press "Scan Install Manifest Archive" button. 
A list of installed packages should show up in "Log Output". 
The results will be cached and will presist over program sessions.
Only need to rerun after installing new contents.

Optional Step 3: Press "Select Scene File" button to select a scene file.

Optional Step 4a: Press "Select Packages Based on Scene" button to select packages based on files referenced by the selected scene.

Step 4b: Select additional packages you want to include.
There is a "Clear Package Selection" button to unselect all. 

Step 5: Press "Select Output Folder Location" button to select where to save the virtual folder. 
Virtual folder need to be on a local drive.
Does not work with network drive.
The source files can be on network drive.

Step 6: Press "Generate Virtual Install Folder" to generate the virtual folder.
There is an option to create it in a subfolder with scene file name.

Step 7: Add the virtual folder as a base folder in Daz Studio. 
Remove base folders that contains contents you dont want daz to see.
Optionally use "Generate Install Script" button to create a Dazscript that will automatically include the virtual folder and open the scene.


## Background Info
The approach taken is to create a virtual folder containing reference to all the file needed in a scene.
By using the virtual folder as daz base folder,
only morphs needed are visibile to Daz, 
making the character load much faster.

It is also possible to manually select packages to install.

Also serve as a product manager that allow you to view all items with much bigger image than shown in daz.


The algorithm:

1. Scan daz install manifest archive to find all installed packages.
2. Scan a scene file to find all the files it need. 
3. Select all the packages containing the required files.
4. Create a virtual folder with symbolic link to all the files. 


## Limitations
* Please see the issue with saving while virtual folder is not added as a base directory.

* Only run on Windows 10.

* Require .NET Desktop Runtime 5 at [https://dotnet.microsoft.com/download/dotnet/5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
Download link:[https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.5-windows-x64-binaries](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-5.0.5-windows-x64-binaries)

* Need to run with developer mode or admin privilege. 
[Due to the permission required to create symbolic link](https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-createsymboliclinka)

* Only work with packages installed by DIM and have an install manifest.

* Character detection depend on default metadata.

## Version Log

V 1.5.0:
* Rearranged how content organised on display.
* Change the way contents are sorted to be more user friendly.
* Add filter by gender for package.

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
