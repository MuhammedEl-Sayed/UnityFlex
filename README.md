# UnityFlex
## Background

  UnityFlex is an **Opensource C#** implementation of the popular and widely used **CSS Flexbox** layout algorithm. It shares many of the same customization options, [documented](https://muhammedel-sayed.github.io/UnityFlex/), and is **completely free to use**.
  
 ## How to use
  1. Begin by downloading/[cloning](https://github.com/MuhammedEl-Sayed/UnityFlex.git) the repository. 
  2. Import the **UnityFlex** folder into your Unity Project.
  3. In your scene, create a **Canvas** UI component and set the UI Scale Mode property in the Canvas Scaler component to **Scale with screen Size**
  4. Create an **Empty Object** in your Canvas and attach the **FlexContainer** script. Be sure to tick the Root Container box.
  5. Create another **Empty Object** in the Root Container Object from Step 4, also give it the **FlexContainer** script. BE sure to tick the Child Container box.
  6. Add your UI elements as children to the Child Container Object from Step 5. Feel free to add more Child Containers and edit the properties in the FlexContainer scripts. Note that the properties in the **Root Conatiner** Object will manipulate the Child Containers only.
