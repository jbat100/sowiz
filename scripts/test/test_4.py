import bpy
import bpy.ops
from bpy import context
from math import sin, cos, pi

def main():

    bpy.context.scene.layers[0] = True
    bpy.context.scene.layers[2] = False
    
    # function shortcuts
    add_cube = bpy.ops.mesh.primitive_cube_add
    add_plane = bpy.ops.mesh.primitive_plane_add
    add_icosphere = bpy.ops.mesh.primitive_icosphere_add
    cursor_location = context.scene.cursor_location
    
    radial_dist = 5.0
    x_size, y_size, z_size = 1.0, 1.0, 1.0
    theta = 0.0
    height = 0.0
    
    radial_dist_2 = 30.0
    theta_2 = 0.0
    
    count = 0
    default_color = bpy.data.materials.new('main_color')
    default_color.diffuse_color = (1.0, 0.0, 1.0)
    
    while theta_2 < pi * 2:
        
        while theta < pi * 2:
            
            #x = cursor_location.x + radial_dist * cos(theta)
            #y = cursor_location.y + radial_dist * sin(theta)
            #z = cursor_location.z
            
            x = radial_dist * cos(theta) + radial_dist_2 * cos(theta_2)
            y = radial_dist * sin(theta) + radial_dist_2 * sin(theta_2)
            z = height
            
            add_plane(location=(x,y,z))
            bpy.ops.transform.resize(value=(x_size, y_size, z_size))
            #bpy.ops.transform.rotate(value=theta, axis=(0.0,0.0,1.0))
            
            #color = bpy.data.materials.new('first_color')
            #offset = ((count + 1) % 100) / 100.0
            #color.diffuse_color = (offset, 0.0, 1.0 - offset)
            
            active_object = bpy.context.selected_objects[0]
            active_object.active_material = default_color
            
            get_particles = bpy.ops.object.particle_system_add()
            bpy.context.object.particle_systems[0].settings.name = 'fountain'
            #åbpy.context.object.particle_systems[0].settings.type = 'HAIR'
            bpy.context.object.particle_systems[0].settings.frame_start = 25.0
            bpy.context.object.particle_systems[0].settings.frame_end = 200.0
            bpy.context.object.particle_systems[0].settings.normal_factor = 30.0
            
            count += 1
            
            theta += pi / 4.0
            
            if theta > pi:
                height += 5.0
            
        theta = 0.0
        theta_2 += pi / 6
        height = 0.0
        
        
    theta = 0.0
    
main()