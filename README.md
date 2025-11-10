> Project is in beta!
# Chelp

This is C#/WPF based GUI app which will help beginners or lazy peoples to compile their CPP files with **no terminal opening**. This is remake of my [python project](https://github.com/qwoj274/C-Compile-Helper).

*Features:*
- Auto searching for and testing compilers
- Auto detecting all the .cpp in chosen directory
- Compiling with chosen compiler and displaying output
- Built-in debug system

*In-dev:*
- Add english (now only russian avaliable)
- Add built-in input textbox for input stream of running program
- Add list of checkboxes where it will be possible to choice compiler args with user-friendly descriptions
- Add built-in download-and-install-compilers system

# Usage
1. In far left menu, select a folder with your .cpp files 
2. Select all the .cpp files you want to compile
3. In middle menu, select desired compiler from list (if they has found on your PC)
4. In far right menu, press the button of the bottom of border
5. OPTIONAL: you can input arguments in field in the top of the far right menu
6. Final program output will be displayed above the "run" button
7. If something goes wrong, you can check debug window below

# Building from source
```
git clone https://github.com/qwoj274/Chelp
cd Chelp/src
dotnet build
cd ChelpApp
dotnet run
```
