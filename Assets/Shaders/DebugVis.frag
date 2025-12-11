#version 410 core

layout(location = 0) in vec4 vColor;
layout(location = 1) in vec4 vClipPos;
layout(location = 0) out vec4 color;

uniform sampler2D u_Depth;

vec3 desaturate(vec3 color, float amount)
{
    // Perceptual luminance (Rec. 709)
    float gray = dot(color, vec3(0.33, 0.33, 0.33));
    return mix(color, vec3(gray), clamp(amount, 0.0, 1.0));
}

void main()
{
    vec3 ndc = vClipPos.xyz / vClipPos.w;

    if (ndc.x < -1 || ndc.x > 1 ||
    ndc.y < -1 || ndc.y > 1)
    discard;

    vec2 uv = ndc.xy * 0.5 + 0.5;
    
    float sceneDepth = texture(u_Depth, uv).r;
    float debugDepth = ndc.z * 0.5 + 0.5;

    float occluded = debugDepth > sceneDepth + 0.0005f ? 1.0f : 0.0f;
    
    float alpha = mix(1.0, 1.0 * vColor.a, occluded);
    color = vec4(mix(vColor.rgb, desaturate(vColor.rgb, 0.7), occluded), alpha * vColor.a);
}
