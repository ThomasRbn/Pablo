import { defineConfig } from "orval";

export default defineConfig({
	pablo: {
		input: {
			target: "./openapi/openapi.json",
		},
		output: {
			mode: "tags-split",
			target: "./src/api/generated",
			schemas: "./src/api/generated/models",
			client: "fetch",
			clean: true,
			override: {
				mutator: {
					path: "./src/api/mutator.ts",
					name: "customFetch",
				},
				fetch: {
					includeHttpResponseReturnType: false,
				},
			},
		},
	},
});
