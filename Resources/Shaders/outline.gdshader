shader_type spatial;
// 'blend_mix' tells Godot to use standard alpha blending
render_mode unshaded, blend_mix, depth_draw_opaque, cull_disabled;

uniform sampler2D albedo_texture : source_color, filter_nearest;
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float width : hint_range(0.0, 64.0) = 1.0;
uniform float spin_rotation = 0.0;

void vertex() {
	// 1. Billboard logic
	MODELVIEW_MATRIX = VIEW_MATRIX * mat4(
		INV_VIEW_MATRIX[0],
		INV_VIEW_MATRIX[1],
		INV_VIEW_MATRIX[2],
		MODEL_MATRIX[3]
	);

	// 2. Internal Spin (rotating vertices around center)
	float s = sin(spin_rotation);
	float c = cos(spin_rotation);
	mat2 rotation_mat = mat2(vec2(c, -s), vec2(s, c));
	VERTEX.xz = rotation_mat * VERTEX.xz;
}

void fragment() {
	// Calculate outline sampling size
	vec2 size = vec2(width) / vec2(textureSize(albedo_texture, 0));

	// Sample the main texture
	vec4 col = texture(albedo_texture, UV);

	// Sampling 4-8 neighbors to determine outline alpha
	float outline = 0.0;
	outline += texture(albedo_texture, UV + vec2(size.x, 0.0)).a;
	outline += texture(albedo_texture, UV + vec2(-size.x, 0.0)).a;
	outline += texture(albedo_texture, UV + vec2(0.0, size.y)).a;
	outline += texture(albedo_texture, UV + vec2(0.0, -size.y)).a;
	outline = min(outline, 1.0);

	// CORE TRANSPARENCY FIX:
	// Set ALBEDO to the color (mixing outline if needed)
	// Set ALPHA to the maximum of either the texture's alpha or the outline's alpha
	ALBEDO = mix(outline_color.rgb, col.rgb, col.a);
	ALPHA = max(col.a, outline);

    // Optional: Alpha Scissor (use this if you see weird transparent sorting issues)
    // if (ALPHA < 0.1) discard;
}