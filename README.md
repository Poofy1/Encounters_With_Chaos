# Encounters With Chaos

This Unity project is a visual demonstration of the concepts explained in the book "Encounters With Chaos" by Denny Gulick. This involves C# scripts as well as HLSL shaders in order to fully demonstrate the mathematical theories. Many options are presented to the user to allow them to tinker with the demonstrations. 



[VIDEO DEMONSTRATION](https://youtu.be/kwg85q2z-YQ)

## Requirements 
- Unity version 2022.3.6f1 or higher

## Hardware
- This software was tested on a RTX 4080 and a i7-10700KF
- Estimated minimum specs:
   - GTX 1660 TI
   - i5-6600K
   - 4 GB RAM
- Settings that effect performance are in red text

## Chapters and Visualizations

- Chapter 1: Period-3 Points Demonstration
   - This visualization explores the concept of period-3 points in the logistic map. It allows you to interactively adjust the parameter 'a' and the initial value 'x0' to observe how the system evolves over a specified number of iterations. The visualization displays the logistic map points and highlights the period-3 points, providing a visual understanding of the stability and bifurcations in the system.
![Encounters_With_Chaos](https://github.com/Poofy1/Encounters_With_Chaos/blob/main/Assets/Demo/Period3.png)

- Chapter 2: Cantor Sets Demonstration
   - This is a very rudimentary visualization of a fractal. By iteratively applying transformations and subtracting portions of the space, it creates a 3D fractal structure resembling the Cantor set. The demonstration allows you to control the number of iterations, and some dynamic lighting options. There are 3 colored light bulbs that float around the scene. This scene includes a randomization button.
![Encounters_With_Chaos](https://github.com/Poofy1/Encounters_With_Chaos/blob/main/Assets/Demo/Cantor.png)

- Chapter 3: Henon Map Demonstration
   - This demonstration presents a unique 3D visualization of the Henon map, a classic example of a chaotic dynamical system. The visualization allows you to adjust parameters such as the constants 'a', 'b', 'c', 'd', and the number of iterations. This scene includes a animation toggle to cycle through different states as well as a randomization button.
![Encounters_With_Chaos](https://github.com/Poofy1/Encounters_With_Chaos/blob/main/Assets/Demo/Henon.png)
  
- Chapter 4: Julia Set Fractal Demonstration
   - In this representation of a 2D Julia Set, users can adjust parameters such as the number of iterations and the complex constants 'x', and 'y' to observe how different values give rise to strikingly different fractal patterns. The visualization also incorporates smooth coloring and a time-varying component to create visually appealing and dynamic representations of the Julia set. Additionally, the demonstration includes intuitive zooming and panning functionality, as well as a randomization button.
![Encounters_With_Chaos](https://github.com/Poofy1/Encounters_With_Chaos/blob/main/Assets/Demo/Julia.png)

- Chapter 5: Lorenz System Demonstration
   - This demonstration show a 3D visualization of the Lorenz system. Users can explore the system's behavior by adjusting key parameters such as the Prandtl number (σ), beta (β), and rho (ρ). The visualization employs dynamic color gradients and smooth trail rendering to create a 3D representations of the system's trajectories. The user has the ability to rotate the system and add new lines at any position with their mouse. There is also a 'Respawn' and 'Randomize' button.
![Encounters_With_Chaos](https://github.com/Poofy1/Encounters_With_Chaos/blob/main/Assets/Demo/Lorenz.png)

## License

This project is licensed under the [MIT License](LICENSE).
