# BIM 360 File Transfer Tool
A desktop program helps you copy files to different projects on BIM 360.
All functions are done. You can find the code in the master branch. 

# Deprecated 
This tool has been deprecated and we will no longer maintain this repo. The file transfer tool is transferred to the Autodesk Replication Tool for Docs. Check it out at https://art.autodesk.com/

# Instruction

- Download the code to your local computer. (Recommend GitHub Desktop App to save your time setting environment).

- Check if you have already install .net 4.8 framework. If not, you can download the runtime from https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48

- Open the :/BIM 360 File Transfer/BIM360FileTransfer.sln with Visual Studio 2022.

- In the Solution Explorer, right click the Solution 'BIM360FileTransfer' and then left click Restore NuGet Packages.

- Once it's done, right click the Solution 'BIM360FileTransfer' and then left click Rebuild Solution.

- Finally, click the green Triangle on the top debug panel.


# Working Functions

- Click Authentication button to open the new browser page.

- Use your hlw email to login.

- Allow all access request. Once it's done, the browser will automatically close and the Load CLOUD button will turn purple.

- Click Load CLOUD button to pull the BIM 360 library online. It will save a json copy in :/BIM 360 File Transfer/BIM360FileTransfer/Resources

- You can restart the app and try Load JSON button.

- Select files in the left source panel.

- Select target folder in the right target panel. You should be able to select multiple target folders using check box or hold Ctrl key.

- Click the > Button to fire the transfer process.

- Create a new version for files with duplicate names.

- Check your BIM 360 page to see the result.

# Known Issues

- Load CLOUD might encounter request limit.

- You can only click empty space or check box to deselect the files and folders.

- Transfer result doesn't show properly inside the program.

- Missing icons to distinguish the folder with files. Now all green.
