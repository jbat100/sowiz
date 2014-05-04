import bpy
import bpy.ops
from bpy import context
from math import sin, cos, pi

# function shortcuts
add_cube = bpy.ops.mesh.primitive_cube_add
cursor_location = context.scene.cursor_location

radial_dist = 2.0
x_size, y_size, z_size = 3.0, 1.5, 0.5
theta = 0.0
height = 0.0

radial_dist_2 = 20.0
theta_2 = 0.0

while theta_2 < pi * 2:
    
    while theta < pi * 2:
        
        #x = cursor_location.x + radial_dist * cos(theta)
        #y = cursor_location.y + radial_dist * sin(theta)
        #z = cursor_location.z
        
        x = radial_dist * cos(theta) + radial_dist_2 * cos(theta_2)
        y = radial_dist * sin(theta) + radial_dist_2 * sin(theta_2)
        z = height
        
        add_cube(location=(x,y,z))
        bpy.ops.transform.resize(value=(x_size, y_size, z_size))
        bpy.ops.transform.rotate(value=theta, axis=(0.0,0.0,1.0))
        
        theta += pi / 4.0
        height += 2.0
        
    theta = 0.0
    theta_2 += pi / 8
    height = 0.0
    
    
theta = 0.0