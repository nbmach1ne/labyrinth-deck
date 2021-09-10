shader_type spatial;
render_mode unshaded;

uniform sampler2D texture : hint_albedo;
uniform vec4 tint: hint_color = vec4(1.0, 1.0, 1.0, 1.0);

uniform float noise_scale = 2;
uniform float time_snap = 0.25;

float snapTime(float time)
{
	return time_snap * round(time / time_snap);
}

float rand(vec2 coord)
{
	return fract(sin(dot(coord, vec2(12.9898, 78.233))) * 43758.5453);
}

void vertex()
{
	float time = snapTime(TIME);
	vec2 noise = vec2(rand(VERTEX.xy + time)) * noise_scale;
	VERTEX += vec3(noise, 0.0);
}

void fragment()
{
	ALBEDO = texture(texture, UV).rgb * tint.rgb;
}