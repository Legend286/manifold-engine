#version 410 core

layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec4 a_Color;

layout(location = 0) out vec4 vColor;
layout(location = 1) out vec4 vClipPos;

uniform mat4 u_View;
uniform mat4 u_Projection;

void main() {
    vColor = a_Color;
    vClipPos = u_Projection * u_View * vec4(a_Position, 1.0f);
    
    gl_Position = vClipPos;
}