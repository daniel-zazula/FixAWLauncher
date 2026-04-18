# Fix AW Launcher

## The problem
If you use your Windows taskbar at the top of your monitor, after you open the Armored Warfare Launcher,
the launcher window starts moving down some 100 pixels per second until it dissapears from the desktop completely.

## The solution
This app repeatedly sets the launcher window top to 100 pixels from the top of the screen.

## Requirements
You neet to have the .NET 10.0 runtime installed on your computer to run this app.

## FAQ
### Why does it happen?
My guess is there is some code in the launcher setting the window position every second and it confuses the top of the desktop client area and the top of the screen.
If your taskbar is at the bottom of the screen, both are 0, but if your taskbar is at the top, the desktop client area starts at the bottom of the taskbar.

### Would it not be better to stop the window from moving?
Yes, but to do that I need access to the launcher's source code.
