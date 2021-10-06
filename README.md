# <img src="wwwroot/images/cube_logo.svg" style="width:40px;margin-top:15px;"/> Headstrong Solutions Website

Asp Net Core MVC website with Markdown middleware 

This was created using the Westwind Markdown solution at: [https://github.com/RickStrahl/Westwind.AspNetCore.Markdown](https://github.com/RickStrahl/Westwind.AspNetCore.Markdown)

Create a new controller by copying an old one, and in the wwwroot/Markdown directory create a symbolic link to the directory you want this controller to look for markdown pages from. Then in the controller, for the Section const string put the name of the symbolic link in.

### Windows Syumbolic Link creation example
```mklink /D C:\temp11111 \\server\share\foldername\```

**WARNING!** Deleting a symbolic link can EASILY have unexpected consequences as a symbolic link merely references the resulting directory location so if you tell the OS to delete a symlink incorrectly it will happily go to the remote linked location and **DELETE EVERYTHING!**
I will not go into how to remove symbolic links here, if you don't know how to do it, go find out! I cannot stress this enough.. Now present you is probably a little annoyed at this, but future you *will* thank this past you when someone indeterminate timeline you know says 'symlinks! God no! I deleted an entire [insert horror story]....'



Before I dissapear off the end of this readme, there is a simple controller that will list the markdown pages currently in the markdown folder
