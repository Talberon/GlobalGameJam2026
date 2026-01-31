shader_type spatial;
// 'unshaded' for consistent UI/Sprite look
// 'depth_test_disabled' if you want it to draw over everything
render_mode unshaded, blend_mix, depth_draw_opaque, cull_disabled;

uniform sampler2D albedo_texture : source_color, filter_nearest;
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float width : hint_range(0.0, 64.0) = 1.0;

void vertex() {
	// 1. Remove the rotation of the model from the ModelView matrix
	// This forces the mesh to face the camera at all times
	MODELVIEW_MATRIX = VIEW_MATRIX * mat4(
		INV_VIEW_MATRIX[0],
		INV_VIEW_MATRIX[1],
		INV_VIEW_MATRIX[2],
		MODEL_MATRIX[3]
	);

	// 2. Handle scaling (optional but recommended)
	// This ensures that if you scale the NPC node, the sprite scales too
	MODELVIEW_MATRIX = MODELVIEW_MATRIX * mat4(
		vec4(length(MODEL_MATRIX[0].xyz), 0.0, 0.0, 0.0),
		vec4(0.0, length(MODEL_MATRIX[1].xyz), 0.0, 0.0),
		vec4(0.0, 0.0, length(MODEL_MATRIX[2].xyz), 0.0),
		vec4(0.0, 0.0, 0.0, 1.0)
	);
}

void fragment() {
	vec2 size = vec2(width) / vec2(textureSize(albedo_texture, 0));
	vec4 col = texture(albedo_texture, UV);

	// Sampling 8 neighbors for the outline
	float outline = 0.0;
	outline += texture(albedo_texture, UV + vec2(size.x, 0.0)).a;
	outline += texture(albedo_texture, UV + vec2(-size.x, 0.0)).a;
	outline += texture(albedo_texture, UV + vec2(0.0, size.y)).a;
	outline += texture(albedo_texture, UV + vec2(0.0, -size.y)).a;
	outline += texture(albedo_texture, UV + vec2(size.x, size.y)).a;
	outline += texture(albedo_texture, UV + vec2(-size.x, size.y)).a;
	outline += texture(albedo_texture, UV + vec2(size.x, -size.y)).a;
	outline += texture(albedo_texture, UV + vec2(-size.x, -size.y)).a;
	outline = min(outline, 1.0);

	vec3 final_rgb = mix(outline_color.rgb, col.rgb, col.a);
	float final_alpha = max(outline, col.a);

	ALBEDO = final_rgb;
	ALPHA = final_alpha;
}