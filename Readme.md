# Daz Package Mananger

## Warning: After opening a scene using Virtual folder, removing the virtual folder as base directory and then save scene will cause daz to save texture maps as absolute path. This will cause error when opening the scene if you delete the virtual folder or open it on another computer. Saving the file again while having the virtual file added will fix the issue.

## Introduction
This program is intended to solve the problem of super slow character load when a large number of morphs are installed. 

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

* Require .net 5

* Need to run with developer mode or admin privilege. 
[Due to the permission required to create symbolic link](https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-createsymboliclinka)

* Minimumal amount of error handlings are implemented. 
Program probabaly only work when used in the intended way.

* Only work with packages installed by DIM and have an install manifest.

* Character detection depend on default metadata.

## How to Use
A prebuild program is saved in Release folder, not sure if it is build correctly.

Step 0: Set Windows to developer mode or run as admin.

Step 1: Select the folder where DIM save install manifest files.

Step 2: Press "Scan Install Manifest Archive" button. 
A list of installed packages should show up. 
The result is caches and will presist over program sessions.
Only need to rerun when installing new contents.

Step 3: Optionally select a scene file and select packages based on the contents of the scene.

Step 4: Select where to save the virtual folder. 
Virtual folder need to be on a local drive.
Does not work with network drive.
The source files can be on network drive.

Step 5: Generate the virtual folder.
There is an option to create it in a subfolder with scene file name.

Step 6: Add the virtual folder as a base folder in Daz Studio. 
Remove base folders that contains contents you dont want daz to see.


## Planned Features

1. Detect character generation.
2. Handle experssions (they are considered as pose?).
3. Filter poses based on generation. 
4. Handle clothings.
5. Handle other item.
6. Implement some sort of search functionality.
7. Detect character gender.
8. Implement filtering.
9. Make scaning install archive async.
10. Make image resizable.

## Version Log

Recommand V1.1.0

V1.0.0
Basic Functionality.
V1.1.0
Bug fix.
Able to find poses.
Improved Performance

## License
[GPL 2.0](https://www.gnu.org/licenses/old-licenses/gpl-2.0.html)
