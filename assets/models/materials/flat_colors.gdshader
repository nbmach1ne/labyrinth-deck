shader_type spatial;
render_mode unshaded;

uniform sampler2D texture : hint_albedo;
uniform vec4 tint: hint_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment()
{
	ALBEDO = texture(texture, UV).rgb * tint.rgb;
}