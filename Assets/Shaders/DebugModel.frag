#version 410 core

layout(location = 0) in vec4 vColor;
layout(location = 1) in vec3 vNormal;
layout(location = 0) out vec4 color;


void main() {
    
    
    color = vec4(vColor);
    color *= dot(normalize(vec3(-0.25f,1.0f,-0.5f)), vNormal);
}