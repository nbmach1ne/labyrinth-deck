shader_type canvas_item;
render_mode unshaded;

uniform float outline_size : hint_range(0.0, 5.0, 1.0) = 1.0;
uniform vec4 outline_color: hint_color;
uniform vec2 texture_size;

void fragment()
{
	vec4 sprite_color = texture(TEXTURE, UV);
	float is_sprite = sprite_color.a;
	float pixels_to_texture_x = outline_size * (1.0 / texture_size.x);
	float pixels_to_texture_y = outline_size * (1.0 / texture_size.y);
	vec2 displacement_x = vec2(pixels_to_texture_x, 0.0);
	vec2 displacement_y = vec2(0.0, pixels_to_texture_y);
	float neighbors_alpha = texture(TEXTURE, UV + displacement_x).a +
							texture(TEXTURE, UV - displacement_x).a +
							texture(TEXTURE, UV + displacement_y).a +
							texture(TEXTURE, UV - displacement_y).a;
							
	neighbors_alpha = neighbors_alpha * (1.0 / neighbors_alpha);
	float is_outline = (1.0 - is_sprite) * neighbors_alpha;
	
	vec4 final_color = is_outline * outline_color + is_sprite * sprite_color;
	COLOR = final_color;
}

