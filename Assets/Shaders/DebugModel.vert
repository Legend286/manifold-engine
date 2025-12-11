#version 410 core

layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec3 a_Normal;
layout(location = 2) in vec4 a_Color;

layout(location = 0) out vec4 vColor;
layout(location = 1) out vec3 vNormal;

uniform mat4 u_Model;
uniform mat4 u_View;
uniform mat4 u_Projection;

void main() {
    vColor = a_Color;
    
    vNormal = inverse(transpose(mat3(u_Model))) * a_Normal;
    
    gl_Position = u_Projection * u_View * u_Model * vec4(a_Position, 1.0f);
}