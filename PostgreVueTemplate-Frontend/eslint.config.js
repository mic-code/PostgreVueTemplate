import eslint from "@eslint/js";
import globals from "globals";
import pluginVue from "eslint-plugin-vue";
import stylistic from "@stylistic/eslint-plugin";
import unocss from "@unocss/eslint-config/flat";
import vueParser from "vue-eslint-parser";
import tsParser from "@typescript-eslint/parser";
import tseslint from "typescript-eslint";


export default [
	// Base configs
	eslint.configs.recommended,
	...tseslint.configs.recommended,
	unocss,

	// Common configuration for JS, TS, Vue files
	{
		files: ["**/*.{js,mjs,cjs,ts,vue}"],
		plugins: {
			"@stylistic": stylistic,
		},
		languageOptions: {
			parserOptions: {
				sourceType: "module",
				ecmaVersion: "latest",
			},
			globals: {
				...globals.browser,
				...globals.node, // Include node for config files, SSR, etc.
				ga: "readonly",
				cordova: "readonly",
				Capacitor: "readonly",
				chrome: "readonly",
				browser: "readonly",
			},
		},
		rules: {
			// Common rules applied to all files
			"prefer-promise-reject-errors": "off",
			indent: ["warn", "tab", { SwitchCase: 1 }],
			quotes: ["error", "double"],
			semi: ["error", "always"],
			"key-spacing": ["warn", { afterColon: true }],
			"switch-colon-spacing": ["error", { after: true, before: false }],
			"brace-style": [2, "allman", { allowSingleLine: true }],
			"space-infix-ops": ["warn"],
			"@typescript-eslint/no-unused-vars": "warn",
			"@typescript-eslint/no-explicit-any": "off",
			"@stylistic/type-annotation-spacing": "warn",
			"no-constant-condition": "warn",
			"prefer-const": "off",
			"array-bracket-spacing": ["warn", "never"],
			"object-curly-spacing": ["warn", "always"],
			"comma-spacing": ["warn", { after: true }],
			"no-mixed-spaces-and-tabs": ["warn"],
			"no-trailing-spaces": "warn",
			"no-empty": "off",
			"no-multi-spaces": ["warn"],
		},
	},

	// Vue specific configuration
	{
		files: ["**/*.vue"],
		plugins: {
			vue: pluginVue,
			// @stylistic is already included in the common config
		},
		languageOptions: {
			parser: vueParser,
			parserOptions: {
				parser: tsParser, // Use TS parser for <script> blocks
				project: "./tsconfig.app.json",
				tsconfigRootDir: import.meta.dirname,
				extraFileExtensions: [".vue"],
			},
			// Globals are inherited from the common config
		},
		rules: {
			// Apply Vue recommended rules
			...pluginVue.configs["flat/recommended"].rules,
			// Override/add specific Vue rules (common rules are already applied by the block above)
			"vue/max-len": ["warn", { code: 1000 }],
			"vue/no-v-text-v-html-on-component": "off",
			"vue/html-closing-bracket-newline": "off",
			"vue/multi-word-component-names": "off",
			"vue/first-attribute-linebreak": "off",
			"vue/max-attributes-per-line": "off",
			"vue/require-default-prop": "off",
			"vue/no-template-shadow": "off",
			"vue/require-v-for-key": "off",
			"vue/no-multi-spaces": "warn",
			"vue/valid-v-slot": "off",
			"vue/no-unused-vars": "warn", // Override TS rule for Vue template usage
			"vue/html-indent": ["warn", "tab", {
				attribute: 1,
				baseIndent: 1,
				closeBracket: 0,
				alignAttributesVertically: true,
			}],
			"vue/html-self-closing": ["warn", {
				html: { void: "never", normal: "any", component: "any" },
			}],
			// Ensure common rules like indent, quotes etc. are not duplicated here
			// They are inherited from the common config block
		},
	},

	// // TypeScript specific configuration (for non-Vue files)
	// {
	// 	// Apply only to TS/JS files, Vue files are handled above
	// 	files: ["**/*.{js,mjs,cjs,ts}"],
	// 	languageOptions: {
	// 		parser: tsParser, // Ensure TS parser is used
	// 		parserOptions: {
	// 			project: "./tsconfig.app.json", // Link to tsconfig for type-aware rules
	// 			tsconfigRootDir: import.meta.dirname,
	// 			sourceType: "module", // Already set in common
	// 		},
	// 		// Globals are inherited from the common config
	// 	},
	// 	rules: {
	// 		// Add any TS-specific overrides here if needed
	// 		// Common rules and tseslint.configs.recommended already apply from previous blocks
	// 	},
	// },

	// Optional: Service worker specific globals (keep commented if not needed)
	// {
	// 	files: ["src-pwa/custom-service-worker.ts"],
	// 	languageOptions: {
	// 		globals: {
	// 			...globals.serviceworker
	// 		}
	// 	}
	// }
];
